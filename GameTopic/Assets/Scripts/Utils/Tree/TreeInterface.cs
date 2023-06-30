using System.Collections;
using System.Collections.Generic;

public interface ITreeNode: IDumpable{
    public ITreeNode GetParent();
    public IList<ITreeNode> GetChildren();

}
