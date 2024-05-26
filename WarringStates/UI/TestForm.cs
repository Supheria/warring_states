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
            InfoMap[info.Name] = info.Info;
            UpdateInfo();
        });
        LocalEvents.Hub.AddListener<List<TestInfo>>(LocalEvents.Test.AddInfoList, infoList =>
        {
            //infoList.ForEach(info => InfoMap.Add(new(info.Name, info.Info)));
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

    Dictionary<string, string> InfoMap { get; } = [];

    private void UpdateInfo()
    {
        Text.Text = new StringBuilder().AppendJoin('\0', InfoMap.ToList(), (sb, s) =>
        {
            sb.Append(s.Key)
            .Append(": ")
            .Append(s.Value)
            .Append('\n');
        }).ToString();
    }
}
