using Microsoft.Extensions.Logging;
using UtilityBelt.Service.Lib;
using UtilityBelt.Service.Lib.Settings;
using UtilityBelt.Service.Views.SettingsEditor;

namespace UtilityFace.Settings;
//Refer to UBService
public sealed class SettingsRoot
{
    //[Summary("Show landblock boundaries")]
    //public static Global<bool> ShowLandblockBoundaries = new(false);

    public class ServiceSettings : ISetting
    {
        [Summary("Test")]
        public Setting<bool> Test = new Global<bool>(true);
    }
    public static ServiceSettings Service = new ServiceSettings();



    //public static TestSettings Test;
}

public class TestSettings : ISetting, IDisposable
{
    //[Summary("Show landblock boundaries")]
    //public Global<bool> ShowLandblockBoundaries = new(false);

    //[Summary("Landblock boundary distance to draw to, setting to 0 only draws current landblock boundaries")]
    //[MinMax(0, 5)]
    //public Global<int> LandblockBoundaryDrawDistance = new(0);

    [Summary("Host to connect to. If the client starts its own server, it will also use this as the listening host.")]
    public Setting<string> Host = new Global<string>("127.0.0.1");

    [Summary("Port to connect to. If the client starts its own server, it will also use this as the listening port.")]
    public Setting<int> Port = new Global<int>(21424);

    [Summary("Attempt to start a local UBNet server if none exists")]
    public Setting<bool> StartServer = new Global<bool>(initialValue: true);

    [Summary("Server idle timeout, in seconds. The server will shut itself down after this many seconds without any connected clients. This is only applicable if this client starts the server.")]
    public Setting<int> Timeout = new Global<int>(5);

    [Summary("Log Level")]
    public Setting<LogLevel> LogLevel = new Global<LogLevel>(Microsoft.Extensions.Logging.LogLevel.Error);

    [Summary("Print Level (This is the level of logging to print to chat)")]
    public Setting<LogLevel> PrintLevel = new Global<LogLevel>(Microsoft.Extensions.Logging.LogLevel.Information);


    public void Dispose()
    {
        //throw new NotImplementedException();
    }

    //public void F()
    //{
    //    var _settingsUI = new SettingsEditor("My Settings", new List<object>() { typeof(MySettingsContainerClass)
    //    //});
    //    // dispose _settingsUI later...
    //}
}