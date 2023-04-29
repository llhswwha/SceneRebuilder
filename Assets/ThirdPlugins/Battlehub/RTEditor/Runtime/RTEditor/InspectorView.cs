using System;
using System.Linq;
using UnityEngine;

using Battlehub.RTCommon;
using UnityObject = UnityEngine.Object;
using Battlehub.RTSL.Interface;

namespace Battlehub.RTEditor
{
    public class InspectorView : RuntimeWindow
    {
        [SerializeField]
        private Transform m_panel = null;

        [SerializeField]
        private GameObject m_addComponentRoot = null;

        [SerializeField]
        private AddComponentControl m_addComponentControl = null;

        private GameObject m_editor;

        private IEditorsMap m_editorsMap;

        private ISettingsComponent m_settingsComponent;

        public static InspectorView Instance;

        protected override void AwakeOverride()
        {
            Instance=this;

            WindowType = RuntimeWindowType.Inspector;
            base.AwakeOverride();

            m_editorsMap = IOC.Resolve<IEditorsMap>();
            m_settingsComponent = IOC.Resolve<ISettingsComponent>();

            Editor.Selection.SelectionChanged += OnRuntimeSelectionChanged;
            CreateEditor();
        }

        protected override void UpdateOverride()
        {
            base.UpdateOverride();
            UnityObject obj = Editor.Selection.activeObject;
            if(obj == null)
            {
                DestroyEditor();
            }
        }

        protected override void OnDestroyOverride()
        {
            base.OnDestroyOverride();
            if(Editor != null && Editor.Selection!=null)
            {
                Editor.Selection.SelectionChanged -= OnRuntimeSelectionChanged;
            }
        }

        private void OnRuntimeSelectionChanged(UnityObject[] unselectedObjects)
        {
            if (m_editor != null &&  unselectedObjects != null && unselectedObjects.Length > 0)
            {
                IRuntimeEditor editor = IOC.Resolve<IRuntimeEditor>();
                if(editor.IsDirty)
                {
                    editor.IsDirty = false;
                    editor.SaveAssets(unselectedObjects, result =>
                    {
                        CreateEditor();
                    });
                }
                else
                {
                    CreateEditor();
                }
            }
            else
            {
                CreateEditor();
            }
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            IRuntimeEditor editor = IOC.Resolve<IRuntimeEditor>();
            if (editor.IsDirty && editor.Selection.activeObject != null)
            {
                editor.IsDirty = false;
                editor.SaveAssets(editor.Selection.objects, result =>
                {
                });
            }
        }

        private void DestroyEditor()
        {
            if (m_editor != null)
            {
                Destroy(m_editor);
            }

            if(m_addComponentRoot != null)
            {
                m_addComponentRoot.SetActive(false);
            }

            if (m_addComponentControl != null)
            {
                m_addComponentControl.ComponentSelected -= OnAddComponent;
            }
        }

        private bool OfSameType(UnityObject[] objects, out Type type)
        {
            type = objects[0].GetType();
            for(int i = 1; i < objects.Length; ++i)
            {
                if(type != objects[i].GetType())
                {
                    return false;
                }
            }
            return true;
        }

        public GameObject ShowMaterialInfo(Material mat){
            
            DestroyEditor();

            if(mat==null){
                Debug.LogError("ShowMaterialInfo mat==null");
                return null;
            }
            GameObject editorPrefab = m_editorsMap.GetMaterialEditor(mat.shader);

                    m_editor = Instantiate(editorPrefab);//GameObjectEditor.Awake
                    m_editor.transform.SetParent(m_panel, false);
                    m_editor.transform.SetAsFirstSibling();

                    MaterialEditor materialEditor=m_editor.GetComponent<MaterialEditor>();
                    materialEditor.Material=mat;

                    Debug.Log("InspectorView.CreateEditor GetMaterialEditor2");
            return editorPrefab;
        }

        public GameObject ShowMaterialInfo(GameObject go){
            GameObject editorPrefab=null;

            if(go==null){
                Debug.LogError("ShowMaterialInfo go==null");
                return editorPrefab;
            }
            
            MeshRenderer renderer=go.GetComponent<MeshRenderer>();
                if(renderer){
                    Material mat=renderer.sharedMaterial;
                    if (mat.shader == null)
                    {
                        return editorPrefab;
                    }

                    Shader shader = mat.shader;
                    for(int i = 0; i < renderer.sharedMaterials.Length; ++i)
                    {
                        Material material = (Material)renderer.sharedMaterials[i];
                        if(material.shader != shader)
                        {
                            return editorPrefab;
                        }
                    }
                    editorPrefab = m_editorsMap.GetMaterialEditor(mat.shader);

                    m_editor = Instantiate(editorPrefab);//GameObjectEditor.Awake
                    m_editor.transform.SetParent(m_panel, false);
                    m_editor.transform.SetAsFirstSibling();

                    MaterialEditor materialEditor=m_editor.GetComponent<MaterialEditor>();
                    materialEditor.Material=mat;

                    Debug.Log("InspectorView.CreateEditor GetMaterialEditor2");
                }
                else{
                    Type objType=typeof(GameObject);
                    if (!m_editorsMap.IsObjectEditorEnabled(objType))
                    {
                        return editorPrefab;
                    }
                    editorPrefab = m_editorsMap.GetObjectEditor(objType);
                    Debug.Log("InspectorView.CreateEditor GetObjectEditor2");
                }
            return editorPrefab;
        }

        private void CreateEditor()
        {
            Debug.Log("InspectorView.CreateEditor Start");
            DestroyEditor();

            if (Editor.Selection.activeObject == null)
            {
                return;
            }

            if(Editor.Selection.objects[0] == null)
            {
                return;
            }

            // Debug.Log("InspectorView.CreateEditor 2");
            UnityObject[] selectedObjects = Editor.Selection.objects.Where(o => o != null).ToArray();
            Type objType;
            if(!OfSameType(selectedObjects, out objType))
            {
                return;
            }

            // Debug.Log("InspectorView.CreateEditor 3");
            ExposeToEditor exposeToEditor = null;
            if (objType == typeof(GameObject))
            {
                exposeToEditor = Editor.Selection.activeGameObject.GetComponent<ExposeToEditor>();
                if (exposeToEditor != null && !exposeToEditor.CanInspect)
                {
                    return;
                }
            }
                       
            Debug.Log("InspectorView.CreateEditor objType:"+objType);
            GameObject editorPrefab;
            m_editor=null;
            if (objType == typeof(Material))
            {
                Material mat = selectedObjects[0] as Material;
                if (mat.shader == null)
                {
                    return;
                }

                Shader shader = mat.shader;
                for(int i = 0; i < selectedObjects.Length; ++i)
                {
                    Material material = (Material)selectedObjects[i];
                    if(material.shader != shader)
                    {
                        return;
                    }
                }

                editorPrefab = m_editorsMap.GetMaterialEditor(mat.shader);
                Debug.Log("InspectorView.CreateEditor GetMaterialEditor");
            }
            // else if (objType == typeof(GameObject)) //cww
            // {
            //     GameObject go = selectedObjects[0] as GameObject;
            //     editorPrefab=ShowMaterialInfo(go);
            // }
            else
            {
                if (!m_editorsMap.IsObjectEditorEnabled(objType))
                {
                    return;
                }
                editorPrefab = m_editorsMap.GetObjectEditor(objType);
                Debug.Log("InspectorView.CreateEditor GetObjectEditor");
            }

            Debug.Log("InspectorView.CreateEditor editorPrefab:"+editorPrefab);
            if (editorPrefab != null && m_editor==null)
            {
                m_editor = Instantiate(editorPrefab);//GameObjectEditor.Awake
                m_editor.transform.SetParent(m_panel, false);
                m_editor.transform.SetAsFirstSibling();
            }

            // Debug.Log("InspectorView.CreateEditor 6");
            if (m_addComponentRoot != null && exposeToEditor && (m_settingsComponent == null || m_settingsComponent.BuiltInWindowsSettings.Inspector.ShowAddComponentButton))
            {
                IProject project = IOC.Resolve<IProject>();
                if(project == null || project.ToAssetItem(Editor.Selection.activeGameObject) == null)
                {
                    m_addComponentRoot.SetActive(true);
                    if (m_addComponentControl != null)
                    {
                        m_addComponentControl.ComponentSelected += OnAddComponent;
                    }
                }
            }
            // Debug.Log("InspectorView.CreateEditor 7");
        }

        private void OnAddComponent(Type type)
        {
            IRuntimeEditor editor = IOC.Resolve<IRuntimeEditor>();
            editor.Undo.BeginRecord();

            GameObject[] gameObjects = editor.Selection.gameObjects;
            for (int i = 0; i < gameObjects.Length; ++i)
            {
                GameObject go = gameObjects[i];
                ExposeToEditor exposeToEditor = go.GetComponent<ExposeToEditor>();
                foreach (RequireComponent requirement in type.GetCustomAttributes(true).OfType<RequireComponent>())
                {
                    if(requirement.m_Type0 != null && !go.GetComponent(requirement.m_Type0))
                    {
                        editor.Undo.AddComponent(exposeToEditor, requirement.m_Type0);
                    }
                    if (requirement.m_Type1 != null && !go.GetComponent(requirement.m_Type1))
                    {
                        editor.Undo.AddComponent(exposeToEditor, requirement.m_Type1);
                    }
                    if (requirement.m_Type2 != null && !go.GetComponent(requirement.m_Type2))
                    {
                        editor.Undo.AddComponent(exposeToEditor, requirement.m_Type2);
                    }
                }
                
                editor.Undo.AddComponent(exposeToEditor, type);
            }

            editor.Undo.EndRecord();       
        }
    }
}
