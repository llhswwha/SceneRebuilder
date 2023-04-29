using Battlehub.RTCommon;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PreviewCreator : SingletonBehaviour<PreviewCreator>
{
    //public static PreviewCreator Instance;

    public void Hide()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void Show()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void Awake()
    {
        //Instance = this;

        if (m_image == null)
        {
            m_image = this.GetComponent<Image>();
        }
    }

    [SerializeField]
    private Image m_image = null;
    [SerializeField]
    private Object m_object = null;

    private Texture2D m_previewTexture;
    private Sprite m_previewSprite;

    public string imagePath;

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        CreateImage();
    //    }
    //}

    public void CreateImage(Image img,Object obj)
    {
        this.m_image = img;
        this.m_object = obj;
        CreateImage();
    }

    public void CreateImage(Object obj,string imagePath)
    {
        Debug.Log($"PreviewCreator.CreateImage obj:{obj} imagePath:{imagePath} Exists:{File.Exists(imagePath)}");
        this.m_object = obj;
        if(File.Exists(imagePath)){
            return;
        }
        CreateImage();
        SaveImage(imagePath);
    }

    public void SaveImage(string file)
    {
        this.imagePath = file;
        SaveImage();
    }

    [ContextMenu("SaveImage")]
    public void SaveImage()
    {
        SaveTextureToFile(imagePath, m_previewTexture);
    }

    public void SaveTextureToFile(string file, Texture2D tex)
    {
        if (string.IsNullOrEmpty(file) || tex == null) return;
        byte[] bytes = tex.EncodeToPNG();
        SaveToFile(file, bytes);
    }

    public void SaveToFile(string file, byte[] data)
    {
        FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write);
        fs.Write(data, 0, data.Length);
        fs.Flush();
        fs.Close();
        https://msdn.microsoft.com/library/ms182334.aspx
        fs.Dispose();
    }

    [ContextMenu("CreateImage")]
    public void CreateImage()
    {
        try
        {
            //Debug.Log("CreateImage 1");
            IResourcePreviewUtility previewUtil = IOC.Resolve<IResourcePreviewUtility>();
            if (previewUtil == null)
            {
                previewUtil = GameObject.FindObjectOfType<ResourcePreviewUtility>();
                if (previewUtil != null)
                {
                    IOC.Register<IResourcePreviewUtility>(previewUtil);
                }
            }
            // Debug.Log($"CreateImage 2 previewUtil:{previewUtil}");

            if (m_previewTexture != null)
            {
                Destroy(m_previewTexture);
            }

            if (m_previewSprite != null)
            {
                Destroy(m_previewSprite);
            }

            if (previewUtil == null)
            {
                Debug.LogWarning($"CreateImage previewUtil == null");
                return;
            }


            if (previewUtil.CanCreatePreview(m_object))
            {
                // Debug.Log($"CreateImage 3 previewUtil:{previewUtil}");
                m_previewTexture = previewUtil.CreatePreview(m_object);
                m_previewSprite = Sprite.Create(m_previewTexture,
                    new Rect(0, 0, m_previewTexture.width, m_previewTexture.height),
                    new Vector2(0.5f, 0.5f));

                m_image.sprite = m_previewSprite;
                Debug.Log($"CreateImage 4 previewUtil:{previewUtil}");
            }
            else{
                Debug.LogError($"CreateImage CanCreatePreview==false m_object:{m_object}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"CreateImage 4 Exception:{ex}");
        }
        
    }

    private void OnDestroy()
    {
        if (m_previewSprite != null)
        {
            Destroy(m_previewSprite);
        }

        if (m_previewTexture != null)
        {
            Destroy(m_previewTexture);
        }
    }
}
