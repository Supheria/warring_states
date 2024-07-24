namespace WarringStates.Server.Net;

internal class LocalNet
{
    public static ServiceManager Server { get; } = new();

    public static int Port { get; set; } = 60;

    public static void Start()
    {
        Server.Start(Port);
    }

    public static void Close()
    {
        Server.Close();
    }
}
