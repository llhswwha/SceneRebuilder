using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelSelection : MonoBehaviour
{
    public static ModelSelection Instance;

    void Awake(){
        Instance=this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MouseClick();
    }

    public bool EnableClick=true;

    private void MouseClick(){
        if (EnableClick && Input.GetMouseButtonUp(0))
        {
            //if (IsClickUGUIorNGUI.Instance && IsClickUGUIorNGUI.Instance.isOverUI) 
            //{
            //    Debug.Log("ModelSelection.MouseClick IsClickUGUIorNGUI.Instance.isOverUI");
            //    return;
            //}
            // this.screenRect = new Rect(0, 0, Screen.width, Screen.height);
            print("ModelSelection Input.mousePosition:"+Input.mousePosition);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//从摄像机发出到点击坐标的射线  
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                Debug.DrawLine(ray.origin, hitInfo.point);//划出射线，只有在scene视图中才能看到  
                GameObject gameObj = hitInfo.collider.gameObject;
                Debug.Log("ModelSelection click object name is " + gameObj.name);
                //if (gameObj.tag == "boot")//当射线碰撞目标为boot类型的物品 ，执行拾取操作  
                //{
                //    Debug.Log("pick up!");
                //}
                SelectGO(gameObj);
            }
            else{
                RecoveryLastClickRender();
            }
        }
    }

    public void RecoveryLastClickRender(){
        if (LastClickRenderer!= GetComponent<Renderer>() && LastClickRenderer!=null)
                {
                    if (ClickRenderInfos.ContainsKey(LastClickRenderer))
                    {
                        RenderSaveInfo info = ClickRenderInfos[LastClickRenderer];
                        LastClickRenderer.material = info.material;
                        LastClickRenderer.gameObject.layer = info.layer;
                    }
                    LastClickRenderer=null;
                }
    }

    public Material TransparentMaterial;

    public string TransparentLayer="TransparentFX";
    
    public static MeshRenderer LastClickRenderer;

    public Dictionary<MeshRenderer, RenderSaveInfo> ClickRenderInfos = new Dictionary<MeshRenderer, RenderSaveInfo>();

    public class RenderSaveInfo
    {
        public Material material;

        public int layer;
    }

        public void SelectGO(GameObject go){
        MeshRenderer renderer=go.GetComponent<MeshRenderer>();
        if(renderer!=null){
            if (!ClickRenderInfos.ContainsKey(renderer))
            {
                RenderSaveInfo info = new RenderSaveInfo();
                info.material = renderer.material;
                info.layer = go.layer;
                ClickRenderInfos.Add(renderer, info);
            }
            renderer.material = TransparentMaterial;

            go.layer = LayerMask.NameToLayer(TransparentLayer);

            RecoveryLastClickRender();
            LastClickRenderer = renderer;
        }
        else{
            RecoveryLastClickRender();
        }
    }
}
