using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.EventProcess;
using System.Text;
using LocalUtilities.TypeToolKit.Text;

namespace WarringStates;

internal class TestForm : ResizeableForm, IEventListener
{
    public TestForm()
    {
        FormClosing += OnFormClosing;
        EventManager.Instance.AddEvent(LocalEventId.TestInfo, this);
    }

    private void OnFormClosing(object? sender, FormClosingEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }

    public override string LocalName { get; set; } = nameof(TestForm);

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

    public class TestInfo(string name, string info) : IEventArgument
    {
        public string Name { get; } = name;

        public string Info { get; } = info;
    }

    Dictionary<string, string> InfoMap { get; } = [];

    public void HandleEvent(int eventId, IEventArgument argument)
    {
        if (eventId == LocalEventId.TestInfo)
        {
            if (argument is not TestInfo info)
                return;
            InfoMap[info.Name] = info.Info;
            UpdateInfo();
        }
    }

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
