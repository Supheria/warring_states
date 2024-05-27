//#define gen

using AtlasGenerator;
using AtlasGenerator.Layout;
using LocalUtilities.SimpleScript.Serialization;
using WarringStates.Map;
using WarringStates.UI;

namespace WarringStates
{
    internal static class Program
    {
        //[STAThread]
        static void Main()
        {
            //var data = new AtlasData(new(300, 300), new(2, 2), new(6, 6), RiverLayout.Type.ForwardSlash, 2.25, 55000, 0.66f);
            var data = new AtlasData(new(100, 100), new(2, 2), new(2, 2), RiverLayout.Type.OneForTest, 2, 5000, 0.66f);
            //var data = new AtlasData(new(500, 300), new(5, 3), new(6, 3), RiverLayout.Type.Horizontal, 2.25, 100000, 0.66f);
            //var data = new AtlasData(new(1000, 1000), new(8, 8), new(8, 8), RiverLayout.Type.ForwardSlash, 7, 650000, 0.75f);
            //var file = "shit_1000.ss";
            //var file = "shit_500_300.ss";
            //var file = "shit_300.ss";
            var file = "shit_50.ss";
#if gen

            var atlas = new Atlas(data);
            atlas.SaveToSimpleScript(false, file);
#else
            var atlas = new Atlas().LoadFromSimpleScript(file);
            atlas.SaveToSimpleScript(false, file);
#endif
            atlas.SetTerrainMap();

            //new TestForm().Show();
            new TestForm() { TopMost = true }.Show();
            Application.Run(new GameForm());
        }
    }
}
