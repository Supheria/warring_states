using LocalUtilities.IocpNet.Common;
using LocalUtilities.SQLiteHelper;
using LocalUtilities.TypeGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Net.Common;
using WarringStates.Server.Map;
using WarringStates.Server.User;
using WarringStates.User;

namespace WarringStates.Server.Net;

internal class PlayerGroup : IRosterItem<string>
{
    public ArchiveInfo ArchiveInfo { get; }

    public LandMapEx LandMap { get; }

    ServiceRoster Group { get; } = [];

    public string Signature => ArchiveInfo.Id;

    public int PlayerCount => Group.Count;

    internal PlayerGroup(ArchiveInfo archiveInfo)
    {
        ArchiveInfo = archiveInfo;
        LandMap = LocalArchive.InitializeLandMap(archiveInfo);
    }

    public void BroadcastMessage(string message)
    {
        Parallel.ForEach(Group, service =>
        {
            service.SendMessage(message);
        });
    }

    public void RelayCommand(CommandReceiver receiver)
    {
        var name = (OperateCode)receiver.OperateCode switch
        {
            OperateCode.Request => receiver.GetArgs<string>(ServiceKey.ReceiveName),
            OperateCode.Callback => receiver.GetArgs<string>(ServiceKey.SendName),
            _ => null,
        } ?? throw new NetException(ServiceCode.MissingCommandArgs, ServiceKey.ReceiveName, ServiceKey.SendName);
        if (!Group.TryGetValue(PlayerTable.CheckoutPlayerId(name), out var user))
            throw new NetException(ServiceCode.PlayerNotExist);
        user.HandleCommand(receiver);
    }

    public void AddPlayer(ServerService service)
    {
        if (!Group.TryAdd(service))
            return;
        service.OnClosed += () =>
        {
            RemovePlayer(service);
        };
        service.OnRelayCommand += RelayCommand;
        UpdatePlayerList();
    }

    public void RemovePlayer(ServerService service)
    {
        if (Group.TryRemove(service))
            UpdatePlayerList();
    }

    private void UpdatePlayerList()
    {
        var playerList = Group.Select(x => x.Player.Name).ToArray();
        Parallel.ForEach(Group, service =>
        {
            service.UpdatePlayerList(playerList);
        });
    }
}
