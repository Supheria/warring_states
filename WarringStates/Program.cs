#define gen

using AtlasGenerator;
using AtlasGenerator.Layout;
using LocalUtilities.SimpleScript.Serialization;

namespace WarringStates
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            var data = new AtlasData("testmap", new(300, 300), new(2, 2), new(6, 6), RiverLayout.Type.ForwardSlash, 2.25, 55000, 0.66f);
            //var data = new AtlasData("testmap", new(1000, 1000), new(8, 8), new(8, 8), RiverLayout.Type.ForwardSlash, 5, 650000, 0.66f);
#if gen

            var atlas = new Atlas(data);
            atlas.SaveToSimpleScript(false, "shit_1000.ss");
#else
            var atlas = new Atlas("testmap").LoadFromSimpleScript("shit_1000.ss");
            atlas.SaveToSimpleScript(false, "shit_1000.ss");
#endif
            atlas.SetTerrainMap();

            Application.Run(new GameForm());
        }
    }
}
