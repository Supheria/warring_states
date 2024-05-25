namespace LocalUtilities.TypeToolKit.EventProcess;

public static class LocalEvents
{
    public static EventHub Hub { get; } = new();
    public static class Types
    {
        public enum Hub
        {
            TestInfo,
            GameFormUpdate,
            ImageUpdate,
            GridUpdate
        }
    }
}
