using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Bundles;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Examples.Bundle;
#if !UNITY_WEBGL
using Loxodon.Framework.Security.Cryptography;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ModelsLauncher : MonoBehaviour
{

    private Rect position = new Rect(5, 5, 150, 40);
    private GUIContent contentON = new GUIContent("Simulation Mode: ON");
    private GUIContent contentOFF = new GUIContent("Simulation Mode: OFF");
    private GUIStyle style;

    private string iv = "5Hh2390dQlVh0AqC";
    private string key = "E4YZgiGQ0aqe5LEJ";
    private IResources resources;

#if UNITY_WEBGL
    void Awake()
    {
        Debug.Log($"ModelsLaucher.Awake");
        DontDestroyOnLoad(this.gameObject);
    }

    IEnumerator Start()
    {
        Debug.Log($"ModelsLaucher.Start 1");
        ApplicationContext context = Context.GetApplicationContext();
        Debug.Log($"ModelsLaucher.Start 2 context:{context}");
        /* Create a BundleManifestLoader. */
        IBundleManifestLoader manifestLoader = new BundleManifestLoader();


        /* Loads BundleManifest. */
        IAsyncResult<BundleManifest> result = manifestLoader.LoadAsync(BundleUtil.GetReadOnlyDirectory() + BundleSetting.ManifestFilename);
        yield return result.WaitForDone();
        BundleManifest manifest = result.Result;

        //manifest.ActiveVariants = new string[] { "", "sd" };
        manifest.ActiveVariants = new string[] { "", "hd" };

        this.resources = CreateResources(manifest);
        Debug.Log($"ModelsLaucher.Start resources:{resources}");
        context.GetContainer().Register<IResources>(this.resources);
    }

    IResources CreateResources(BundleManifest manifest)
    {
        IResources resources = null;
#if UNITY_EDITOR
        if (SimulationSetting.IsSimulationMode)
        {
            Debug.Log("Use SimulationResources. Run In Editor");

            /* Create a PathInfoParser. */
            //IPathInfoParser pathInfoParser = new SimplePathInfoParser("@");
            IPathInfoParser pathInfoParser = new SimulationAutoMappingPathInfoParser();

            /* Create a BundleManager */
            IBundleManager manager = new SimulationBundleManager();

            /* Create a BundleResources */
            resources = new SimulationResources(pathInfoParser, manager);
        }
        else
#endif
        {
            /* Create a PathInfoParser. */
            //IPathInfoParser pathInfoParser = new SimplePathInfoParser("@");
            IPathInfoParser pathInfoParser = new AutoMappingPathInfoParser(manifest);

            /* Create a BundleLoaderBuilder */
            //ILoaderBuilder builder = new WWWBundleLoaderBuilder(new Uri(BundleUtil.GetReadOnlyDirectory()), false);

            /* AES128_CBC_PKCS7 */
            RijndaelCryptograph rijndaelCryptograph = new RijndaelCryptograph(128, Encoding.ASCII.GetBytes(this.key), Encoding.ASCII.GetBytes(this.iv));

            /* Use a custom BundleLoaderBuilder */
            ILoaderBuilder builder = new CustomBundleLoaderBuilder(new Uri(BundleUtil.GetReadOnlyDirectory()), false, rijndaelCryptograph);

            /* Create a BundleManager */
            IBundleManager manager = new BundleManager(manifest, builder);

            /* Create a BundleResources */
            resources = new BundleResources(pathInfoParser, manager);
        }
        Debug.Log($"ModelsLaucher.CreateResources 1 resources:{resources}");
        return resources;
    }
#else
            void Awake()
        {           
            DontDestroyOnLoad(this.gameObject);
            this.resources = CreateResources();

            ApplicationContext context = Context.GetApplicationContext();
            context.GetContainer().Register<IResources>(this.resources);
        }

        IResources CreateResources()
        {
            IResources resources = null;
#if UNITY_EDITOR
            if (SimulationSetting.IsSimulationMode)
            {
                Debug.Log("Use SimulationResources. Run In Editor");

                /* Create a PathInfoParser. */
                //IPathInfoParser pathInfoParser = new SimplePathInfoParser("@");
                IPathInfoParser pathInfoParser = new SimulationAutoMappingPathInfoParser();

                /* Create a BundleManager */
                IBundleManager manager = new SimulationBundleManager();

                /* Create a BundleResources */
                resources = new SimulationResources(pathInfoParser, manager);
            }
            else
#endif
            {
                /* Create a BundleManifestLoader. */
                IBundleManifestLoader manifestLoader = new BundleManifestLoader();

                //NotSupportedException: Because WebGL is single-threaded, this method is not supported,please use LoadAsync instead.at Loxodon.Framework.Bundles.BundleManifestLoader.Load
                //WebGL是单线程不支持以下方法，用LoadAsync代替
                /* Loads BundleManifest. */
                BundleManifest manifest = manifestLoader.Load(BundleUtil.GetReadOnlyDirectory() + BundleSetting.ManifestFilename);

                //manifest.ActiveVariants = new string[] { "", "sd" };
                manifest.ActiveVariants = new string[] { "", "hd" };

                /* Create a PathInfoParser. */
                //IPathInfoParser pathInfoParser = new SimplePathInfoParser("@");
                IPathInfoParser pathInfoParser = new AutoMappingPathInfoParser(manifest);

                /* Create a BundleLoaderBuilder */
                //ILoaderBuilder builder = new WWWBundleLoaderBuilder(new Uri(BundleUtil.GetReadOnlyDirectory()), false);

                /* AES128_CBC_PKCS7 */
                //RijndaelCryptograph rijndaelCryptograph = new RijndaelCryptograph(128, Encoding.ASCII.GetBytes(this.key), Encoding.ASCII.GetBytes(this.iv));
                IStreamDecryptor decryptor = CryptographUtil.GetDecryptor(Algorithm.AES128_CBC_PKCS7, Encoding.ASCII.GetBytes(this.key), Encoding.ASCII.GetBytes(this.iv));

                /* Use a custom BundleLoaderBuilder */
                ILoaderBuilder builder = new CustomBundleLoaderBuilder(new Uri(BundleUtil.GetReadOnlyDirectory()), false, decryptor);

                /* Create a BundleManager */
                IBundleManager manager = new BundleManager(manifest, builder);

                /* Create a BundleResources */
                resources = new BundleResources(pathInfoParser, manager);
            }

            Debug.Log($"ModelsLaucher.CreateResources 2 resources:{resources}");
            return resources;
        }
#endif

#if UNITY_EDITOR
    void OnGUI()
    {
        //if (style == null)
        //    style = new GUIStyle("AssetLabel");

        //if (SimulationSetting.IsSimulationMode)
        //    GUI.Label(position, contentON, style);
        //else
        //    GUI.Label(position, contentOFF, style);
    }
#endif
}
