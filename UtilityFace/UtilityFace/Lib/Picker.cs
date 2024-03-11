using AcClient;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityBelt.Scripting.Interop;
using UtilityBelt.Service;
using UtilityBelt.Service.Lib.ACClientModule;

namespace UtilityFace.Lib {
    public unsafe class Picker : IGamePicking {
        /// <inheritdoc/>
        public override Coordinates? PickTerrain() {
            return PickTerrain(*Render.selection_x, *Render.selection_y);
        }

        /// <inheritdoc/>
        public override Coordinates? PickTerrain(int screenX, int screenY) {
            if (CPlayerSystem.s_pPlayerSystem[0]->IsOutside() == 1) {
                return PickLandscape(screenX, screenY);
            }
            else {
                return PickEnvironment(screenX, screenY);
            }
        }

        public uint grab_visible_cells<UInt16>(ref LScape This) => ((delegate* unmanaged[Thiscall]<ref LScape, uint>)0x00505920)(ref This);
        //.text:00505920 ; unsigned int __thiscall LScape::grab_visible_cells(LScape *this)

        private float _o = 1;
        private DateTime _t = DateTime.UtcNow;
        private Coordinates PickEnvironment(int screenX, int screenY) {
            /*
            var ui = ClientUISystem.GetUISystem();
            var cs = ui->AccessCameraSet();
            var cm = cs->cm;
            var smartbox = cs->sbox;
            var lscape = smartbox->lscape;
            var color = new _D3DCOLORVALUE() {
                a = 0.3f,
                b = 1,
                g = 0,
                r = 0
            };
            for (var i = 0; i < smartbox->viewer_cell->num_shadow_objects; i++) {
                try {
                    //smartbox->viewer_cell->cPartCell.shadow_part_list.data[i]->part->material->d3d_material.Diffuse = color;
                    var p = smartbox->viewer_cell->shadow_object_list.data[i]->cell;
                    _o += (float)(DateTime.UtcNow - _t).TotalSeconds / 10000f;
                    p->pos.frame.set_heading(_o);
                    if (_o > 360) _o = 0;
                    ImGui.Text($"{p->pos.frame.get_heading()}");
                }
                catch { }
            }
            ImGui.Text($"Cells: {smartbox->viewer_cell->num_shadow_objects}");
            */
            return null;
        }

        private Coordinates? PickLandscape(int screenX, int screenY) {
            Coordinates? bestHit = null;
            try {
                var cf = SmartBox.smartbox[0]->viewer.frame.m_fOrigin;
                var rayOrigin = new System.Numerics.Vector3(cf.x, cf.y, cf.z);

                var pickRay = new AC1Legacy.Vector3();
                Render.pick_ray(&pickRay, screenX, screenY);
                var rayDirection = new System.Numerics.Vector3(pickRay.a0.x, pickRay.a0.y, pickRay.a0.z);

                var ui = ClientUISystem.GetUISystem();
                var cs = ui->AccessCameraSet();
                var cm = cs->cm;
                var smartbox = cs->sbox;
                var lscape = smartbox->lscape;

                var cLbX = (lscape->loaded_cell_id >> 24) & 0xFF;
                var cLbY = (lscape->loaded_cell_id >> 16) & 0xFF;

                var camera = new Coordinates(SmartBox.smartbox[0]->viewer.objcell_id, cf.x, cf.y, cf.z);
                var camLbX = (camera.LandCell >> 24) & 0xFF;
                var camLbY = (camera.LandCell >> 16) & 0xFF;

                for (var lbIdxX = 0; lbIdxX < lscape->mid_width; ++lbIdxX) {
                    for (var lbIdxY = 0; lbIdxY < lscape->mid_width; ++lbIdxY) {
                        var lb = lscape->land_blocks[lbIdxX + lbIdxY * lscape->mid_width];
                        var lbX = (lb->block_coord.x / 8);
                        var lbY = (lb->block_coord.y / 8);
                        var lbid = (uint)((lbX << 24) + (lbY << 16));
                        var visible = lb->in_view != BoundingType.OUTSIDE;
                        if (visible) {
                            for (int cellIdxX = 0; cellIdxX < lb->cLandBlockStruct.side_cell_count; cellIdxX++) {
                                for (int cellIdxY = 0; cellIdxY < lb->cLandBlockStruct.side_cell_count; cellIdxY++) {
                                    var idx = ((cellIdxY + cellIdxX * lb->cLandBlockStruct.side_cell_count));
                                    var idx2 = (2 * (cellIdxY + cellIdxX * lb->cLandBlockStruct.side_polygon_count));
                                    var cell = lb->cLandBlockStruct.lcell[idx];

                                    var m = lb->cLandBlockStruct.side_cell_count - 1;
                                    var cellIsVisible = cell.in_view != BoundingType.OUTSIDE;

                                    if (cellIsVisible) {
                                        for (var polyIdx = 0; polyIdx < 2; polyIdx++) {
                                            var _ox = (lbX - camLbX) * 192f;
                                            var _oy = (lbY - camLbY) * 192f;
                                            var poly = cell.polygons[polyIdx];

                                            var vert0 = new System.Numerics.Vector3(poly->vertices[0]->x + _ox, poly->vertices[0]->y + _oy, poly->vertices[0]->z);
                                            var vert1 = new System.Numerics.Vector3(poly->vertices[1]->x + _ox, poly->vertices[1]->y + _oy, poly->vertices[1]->z);
                                            var vert2 = new System.Numerics.Vector3(poly->vertices[2]->x + _ox, poly->vertices[2]->y + _oy, poly->vertices[2]->z);

                                            var hit = Collision.GetTimeAndUvCoord(rayOrigin, rayDirection, vert0, vert1, vert2);

                                            if (hit is not null) {
                                                var hitLoc = Collision.GetTrilinearCoordinateOfTheHit(hit.Value.X, rayOrigin, rayDirection);
                                                var newHit = new Coordinates(lbid, hitLoc.X - _ox, hitLoc.Y - _oy, hitLoc.Z);
                                                if (bestHit == null || newHit.DistanceTo(camera) < bestHit.DistanceTo(camera)) {
                                                    bestHit = newHit;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { UBService.LogException(ex); }

            return bestHit;
        }

        public override WorldObject? PickWorldObject() {
            try {
                var hoveredId = AcClient.SmartBox.get_found_object_id();
                if (G.Game?.World?.TryGet(hoveredId, out var wo) == true) {
                    return wo;
                }
            }
            catch (Exception ex) { UBService.LogException(ex); }

            return null;
        }
    }
}
