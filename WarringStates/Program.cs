//#define gen

using AltitudeMapGenerator;
using AltitudeMapGenerator.Layout;
using WarringStates.Map;
using WarringStates.Terrain;
using WarringStates.UI;
using WarringStates.User;

namespace WarringStates;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        new ClientForm().Show();
        Application.Run(new ServerForm());
        //new TestCGraphics().ShowDialog();
        //var data = new AltitudeMapData(new(300, 300), new(2, 2), new(6, 6), RiverLayout.Type.ForwardSlash, 2.25, 55000, 0.66f);
        var data = new AltitudeMapData(new(100, 100), new(2, 2), new(2, 2), RiverLayout.Types.OneForTest, 2, 5000, 0.66f);
        //var data = new AltitudeMapData(new(500, 300), new(5, 3), new(6, 3), RiverLayout.Type.Horizontal, 2.25, 100000, 0.66f);
        //var data = new AltitudeMapData(new(1000, 1000), new(8, 8), new(8, 8), RiverLayout.Type.ForwardSlash, 7, 650000, 0.75f);
        //var file = "shit_1000.ss";
        var file = "shit_500_300.ss";
        //var file = "shit_300.ss";
        //var file = "shit_50.ss";
#if gen

//            var atlas = new AltitudeMap(data);
//            atlas.SaveToSimpleScript(false, file);
//#else
//            var atlas = new AltitudeMap().LoadFromSimpleScript(file);
//            atlas.SaveToSimpleScript(false, file);
#endif
        //atlas.Relocate([]);
        //var suc1 = SourceLand.TryBuild(new(-1, -1), SourceLand.Types.FarmLand, out var land1);
        //var suc2 = SourceLand.TryBuild(new(12, 13), SourceLand.Types.WoodLand, out var land2);
        //var suc3 = SourceLand.TryBuild(new(47, 70), SourceLand.Types.FishLand, out var land3);
        //atlas.Relocate([land1, land2, land3]);
        //var suc1 = SourceLand.TryBuild(new(-1, -1), SourceLand.Types.FarmLand, out var land1);
        //var suc2 = SourceLand.TryBuild(new(10, 264), SourceLand.Types.FishLand, out var land2);
        //if (LocalSaves.TryGetArchive(0, out var r))
        //Atlas.Relocate(r);

        //new TestForm().Show();
        new TestForm() { TopMost = true }.Show();
        //Application.Run(new InitializeForm());
        Application.Run(new MainForm());
    }
}
