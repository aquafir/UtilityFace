using AcClient;
using Decal.Adapter;
using Decal.Adapter.Wrappers;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityBelt.Service.Views;

namespace PickerDemo.Lib {
    public class ManagedMesh : IDisposable {
        public List<ManagedMaterial> Materials { get; } = new List<ManagedMaterial>();
        public Mesh DxMesh { get; }

        public ManagedMesh(Mesh dxMesh) {
            DxMesh = dxMesh;
        }

        public void AddMaterial(ManagedMaterial material) {
            Materials.Add(material);
        }

        public void Dispose() {
            foreach (var mat in Materials) {
                mat.Dispose();
            }
            Materials.Clear();
        }
        internal void Render(Device D3Ddevice) {
            for (var i = 0; i < Materials.Count; i++) {
                D3Ddevice.Material = Materials[i].Material;
                if (Materials[i].Texture is not null) {
                    D3Ddevice.SetTexture(0, Materials[i].Texture.Texture);
                }
                else {
                    D3Ddevice.SetTexture(0, null);
                }

                DxMesh.DrawSubset(i);
            }
        }
    }
}
