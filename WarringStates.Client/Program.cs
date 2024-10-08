using WarringStates.Client.UI;

namespace WarringStates.Client
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            //new ClientForm("client 1").Show();
            new TestForm() { TopMost = true }.Show();
            Application.Run(new ClientForm("client 2"));
        }
    }
}