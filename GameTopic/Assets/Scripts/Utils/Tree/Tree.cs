using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Tree
{
    public ITreeNode root;
    public Tree(ITreeNode root){
        this.root = root;
    }

    public void TraverseBFS(Action<ITreeNode> action){
        var queue = new Queue<ITreeNode>();
        queue.Enqueue(root);
        while (queue.Count > 0){
            var node = queue.Dequeue();
            action(node);
            foreach (var child in node.Children){
                queue.Enqueue(child);
            }
        }
    }

    public void TraverseDFS(Action<ITreeNode> action){
        var stack = new Stack<ITreeNode>();
        stack.Push(root);
        while (stack.Count > 0){
            var node = stack.Pop();
            action(node);
            foreach (var child in node.Children){
                stack.Push(child);
            }
        }
    }

    public (TreeInfo<T>, Dictionary<ITreeNode, int>) Dump<T>() where T: class, IInfo{
        var treeInfo = new TreeInfo<T>();
        var localID = 0;
        var tempDictionary = new Dictionary<ITreeNode, int>();
        treeInfo.rootID = localID;
        TraverseBFS((node) => {
            var info = node.Dump() as T;
            treeInfo.NodeInfoMap.Add(localID, info);
            tempDictionary.Add(node, localID);
            localID++;
        });

        foreach (var (node, _localID) in tempDictionary){
            var parent = node.Parent;
            if (parent == null){
                continue;
            }
            var parentID = tempDictionary[parent];
            var nodeID = _localID;
            treeInfo.EdgeInfoList.Add((parentID, nodeID));
        }

        return (treeInfo, tempDictionary);

    }
    
    
}
