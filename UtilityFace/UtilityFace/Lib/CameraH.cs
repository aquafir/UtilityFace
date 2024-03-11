using AcClient;
using Decal.Adapter;
using ImGuiNET;
using ImGuizmoNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Frame = AcClient.Frame;
//using Vector3 = System.Numerics.Vector3;

namespace UtilityFace.Lib {
    public unsafe class CameraH {
        public CameraH() {

        }

        public static AC1Legacy.Vector3* Frame_globaltolocal(ref Frame This, AC1Legacy.Vector3* res, AC1Legacy.Vector3* _in) => ((delegate* unmanaged[Thiscall]<ref Frame, AC1Legacy.Vector3*, AC1Legacy.Vector3*, AC1Legacy.Vector3*>)0x004526C0)(ref This, res, _in);
        //.text:004526C0 ; float *__thiscall Frame::globaltolocal(float *this, float *, float *)

        public Matrix4x4 GetViewTransform() {
            var smartbox = SmartBox.smartbox[0];
            var viewerPos = smartbox->viewer;
            var m_fl2gv = viewerPos.frame.m_fl2gv;

            var _in = new AC1Legacy.Vector3();
            var p = new AC1Legacy.Vector3();
            Frame_globaltolocal(ref viewerPos.frame, &p, &_in);

            var xAxis = new Vector3(m_fl2gv[0], m_fl2gv[1], m_fl2gv[2]);
            // Negate Y-axis for right-handed system
            var yAxis = -new Vector3(m_fl2gv[3], m_fl2gv[4], m_fl2gv[5]);
            var zAxis = new Vector3(m_fl2gv[6], m_fl2gv[7], m_fl2gv[8]);

            var res = Matrix4x4.Identity;

            res.M11 = xAxis.X;
            res.M12 = zAxis.X;
            res.M13 = yAxis.X; 
            res.M14 = 0;

            res.M21 = xAxis.Y;
            res.M22 = zAxis.Y;
            res.M23 = yAxis.Y;
            res.M24 = 0;

            res.M31 = xAxis.Z;
            res.M32 = zAxis.Z;
            res.M33 = yAxis.Z;
            res.M34 = 0;

            res.M41 = p.a0.x;
            res.M42 = p.a0.z;
            res.M43 = -p.a0.y; // Negate Z-axis for right-handed system
            res.M44 = 1;

            // Right-handed coordinate system adjustment
            var rhBasis = new Matrix4x4(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1);

            res = Matrix4x4.Multiply(rhBasis, res);

            return res;
        }

        public unsafe static int GetChildRecursive(ref UIElement This, uint _ID) {
            return ((delegate* unmanaged[Thiscall]<ref UIElement, uint, int>)4602880)(ref This, _ID);
        }

        internal Matrix4x4 GetProjection() {
            var aspectRatio = RenderDevice.render_device[0]->m_ViewportAspectRatio;
            var fov = SmartBox.smartbox[0]->m_fGameFOV / (aspectRatio - 0.1f);

            return Matrix4x4.CreatePerspectiveFieldOfView(fov, aspectRatio, *Render.znear, *Render.zfar);
        }
    }
}
