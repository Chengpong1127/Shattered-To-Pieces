using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree
{
    public ITreeNode root;
    public Tree(ITreeNode root){
        this.root = root;
    }
    public void Traverse(){
        Traverse(root);
    }
    public void Traverse(ITreeNode node){
        Debug.Log(node);
        foreach (var child in node.GetChildren()){
            Traverse(child);
        }
    }

    public void TraverseBFS(){
        var queue = new Queue<ITreeNode>();
        queue.Enqueue(root);
        while (queue.Count > 0){
            var node = queue.Dequeue();
            Debug.Log(node);
            foreach (var child in node.GetChildren()){
                queue.Enqueue(child);
            }
        }
    }

    public void TraverseDFS(){
        var stack = new Stack<ITreeNode>();
        stack.Push(root);
        while (stack.Count > 0){
            var node = stack.Pop();
            Debug.Log(node);
            foreach (var child in node.GetChildren()){
                stack.Push(child);
            }
        }
    }

    public void Dump(){
        Dump(root);
    }

    public void Dump(ITreeNode node){
        Debug.Log(node);
        foreach (var child in node.GetChildren()){
            Dump(child);
        }
    }
    
}
