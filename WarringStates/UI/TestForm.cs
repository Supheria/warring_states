using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Text;
using System.Text;
using WarringStates.Events;

namespace WarringStates.UI;

internal class TestForm : ResizeableForm
{
    public TestForm()
    {
        FormClosing += OnFormClosing;
        LocalEvents.Hub.AddListener<TestInfo>(LocalEvents.Test.AddInfo, info =>
        {
            InfoList.Add(new(info.Name, info.Info));
            UpdateInfo();
        });
        LocalEvents.Hub.AddListener<List<TestInfo>>(LocalEvents.Test.AddInfoList, infoList =>
        {
            infoList.ForEach(info => InfoList.Add(new(info.Name, info.Info)));
            UpdateInfo();
        });
        LocalEvents.Hub.AddListener<TestInfo>(LocalEvents.Test.AddSingleInfo, info =>
        {
            InfoMap[info.Name] = info.Info;
            UpdateInfo();
        });
        LocalEvents.Hub.AddListener<List<TestInfo>>(LocalEvents.Test.AddInfoList, infoList =>
        {
            infoList.ForEach(info => InfoMap[info.Name] = info.Info);
            UpdateInfo();
        });
        LocalEvents.Hub.AddListener<TestInfo>(LocalEvents.Test.AddInfoForMax, info =>
        {
            if (InfoMap.TryGetValue(info.Name, out var str))
            {
                var value = int.Parse(str);
                InfoMap[info.Name] = Math.Max(value, int.Parse(info.Info)).ToString();
            }
            else
                InfoMap[info.Name] = info.Info;
            UpdateInfo();
        });
    }

    private void OnFormClosing(object? sender, FormClosingEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }

    public override string LocalName => nameof(TestForm);

    new RichTextBox Text { get; } = new()
    {
        Dock = DockStyle.Fill,
    };

    protected override void InitializeComponent()
    {
        Controls.AddRange([
            Text
            ]);
    }

    public class TestInfo(string name, string info)
    {
        public string Name { get; } = name;

        public string Info { get; } = info;
    }

    List<KeyValuePair<string, string>> InfoList { get; } = [];

    Dictionary<string, string> InfoMap { get; } = [];

    private void UpdateInfo()
    {
        var list = new List<KeyValuePair<string, string>>();
        list.AddRange(InfoMap);
        list.AddRange(InfoList);
        Text.Text = new StringBuilder().AppendJoin('\0', list, (sb, s) =>
        {
            sb.Append(s.Key)
            .Append(": ")
            .Append(s.Value)
            .Append('\n');
        }).ToString();
    }
}
