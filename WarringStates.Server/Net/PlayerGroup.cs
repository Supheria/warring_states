using LocalUtilities.IocpNet.Common;
using LocalUtilities.TypeGeneral;
using WarringStates.Flow;
using WarringStates.Map;
using WarringStates.Net.Common;
using WarringStates.Server.Data;
using WarringStates.Server.Map;
using WarringStates.Server.User;
using WarringStates.User;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace WarringStates.Server.Net;

internal class PlayerGroup : IRosterItem<string>
{
    public ArchiveInfo ArchiveInfo { get; }

    public LandMapEx LandMap { get; }

    ServiceRoster Group { get; } = [];

    public string Signature => ArchiveInfo.Id;

    public int PlayerCount => Group.Count;

    SpanFlow SpanFlow { get; } = new();

    public PlayerGroup(ArchiveInfo archiveInfo)
    {
        ArchiveInfo = archiveInfo;
        LandMap = LocalArchive.InitializeLandMap(archiveInfo);
        SpanFlow.Relocate(LocalArchive.LoadCurrentSpan(archiveInfo));
        SpanFlow.Tick += UpdateCurrentDate;
        SpanFlow.Start();
    }

    public PlayerGroup()
    {
        ArchiveInfo = new();
        LandMap = new();
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
        service.PlayerGroup = this;
        CheckNewPlayer(service);
        UpdatePlayerList();
    }

    private void CheckNewPlayer(ServerService service)
    {
        var owners = LocalArchive.GetOwnerSites(ArchiveInfo, service.Player.Id);
        if (owners.Count is not 0)
            return;
        var owner = LandMap.SetRandomSite(service.Player.Id);
        LocalArchive.SetOwnerSites(ArchiveInfo, owner.Site, owner.LandType, service.Player.Id);
    }

    public void RemovePlayer(ServerService service)
    {
        service.PlayerGroup = null;
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

    private void UpdateCurrentDate(SpanFlowTickOnArgs args)
    {
        Parallel.ForEach(Group, service =>
        {
            if (service.Joined)
                service.UpdateCurrentDate(args);
        });
    }

    public bool BuildLand(Coordinate site, SourceLandTypes type, string playerId, out VisibleLands vision)
    {
        vision = new();
        if (!LandMap.AddSouceLand(site, type))
            return false;
        LocalArchive.SetOwnerSites(ArchiveInfo, site, type, playerId);
        LandMap.GetVision(site, vision);
        return true;
    }
}
