using AcClient;
using Decal.Adapter;
using ImGuiNET;
using ImGuizmoNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
//using Vector3 = System.Numerics.Vector3;

namespace UtilityFace.Lib {
    public unsafe class PhysicsObjTransformer : IDisposable {
        [Flags]
        private enum UpdateType {
            None = 0x00000000,
            Translation = 0x00000001,
            Rotation = 0x00000002,
            Scale = 0x00000004,
            All = Translation | Rotation | Scale
        }

        private CPhysicsObj* _obj;
        private OPERATION mCurrentGizmoOperation = OPERATION.TRANSLATE;
        private MODE mCurrentGizmoMode = MODE.WORLD;

        public bool UseSnap { get; set; }
        public bool LockToGround { get; set; } = false;
        public float SnapTranslation { get; set; } = 1;
        public float SnapRotation { get; set; } = 1;
        public float SnapScale { get; set; }
        public float TranslationDragSpeed => UseSnap ? SnapTranslation : 0.01f;
        public float RotationDragSpeed => UseSnap ? SnapRotation : 1f;
        public float ScaleDragSpeed => UseSnap ? SnapScale : 0.01f;

        public Vector3 Rotation => _obj is null ? Vector3.Zero : Vector3.Zero;

        public PhysicsObjTransformer() {

        }

        public PhysicsObjTransformer(CPhysicsObj* obj) : this() {
            SetTarget(obj);
        }

        public void SetTarget(CPhysicsObj* obj) {
            _obj = obj;
        }

        public bool HasTarget() {
            return _obj is not null;
        }

        public void ClearTarget() {
            _obj = null;
        }

        private unsafe static int GetChildRecursive(ref UIElement This, uint _ID) {
            return ((delegate* unmanaged[Thiscall]<ref UIElement, uint, int>)4602880)(ref This, _ID);
        }

        public void Render(bool drawEditor) {
            if (_obj is null) return;

            var io = ImGui.GetIO();

            var t = *UIElementManager.s_pInstance->m_pRootElement;
            var sbox = ((UIElement*)GetChildRecursive(ref t, 0x1000049Au))->a0.m_box;

            ImGuizmo.Enable(true);
            ImGuizmo.SetRect(sbox.m_x0, sbox.m_y0, sbox.m_x1 - sbox.m_x0, sbox.m_y1 - sbox.m_y0);
            ImGuizmo.AllowAxisFlip(false);
            ImGuizmo.SetOrthographic(false);

            var viewTransform = G.CameraH.GetViewTransform();
            var viewTransform2 = Matrix4x4.Transpose(viewTransform);
            var proj = G.CameraH.GetProjection();
            var objMat = ObjectMatrix();


            Vector3 translationV = Vector3.Zero;
            Vector3 rotationV = Vector3.Zero;
            Vector3 scaleV = Vector3.Zero;

            ImGuizmo.DecomposeMatrixToComponents(ref viewTransform.M11, ref translationV.X, ref rotationV.X, ref scaleV.X);

            if (ImGuizmo.Manipulate(ref viewTransform.M11, ref proj.M11, mCurrentGizmoOperation, mCurrentGizmoMode, ref objMat.M11)) {
                var updateFlags = UpdateType.None;

                if (((uint)mCurrentGizmoOperation & (uint)OPERATION.TRANSLATE) != 0) {
                    updateFlags |= UpdateType.Translation;
                }
                if (((uint)mCurrentGizmoOperation & (uint)OPERATION.ROTATE) != 0) {
                    updateFlags |= UpdateType.Rotation;
                }
                if (((uint)mCurrentGizmoOperation & (uint)OPERATION.SCALE) != 0) {
                    updateFlags |= UpdateType.Scale;
                }

                if (updateFlags != UpdateType.None) {
                    UpdateObject(objMat, updateFlags);
                }
            }

            var size = new Vector2(150, 150);
            var offset = new Vector2(io.DisplaySize.X - 270, -10);
            ImGuizmo.ViewManipulate(ref viewTransform.M11, ref proj.M11, mCurrentGizmoOperation, mCurrentGizmoMode, ref objMat.M11, 0, offset, size, 0);

            if (drawEditor) {
                if (ImGui.RadioButton("Translate###TranslateOp", mCurrentGizmoOperation == OPERATION.TRANSLATE)) {
                    mCurrentGizmoOperation = OPERATION.TRANSLATE;
                }
                ImGui.SameLine();
                if (ImGui.RadioButton("Rotate###RotateOp", mCurrentGizmoOperation == OPERATION.ROTATE)) {
                    mCurrentGizmoOperation = OPERATION.ROTATE;
                }
                ImGui.SameLine();
                if (ImGui.RadioButton("Scale###ScaleOp", mCurrentGizmoOperation == OPERATION.SCALE_Z)) {
                    mCurrentGizmoOperation = OPERATION.SCALE_Z;
                }

                /*
                // todo...
                if (mCurrentGizmoOperation == OPERATION.TRANSLATE) {
                    var lockToGround = LockToGround;
                    if (ImGui.Checkbox("Lock to ground", ref lockToGround)) {
                        LockToGround = lockToGround;
                    }
                }

                RenderSnapSettings();
                */

                RenderTranslateRotateScaleInputs(objMat);
            }
        }

        private void RenderSnapSettings() {
            var useSnap = UseSnap;
            if (ImGui.Checkbox("Snap", ref useSnap)) {
                UseSnap = useSnap;
            }
            ImGui.SameLine();
            switch (mCurrentGizmoOperation) {
                case OPERATION.TRANSLATE:
                    var snapTranslation = SnapTranslation;
                    if (ImGui.DragFloat("XYZ", ref snapTranslation, 0.1f)) {
                        SnapTranslation = snapTranslation;
                    }
                    break;
                case OPERATION.ROTATE:
                    var snapRotation = SnapRotation;
                    if (ImGui.DragFloat("XYZ", ref snapRotation, 0.1f)) {
                        SnapRotation = snapRotation;
                    }
                    break;
                case OPERATION.SCALE_Z:
                    var snapScale = SnapScale;
                    if (ImGui.DragFloat("XYZ", ref snapScale, 0.1f)) {
                        SnapScale = snapScale;
                    }
                    break;
            }
        }

        private void RenderTranslateRotateScaleInputs(Matrix4x4 objMat) {
            Vector3 translationV = Vector3.Zero;
            Vector3 rotationV = Vector3.Zero;
            Vector3 scaleV = Vector3.Zero;

            ImGuizmo.DecomposeMatrixToComponents(ref objMat.M11, ref translationV.X, ref rotationV.X, ref scaleV.X);

            var updateFlags = UpdateType.None;

            if (ImGui.DragFloat3("Translation (X/Y/Z)", ref translationV, TranslationDragSpeed)) {
                updateFlags |= UpdateType.Translation;
            }
            if (ImGui.DragFloat3("Rotation (X/Y/Z)", ref rotationV, RotationDragSpeed)) {
                updateFlags |= UpdateType.Rotation;
            }
            if (ImGui.DragFloat("Scale (XYZ)", ref scaleV.Z, ScaleDragSpeed)) {
                updateFlags |= UpdateType.Scale;
                scaleV.Y = scaleV.Z;
                scaleV.X = scaleV.Z;
            }

            if (updateFlags != UpdateType.None) {
                var mat = Matrix4x4.Identity;
                ImGuizmo.RecomposeMatrixFromComponents(ref translationV.X, ref rotationV.X, ref scaleV.X, ref mat.M11);
                UpdateObject(mat, updateFlags);
            }
        }

        private void UpdateObject(Matrix4x4 objMat, UpdateType updateType) {
            if (_obj is null) return;

            var picked = G.Picker.PickTerrain();

            Vector3 translationV = Vector3.Zero;
            Vector3 rotationV = Vector3.Zero;
            Vector3 scaleV = Vector3.Zero;

            ImGuizmo.DecomposeMatrixToComponents(ref objMat.M11, ref translationV.X, ref rotationV.X, ref scaleV.X);

            if (updateType.HasFlag(UpdateType.Translation)) {
                _obj->m_position.frame.m_fOrigin.x = translationV.X;
                _obj->m_position.frame.m_fOrigin.y = translationV.Y;
                _obj->m_position.frame.m_fOrigin.z = translationV.Z;
            }

            if (updateType.HasFlag(UpdateType.Scale)) {
                _obj->SetScale((float)Math.Round(scaleV.Z, 2), 0.001f);
            }

            if (updateType.HasFlag(UpdateType.Rotation)) {
                var q = Quaternion.CreateFromRotationMatrix(objMat);
                _obj->m_position.frame.qw = (float)Math.Round(q.W, 2);
                _obj->m_position.frame.qx = (float)Math.Round(q.X, 2);
                _obj->m_position.frame.qy = (float)Math.Round(q.Y, 2);
                _obj->m_position.frame.qz = (float)Math.Round(q.Z, 2);
            }
        }

        public Matrix4x4 ObjectMatrix() {
            if (_obj is null) {
                return Matrix4x4.Identity;
            }
            var frame = _obj->m_position.frame;

            var q = new Quaternion((float)Math.Round(frame.qx, 2), (float)Math.Round(frame.qy, 2), (float)Math.Round(frame.qz, 2), (float)Math.Round(frame.qw, 2));

            var rotM = Matrix4x4.CreateFromQuaternion(q);
            var scaleM = Matrix4x4.CreateScale(_obj->m_scale, _obj->m_scale, _obj->m_scale);
            var translateM = Matrix4x4.CreateTranslation(frame.m_fOrigin.x, frame.m_fOrigin.y, frame.m_fOrigin.z);

            return scaleM * rotM * translateM;
        }
        public void Dispose() {
            
        }
    }
}
