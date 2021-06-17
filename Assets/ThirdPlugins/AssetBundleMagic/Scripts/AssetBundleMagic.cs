using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Jacovone.AssetBundleMagic.Util;

namespace Jacovone.AssetBundleMagic
{
    /// <summary>
    /// Bundles are used in several circumstances in UNITY development. With bundles, you can postpone the asset download 
    /// at startup time, you can update your game without re-publish it, you can manage asset variants to support low-level devices, 
    /// you can manage dynamic load/unload of entire parts of your game, and so on. 
    /// AssetBundleMagic is born with the goal of simplifying the workflow of asset bundles management, 
    /// by driving the user in a well-defined process to support all possible use cases.
    /// </summary>
    public class AssetBundleMagic : MonoBehaviour
    {

        /// <summary>
        /// This class act as a tool to monitor in progress downloads. Each method that
        /// download anythin, will return an instance of this class.
        /// </summary>
        public abstract class Progress
        {
            /// <summary>
            /// Return the current progress of download operation.
            /// </summary>
            /// <returns>The current progress of download operation as float [0..1].</returns>
            public abstract float GetProgress ();
        }

        /// <summary>
        /// A specific instance of Progress that monitors a bundle
        /// loading from a file.
        /// </summary>
        public class LoadFileProgress : Progress
        {
            private AssetBundleCreateRequest _r;

            public LoadFileProgress (AssetBundleCreateRequest r)
            {
                _r = r;
            }
            public override float GetProgress ()
            {
                return _r.progress;
            }
        }

        /// <summary>
        /// 用于指示场景加载进度 
        /// 地形:加载时模型加载时间为3s，场景加载时间为1s
        /// J1:加载时模型加载时间为8s，场景加载时间为8s
        /// </summary>
        public class LoadSceneProgress : Progress
        {
            private AsyncOperation _r;

            public LoadSceneProgress(AsyncOperation r)
            {
                _r = r;
            }
            public override float GetProgress()
            {
                if (_r == null) return 0;
                return _r.progress;
            }
        }

        /// <summary>
        /// A specific instance of Progress that monitors a bundle
        /// loading from a file.
        /// </summary>
        public class LoadFileAndSceneProgress : Progress
        {
            private AssetBundleCreateRequest _r1;

            private AsyncOperation _r2;

            public LoadFileAndSceneProgress(AssetBundleCreateRequest r)
            {
                _r1 = r;
            }

            /// <summary>
            /// 模型加载部分占比例多少
            /// </summary>
            public static float bundlePower = 0.9f;

            public override float GetProgress()
            {
                float progress = _r1.progress;
                if (ShowSceneProgress)
                {
                    if (_r2 != null)
                    {
                        progress = (progress* bundlePower + _r2.progress*(1- bundlePower));
                    }
                    else
                    {
                        progress = (progress * bundlePower + 0 * (1 - bundlePower));
                    }
                }
                return progress;
            }

            public bool ShowSceneProgress = false;

            public void SetSceneProgress(AsyncOperation r)
            {
                _r2 = r;
            }
        }

        /// <summary>
        /// Specific instance of Progress that monitors a download operation.
        /// </summary>
        public class DownloadProgress : Progress
        {

            private UnityWebRequest _wr;

            public DownloadProgress (UnityWebRequest wr)
            {
                _wr = wr;
            }

            public override float GetProgress ()
            {
                return _wr.downloadProgress;
            }
        }

        /// <summary>
        /// Signals that the bundle download was initiated.
        /// </summary>
        /// <param name="p">The name of the bundle being downloaded.</param>
        public delegate void LoadBundleStartedDelegate (Progress p);

        /// <summary>
        /// Signals that the bundle download is terminated.
        /// </summary>
        /// <param name="ab">The resulting bundle instance.</param>
        public delegate void LoadBundleFinisehdDelegate (AssetBundle ab);

        /// <summary>
        /// Signals that the bundle download was terminated with error.
        /// </summary>
        /// <param name="error">The error string.</param>
        public delegate void LoadBundleErrorDelegate (string error);

        /// <summary>
        /// Signals that Versions.txt download was successfully terminated.
        /// </summary>
        /// <param name="versions">The content of Versions.txt.</param>
        public delegate void DownloadVersionsFinisehdDelegate (string versions);

        /// <summary>
        /// Signals that download of Version.txt was terminated with error.
        /// </summary>
        /// <param name="error">The error string.</param>
        public delegate void DownloadVersionsErrorDelegate (string error);

        /// <summary>
        /// The internal instance of the class AssetBundleMagic.
        /// </summary>
        private static AssetBundleMagic _instance;

        /// <summary>
        /// The internal instance of the class AssetBundleMagic
        /// </summary>
        public static AssetBundleMagic Instance {
            get {
                return _instance;
            }
        }

        /// <summary>
        /// The internal map of CRCs
        /// </summary>
        private Dictionary<string, uint> _crcs;

        /// <summary>
        /// The internal map of CRCs
        /// </summary>
        public static Dictionary<string,uint> CRCs {
            get {
                if (Instance._crcs == null) {
                    Instance._crcs = new Dictionary<string,uint> ();
                }
                return Instance._crcs;
            }
        }

        /// <summary>
        /// The internal map of the bundle versions.
        /// </summary>
        private Dictionary<string, uint> _versions;

        /// <summary>
        /// The internal map of the bundle versions.
        /// </summary>
        public static Dictionary<string,uint> Versions {
            get {
                if (Instance._versions == null) {
                    Instance._versions = new Dictionary<string,uint> ();
                }
                return Instance._versions;
            }
        }

        /// <summary>
        /// The internal map of bundles, by name.
        /// </summary>
        private Dictionary<string,AssetBundle> _bundles;

        /// <summary>
        /// The internal map of bundles, by name.
        /// </summary>
        public static Dictionary<string,AssetBundle> Bundles {
            get {
                if (Instance._bundles == null) {
                    Instance._bundles = new Dictionary<string,AssetBundle> ();
                }
                return Instance._bundles;
            }
        }

        /// <summary>
        /// The base URL, from which download bundles and Versions.txt file.
        /// </summary>
        public string BundlesBaseUrl = "http://127.0.0.1:8000";

        /// <summary>
        /// The base path of bundles, where AssetBundleMagic will generate bundles, and from
        /// which bundles are loaded, when the user load bundles locally via scripting.
        /// </summary>
        public string BundlesBasePath = "Assets/StreamingAssets";

        /// <summary>
        /// Disable the HTTP server cache. If ttrue, AssetBundleMagic, will insert
        /// some headers in the HTTP request to try to disable server HTTP cache.
        /// </summary>
        public bool DisableHTTPServerCache = true;

        /// <summary>
        /// Manages the AssetBundleMagic Test mode. In test mode, AssetBundleMagic will
        /// laod bundles from local, also if you use APIs that try to download from the network.
        /// </summary>
        public bool TestMode = false;

        /// <summary>
        /// Build bundles for iOS?
        /// </summary>
        public bool BuildIosBundle;

        /// <summary>
        /// Build bundles for Android?
        /// </summary>
        public bool BuildAndroidBundle;

        /// <summary>
        /// Build bundles for OSX (Universal)?
        /// </summary>
        public bool BuildOSXBundle;
        ///增加webgl
        public bool BuildWebGLPlayerBundle;
        /// <summary>
        /// Build bundles for Windows?
        /// </summary>
        public bool BuildWindowsBundle;

        [SerializeField]
        public VersionsDictionary BuildVersions;

        /// <summary>
        /// Akake standard MonoBehavior method, manage Don't Destroy On Load mechanism.
        /// </summary>
        void Awake ()
        {
            DontDestroyOnLoad (this);

            if (Instance == null) {
                _instance = this;
            } else {
                DestroyObject (gameObject);
            }
        }

        /// <summary>
        /// Unload a specific bundle if loaded, and removes the bundle name from the list
        /// of current loaded bundles inside AssetBundleMagic package.
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="unloadAssets"></param>
        public static bool UnloadBundle (string bundleName, bool unloadAssets)
        {
            Debug.Log("AssetBundleMagic.UnloadBundle:"+bundleName+","+ unloadAssets);
            if (Bundles.ContainsKey (bundleName)) {
                if (Bundles [bundleName] != null)
                    Bundles [bundleName].Unload (unloadAssets);
                Bundles.Remove (bundleName);
                return true;
            }
            else
            {
                Debug.LogError("AssetBundleMagic.UnloadBundle 不存在该Bundle:" + bundleName + "," + unloadAssets);
                return false;
            }
        }

        public static bool UnloadBundleEx(string bundleName)
        {
            //return UnloadBundle(bundleName, true);
            WaitForUnload(bundleName);
            return true;
        }

        public static List<string> unloadBundleWaitList = new List<string>();

        /// <summary>
        /// 2019_05_22_cww:加入等待卸载的列表，配合DoUnload使用，DoUnload在OnDestory里面调用。
        /// 处理卸载->延迟Destroy->加载->已经存在（还没卸载）->不加载 -> Destroy -> 卸载 -> 出错 的问题，这个问题会显示为只剩下紫色的门，一般在摄像机快速来回移动（自动调整摄像机距离代码导致的）时可能会出现。
        /// </summary>
        /// <param name="bundleName"></param>
        public static void WaitForUnload(string bundleName)
        {
            if (!unloadBundleWaitList.Contains(bundleName))
            {
                unloadBundleWaitList.Add(bundleName);
            }
            else
            {
                Debug.LogError("AssetBundleMagic.WaitForUnload unloadBundleWaitList.Contains(bundleName) bundleName="+ bundleName);
            }
        }

        public static void DoUnload()
        {
            foreach (var item in unloadBundleWaitList)
            {
                UnloadBundle(item, true);
            }
            unloadBundleWaitList.Clear();
        }

        public static bool ContainsBundle(string bundleName)
        {
            return Bundles.ContainsKey(bundleName);
        }

        public static void UnloadBundles(string[] bundleNames, bool unloadAssets)
        {
            foreach (var item in bundleNames)
            {
                UnloadBundle(item, unloadAssets);
            }
        }

        /// <summary>
        /// This is the simplest method to load a bundle. You specify the name of the bundle (bundleName) 
        /// and AssetBundleMagic will return the bundle as method result, synchronously. This method is 
        /// not the better way to load an asset bundle, except for very particular situations.
        /// </summary>
        /// <param name="bundleName">The name of the requested bundle.</param>
        /// <returns>The loaded bundle instance.</returns>
        public static AssetBundle LoadBundle (string bundleName)
        {
            AssetBundle ab = AssetBundle.LoadFromFile (Application.streamingAssetsPath + "/" + CurrentPlatformString () + "/" + bundleName);
            Bundles.Add (bundleName, ab);
            return ab;
        }

        /// <summary>
        /// This is the asynchronous version of the LoadBundle(string) method, and let you supply a delegate to receive 
        /// the bundle when it is ready to use. In the meantime, you can use the AssetBundleMagic.Progress instance 
        /// returned from the method, to check the progress status of the download. The Progress class exposes 
        /// the GetProgress() method to check it.
        /// </summary>
        /// <param name="bundleName">The name of the requested bundle.</param>
        /// <param name="finished">Delegate method called when the bundle is successfully loaded, it provide the bundle instance.</param>
        /// <returns>An AssetBundleMagic.Progress instance suitable to follow the current download progress.</returns>
        public static Progress LoadBundle (string bundleName, LoadBundleFinisehdDelegate finished)
        {
            if (Bundles.ContainsKey(bundleName))
            {
                if (unloadBundleWaitList.Contains(bundleName))
                {
                    unloadBundleWaitList.Remove(bundleName);//不用卸载了
                }

                AssetBundle ab = Bundles[bundleName];
                Debug.Log("AssetBundleMagic LoadBundle Bundles.ContainsKey:" + bundleName);
                if (finished != null)
                {
                    finished(ab);
                }
                return null;
            }
            Debug.Log("LoadBundle bundleName:"+ bundleName);
            AssetBundleCreateRequest r = AssetBundle.LoadFromFileAsync (Application.streamingAssetsPath + "/" + CurrentPlatformString () + "/" + bundleName);
            Instance.StartCoroutine (Instance.LoadBundleCoroutine (r, bundleName, finished));
            return new LoadFileAndSceneProgress(r);
        }

        /// <summary>
        /// The Coroutine used to implement LoadBundle in asynchronous mode.
        /// </summary>
        /// <param name="r">The request.</param>
        /// <param name="bundleName">The name of the bundle.</param>
        /// <param name="finished">Callback method delegate called when the load of the bundles has terminated successfully.</param>
        /// <returns></returns>
        private IEnumerator LoadBundleCoroutine (AssetBundleCreateRequest r, string bundleName, LoadBundleFinisehdDelegate finished)
        {
            //Debug.Log("LoadBundleCoroutine bundleName1:" + bundleName);
            yield return r;
            //Debug.Log("LoadBundleCoroutine bundleName2:" + bundleName);
            if (!Bundles.ContainsKey(bundleName))
            {
                //Bundles.Add(bundleName, r.assetBundle);
                //finished.Invoke(r.assetBundle);

                AssetBundle bundle = r.assetBundle;
                Bundles.Add(bundleName, bundle);
                //Debug.Log("LoadBundleCoroutine bundleName3:" + bundleName);
                //Debug.Log("LoadBundleCoroutine finished !");
                finished.Invoke(bundle);
                //Debug.Log("LoadBundleCoroutine bundleName4:" + bundleName);
            }
            else
            {
                Debug.LogError("LoadBundleCoroutine Bundles.ContainsKey:"+ bundleName);
            }
        }

        /// <summary>
        /// This method is equivalent to the previous one, except for the fact that AssetBundleMagic first check 
        /// versions of bundles by downloading a fresh copy of Version.txt file. For that reason, this method doesn’t 
        /// start immediately the download of the bundle (because it must download Versions.txt first), and returns nothing (void). 
        /// When the download of the bundle starts, the LoadBundleStartedDelegate is called, and the Progress is passed to it, 
        /// so you can monitor the download progress. As in the previous version of the method, delegate methods are called to 
        /// signal the download finished or download error situations.
        /// Tip: Use this method if you want to be secure to download the most updated version of a bundle.
        /// </summary>
        /// <param name="bundleName">The bundle name.</param>
        /// <param name="started">Callback delegate method called when the download starts.</param>
        /// <param name="finished">Callback delegate method called when the download finished successfully.</param>
        /// <param name="error">Callback delegate method called when the download finished with error.</param>
        public static void DownloadUpdatedBundle (string bundleName, LoadBundleStartedDelegate started, LoadBundleFinisehdDelegate finished, LoadBundleErrorDelegate error)
        {
            if (Instance.TestMode) {
                started (LoadBundle (bundleName, finished));
            } else {
                DownloadVersions (delegate (string versions) {

                    Progress p = DownloadBundle (bundleName, delegate (AssetBundle ab) {
                        finished (ab);
                    }, delegate (string errorString) {
                        error (errorString);
                    });

                    started (p);

                }, delegate (string errorString) {
                    error ("Error updating bundle versions: " + errorString);
                });
            }
        }

        /// <summary>
        /// This is the main method to download a bundle. You specify the bundle name, and AssetBundleMagic download the bundle 
        /// giving you the chance to monitor the download process via AssetBundleMagic.Process instance, immediately returned by this method.
        /// When the download has finished, the LoadBundleFinishedDelegate is called, passing the downloaded asset bundle.In case of error, 
        /// the LoadBundleErrorDelegate is called, passing the error string.
        /// Tip: Call this method to download bundles from a server, without the need to check last-minute updates of the downloaded bundle.
        /// </summary>
        /// <param name="bundleName">The bundle name</param>
        /// <param name="finished">Callback delegate method called when the download has finished.</param>
        /// <param name="error">Callback delegate method called when the download terminated with error.</param>
        /// <returns></returns>
        public static Progress DownloadBundle (string bundleName, LoadBundleFinisehdDelegate finished, LoadBundleErrorDelegate error)
        {
            if (Instance.TestMode) {
                return LoadBundle (bundleName, finished);
            } else {
                string url = Instance.BundlesBaseUrl + "/" + CurrentPlatformString () + "/" + bundleName;
                UnityWebRequest wr;

                if (Versions.ContainsKey (bundleName)) {
                    wr = UnityWebRequestAssetBundle.GetAssetBundle (url,
                        Versions [bundleName], CRCs [url]);
                } else {
                    wr = UnityWebRequestAssetBundle.GetAssetBundle (url);
                }

                if (Instance.DisableHTTPServerCache) {
                    wr.SetRequestHeader ("Cache-Control", "no-cache, no-store, must-revalidate");
                    wr.SetRequestHeader ("Pragma", "no-cache");
                    wr.SetRequestHeader ("Expires", "0");
                }

                Instance.StartCoroutine (Instance.DownloadBundleCoroutine (wr, bundleName, finished, error));
                return new DownloadProgress (wr);
            }
        }

        /// <summary>
        /// The Coroutine used to implement DownloadBundle method.
        /// </summary>
        /// <param name="wr">The UnityWebRequest already prepared for the download-</param>
        /// <param name="finished">The called delegate method when the bundle was successfully downloaded.</param>
        /// <param name="error">The called delegate method when there is an error in downloading the bundle.</param>
        /// <param name="bundleName">The name of the bundle.</param>
        /// <returns></returns>
        private IEnumerator DownloadBundleCoroutine (UnityWebRequest wr, string bundleName, LoadBundleFinisehdDelegate finished, LoadBundleErrorDelegate error)
        {
            yield return wr.Send ();

            if (wr.isNetworkError) {
                error (wr.error);
            } else {
                
                AssetBundle ab = ((DownloadHandlerAssetBundle)wr.downloadHandler).assetBundle;

                if (ab == null) {
                    error ("Error loading bundle, probably another bundle with same files is already loaded.");
                } else {
                    if (Bundles.ContainsKey (bundleName)) {
                        Bundles.Remove (bundleName);
                    }

                    Bundles.Add (bundleName, ab);
                    finished.Invoke (ab);
                }
            }
        }

        /// <summary>
        /// This is the call you have to made, for update versions information inside AssetBundleMagic engine. 
        /// Typically, this is the first call to the AssetBundleMagic package, and if you aren’t interested in 
        /// hot update of bundles, this call is made only once per run of the game. If you don’t call this method, 
        /// AssetBundleMagic doesn’t know versions on the server, and the it always proceeds to download bundles when requested.
        /// The success delegate is called once the versions file is download and processed.The content of the Versions.txt file is passed to the delegate only for debugging purposes; when the delegate is called, its content has already been processed, and internal versions state of AssetBundleMagic package has already been updated.
        /// Tip: Call this method at least one time, at the game start.
        /// </summary>
        /// <param name="finished">The called delegate method when the bundle was successfully downloaded.</param>
        /// <param name="error">The called delegate method when there is an error in downloading the bundle.</param>
        public static void DownloadVersions (DownloadVersionsFinisehdDelegate finished, DownloadVersionsErrorDelegate error)
        {
            if (Instance.TestMode) {
                finished ("Fake Version.txt content");
            } else {
                UnityWebRequest wr = UnityWebRequest.Get (Instance.BundlesBaseUrl + "/" + CurrentPlatformString () + "/Versions.txt");

                // If requested, the package try to download server-side cache management by setting
                // up some header in the request
                if (Instance.DisableHTTPServerCache) {
                    wr.SetRequestHeader ("Cache-Control", "no-cache, no-store, must-revalidate");
                    wr.SetRequestHeader ("Pragma", "no-cache");
                    wr.SetRequestHeader ("Expires", "0");
                }

                Instance.StartCoroutine (Instance.DownloadVersionsCoroutine (wr, finished, error));
            }
        }

        /// <summary>
        /// The Coroutine used to implement DownloadVersions mthod.
        /// </summary>
        /// <param name="wr">The UnityWebRequest already prepared for the download-</param>
        /// <param name="finished">The called delegate method when the bundle was successfully downloaded.</param>
        /// <param name="error">The called delegate method when there is an error in downloading the bundle.</param>
        /// <returns></returns>
        private IEnumerator DownloadVersionsCoroutine (UnityWebRequest wr, DownloadVersionsFinisehdDelegate finished, DownloadVersionsErrorDelegate error)
        {
            yield return wr.Send ();

            if (wr.isNetworkError) {
                error (wr.error);
            } else {
                string result = wr.downloadHandler.text;
                Debug.Log("version result");
                Debug.Log(result);
                // Deserialize the versions.txt file
                VersionDataCollection dataCollection = JsonUtility.FromJson<VersionDataCollection> (result);

                if (dataCollection == null) {
                    error ("Unable to parse version JSON: " + result);
                    yield break;
                } else {
                    // Update the internal state of versions and CRCs
                    VersionData[] datas = dataCollection.bundles;

                    for (int idx = 0; idx < datas.Length; idx++) {
                        if (Versions.ContainsKey (datas [idx].bundleName)) {
                            Versions [datas [idx].bundleName] = datas [idx].version;
                        } else {
                            Versions.Add (datas [idx].bundleName, datas [idx].version);
                        }
                        Debug.Log(CurrentPlatformString());
                        if (CRCs.ContainsKey (Instance.BundlesBaseUrl + "/" + CurrentPlatformString () + "/" + datas [idx].bundleName)) {
                            CRCs [Instance.BundlesBaseUrl + "/" + CurrentPlatformString () + "/" + datas [idx].bundleName] = datas [idx].crc;
                        } else {
                            CRCs.Add (Instance.BundlesBaseUrl + "/" + CurrentPlatformString () + "/" + datas [idx].bundleName, datas [idx].crc);
                        }
                    }
                }

                finished.Invoke (result);
            }
        }

        /// <summary>
        /// Cleans the Asset Bundle cache managed by UNITY. If th eUNITY cache is
        /// in use the cache is not cleaned.
        /// </summary>
        public static void CleanBundlesCache ()
        {
            if (!Caching.ClearCache ()) {
                Debug.Log ("Cache Cleaning not succeeded. Cache in use.");
            }
            ;
        }

        /// <summary>
        /// Return a string representing the current application platform. Used to compose
        /// directory name when create bundles, and when search for bundles locally.
        /// </summary>
        /// <returns></returns>
        private static string CurrentPlatformString ()
        {
            switch (Application.platform) {
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.OSXEditor:
                return "macOS";
            case RuntimePlatform.Android:
                return "Android";
            case RuntimePlatform.IPhonePlayer:
                return "iOS";
            case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    return "Windows";
                ///增加webgl
                case RuntimePlatform.WebGLPlayer:
                   return "Windows";
            default:
                    //  return "unknow";
                    ///增加webgl后unknown下没有模型写成Windows
                    return "Windows";  

            }
        }
    }
}
