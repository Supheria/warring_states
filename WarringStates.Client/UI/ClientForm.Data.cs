using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarringStates.Client.UI;

partial class ClientForm
{
    protected override Type FormDataType => typeof(ClientData);

    private class ClientData : FormData
    {
        public string HostAddress { get; set; } = "127.0.0.1";

        public int HostPort { get; set; } = 60;

        public string UserName { get; set; } = "";

        public string Password { get; set; } = "";

        public string DirName { get; set; } = "";

        public string FilePath { get; set; } = "";
    }

    protected override void OnLoad(object? data)
    {
        if (data is not ClientData clientData)
            return;
        Address.Text = clientData.HostAddress;
        Port.Value = clientData.HostPort;
        PlayerName.Text = clientData.UserName;
        Password.Text = clientData.Password;
        DirName.Text = clientData.DirName;
        FilePath.Text = clientData.FilePath;
    }

    protected override FormData OnSave()
    {
        return new ClientData()
        {
            HostAddress = Address.Text,
            HostPort = (int)Port.Value,
            UserName = PlayerName.Text,
            Password = Password.Text,
            DirName = DirName.Text,
            FilePath = FilePath.Text,
        };
    }

}
