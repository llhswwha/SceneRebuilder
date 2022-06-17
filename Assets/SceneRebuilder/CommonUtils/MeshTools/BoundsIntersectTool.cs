using CommonUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsIntersectTool : MonoBehaviour
{
    public GameObject targetObject;

    public GameObject targetContainer;

    public static Bounds CaculateBounds(GameObject root)
    {
        BoxCollider box = ColliderExtension.AddCollider(root);
        Bounds bounds = box.bounds;
        GameObject.DestroyImmediate(box);
        return bounds;
    }

    [ContextMenu("Test")]
    public void Test()
    {
        BoxCollider[] boxes = targetContainer.GetComponents<BoxCollider>();
        if (boxes.Length == 0)
        {
            targetContainer.AddComponent<BoxCollider>();
            boxes = targetContainer.GetComponents<BoxCollider>();
        }
        Bounds b0 = CaculateBounds(targetObject);
        for (int i = 0; i < boxes.Length; i++)
        {
            BoxCollider b = boxes[i];
            float f1 = ColliderExtension.BoundsContainedPercentage(b0, b.bounds);
            float f2 = ColliderExtension.BoundsContainedPercentage(b.bounds, b0);
            Debug.Log($"Test_{targetObject.name}[{i}] Percent:{f1} | {f2}");
        }
    }


}
