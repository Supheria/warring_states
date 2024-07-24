using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Events;
using WarringStates.User;

namespace WarringStates.Client.User;

internal class FetchPlayerArchiveArgs(PlayerArchive archive) : ICallbackArgs
{
    public PlayerArchive Archive { get; } = archive;
}
