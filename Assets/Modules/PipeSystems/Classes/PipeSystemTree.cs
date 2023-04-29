using System.Collections;
using System.Collections.Generic;
using DbModel.Location.Pipes;
using UnityEngine;

public class PipeSystemTree
{
    public PipeSystemTreeNode RootNode=new PipeSystemTreeNode();

    public PipeSystemTree()
    {

    }

    public PipeSystemTree(PipeSystem[] pipeSystems)
    {
        if(pipeSystems==null){
            Debug.LogError("PipeSystemTree pipeSystems==null ");
            return;
        }
        RootNode.Children=new List<PipeSystemTreeNode>();

        Dictionary<string,List<PipeSystem>> dict=new Dictionary<string, List<PipeSystem>>();
        foreach(var sys in pipeSystems){
            if(!dict.ContainsKey(sys.Type)){
                dict.Add(sys.Type,new List<PipeSystem>());
            }
            dict[sys.Type].Add(sys);
        }
        
        foreach(var key in dict.Keys){
            var list=dict[key];
            PipeSystemTreeNode node=new PipeSystemTreeNode();
            node.Name=key;
            node.Items=list;
            RootNode.Children.Add(node);
        }
    }
}

public class PipeSystemTreeNode
{
    public string Name;
    public List<PipeSystemTreeNode> Children {get;set;}

    public List<PipeSystem> Items {get;set;}

    public PipeSystemTreeNode()
    {

    }

    public PipeSystemTreeNode(PipeSystem pipeSystem)
    {
        Name=pipeSystem.Type;
        Items=new List<PipeSystem>();
        Items.Add(pipeSystem);
    }
}
