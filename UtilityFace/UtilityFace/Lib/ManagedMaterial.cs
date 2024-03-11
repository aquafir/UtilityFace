using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityBelt.Service.Views;

namespace PickerDemo.Lib {
    public class ManagedMaterial : IDisposable {
        public string Name { get; }
        public ManagedTexture Texture { get; set; }
        public Material Material { get; set; }

        public ManagedMaterial(string name, Material material, ManagedTexture? texture = null) {
            Name = name;
            Material = material;
            Texture = texture;
        }

        public void Dispose() {
            Texture?.Dispose();
        }
    }
}
