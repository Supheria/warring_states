using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Client.User;

namespace WarringStates.Client.Net;

public class LocalNet
{
    public static ClientService Service { get; } = new();

    public static string ServerAddress { get; set; } = "127.0.0.1";

    public static int ServerPort { get; set; } = 60;

    public static string PlayerName { get; set; } = "admin";

    public static string PlayerPassword { get; set; } = "password";

    public static void Login()
    {
        Service.Login(ServerAddress, ServerPort, PlayerName, PlayerPassword);
    }

    public static void Logout()
    {
        Service.Dispose();
    }
}
