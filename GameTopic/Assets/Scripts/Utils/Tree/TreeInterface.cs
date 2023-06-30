using System.Collections;
using System.Collections.Generic;

public interface ITreeNode{
    public ITreeNode GetParent();
    public IList<ITreeNode> GetChildren();
    
}
