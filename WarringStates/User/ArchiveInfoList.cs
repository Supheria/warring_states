using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarringStates.User;

namespace WarringStates.User;

public class ArchiveInfoList : IList<ArchiveInfo>
{
    List<ArchiveInfo> InfoList { get; } = [];

    public int Count => InfoList.Count;

    public bool IsReadOnly => true;

    public ArchiveInfo this[int index] { get => InfoList[index]; set => InfoList[index] = value; }

    public ArchiveInfo[] GetArchiveInfos()
    {
        return InfoList.ToArray();
    }

    public bool TryGetValue(int index, [NotNullWhen(true)] out ArchiveInfo? info)
    {
        info = null;
        if (index < 0 || index >= Count)
            return false;
        info = InfoList[index];
        return true;
    }

    public int IndexOf(ArchiveInfo item)
    {
        return InfoList.IndexOf(item);
    }

    public void Insert(int index, ArchiveInfo item)
    {
        InfoList.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        InfoList.RemoveAt(index);
    }

    public void Add(ArchiveInfo item)
    {
        InfoList.Add(item);
    }

    public void Clear()
    {
        InfoList.Clear();
    }

    public bool Contains(ArchiveInfo item)
    {
        return InfoList.Contains(item);
    }

    public void CopyTo(ArchiveInfo[] array, int arrayIndex)
    {
        InfoList.CopyTo(array, arrayIndex);
    }

    public bool Remove(ArchiveInfo item)
    {
        return InfoList.Remove(item);
    }

    public IEnumerator<ArchiveInfo> GetEnumerator()
    {
        return InfoList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return InfoList.GetEnumerator();
    }
}
