namespace UtilityFace.Helpers;
public static class WorldObjectExtensions
{
    /// <summary>
    /// Eventually will not be needed but currently shorthand for an optimistic Use that fails immediately
    /// </summary>
    /// <param name="obj"></param>
    public static void TryUse(this WorldObject obj) => obj.Use(new ActionOptions() { MaxRetryCount = 0, TimeoutMilliseconds = 100 });
}
