using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NodeInfo 
{
    //public static int ID = 0;

    //private int id;


    public string Id = "";

    public string nodeName;

    public string className;

    public string typeName;

    public TransformInfo transform;

    public List<NodeInfo> children;

    //public void SetId()
    //{
    //    //id = ID++;
    //}

    public string GetId()
    {
        if (Id == "")
        {
            Id = GetId(nodeName);
        }
        return Id;
    }

    public static string GetId(string n)
    {
        try
        {
            string[] parts = n.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
            return parts[1];
        }
        catch (Exception ex)
        {
            //Debug.LogError("GetId:"+n+" Exception:"+ex);
            return n;
        }
        
    }

    public static string GetTypeName(string n)
    {
        try
        {
            string[] parts = n.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
            return parts[0];
        }
        catch (Exception ex)
        {
            //Debug.LogError("GetId:"+n+" Exception:"+ex);
            return n;
        }

    }

    public override string ToString()
    {
        return string.Format("NodeInfo:({0},{1},{2},{3})", nodeName, typeName, className,children.Count);
    }

    public void AddNode()
    {

    }

    private Dictionary<string, NodeInfo> nodeDic;

    private Dictionary<string, int> nodeIndexDic;

    public int FindNode(string name)
    {
        if (nodeDic==null)
        {
            Debug.Log("Init Node Dict Start");
            var allChildren = GetAllChildren(true, false) ;
            if (allChildren == null) return -1;
            
            nodeDic = new Dictionary<string, NodeInfo>();
            nodeIndexDic = new Dictionary<string, int>();
            for (int i = 0; i < allChildren.Count; i++)
            {
                NodeInfo item = allChildren[i];
                nodeDic.Add(item.nodeName, item);
                nodeIndexDic.Add(item.nodeName, i);
            }
            Debug.Log("Init Node Dict End");
        }
        
        if (nodeIndexDic.ContainsKey(name))
        {
            return nodeIndexDic[name];
        }
        else
        {
            return -1;
        }
    }

    public void SetTransform(GameObject obj)
    {
        Vector3 pos= transform.pos.ToPos();
        //Debug.Log("SetTransform:"+transform.pos+"->"+ pos);
        obj.transform.position = pos;
        //obj.transform.rotation = Quaternion.Euler(transform.rotation.ToRotation());

        //(10,20,30)
        //obj.transform.Rotate(new Vector3(transform.rotation.x, 0, 0),Space.World);//必须是Space.World
        //obj.transform.Rotate(new Vector3(0, 0, transform.rotation.y), Space.World);
        //obj.transform.Rotate(new Vector3(0, -transform.rotation.z, 0), Space.World);

        obj.transform.rotation = GetRotation();

        //(90,-90,0)
        //obj.transform.Rotate(new Vector3(transform.rotation.x, 0, 0));
        //obj.transform.Rotate(new Vector3(0, -transform.rotation.y, 0));
        //obj.transform.Rotate(new Vector3(0, 0, -transform.rotation.z));

        //obj.transform.localScale = transform.scale.ToScale();--比例不做处理，3dmax导入的模型比例都应该是1
    }


    public Vector3 GetPos()
    {
        return transform.pos.ToPos();
    }

    public Quaternion GetRotation()
    {
        return GetRotation(transform.rotation.x, transform.rotation.y, transform.rotation.z);
    }

    public static GameObject RotationGameObj;
    public static Quaternion GetRotation(float x,float y,float z)
    {
        //if (RotationGameObj == null)
        //{
        //    RotationGameObj = new GameObject();
        //    RotationGameObj.name = "RotationGameObj";
        //}
        //RotationGameObj.transform.rotation = Quaternion.identity;

        ////数学不好，用笨办法
        //RotationGameObj.transform.Rotate(new Vector3(x, 0, 0), Space.World);//必须是Space.World
        //RotationGameObj.transform.Rotate(new Vector3(0, 0, y), Space.World);
        //RotationGameObj.transform.Rotate(new Vector3(0, -z, 0), Space.World);
        //return RotationGameObj.transform.rotation;


        ////稍微研究了一下
        //Quaternion rotation0 = Quaternion.identity;
        //rotation0 = Quaternion.AngleAxis(x, Vector3.right) * rotation0;
        //rotation0 = Quaternion.AngleAxis(y, Vector3.forward) * rotation0;
        //rotation0 = Quaternion.AngleAxis(z, Vector3.down) * rotation0;
        //return rotation0;

        Quaternion rotation1 = Quaternion.AngleAxis(z, Vector3.down) * Quaternion.AngleAxis(y, Vector3.forward) * Quaternion.AngleAxis(x, Vector3.right)* Quaternion.identity;//合并
        return rotation1;
    }

    public NodeInfo Copy(Vector3 offset)
    {
        NodeInfo node = new NodeInfo();
        node.nodeName = this.nodeName;
        node.className = this.className;
        node.typeName = this.typeName;
        node.transform = this.transform.Copy(offset);
        return node;
    }


    public List<NodeInfo> GetAllChildren(bool isRoot,bool containSelf)
    {
        List<NodeInfo> result = new List<NodeInfo>();
        if (children != null)
        {
            foreach (var item in children)
            {
                var subList = item.GetAllChildren(false, true);
                result.AddRange(subList);
            }
        }
        if(isRoot)
        {
            if (containSelf)
            {
                result.Add(this);
            }
        }
        else
        {
            result.Add(this);
        }
       
        return result;
    }

    public Vector3 GetCenter()
    {
        var nodes = GetAllChildren(true,false);
        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float minZ = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;
        float maxZ = float.MinValue;
        foreach (var node in nodes)
        {
            var pos = node.GetPos();
            if (pos.x < minX)
            {
                minX = pos.x;
            }
            if (pos.y < minY)
            {
                minY = pos.y;
            }
            if (pos.z < minZ)
            {
                minZ = pos.z;
            }
            if (pos.x > maxX)
            {
                maxX = pos.x;
            }
            if (pos.y > maxY)
            {
                maxY = pos.y;
            }
            if (pos.z > maxZ)
            {
                maxZ = pos.z;
            }
        }

        float centerX = (minX + maxX) / 2.0f;
        float centerY = (minY + maxY) / 2.0f;
        float centerZ = (minZ + maxZ) / 2.0f;
        return new Vector3(centerX, centerY, centerZ);
    }

    internal void CopyChildren(int count, Vector3 offset)
    {
        List<NodeInfo> copy = new List<NodeInfo>();
        copy.AddRange(children);
        foreach (var item in children)
        {
            for(int i = 0; i < count; i++)
            {
                var newNode = item.Copy(offset*(i+1));
                copy.Add(newNode);
            }
        }
        //children = copy.ToArray();
        children = copy;
    }

    internal List<NodeInfo> UpdateChildren(List<NodeInfo> newNodes)
    {
        List<NodeInfo> updateNodes = new List<NodeInfo>();
        foreach (var newNode in newNodes)
        {
            int id = FindNode(newNode.nodeName);
            if (id != -1)
            {
                //((NodeInfo)oldNode).UpdateInfo(newNode);
                //updateNodes.Add((NodeInfo)oldNode);
                this.children[id].UpdateInfo(newNode);
                var node = this.children[id];
                updateNodes.Add(this.children[id]);
            }
            else
            {
                
                children.Add(newNode);//存在改名的情况
                Debug.LogWarning("UpdateChildren oldNode == null :" + newNode.nodeName);
            }
        }
        return updateNodes;
    }

    private void UpdateInfo(NodeInfo newNode)
    {
        //Debug.Log("UpdateInfo :" + newNode.nodeName);
        this.transform = newNode.transform;
        this.className = newNode.className;
        if (this.typeName != newNode.typeName)
        {
            
            //Debug.Log(string.Format("{0}->{1}", this.typeName, newNode.typeName));
            this.typeName = newNode.typeName;
        }
    }

    //public void Save(string path)
    //{
    //    UnityEditor.AssetDatabase.CreateAsset(this, path);

    //}

    //public static NodeInfo Load(string path)
    //{
    //    //UnityEditor.AssetDatabase.CreateAsset(this, path);
    //    return null;
    //}
}


