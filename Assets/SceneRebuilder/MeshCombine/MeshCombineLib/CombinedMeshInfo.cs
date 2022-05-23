using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinedMeshInfo : MonoBehaviour
{
    public CombinedMesh combinedMesh;
    // Start is called before the first frame update

    [ContextMenu("Combine")]

    public void Combine(){
        combinedMesh.Refresh();
    }
}
