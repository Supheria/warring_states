﻿using LocalUtilities.TypeGeneral;
using WarringStates.Flow;
using WarringStates.Server.Net;
using WarringStates.Server.UI.Component;

namespace WarringStates.Server.UI;

internal partial class ServerForm : ResizeableForm
{
    public override string InitializeName => nameof(ServerForm);

    NumericUpDown Port { get; } = new()
    {
        Value = LocalNet.Port,
    };

    Label ParallelCount { get; } = new()
    {
        TextAlign = ContentAlignment.MiddleCenter,
        Text = "0",
    };

    RichTextBox MessageBox { get; } = new();

    TextBox SendBox { get; } = new()
    {
        Multiline = true,
    };

    Button SendButton { get; } = new()
    {
        Text = "Send",
    };

    ArchiveSelector ArchiveSelector { get; } = new();

    SpanFlow SpanFlow { get; set; } = new();

    AnimateFlow AnimateFlow { get; set; } = new();

    public ServerForm()
    {
        Text = "Server";
        MinimumSize = new(800, 550);
        Controls.AddRange([
            Port,
            ParallelCount,
            MessageBox,
            SendBox,
            SendButton,
            ArchiveSelector
            ]);
        ArchiveSelector.EnableListener();
    }

    protected override void Redraw()
    {
        base.Redraw();
        var width = (ClientWidth - Padding * 5) / 2;
        var top = ClientTop + Padding;
        var height = Port.Height;
        //
        Port.Bounds = new(
            ClientLeft + Padding,
            top,
            width,
            height
            );
        ParallelCount.Bounds = new(
            Port.Right + Padding,
            top,
            width,
            height
            );
        //
        width = (ClientWidth - Padding * 3) / 5;
        top = Port.Bottom + Padding;
        height = ClientHeight - Port.Height - SendButton.Height - Padding * 4;
        //
        ArchiveSelector.Bounds = new(
            ClientLeft + Padding,
            top,
            width * 3,
            height
            );
        //
        MessageBox.Bounds = new(
            ArchiveSelector.Right + Padding,
            top,
            width * 2,
            height
            );
        //
        top = MessageBox.Bottom + Padding;
        height = SendBox.Height;
        //
        SendBox.Bounds = new(
            ClientLeft + Padding,
            top,
            width * 2 + Padding,
            height
            );
        //
        SendButton.Bounds = new(
            SendBox.Right + Padding,
            top,
            width,
            height
            );
    }
}
