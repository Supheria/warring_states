using WarringStates.Client.Events;
using WarringStates.Client.Net;
using WarringStates.Client.User;
using WarringStates.User;

namespace WarringStates.Client.UI;

partial class ArchiveSelector
{
    private void AddOperations()
    {
        LoginButton.Click += (_, _) => LocalNet.Login();
        LogoutButton.Click += (_, _) => LocalNet.Logout();
        Selector.SelectedChanged += Selector_SelectedChanged;
        LocalNet.Service.OnLogined += Service_OnLogined;
        LocalNet.Service.OnClosed += Service_OnClosed;
    }

    private void Service_OnLogined()
    {
        LoginButton.CanSelect = false;
        LogoutButton.CanSelect = true;
        LoginButton.Redraw();
        LoginButton.Invalidate();
    }

    private void Service_OnClosed()
    {
        LoginButton.CanSelect = true;
        LogoutButton.CanSelect = false;
        LogoutButton.Redraw();
        LogoutButton.Invalidate();
        LocalArchives.ReLocate([]);
    }

    private void Selector_SelectedChanged(object? sender, EventArgs e)
    {
        if (Selector.SelectedIndex is -1)
            JoinButton.CanSelect = false;
        else
            JoinButton.CanSelect = true;
        if (LocalArchives.ArchiveInfoList.TryGetValue(Selector.SelectedIndex, out var info))
            LocalNet.Service.FetchThumbnail(info.Id);
        else
        {
            Thumbnail.SetThumbnailVoid();
            Thumbnail.Redraw();
            Thumbnail.Invalidate();
        }
    }
}
