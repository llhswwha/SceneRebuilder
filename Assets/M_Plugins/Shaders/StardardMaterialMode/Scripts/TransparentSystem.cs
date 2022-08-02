using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StardardShader;
using Mogoson.CameraExtension;
public class TransparentSystem : MonoBehaviour
{
    //public static bool SEnabled=false;
    public static TransparentSystem Instance;

    public Transform target;

    public Vector3 targetPos;

    public Camera cam;

    public Vector3 camPos;

    public bool EnableTransparent=false;

    void Awake(){
        Instance=this;
    }

    // Start is called before the first frame update

    public Dictionary<Material,Material> matBack=new Dictionary<Material, Material>();

    public List<Material> transparentedMats=new List<Material>();

    public List<GameObject> hitObjs=new List<GameObject>();

    // Update is called once per frame
    void Update()
    {
        if(cam==null){
            cam=Camera.main;
        }
        if(cam!=null){
            camPos=cam.transform.position;
            AroundAlignCamera aroundAlign=cam.GetComponent<AroundAlignCamera>();
            if(aroundAlign!=null){
                target=aroundAlign.GetTarget();
            }
        }

        if(target!=null)
        {
            targetPos=target.position;
        }

        if(targetPos==Vector3.zero)return;
        if(camPos==Vector3.zero)return;
        if(EnableTransparent==false){
            if(transparentedMats.Count>0){
                Debug.Log("transparentedMats:"+transparentedMats.Count);
                foreach(var mat in transparentedMats)
                {
                    var mat2=matBack[mat];
                    mat.CopyPropertiesFromMaterial(mat2);
                    matBack.Remove(mat);
                }
                transparentedMats.Clear();
            }
            return;
        }

        Ray ray=new Ray(camPos,targetPos-camPos);
        Debug.DrawRay(camPos,targetPos-camPos,Color.red,0.1f);

        RaycastHit[] hitInfos=Physics.RaycastAll(ray);

        List<Material> materials=new List<Material>();
        string txt="";
        List<GameObject> objs=new List<GameObject>();
        for(int i=0;i<hitInfos.Length;i++)
        {
            RaycastHit hit = hitInfos[i];
            GameObject go=hit.collider.gameObject;
            ObjectTagInfo tag=go.GetComponent<ObjectTagInfo>();
            //Debug.Log("hit:"+go);
            txt+=go.name+";";
            if(tag!=null)
            {
                if(tag.items.Contains(ObjectTags.Structure))
                {
                    Renderer[] renderers=go.GetComponentsInChildren<Renderer>();
                    foreach(var renderer in renderers){
                        foreach(var mat in renderer.materials)
                        {
                            if(!materials.Contains(mat)){
                                materials.Add(mat);
                            }
                        }
                        //Material mat=renderer.material;
                        //
                    }
                }
            }
            objs.Add(go);
        }
        //Debug.Log("hit:"+hitInfos.Length+"|"+txt);
        List<Material> materials2=new List<Material>(transparentedMats);
        //Debug.Log("materials:"+materials.Count);
        foreach(var mat in materials){
            materials2.Remove(mat);
            if(transparentedMats.Contains(mat)){//已经透明了
                //continue;
            }
            else{
                //if(matBack)
                matBack.Add(mat,new Material(mat));
                Color color=new Color(mat.color.r,mat.color.g,mat.color.b,0.1f);
                StardardShaderMatHelper.SetMaterialRenderingMode(mat, RenderingMode.Transparent,RenderPipeline.HDRP,color);
                mat.SetColor("_BaseColor", color);
            }
        }
        if(materials2.Count>0){
            Debug.Log("materials2:"+materials2.Count);
            foreach(var mat in materials2){
                var mat2=matBack[mat];
                mat.CopyPropertiesFromMaterial(mat2);
                matBack.Remove(mat);
            }
        }
        

        transparentedMats=materials;
        hitObjs=objs;
    }
}
