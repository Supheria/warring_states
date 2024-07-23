using WarringStates.Client.Events;
using WarringStates.Client.User;
using WarringStates.User;

namespace WarringStates.Client.UI;

partial class ArchiveSelector
{
    private void AddOperations()
    {
        LoginButton.Click += LoginButton_Click;
        LogoutButton.Click += LogoutButton_Click;
        Selector.SelectedChanged += Selector_SelectedChanged;
    }

    private void LoginButton_Click(object? sender, EventArgs e)
    {
        LoginButton.CanSelect = false;
        LogoutButton.CanSelect = true;
        LocalEvents.TryBroadcast(LocalEvents.UserInterface.Login);
    }

    private void LogoutButton_Click(object? sender, EventArgs e)
    {
        LoginButton.CanSelect = true;
        LogoutButton.CanSelect = false;
        LocalEvents.TryBroadcast(LocalEvents.UserInterface.Logout);
    }

    private void Selector_SelectedChanged(object? sender, EventArgs e)
    {
        if (Selector.SelectedIndex is -1)
            JoinButton.CanSelect = false;
        else
            JoinButton.CanSelect = true;
        if (LocalArchives.ArchiveInfoList.TryGetValue(Selector.SelectedIndex, out var info))
        {
            var sendArgs = new ArchiveIdArgs(info.Id);
            LocalEvents.TryBroadcast<ArchiveIdArgs>(LocalEvents.UserInterface.RequestFetchThumbnail, sendArgs);
        }
        else
        {
            Thumbnail.SetThumbnailVoid();
            Thumbnail.Redraw();
            Thumbnail.Invalidate();
        }
    }
}
