using System.Collections.Generic;
using System.Linq;

public class TreeInfo<T>: IInfo where T: IInfo{
    public int rootID;
    public Dictionary<int, T> NodeInfoMap = new Dictionary<int, T>();
    public List<(int, int)> EdgeInfoList = new List<(int, int)>();

    public override bool Equals(object obj)
    {
        return obj is TreeInfo<T> info &&
                rootID == info.rootID &&
                NodeInfoMap.Count == info.NodeInfoMap.Count && !NodeInfoMap.Except(info.NodeInfoMap).Any()
                && EdgeInfoList.Count == info.EdgeInfoList.Count && !EdgeInfoList.Except(info.EdgeInfoList).Any();
    }

    public override int GetHashCode()
    {
        int hashCode = 1861411795;
        hashCode = hashCode * -1521134295 + rootID.GetHashCode();
        hashCode = hashCode * -1521134295 + NodeInfoMap.GetHashCode();
        hashCode = hashCode * -1521134295 + EdgeInfoList.GetHashCode();
        return hashCode;
    }
}