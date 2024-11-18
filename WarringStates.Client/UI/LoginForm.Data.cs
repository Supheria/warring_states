namespace WarringStates.Client.UI;

partial class LoginForm
{
    public class LoginData
    {
        public string HostAddress { get; set; } = "127.0.0.1";

        public int HostPort { get; set; } = 60;

        public string UserName { get; set; } = "";

        public string Password { get; set; } = "";
    }

    public void Load(LoginData data)
    {
        Address.Text = data.HostAddress;
        Port.Value = data.HostPort;
        PlayerName.Text = data.UserName;
        Password.Text = data.Password;
    }

    public LoginData Save()
    {
        return new LoginData()
        {
            HostAddress = Address.Text,
            HostPort = (int)Port.Value,
            UserName = PlayerName.Text,
            Password = Password.Text,
        };
    }
}
