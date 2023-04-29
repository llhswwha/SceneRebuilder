using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battlehub.RTCommon
{
    public static class MaterialUtils 
    {
        public static void InstantiateMaterial(Material material,GameObject go){
                if(material==null){
                    Debug.LogError("ChangeMaterial material==null");
                    return;
                }
                if(go==null){
                    Debug.LogError("ChangeMaterial go==null");
                    return;
                }
                string matName=material.name;
                Debug.Log("OnEdit:"+material+"|"+go);
                if(!matName.Contains("(Clone)") && !matName.Contains("(Instance)"))
                {
                    Renderer renderer=go.GetComponent<Renderer>();
                    if(renderer!=null){
                        // 
                        // bool isMaterialChanged=false;
                        for(int i=0;i<renderer.sharedMaterials.Length;i++)
                        {
                            Debug.Log("mat:"+renderer.sharedMaterials[i]);
                            if(renderer.sharedMaterials[i]==material){
                                var materials=renderer.materials;
                                material=GameObject.Instantiate(material) as Material;
                                material.name=matName+"(Instance)";
                                materials[i]=material;
                                renderer.materials=materials;//
                                // isMaterialChanged=true;
                                // Debug.LogError("OnEdit newMaterial :"+renderer.materials[i]);
                                break;
                            }
                        }
                    }
                }
        }
    }
}

