using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Jacovone.AssetBundleMagic
{
    /// <summary>
    /// ChunkManager is a Class that manages chunks. A chunk is a UNITY scene that represent a piece of game level.
    /// The idea is that each chunk has a specified position, a "enter trigger radius" and a "exit trigger radius".
    /// When the player enter within the enter trigger radius, the ChunkManager loads the chunk, so load the 
    /// corresponding scene asynchronously. When the player exit the "exit trigger radius", the ChunkManager
    /// unload that.
    /// Each chunk must specify a position, a radius for enter, a radius for exit, and a scene name to load/unload
    /// when appropriate. ChunkManager can use bundles (via AssetBundleMagic) to manage download of asset bundles
    /// dynamically.
    /// </summary>
    public class ChunkManager : MonoBehaviour
    {
        /// <summary>
        /// Fired when the download (or locally load) of all asset bundles for a specified chunk has initiated.
        /// </summary>
        /// <param name="progress">The Progress instance for monitoring of the load.</param>
        public delegate void LoadAllBundlesStarted (AssetBundleMagic.Progress progress);

        /// <summary>
        /// Fired when the download (or locally load) of all asset bundles for a specified chunk has termianted.
        /// </summary>
        /// <param name="chunkIndex">The chunk index that has terminated.</param>
        public delegate void LoadAllBundlesFinished (int chunkIndex);

        /// <summary>
        /// A bundle definition represents a single instance of bundle within the ChunkManager.
        /// </summary>
        [Serializable]
        public class BundleDef
        {
            public string bundleName;
            public bool fromFile;
            public bool checkVersion;

            public override string ToString()
            {
                return string.Format("name:{0},file:{1},check:{2}",bundleName,fromFile,checkVersion);
            }
        }

        /// <summary>
        /// A Chunk is a compelte description of a piece of levelthat can be
        /// loaded and unloaded under particular circumstances.
        /// </summary>
        [Serializable]
        public class Chunk
        {
            public string sceneName;
            public Vector3 center;
            public float loadDistance;
            public float unloadDistance;
            public BundleDef[] bundleList = new BundleDef[]{ };

            public UnityEvent onLoad;
            public UnityEvent onUnload;

            public override string ToString()
            {
                return string.Format("sceneName:{0},loadDistance:{1},unloadDistance:{2}", sceneName, loadDistance, unloadDistance);
            }
        }

        /// <summary>
        /// The subject that is monitored to activate/deactivate chunks.
        /// </summary>
        public Transform subject;

        /// <summary>
        /// The interval for checking player position.
        /// </summary>
        public float interval;

        /// <summary>
        /// Array of all defined chunks.
        /// </summary>
        public Chunk[] chunks;

        /// <summary>
        /// Indicates if the ChunkManager have to refresh asset bundles versions (Versions.txt file)
        /// to refresh bundle's version on the server (only for remote bundles).
        /// </summary>
        public bool downloadVersionsAtStartup;

        /// <summary>
        /// DistanceBias allow to globally tweak enter and exit raius of all chunks togheter.
        /// </summary>
        public float distanceBias = 1f;

        ///// <summary>
        ///// The current progress for current download operation.
        ///// </summary>
        //public AssetBundleMagic.Progress currentProgress;

        public Dictionary<string, AssetBundleMagic.Progress> sceneProgress = new Dictionary<string, AssetBundleMagic.Progress>();

        public bool isBusy = false;

        public float GetProgress()
        {
            float progress = 0;
            if (sceneProgress.Count > 0)
            {
                foreach (var item in sceneProgress.Values)
                {
                    if (item == null) continue;
                    progress += item.GetProgress();
                }
                progress = progress / sceneProgress.Count;
            }
            return progress;
        }

        /// <summary>
        /// Last time ChunkManager has checked player's position.
        /// </summary>
        private float lastCheckTime = 0f;

        /// <summary>
        /// Indicates that ChunkManager is working. If this member is false, it means that
        /// ChunkManager don't yet downloaded Versions.txt files for asset bundles. 
        /// </summary>
        private bool started = false;

        public static ChunkManager Instance;

        void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// The Start() method check if the user requests download of Versions.txt file.
        /// If so starts the download, and set the "started" value to false.
        /// </summary>
        void Start ()
        {
            started = true;
            if (downloadVersionsAtStartup) {
                started = false;
                AssetBundleMagic.DownloadVersions (
                    delegate(string v) {
                        started = true;
                        Debug.Log("Versions.txt downloaded: " + v);
                    },
                    delegate(string error) {
                        started = true;
                        Debug.LogError (error);
                        Debug.LogWarning ("The ChunkManager can't download versions. This will disable cache on bundle versions.");
                    });
            }
        }
	
        /// <summary>
        /// In the Update method, if the started value is true, we check the player position
        /// respect all defined chunks via CheckDistances() method.
        /// </summary>
        void Update ()
        {
            if (started)
            {
                if (lastCheckTime + interval <= Time.time)
                {
                    CheckDistances();
                    lastCheckTime = Time.time;
                }
            }
        }

        public bool EnableUnload = true;

        AsyncOperation sceneLoadOperation;

        /// <summary>
        /// Is the main method to check if the player has entered a new chunk zone, or exited an older one.
        /// This method is called by Update() method each "interval" seconds.
        /// </summary>
        void CheckDistances ()
        {
            //// Don't enqueue multiple requests
            //if (currentProgress != null)
            //    return;

            if (isBusy)
            {
                return;
            }

            Chunk loadChunk = null;
            float loadChunkDistance = float.MaxValue;
            int loadChunkIndex = -1;
            //List<Chunk> loadChunks = new List<Chunk>();

            for (int i = 0; i < chunks.Length; i++) {
                var chunk = chunks[i];
                var sceneName = chunk.sceneName;
                var distance = Vector3.Distance(chunk.center, subject.position);
                var unloadDistance = distanceBias * chunk.unloadDistance;
                var loadDistance = distanceBias * chunk.loadDistance;
                //Debug.Log(string.Format("[{0}] distance={1},unload={2},load={3}",i,distance,unloadDistance,loadDistance));
                var scene = SceneManager.GetSceneByName(chunk.sceneName);
                if (distance >= unloadDistance && scene.isLoaded && EnableUnload) {
                    SceneManager.UnloadSceneAsync (chunk.sceneName);
                    var bundleList = chunk.bundleList;
                    if (bundleList.Length > 0) {
                        for (int j = 0; j < bundleList.Length; j++) {
                            AssetBundleMagic.UnloadBundle (bundleList[j].bundleName, false);
                        }
                    }
                    chunks [i].onUnload.Invoke ();
                }
                if (distance <= loadDistance && !scene.isLoaded) {
                      Debug.Log(string.Format("[{0}] distance={1},unload={2},load={3},distanceBias={4}", i, distance, unloadDistance, loadDistance, distanceBias));
                    if(distance < loadChunkDistance)//找出距离最近的
                    {
                        loadChunkIndex = i;
                        loadChunk = chunk;
                        loadChunkDistance = distance;
                    }
                }
            }

            if (loadChunk != null)
            {
                isBusy = true;
                var i = loadChunkIndex;
                var chunk = loadChunk;
                var sceneName = chunk.sceneName;
                LoadAllBundles(i, 0,
                        delegate (AssetBundleMagic.Progress progress) {
                            if (progress == null) return;
                            //currentProgress = progress;
                            var sName = sceneName;
                            sceneProgress[sName] = progress;

                            Log.Info("setProgress start");
                        },
                        delegate (int chunkIndex) {
                            Log.Info("CheckDistances finished !");
                            var sName = chunks[chunkIndex].sceneName;
                            sceneLoadOperation = SceneManager.LoadSceneAsync(sName, LoadSceneMode.Additive);
                            //currentProgress = new AssetBundleMagic.LoadSceneProgress(sceneLoadOperation);
                            sceneProgress[sName] = new AssetBundleMagic.LoadSceneProgress(sceneLoadOperation); ;
                            sceneLoadOperation.completed += (op) =>
                            {
                                Log.Info("sceneLoadOperation.completed");
                                //currentProgress = null;
                                sceneProgress.Remove(sName);
                                isBusy = false;
                                Log.Info(chunks[chunkIndex].onLoad);
                                chunks[chunkIndex].onLoad.Invoke();
                            };
                            //SceneManager.LoadScene(chunks[chunkIndex].sceneName, LoadSceneMode.Additive);
                            //currentProgress = null;//2019_03_20_cww:LoadSceneAsync时这个要等场景加载完成后再设置，这里马上设置会导致重复加载AssetBundle，出错
                            //chunks[chunkIndex].onLoad.Invoke();
                            //Log.Info("setProgress end");
                        },
                        delegate (string error) {
                            var sName = sceneName;
                            Debug.LogError(error);
                            //currentProgress = null;
                            isBusy = false;
                            sceneProgress.Remove(sName);
                        });
            }
        }


        public void LoadBundle(string bundleName, string sceneName, Action loadSceneFinished)
        {
            Debug.Log("ChunManager.LoadBundle : " + bundleName + "," + sceneName);

            var p = AssetBundleMagic.LoadBundle(bundleName,
                                                       delegate (AssetBundle ab)
                                                       {
                                                           Debug.Log("AssetBundleMagic.LoadBundle finished !");
                                                           sceneLoadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

                                                           if (sceneProgress.ContainsKey(sceneName))
                                                           {
                                                               AssetBundleMagic.LoadFileAndSceneProgress p2 = sceneProgress[sceneName] as AssetBundleMagic.LoadFileAndSceneProgress;
                                                               if (p2!=null)
                                                               {
                                                                   p2.SetSceneProgress(sceneLoadOperation);
                                                               }
                                                           }
                                                           else
                                                           {
                                                               sceneProgress[sceneName] = new AssetBundleMagic.LoadSceneProgress(sceneLoadOperation);
                                                           }

                                                           
                                                           sceneLoadOperation.completed += (op) =>
                                                           {
                                                               //Log.Info(chunks[chunkIndex].onLoad);
                                                               //chunks[chunkIndex].onLoad.Invoke();
                                                               if (loadSceneFinished != null)
                                                               {
                                                                   loadSceneFinished();
                                                               }

                                                               Log.Info("sceneLoadOperation.completed");
                                                               sceneProgress.Remove(sceneName);

                                                               if(sceneProgress.Count==0)//多个加载
                                                                isBusy = false;
                                                           };
                                                       });
            if (p != null)
            {
                isBusy = true;

                AssetBundleMagic.LoadFileAndSceneProgress p2 = p as AssetBundleMagic.LoadFileAndSceneProgress;
                if (p2 != null)
                {
                    p2.ShowSceneProgress = true;
                }

                sceneProgress[sceneName] = p;
            }
            else
            {
                if (loadSceneFinished != null)
                {
                    loadSceneFinished();
                }
                isBusy = false;
            }
        }

        public void LoadBundle(string bundleName,string assetName, Action<GameObject> objLoadFinished)
        {
            Debug.Log("ChunManager.LoadBundle : " + bundleName + "," + assetName);

            var p = AssetBundleMagic.LoadBundle(bundleName,
                                                       delegate (AssetBundle ab)
                                                       {
                                                           Debug.Log("AssetBundleMagic.LoadBundle finished !");
                                                           //sceneLoadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                                                           //sceneProgress[assetName] = new AssetBundleMagic.LoadSceneProgress(sceneLoadOperation);
                                                           //sceneLoadOperation.completed += (op) =>
                                                           //{
                                                           //    //if (loadSceneFinished != null)
                                                           //    //{
                                                           //    //    loadSceneFinished();
                                                           //    //}
                                                           //    Log.Info("sceneLoadOperation.completed");
                                                           //    sceneProgress.Remove(sceneName);
                                                           //    if (sceneProgress.Count == 0)//多个加载
                                                           //        isBusy = false;
                                                           //};

                                                           isBusy = false;
                                                           if (ab != null)
                                                           {
                                                               GameObject obj = ab.LoadAsset<GameObject>(assetName);
                                                               if (objLoadFinished != null)
                                                               {
                                                                   objLoadFinished(obj);
                                                               }
                                                           }
                                                           else
                                                           {
                                                               if (objLoadFinished != null)
                                                               {
                                                                   objLoadFinished(null);
                                                               }
                                                           }
                                                           
                                                       });
            if (p != null)
            {
                isBusy = true;
                //sceneProgress[assetName] = p;
            }
            else
            {
                //if (loadSceneFinished != null)
                //{
                //    loadSceneFinished();
                //}
                isBusy = false;
            }
        }

        /// <summary>
        /// This method is responsible to recursively load all bundle needed by a particular chunk
        /// using the AssetBundleMagic class.
        /// </summary>
        /// <param name="chunkIndex">The chunk index for which download bundles.</param>
        /// <param name="startingBundle">The starting bundle index to load (allow recursive work).</param>
        /// <param name="started">The callback delegate method called when a download is started.</param>
        /// <param name="finished">The callback delegate method called when a download is terminated.</param>
        /// <param name="error">The callback delegate method called when an error occurs.</param>
        void LoadAllBundles (int chunkIndex, int startingBundle, LoadAllBundlesStarted started, LoadAllBundlesFinished finished, AssetBundleMagic.LoadBundleErrorDelegate error)
        {
            Debug.Log(string.Format("LoadAllBundles 1 [{0}] [{1}]", chunkIndex, startingBundle));
            var bundleList = chunks[chunkIndex].bundleList;
            if (bundleList.Length > startingBundle)
            {
                var bundle = bundleList[startingBundle];
                Debug.Log(string.Format("LoadAllBundles 2 [{0}] [{1}] {2}", chunkIndex, startingBundle, bundle));
                if (bundle.fromFile)
                {
                    AssetBundleMagic.Progress p = AssetBundleMagic.LoadBundle(bundle.bundleName,
                                                        delegate (AssetBundle ab)
                                                        {
                                                            Debug.Log("AssetBundleMagic.LoadBundle finished !");
                                                            LoadAllBundles(chunkIndex, startingBundle + 1, started, finished, error);
                                                        });
                    Debug.Log("LoadAllBundles started !");
                    started(p);
                }
                else
                {
                    if (bundle.checkVersion)
                    {
                        AssetBundleMagic.DownloadUpdatedBundle(bundle.bundleName,
                            delegate (AssetBundleMagic.Progress progress)
                            {
                                started(progress);
                            },
                            delegate (AssetBundle ab)
                            {
                                //currentProgress = null;
                                var sName = chunks[chunkIndex].sceneName;
                                sceneProgress.Remove(sName);
                                isBusy = false;
                                LoadAllBundles(chunkIndex, startingBundle + 1, started, finished, error);
                            },
                            error
                        );
                    }
                    else
                    {
                        AssetBundleMagic.Progress p =
                            AssetBundleMagic.DownloadBundle(bundle.bundleName,
                                delegate (AssetBundle ab)
                                {
                                    LoadAllBundles(chunkIndex, startingBundle + 1, started, finished, error);
                                },
                                error
                            );
                        started(p);
                    }
                }
            }
            else
            {
                Debug.Log("LoadAllBundles finished!");
                finished(chunkIndex);
            }
        }
    }
}