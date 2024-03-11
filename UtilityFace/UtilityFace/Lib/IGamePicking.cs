using AcClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityBelt.Scripting.Interop;
using UtilityBelt.Service.Lib.ACClientModule;

namespace UtilityFace.Lib {
    /// <summary>
    /// Interface for 3D picking / object intersection
    /// </summary>
    public abstract class IGamePicking {
        /// <summary>
        /// Find a world object under the mouse
        /// </summary>
        /// <returns>The WorldObject under the mouse, if any</returns>
        public abstract WorldObject? PickWorldObject();

        /// <summary>
        /// Find the terrain coordinates at the current cursor position.
        /// </summary>
        /// <returns>A walkable Position if one was found</returns>
        public abstract Coordinates? PickTerrain();

        /// <summary>
        /// Find the terrain coordinates at the specified window position.
        /// 
        /// The top-left corner is at (0,0), and coordinates increase by moving down and to the right.
        /// The bottom-right corner is at (WINDOW_WIDTH, WINDOW_HEIGHT).
        /// 
        /// This position is not guaranteed to actually be walkable because it just checks for a poly
        /// that has a walkable slope, but does not check that your character will actually fit there
        /// with its physics model. It checks against environment / terrain / static physics polys.
        /// This will ignore dynamic world object spawns.
        /// </summary>
        /// <param name="x">X window coordinate.</param>
        /// <param name="y">Y window coordinate.</param>
        /// <returns>A walkable Position if one was found</returns>
        public abstract Coordinates? PickTerrain(int x, int y);
    }
}
