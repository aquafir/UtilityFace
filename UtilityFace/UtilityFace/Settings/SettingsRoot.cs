using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityBelt.Service.Lib.Settings;

namespace UtilityFace.Settings;
public static class SettingsRoot
{
    [Summary("Show landblock boundaries")]
    public static Global<bool> ShowLandblockBoundaries = new(false);

    public static TestSettings Foo = new();

}

public class TestSettings : ISetting
{
    [Summary("Show landblock boundaries")]
    public Global<bool> ShowLandblockBoundaries = new(false);

    [Summary("Landblock boundary distance to draw to, setting to 0 only draws current landblock boundaries")]
    [MinMax(0, 5)]
    public Global<int> LandblockBoundaryDrawDistance = new(0);
}