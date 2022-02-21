using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMeshModel : MonoBehaviour
{
    public int VertexCount = 0;

    public void ShowOBB()
    {
        OBBCollider.ShowOBB(this.gameObject, true);
    }

    public virtual void GetModelInfo()
    {

    }

    public virtual void GetModelInfo_Job()
    {

    }
}
