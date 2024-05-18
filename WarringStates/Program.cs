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

            //var data = new AtlasData(nameof(Atlas), new(300, 300), new(2, 2), new(6, 6), RiverLayout.Type.BackwardSlash, 2.25, 45000, 0.66f, new RandomPointsGenerationGaussian());
            //Terrain.Atlas = new(data);
            //Terrain.Atlas.SaveToSimpleScript(false);

            //ApplicationConfiguration.Initialize();
            Application.Run(new GameForm());
        }
    }
}