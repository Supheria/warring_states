using WarringStates.Client.Events;
using WarringStates.Client.Net;

namespace WarringStates.Client.UI;

partial class LoginForm
{
    protected override void AddOperation()
    {
        base.AddOperation();
        JoinButton.Click += JoinButton_Click;
    }

    private void JoinButton_Click(object? sender, EventArgs e)
    {
        LocalNet.Login();
        LocalNet.Service.JoinArchive();
    }
}
