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
            foreach (var child in node.GetChildren()){
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
            foreach (var child in node.GetChildren()){
                stack.Push(child);
            }
        }
    }

    public TreeInfo Dump(){
        var treeInfo = new TreeInfo();
        var localID = 0;
        var tempDictionary = new Dictionary<ITreeNode, int>();
        treeInfo.rootID = localID++;
        TraverseBFS((node) => {
            var gameComponent = node;
            treeInfo.NodeInfoMap.Add(localID, gameComponent.Dump());
            tempDictionary.Add(node, localID);
            localID++;
        });

        foreach (var nodePair in treeInfo.NodeInfoMap){
            var node = nodePair.Value as ITreeNode;
            var parent = node.GetParent();
            if (parent == null){
                continue;
            }
            var parentID = tempDictionary[parent];
            var nodeID = tempDictionary[node];
            treeInfo.EdgeInfoList.Add((parentID, nodeID));
        }

        return treeInfo;

    }
    
    
}
