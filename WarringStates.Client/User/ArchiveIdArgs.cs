using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Events;

namespace WarringStates.Client.User;

internal class ArchiveIdArgs(string value) : ICallbackArgs
{
    public string Value { get; } = value;
}
