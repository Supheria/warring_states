using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Client.Events;
using WarringStates.Client.User;

namespace WarringStates.Client.UI;

partial class ClientForm
{
    private void EnableListener()
    {
        LocalEvents.TryAddListener(LocalEvents.UserInterface.MainFormToClose, Close);
        LocalEvents.TryAddListener(LocalEvents.UserInterface.Login, Login);
        LocalEvents.TryAddListener(LocalEvents.UserInterface.Logout, Logout);
        LocalEvents.TryAddListener<ArchiveIdArgs>(LocalEvents.UserInterface.RequestFetchThumbnail, FetchThumbnail);
    }

    private void FetchThumbnail(ArchiveIdArgs id)
    {
        Client.FetchThumbnail(id.Value);
    }

    private void Login()
    {
        Client.Login(HostAddress.Text, (int)HostPort.Value, PlayerName.Text, Password.Text);
    }

    private void Logout()
    {
        Client.Dispose();
        LocalArchives.ReLocate([]);
    }
}
