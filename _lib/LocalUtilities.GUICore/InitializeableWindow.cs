using Avalonia.Controls;
using Avalonia.Interactivity;
using LocalUtilities.FileHelper;
using LocalUtilities.SimpleScript;
using System;

namespace LocalUtilities.GUICore;

public abstract class InitializeableWindow : Window, IInitializeable
{
    public abstract string InitializeName { get; }

    public string IniFileExtension { get; } = "ss";

    protected static SsSignTable SignTable { get; } = new();

    protected virtual Type WindowDataType => typeof(WindowData);

    protected class WindowData
    {
        public virtual double MinWidth { get; set; }

        public virtual double MinHeight { get; set; }

        public virtual double MaxWidth { get; set; }

        public virtual double MaxHeight { get; set; }

        public virtual double Width { get; set; }

        public virtual double Height { get; set; }

        public virtual int Left { get; set; }

        public virtual int Top { get; set; }

        public virtual WindowState WindowState { get; set; }
    }

    protected sealed override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        try
        {
            var data = SerializeTool.DeserializeFile(WindowDataType, new(), SignTable, this.GetInitializeFilePath());
            OnLoad(ref data);
            if (data is not WindowData windowData)
                return;
            MinWidth = windowData.MinWidth;
            MinHeight = windowData.MinHeight;
            MaxWidth = windowData.MaxWidth;
            MaxHeight = windowData.MaxHeight;
            Bounds = new(0, 0, windowData.Width, windowData.Height);
            Position = new(windowData.Left, windowData.Top);
        }
        catch { }
    }

    protected virtual void OnLoad(ref object? windowData)
    {

    }

    protected sealed override void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);
        try
        {
            var windowData = OnSave();
            windowData.MinWidth = MinWidth;
            windowData.MinHeight = MinHeight;
            windowData.MaxWidth = MaxWidth;
            windowData.MaxHeight = MaxHeight;
            windowData.Width = Bounds.Width;
            windowData.Height = Bounds.Height;
            windowData.Left = Position.X;
            windowData.Top = Position.Y;
            SerializeTool.SerializeFile(windowData, new(), SignTable, true, this.GetInitializeFilePath());
        }
        catch { }
    }

    protected virtual WindowData OnSave()
    {
        return new();
    }
}