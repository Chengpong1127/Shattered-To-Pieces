using System.Collections;
using System.Collections.Generic;

public interface ITreeNode: IStorable{
    public ITreeNode GetParent();
    public ITreeNode GetRoot();
    public IList<ITreeNode> GetChildren();
}
