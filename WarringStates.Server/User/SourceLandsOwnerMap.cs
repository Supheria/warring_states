using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.Map.Terrain;

namespace WarringStates.Server.User;

public class SourceLandsOwnerMap : SerializableTagValues<string, List<SourceLand>>
{
    public override string LocalName => nameof(SourceLandsOwnerMap);

    protected override Func<string, string> WriteTag => s => s;

    protected override Func<List<SourceLand>, List<string>> WriteValue => c => c.Select(x => x.ToSsString()).ToList();

    protected override Func<string, string> ReadTag => s => s;

    protected override Func<List<string>, List<SourceLand>> ReadValue => c => c.Select(s => new SourceLand().ParseSs(s)).ToList();
}
