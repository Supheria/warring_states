using WarringStates.Client.Events;
using WarringStates.Client.Net;

namespace WarringStates.Client.UI;

partial class LoginForm
{
    protected override void AddOperation()
    {
        base.AddOperation();
        Address.TextChanged += Address_TextChanged;
        Port.ValueChanged += Port_ValueChanged;
        PlayerName.TextChanged += PlayerName_TextChanged;
        Password.TextChanged += Password_TextChanged;
        LocalNet.Service.OnLogined += Service_OnLogined;
        LocalNet.Service.OnClosed += Service_OnClosed;
        LoginButton.Click += LoginButton_Click;
        JoinButton.Click += JoinButton_Click;
        LogoutButton.Click += LogoutButton_Click;
    }

    private void Password_TextChanged(object? sender, EventArgs e)
    {
        LocalNet.PlayerPassword = Password.Text;
    }

    private void PlayerName_TextChanged(object? sender, EventArgs e)
    {
        LocalNet.PlayerName = PlayerName.Text;
    }

    private void Port_ValueChanged(object? sender, EventArgs e)
    {
        LocalNet.ServerPort = (int)Port.Value;
    }

    private void Address_TextChanged(object? sender, EventArgs e)
    {
        LocalNet.ServerAddress = Address.Text;
    }

    private void Service_OnLogined()
    {
        BeginInvoke(() =>
        {
            Address.Enabled = false;
            Port.Enabled = false;
            PlayerName.Enabled = false;
            Password.Enabled = false;
            LoginButton.CanSelect = false;
            JoinButton.CanSelect = true;
            LogoutButton.CanSelect = true;
            Update();
        });
        //StartGame();
    }

    private void Service_OnClosed()
    {
        BeginInvoke(() =>
        {
            Address.Enabled = true;
            Port.Enabled = true;
            PlayerName.Enabled = true;
            Password.Enabled = true;
            LoginButton.CanSelect = true;
            JoinButton.CanSelect = false;
            LogoutButton.CanSelect = false;
            Update();
        });
    }

    private void LoginButton_Click(object? sender, EventArgs e)
    {
        LocalNet.Login();
    }

    private void JoinButton_Click(object? sender, EventArgs e)
    {
        LocalNet.Service.JoinArchive();
    }

    private void LogoutButton_Click(object? sender, EventArgs e)
    {
        LocalNet.Logout();
    }
}
