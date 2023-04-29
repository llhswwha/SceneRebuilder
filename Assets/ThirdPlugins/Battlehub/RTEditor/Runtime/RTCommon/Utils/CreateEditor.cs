using System;
using UnityEngine;
using UnityEngine.UI;
using Battlehub.RTEditor;

namespace Battlehub.RTCommon
{
    [DefaultExecutionOrder(-100)]
    public class CreateEditor : MonoBehaviour, IRTEState
    {
        public event Action<object> Created;

        public event Action<object> Destroyed;

        public event Action<object> Closed;

        public bool IsCreated
        {
            get { return m_editor != null; }
        }

        [SerializeField]
        private Button m_createEditorButton = null;

        public Button EditButon
        {
            get{
                return m_createEditorButton;
            }
        }

        [SerializeField]
        private RTEBase m_editorPrefab = null;
        [SerializeField]
        private Splash m_splashPrefab = null;

        private RTEBase m_editor;

        private void Awake()
        {
            IOC.RegisterFallback<IRTEState>(this);
            m_editor = (RTEBase)FindObjectOfType(m_editorPrefab.GetType());

            Debug.LogError("CreateEditor.Awake m_editor:"+m_editor);
            if (m_editor != null)
            {
                if (m_editor.IsOpened)
                {
                    m_editor.IsOpenedChanged += OnIsOpenedChanged;
                    gameObject.SetActive(false);
                }
            }
            m_createEditorButton.onClick.AddListener(OnOpen);
        }

        private void OnDestroy()
        {
            IOC.UnregisterFallback<IRTEState>(this);
            if (m_createEditorButton != null)
            {
                m_createEditorButton.onClick.RemoveListener(OnOpen);
            }
            if (m_editor != null)
            {
                m_editor.IsOpenedChanged -= OnIsOpenedChanged;
            }
        }

        public event Action<object> BeforeOpen;

        public void OnOpen()
        {
            if(BeforeOpen!=null){
                BeforeOpen(this);
            }
            Debug.Log("OnOpen");
            if (m_splashPrefab != null)
            {
                Debug.Log("OnOpen 1:"+m_splashPrefab);
                Splash splash=Instantiate(m_splashPrefab);
                Debug.Log("splash :"+splash);
                splash.Show(() => InstantiateRuntimeEditor());
            }
            else
            {
                 Debug.Log("OnOpen 2");
                InstantiateRuntimeEditor();
            }
        }

        private void InstantiateRuntimeEditor()
        {
            Debug.Log("InstantiateRuntimeEditor:"+m_editorPrefab);
            m_editor = Instantiate(m_editorPrefab);
            m_editor.name = "RuntimeEditor";
            m_editor.IsOpenedChanged += OnIsOpenedChanged;
            m_editor.transform.SetAsFirstSibling();
            if (Created != null)
            {
                Created(m_editor);
            }
            gameObject.SetActive(false);

            Editor=m_editor.GetComponent<RuntimeEditor>();
        }

        public RuntimeEditor Editor;

        private void OnIsOpenedChanged()
        {
            if (m_editor != null)
            {
                if (!m_editor.IsOpened)
                {
                    m_editor.IsOpenedChanged -= OnIsOpenedChanged;

                    if (this != null)
                    {
                        gameObject.SetActive(true);
                    }

                    Destroy(m_editor);
                    if(Destroyed!=null){
                        Destroyed(m_editor);
                    }
                    if (Closed != null)
                    {
                        Closed(m_editor);
                    }
                }
            }
        }
    }
}

