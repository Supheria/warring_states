using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DlaTest;

public partial class TestForm : Form
{
    public float Total { get; set; } = 0;

    public float Now { get; set; } = 0;

    public TestForm()
    {
        InitializeComponent();
    }

    public void Progress()
    {
        var percent = Now / Total * 100;
        this.Text = Math.Round(percent, 2).ToString();
        Invalidate();
    }
}
