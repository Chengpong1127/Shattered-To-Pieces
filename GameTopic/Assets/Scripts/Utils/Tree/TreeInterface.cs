using System.Collections;
using System.Collections.Generic;

public interface ITreeNode: IStorable{
    public ITreeNode GetParent();
    public IList<ITreeNode> GetChildren();

}
