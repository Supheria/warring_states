using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Text;
using System.Text;
using WarringStates.Events;
using WarringStates.Server.Events;

namespace WarringStates.Server.UI;

internal class TestForm : ResizeableForm
{
    public TestForm()
    {
        Controls.AddRange([
            Text
            ]);
        FormClosing += OnFormClosing;
        LocalEvents.TryAddListener<StringInfo>(LocalEvents.Test.AddInfo, info =>
        {
            InfoList.Add(new(info.Name, info.Info));
            UpdateInfo();
        });
        //LocalEvents.AddListener<List<StringInfo>>(LocalEvents.Test.AddInfoList, infoList =>
        //{
        //    infoList.ForEach(info => InfoList.Add(new(info.Name, info.Info)));
        //    UpdateInfo();
        //});
        LocalEvents.TryAddListener<StringInfo>(LocalEvents.Test.AddSingleInfo, info =>
        {
            InfoMap[info.Name] = info.Info;
            UpdateInfo();
        });
        //LocalEvents.TryAddListener<List<StringInfo>>(LocalEvents.Test.AddInfoList, infoList =>
        //{
        //    infoList.ForEach(info => InfoMap[info.Name] = info.Info);
        //    UpdateInfo();
        //});
        LocalEvents.TryAddListener<ValueInfo>(LocalEvents.Test.ValueForMax, info =>
        {
            if (ValueMap.TryGetValue(info.Name, out var value))
            {
                ValueMap[info.Name] = Math.Max(value, info.Value);
            }
            else
                ValueMap[info.Name] = info.Value;
            UpdateInfo();
        });
        LocalEvents.TryAddListener<ValueInfo>(LocalEvents.Test.AddValue, info =>
        {
            if (ValueMap.TryGetValue(info.Name, out var value))
            {
                ValueMap[info.Name] += value;
            }
            else
                ValueMap[info.Name] = value;
            UpdateInfo();
        });
        //LocalEvents.TryAddListener<string>(LocalEvents.Test.ClearValue, name =>
        //{
        //    ValueMap[name] = 0;
        //    UpdateInfo();
        //});
    }

    private void OnFormClosing(object? sender, FormClosingEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }

    public override string InitializeName => nameof(TestForm);

    new RichTextBox Text { get; } = new()
    {
        Dock = DockStyle.Fill,
    };

    public class StringInfo(string name, string info) : EventArgs
    {
        public string Name { get; } = name;

        public string Info { get; } = info;
    }

    public class ValueInfo(string name, int value) : EventArgs
    {
        public string Name { get; } = name;

        public int Value { get; } = value;
    }

    List<KeyValuePair<string, string>> InfoList { get; } = [];

    Dictionary<string, string> InfoMap { get; } = [];

    Dictionary<string, int> ValueMap { get; } = [];

    private void UpdateInfo()
    {
        var list = new List<KeyValuePair<string, string>>();
        list.AddRange(InfoMap);
        list.AddRange(InfoList);
        list.AddRange(ValueMap.Select(v => new KeyValuePair<string, string>(v.Key, v.Value.ToString())));
        Text.Text = new StringBuilder().AppendJoin('\0', list, (sb, s) =>
        {
            sb.Append(s.Key)
            .Append(": ")
            .Append(s.Value)
            .Append('\n');
        }).ToString();
    }
}
