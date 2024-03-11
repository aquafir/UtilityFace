using UtilityFace.Lib;

namespace UtilityFace;

//Global stuff placeholder?
public static class G
{
    public readonly static Game Game = new();

    public readonly static ActionOptions Fail = new()
    {
        MaxRetryCount = 0,
        TimeoutMilliseconds = 100,
    };

    public readonly static Picker Picker = new();
    public readonly static CameraH CameraH = new ();

}


