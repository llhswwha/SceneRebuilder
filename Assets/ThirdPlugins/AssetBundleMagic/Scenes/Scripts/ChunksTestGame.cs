using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jacovone.AssetBundleMagic;


public class ChunksTestGame : MonoBehaviour {

    public ChunkManager cm;

	// Use this for initialization
	void Start () {
        if(cm==null)
            cm = GameObject.Find("ChunkManager").GetComponent<ChunkManager>();
	}

    public void SetDistanceBias(float distanceBias)
    {
        Debug.Log("SetDistanceBias:"+ distanceBias);
        cm.distanceBias = distanceBias;
    }

    public void CleanCache()
    {
        AssetBundleMagic.CleanBundlesCache();
    }

    public void ActiveChunkManager()
    {
        cm.enabled = true;
    }
}
