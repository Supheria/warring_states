using AtlasGenerator;
using AtlasGenerator.Layout;
using LocalUtilities.SimpleScript.Serialization;

namespace WarringStates
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

            //var data = new AtlasData("testmap", new(300, 300), new(2, 2), new(3, 6), RiverLayout.Type.ForwardSlash, 2.25, 55000, 0.66f);
            //var atlas = new Atlas(data);
            //atlas.SaveToSimpleScript(true, "shit.ss");
            var atlas = new Atlas("testmap").LoadFromSimpleScript("shit.ss");
            atlas.SaveToSimpleScript(false, "shit.ss");
            atlas.SetTerrainMap();

            //ApplicationConfiguration.Initialize();
            Application.Run(new GameForm());
        }
    }
}