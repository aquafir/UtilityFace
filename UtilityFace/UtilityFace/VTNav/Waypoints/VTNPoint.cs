using System.Drawing;
using UtilityBelt.Service.Lib.ACClientModule;

namespace UtilityBelt.Lib.VTNav.Waypoints
{
    public class VTNPoint
    {
        protected static ACDecalD3D ac = new();
        protected static Game game = new Game();
        protected static uint color = (uint)Color.Green.ToArgb();

        public float NS = 0.0f;
        public float EW = 0.0f;
        public float Z = 0.0f;
        public List<DecalD3DObj> shapes = new();


        public VTNPoint Previous = null;

        internal StreamReader sr;
        internal int index = 0;
        internal VTNavRoute route;
        internal bool disposed = false;

        internal eWaypointType Type = eWaypointType.Point;

        public VTNPoint(StreamReader reader, VTNavRoute parentRoute, int index)
        {
            
            sr = reader;
            route = parentRoute;
            this.index = index;
        }

        public VTNPoint GetPreviousPoint()
        {
            if (index == 0 && route.NavType == eNavType.Once && route.NavOffset > 0)
            {
                return route.Points[route.NavOffset - 1];
            }

            for (var i = index - 1; i >= 0; i--)
            {
                var t = route.Points[i].Type;
                if (t == eWaypointType.Point)
                {
                    return route.Points[i];
                }
                else if (t == eWaypointType.OpenVendor)
                {
                    var ov = (VTNOpenVendor)route.Points[i];
                    if (ov.closestDistance < Double.MaxValue)
                    {
                        return route.Points[i];
                    }
                }
                else if (t == eWaypointType.UseNPC)
                {
                    var ov = (VTNUseNPC)route.Points[i];
                    if (ov.closestDistance < Double.MaxValue)
                    {
                        return route.Points[i];
                    }
                }
                else if (t == eWaypointType.Portal || t == eWaypointType.Portal2)
                {
                    break;
                }
            }

            return null;
        }

        public VTNPoint GetNextPoint()
        {
            for (var i = index + 1; i < route.Points.Count - 1; i++)
            {
                if (route.Points[i].Type == eWaypointType.Point) return route.Points[i];
            }

            return null;
        }

        public bool Parse()
        {
            if (!float.TryParse(sr.ReadLine(), out EW)) return false;
            if (!float.TryParse(sr.ReadLine(), out NS)) return false;
            if (!float.TryParse(sr.ReadLine(), out Z)) return false;

            // i dont know what this value is, always 0?
            sr.ReadLine();

            return true;
        }

        internal virtual void Write(StreamWriter file)
        {
            file.WriteLine((int)Type);
            file.WriteLine(EW);
            file.WriteLine(NS);
            file.WriteLine(Z);
            file.WriteLine(0);
        }

        public double DistanceTo(VTNPoint rp)
        {
            return Math.Abs(Math.Sqrt(Math.Pow(rp.NS - NS, 2) + Math.Pow(rp.EW - EW, 2) + Math.Pow(rp.Z - Z, 2))) * 240;
        }

        //public void DrawLineTo(VTNPoint Previous, Color color) {
        public void DrawLineTo()
        {
            if (Previous == null) return;

            var d = DistanceTo(Previous);
            Log.Chat($"Distance: {d}");
            if (d <= 0) return;

            var obj = ac.NewD3DObj();
            obj.Visible = false;
            obj.Color = color;
            obj.SetShape(DecalD3DShape.Cube);
            obj.Anchor((Previous.EW + EW) / 2, (Previous.NS + NS) / 2, (Previous.Z + Z) * 120 + 0.05f);
            obj.OrientToCoords(EW, NS, Z * 240 + 0.05f, true);
            obj.ScaleX = 0.25f;
            obj.ScaleZ = 0.25f;
            obj.ScaleY = (float)DistanceTo(Previous);
            obj.Visible = true;

            shapes.Add(obj);
        }

        public void DrawText(string text, VTNPoint rp, float height, Color color)
        {
            //if (rp == null) return;

            //var textObj = CoreManager.Current.D3DService.MarkCoordsWith3DText(0, 0, 0, text, "arial", 0);
            //textObj.Visible = false;
            //textObj.Color = color.ToArgb();

            //textObj.Anchor((float)rp.NS, (float)rp.EW, (float)(rp.Z * 240) + height + (float)route.GetZOffset(rp.NS, rp.EW));
            //route.AddOffset(rp.NS, rp.EW, 0.25f);

            //textObj.Scale(0.2f);
            //textObj.OrientToCamera(true);
            //textObj.Visible = true;

            //shapes.Add(textObj);
        }

        public void DrawIcon(int iconId, float height, VTNPoint point)
        {
            //if (point == null) return;

            //var icon = CoreManager.Current.D3DService.MarkCoordsWithIcon((float)point.NS, (float)point.EW, (float)(point.Z * 240) + height + (float)route.GetZOffset(point.NS, point.EW), iconId);
            //route.AddOffset(point.NS, point.EW, 0.5);

            //icon.OrientToCamera(true);
            //icon.Visible = true;
            //icon.Scale(0.5f);

            //shapes.Add(icon);
        }

        public virtual void Draw()
        {
            // we dont want to draw lines to the previous point if it was a recall or portal or jump
            if (Previous == null || Previous.Type == eWaypointType.Recall || Previous.Type == eWaypointType.Portal || Previous.Type == eWaypointType.Portal2 || Previous.Type == eWaypointType.Jump)
            {
                Log.Chat("Skipped");
                return;
            }

            var obj = ac.NewD3DObj();
            obj.Visible = false;
            obj.Color = 0xAA55ff55;
            obj.SetShape(DecalD3DShape.Cube);
            obj.Anchor(NS, EW, Z*240+.05f);//(prevPoint.EastWest + EastWest) / 2, (prevPoint.NorthSouth + NorthSouth) / 2, (prevPoint.Z + Z) * 120 + 0.05f);
            obj.ScaleX = 1.25f;
            obj.ScaleZ = 1.25f;
            obj.ScaleY = 1.25f;
            obj.Visible = true;
            //return obj;
            shapes.Add(obj);

            DrawLineTo();
            //DrawLineTo(Previous, color);
            //lineMarker = new LineMarker(EW, NS, Previous.EW, Previous.NS, color, 2)
            //{
            //    MinZoomLevel = 0,
            //    MaxZoomLevel = 1
            //};
            //UtilityBeltPlugin.Instance.LandscapeMaps.AddMarker(lineMarker);
        }

        internal void ClearShapes()
        {
            foreach (var shape in shapes)
            {
                try { shape.Visible = false; } catch { }
                shape.Dispose();
            }

            shapes.Clear();

            //if (lineMarker != null)
            //    lineMarker.Dispose();
            //lineMarker = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    ClearShapes();
                }
                disposed = true;
            }
        }





        public DecalD3DObj Mark(VTNPoint prevPoint)
        {
            var obj = ac.NewD3DObj();
            obj.Visible = false;
            obj.Color = 0xAA55ff55;
            obj.SetShape(DecalD3DShape.Cube);
            obj.Anchor((prevPoint.NS + NS) / 2, (prevPoint.EW + EW) / 2, (prevPoint.Z + Z) * 120 + 0.05f);
            obj.OrientToCoords(NS, EW, Z * 240 + 0.05f, true);
            obj.ScaleX = 0.25f;
            obj.ScaleZ = 0.25f;
            obj.ScaleY = (float)DistanceTo(prevPoint);
            obj.Visible = true; 
            return obj;
        }
    }
}
