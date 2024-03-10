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
}


