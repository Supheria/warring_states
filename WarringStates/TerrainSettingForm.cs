using LocalUtilities.TypeGeneral;

namespace WarringStates;

internal class TerrainSettingForm : ResizeableForm
{
    public TerrainSettingForm()
    {
        FormClosing += OnFormClosing;
    }

    private void OnFormClosing(object? sender, FormClosingEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }

    public override string LocalName { get; set; } = nameof(TerrainSettingForm);



    protected override void InitializeComponent()
    {

    }

    
}
