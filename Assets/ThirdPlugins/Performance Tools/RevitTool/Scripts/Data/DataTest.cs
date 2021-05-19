using UnityEngine;

public class DataTest : MonoBehaviour
{
    public DataA asset;
    // Start is called before the first frame update
    void Start()
    {
        asset = ScriptableObject.CreateInstance<DataA>();
        //asset = new DataA();
        //asset.a = 123;
        //AssetDatabase.CreateAsset(asset, "Assets/Resources/DataA.asset");

        //Point3Info p3 = new Point3Info();
        //p3.x = 1;
        //p3.y = 2;
        //p3.z = 3;
        //AssetDatabase.CreateAsset(p3, "Assets/Resources/Point3Info.asset");

        //AssetDatabase.SaveAssets();

        //Point3Info p3=Resources.Load<Point3Info>("Point3Info");
        //print("p3:" + (p3==null)+","+p3);

        //TransformInfo transformInfo = new TransformInfo();
        //transformInfo.Init();
        //AssetDatabase.CreateAsset(transformInfo, "Assets/Resources/TransformInfo.asset");

        

        
    }

    [ContextMenu("Save")]
    public void Save()
    {
        var node = new NodeInfo();
        node.nodeName = "123";
        NodeInfoAsset.Save(node,"123", "Assets/Resources/NodeInfoAsset.asset");
    }

    [ContextMenu("Load")]
    public void Load()
    {
        Debug.Log("Load");
        NodeInfoAsset p3 = Resources.Load<NodeInfoAsset>("NodeInfoAsset");
        print("p3:" + (p3 == null) + "," + p3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
