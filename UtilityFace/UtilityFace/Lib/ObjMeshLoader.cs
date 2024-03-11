using Decal.Adapter;
using JeremyAnsel.Media.WavefrontObj;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityBelt.Scripting.Interop;
using UtilityBelt.Service;
using UtilityBelt.Service.Views;

namespace PickerDemo.Lib {
    public static class ObjMeshLoader {
        private static Dictionary<string, ManagedTexture>  loadedTextures = new Dictionary<string, ManagedTexture>();
        public static ManagedMesh? FromFile(string filePath) {
            return FromObjFile(ObjFile.FromFile(filePath), Path.GetDirectoryName(filePath));
        }

        public static ManagedMesh? FromStream(Stream stream) {
            return FromObjFile(ObjFile.FromStream(stream), PluginCore.AssemblyDirectory);
        }

        private static ManagedMesh? FromObjFile(ObjFile obj, string directory) {
            try {
                var options = MeshFlags.Managed | MeshFlags.Use32Bit;
                var numFaces = obj.Faces.Sum(f => f.Vertices.Count - 2);
                var numVertices = numFaces * 3;
                var dxMesh = new Mesh(numVertices * 10, numVertices, options, CustomVertex.PositionNormalTextured.Format, PluginCore.D3Ddevice);
                var mesh = new ManagedMesh(dxMesh);

                // load materials
                foreach (var materialLibFile in obj.MaterialLibraries) {
                    var matFilePath = System.IO.Path.Combine(directory, materialLibFile);

                    var materialLib = ObjMaterialFile.FromFile(matFilePath);
                    foreach (var material in materialLib.Materials) {
                        Material mat = new Material();

                        if (material.DiffuseColor?.Color is not null) {
                            mat.Diffuse = Color.FromArgb(255, (int)(material.DiffuseColor.Color.X * 255f), (int)(material.DiffuseColor.Color.Y * 255f), (int)(material.DiffuseColor.Color.Z * 255f));
                        }
                        if (material.AmbientColor?.Color is not null) {
                            mat.Ambient = Color.FromArgb(255, (int)(material.AmbientColor.Color.X * 255f), (int)(material.AmbientColor.Color.Y * 255f), (int)(material.AmbientColor.Color.Z * 255f));
                        }
                        if (material.SpecularColor?.Color is not null) {
                            mat.Specular = Color.FromArgb(255, (int)(material.SpecularColor.Color.X * 255f), (int)(material.SpecularColor.Color.Y * 255f), (int)(material.SpecularColor.Color.Z * 255f));
                        }
                        if (material.EmissiveColor?.Color is not null) {
                            mat.Emissive = Color.FromArgb(255, (int)(material.EmissiveColor.Color.X * 255f), (int)(material.EmissiveColor.Color.Y * 255f), (int)(material.EmissiveColor.Color.Z * 255f));
                        }

                        var managedMaterial = new ManagedMaterial(material.Name, mat);
                        if (!string.IsNullOrWhiteSpace(material.DiffuseMap?.FileName)) {
                            var textureFile = System.IO.Path.Combine(directory, material.DiffuseMap.FileName); 
                            if (loadedTextures.ContainsKey(textureFile)) {
                                managedMaterial.Texture = loadedTextures[textureFile];
                            }
                            else {
                                try {
                                    CoreManager.Current.Actions.AddChatText($"Texture: {textureFile}", 1);
                                    managedMaterial.Texture = new ManagedTexture(textureFile);
                                    loadedTextures.Add(textureFile, managedMaterial.Texture);
                                }
                                catch (Exception ex) { UBService.LogException(ex); }
                            }
                        }

                        mesh.AddMaterial(managedMaterial);
                    }
                }

                var verts = new List<CustomVertex.PositionNormalTextured>();
                var attributes = new List<int>();
                var indices = new List<int>();

                foreach (var face in obj.Faces) {
                    var matIdx = mesh.Materials.FindIndex(m => m.Name == face.MaterialName);
                    for (var i = 1; i < face.Vertices.Count - 1; i++) {
                        AddFaceVerts(obj, face, i, ref verts, ref indices);
                        attributes.Add(matIdx);
                    }
                }
                CoreManager.Current.Actions.AddChatText($"Faces: {numFaces} attr: {attributes.Count}", 1);

                using (var vertexBuffer = dxMesh.LockVertexBuffer(LockFlags.None)) {
                    vertexBuffer.Write(verts.ToArray());
                    dxMesh.UnlockVertexBuffer();
                }

                using (var indexBuffer = dxMesh.LockIndexBuffer(LockFlags.None)) {
                    dxMesh.SetIndexBufferData(indices.ToArray(), LockFlags.None);
                    dxMesh.UnlockIndexBuffer();
                }

                using (var attributeBuffer = dxMesh.LockAttributeBuffer(LockFlags.Discard)) {
                    attributeBuffer.Write(attributes.ToArray());
                    dxMesh.UnlockAttributeBuffer();
                }

                /*
                int[] aAdjacency = new int[obj.Faces.Sum(f => (f.Vertices.Count - 2) * 3)];
                dxMesh.GenerateAdjacency(1e-6f, aAdjacency);
                dxMesh.OptimizeInPlace(MeshFlags.OptimizeAttributeSort | MeshFlags.OptimizeVertexCache, aAdjacency);
                */

                return mesh;
            }
            catch (Exception ex) { UBService.LogException(ex); }

            return null;
        }

        private static void AddFaceVerts(ObjFile obj, ObjFace face, int i, ref List<CustomVertex.PositionNormalTextured> verts, ref List<int> indices) {
            AddFaceVert(obj, face, 0, ref verts, ref indices);
            AddFaceVert(obj, face, i, ref verts, ref indices);
            AddFaceVert(obj, face, i + 1, ref verts, ref indices);
        }

        private static void AddFaceVert(ObjFile obj, ObjFace face, int i, ref List<CustomVertex.PositionNormalTextured> verts, ref List<int> indices) {
            var fv = face.Vertices[i];
            var pos = new Vector3(obj.Vertices[fv.Vertex - 1].Position.X, -obj.Vertices[fv.Vertex - 1].Position.Y, obj.Vertices[fv.Vertex - 1].Position.Z);
            var normal = new Vector3(obj.VertexNormals[fv.Normal - 1].X, obj.VertexNormals[fv.Normal - 1].Y, obj.VertexNormals[fv.Normal - 1].Z);
            var t = new Vector2(obj.TextureVertices[fv.Texture - 1].X, obj.TextureVertices[fv.Texture - 1].Y);

            verts.Add(new CustomVertex.PositionNormalTextured(pos, normal, t.X, 1- t.Y));
            indices.Add(verts.Count - 1);
        }
    }
}
