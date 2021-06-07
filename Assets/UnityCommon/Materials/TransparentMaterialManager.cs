using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class TransparentMaterialManager : MonoBehaviour
{

    private static List<Material2Material> List=new List<Material2Material>();
    private static Dictionary<Material,Material> dictionary=new Dictionary<Material, Material>();

    public static Material GetTransparentMaterial(Material material)
    {
        if (dictionary.ContainsKey(material))
            return dictionary[material];

        Material material2 = new Material(material);
        dictionary.Add(material, material2);
        Material2Material m2m = new Material2Material();
        m2m.Material1 = material;
        m2m.Material2 = material2;
        List.Add(m2m);

        return material2;
    }

    public Material2Material[] Materials;

    void Update()
    {
        if (Materials.Length != List.Count)
        {
            Materials = List.ToArray();
        }
    }
}
