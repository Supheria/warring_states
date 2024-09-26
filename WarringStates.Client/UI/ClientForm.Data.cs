namespace WarringStates.Client.UI;

partial class ClientForm
{
    protected override Type FormDataType => typeof(ClientData);

    private class ClientData : FormData
    {
        public string DirName { get; set; } = "";

        public string FilePath { get; set; } = "";

        public LoginForm.LoginData LoginData { get; set; } = new();
    }

    protected override void OnLoad(object? data)
    {
        if (data is not ClientData clientData)
            return;
        DirName.Text = clientData.DirName;
        FilePath.Text = clientData.FilePath;
        Login.Load(clientData.LoginData);
    }

    protected override FormData OnSave()
    {
        return new ClientData()
        {
            DirName = DirName.Text,
            FilePath = FilePath.Text,
            LoginData = Login.Save()
        };
    }

}
