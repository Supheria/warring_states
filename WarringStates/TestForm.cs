﻿using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.EventProcess;
using LocalUtilities.TypeToolKit.Text;
using System.Text;

namespace WarringStates;

internal class TestForm : ResizeableForm
{
    public TestForm()
    {
        FormClosing += OnFormClosing;
        LocalEvents.Hub.AddListener<TestInfo>(LocalEventNames.TestInfo, info =>
        {
            InfoMap[info.Name] = info.Info;
            UpdateInfo();
        });
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