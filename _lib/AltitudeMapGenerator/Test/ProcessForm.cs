using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Mathematic;
using System.ComponentModel;
using System.Net.Sockets;
using System.Windows.Forms;

namespace AltitudeMapGenerator.Test;

public partial class ProcessForm : ResizeableForm
{

    public float Total { get; set; } = 0;

    int Now { get; set; } = 0;

    Label Label { get; } = new()
    {
        Dock = DockStyle.Fill,
        Text = "aa"
    };

    public override string InitializeName => nameof(ProcessForm);

    public ProcessForm()
    {
        Controls.Add(Label);
    }

    public void Progress()
    {
        Invoke(() =>
        {
            Label.Text = Math.Round(++Now / Total * 100, 2).ToString();
            Update();
        });
    }

    public void Reset(int total)
    {
        Total = total;
        Now = 0;
    }
}
