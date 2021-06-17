using Jacovone.AssetBundleMagic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetSceneLoadingIndicator : MonoBehaviour {

    private ChunkManager _chunkManager;

    void Start()
    {
        GameObject go = GameObject.Find("ChunkManager");
        if (go != null)
        {
            _chunkManager = go.GetComponent<ChunkManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ProgressbarLoad.Instance == null) return;
        if (_chunkManager != null)
        {
            //if (_chunkManager.currentProgress != null)
            if (_chunkManager.isBusy)
            {
                //RotatingLoading.enabled = true;
                //ColoredCircle.enabled = true;
                var progress = _chunkManager.GetProgress();
                //ColoredCircle.fillAmount = progress;
                ////Debug.Log("load progress:"+progress);
                ProgressbarLoad.Instance.Show(progress);
            }
            else
            {
                ////Debug.Log("load finieshed!");
                //RotatingLoading.enabled = false;
                //ColoredCircle.enabled = false;
                //ProgressbarLoad.Instance.Hide();
            }
        }
        else
        {

            //RotatingLoading.enabled = false;
            //ColoredCircle.enabled = false;
            //ProgressbarLoad.Instance.Hide();
        }
    }
}
