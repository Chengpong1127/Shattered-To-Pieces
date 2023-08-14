using System.Collections;
using System.Collections.Generic;

public interface ITreeNode: IStorable{
    public ITreeNode GetRoot();
    public ITreeNode Parent { get; }
    public IList<ITreeNode> Children { get; }
}
