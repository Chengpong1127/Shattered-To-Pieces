using System.Collections.Generic;
using System.Linq;

public class TreeInfo<T>: IInfo where T: IInfo{
    public int rootID;
    public Dictionary<int, T> NodeInfoMap = new();
    public List<(int, int)> EdgeInfoList = new();

    public override bool Equals(object obj)
    {
        return obj is TreeInfo<T> info &&
                rootID == info.rootID &&
                NodeInfoMap.Count == info.NodeInfoMap.Count && !NodeInfoMap.Except(info.NodeInfoMap).Any()
                && EdgeInfoList.Count == info.EdgeInfoList.Count && !EdgeInfoList.Except(info.EdgeInfoList).Any();
    }

    public override int GetHashCode()
    {
        return System.HashCode.Combine(rootID, NodeInfoMap, EdgeInfoList);
    }
}