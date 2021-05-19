using AdvancedCullingSystem.DynamicCullingCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace AdvancedCullingSystem.StaticCullingCore
{
    public class StaticCulling : MonoBehaviour
    {
        public static void Clear()
        {
            foreach (var culling in FindObjectsOfType<StaticCulling>())
                DestroyImmediate(culling.gameObject);

            foreach (var tree in FindObjectsOfType<BinaryTree>())
                DestroyImmediate(tree.gameObject);
        }


        public static StaticCulling Instance { get; private set; }

        [SerializeField]
        [HideInInspector]
        private MeshRenderer[] _renderers;

        [SerializeField]
        //[HideInInspector] //cww delete
        private BinaryTree[] _trees;

        [SerializeField]
        private List<Camera> _cameras;

        [Header("Editor only")]

        [SerializeField]
        public bool _enableFrustum;

        [SerializeField]
        private bool _drawCullingArea;

        [SerializeField]
        public StaticCullingMaster cullingMaster;


        public static StaticCulling Create(MeshRenderer[] renderers, BinaryTree[] trees, Camera[] cameras = null)
        {
            StaticCulling staticCulling = new GameObject("Static Culling").AddComponent<StaticCulling>();

            staticCulling._renderers = renderers;
            staticCulling._trees = trees;
            //staticCulling._drawCullingArea = true;//cww add 避免每次都要手动去选上
            staticCulling._enableFrustum = true;

            if (cameras != null)
            {
                staticCulling._cameras = new List<Camera>();

                for (int i = 0; i < cameras.Length; i++)
                    staticCulling.AddCamera(cameras[i]);
            }

            return staticCulling;
        }


        private void Awake()
        {
            Instance = this;
        }

        public DynamicCulling dynamicCulling;

        private void GetDynamicCulling()
        {
            if (dynamicCulling == null)
            {
                dynamicCulling = DynamicCulling.Instance;
            }
            if (dynamicCulling == null)
            {
                dynamicCulling = GameObject.FindObjectOfType<DynamicCulling>();
            }
            if (dynamicCulling == null)
            {
                Debug.LogError("dynamicCulling == null");
            }
        }

        private void Start()
        {
            CheckCameras();

            SelectedCount = _renderers.Length;

            if (renderDict.Count == 0)
            {
                renderDict.Clear();
                foreach (var render in FindObjectsOfType<MeshRenderer>())
                {
                    renderDict.Add(render.GetInstanceID(), render);
                }
                AllCount = renderDict.Count;
            }

            //UpdateRenderersVisibleNew();//手动一步步测试配合测试，禁用Update
            //UpdateRenderersVisible();//

            GetDynamicCulling();


            if (dynamicCulling != null)
            {
                dynamicCulling.SetCullingEnable(false) ;
            }
        }

        Dictionary<int, MeshRenderer> renderDict = new Dictionary<int, MeshRenderer>();//cww_Add

        public bool EnableCulling = true;//cww_add

        //private List<MeshRenderer> visibleRenderersList = new List<MeshRenderer>();

        public int VisibleCount = 0;

        public int SelectedCount = 0;

        public int AllCount = 0;

        public double UpdateTime = 0;

        public double DisableRendersTime = 0;

        public double EnableRendersTime = 0;

        public double DynamicCullingTime = 0;

        public double GetNodeTime = 0;

        public Dictionary<Camera,CameraInfo> cameraInfos = new Dictionary<Camera, CameraInfo>();

        public bool IsFirstUpdate = true;

        public string LastNode;

        private DateTime LastNodeTime;

        public string CurrentNode;

        private DateTime CurrentNodeTime;

        public double MoveNodeTime = 0;

        [ContextMenu("UpdateRenderersVisible")]
        private void UpdateRenderersVisible()
        {
            DateTime start = DateTime.Now;
            try
            {
                if (EnableCulling == false) return;
                DisableRenderers();//隐藏所有相关物体 //3.3w个物体，11ms，设置renderer.enable的话需要4ms

                List<Renderer> visibleRenderersList = new List<Renderer>();
                DisableRendersTime = (DateTime.Now - start).TotalMilliseconds;//11ms
                int i = 0;
                while (i < _cameras.Count)
                {
                    Camera cam = _cameras[i];
                    if (cam == null)
                    {
                        Debug.Log("StaticCulling::looks like camera was destroyed");
                        _cameras.RemoveAt(i);
                        if (!CheckCameras())
                            return;
                        continue;
                    }
                    //获取CameraInfo
                    CameraInfo info = null;
                    if (!cameraInfos.ContainsKey(cam))
                    {
                        info = new CameraInfo();
                        info.camera = cam;
                        cameraInfos.Add(cam, info);
                    }
                    else
                    {
                        info = cameraInfos[cam];
                    }
                    bool insideArea = false;
                    Vector3 pos = cam.transform.position;//摄像头坐标
                    for (int c = 0; c < _trees.Length; c++)
                    {
                        var tree = _trees[c];
                        if (new Bounds(tree.rootNode.center, tree.rootNode.size).Contains(pos))//找到包含该坐标的Tree
                        {
                            if (dynamicCulling != null)
                            {
                                dynamicCulling.SetCullingEnable(false);//采样范围内，使用静态遮挡剔除
                            }

                            DateTime start2 = DateTime.Now;
                            var node = tree.GetNode(pos);//根据坐标再找到找到TreeNode(Cell);
                            CurrentNode = node.name;
                            info.treeNode = node;//保存摄像头所在的区域(Cell/TreeNode)
                            GetNodeTime = (DateTime.Now - start2).TotalMilliseconds; //0
                            DateTime start3 = DateTime.Now;
                            EnableRenderersEx(cam, node.visibleRenderers, visibleRenderersList);//显示该Cell内可见的物体
                            //GetVisibleRenderers(cam, node.visibleRenderers, visibleRenderersList);//显示该Cell内可见的物体
                            insideArea = true;
                            EnableRendersTime = (DateTime.Now - start3).TotalMilliseconds; //18ms
                            break;
                        }
                    }
                    if (!insideArea)
                    {
                        AfterOutAreas();//不在采样区域内时则显示所有的模型-》采样范围外，使用动态遮挡剔除
                        return;
                    }
                    else
                    {

                    }
                    i++;
                }
                if (_trees.Length > 0 || _cameras.Count > 0)//cww:多个摄像机或者采用区域的情况下才有可能有重复的renderers
                {
                    visibleRenderersList = visibleRenderersList.Distinct().ToList();
                }

                //foreach (var item in visibleRenderersList)
                //{
                //    item.ShowRenderer();
                //}

                VisibleCount = visibleRenderersList.Count;
            }
            catch (System.Exception ex)
            {
                Debug.LogError("StaticCulling::get exception");
                Debug.LogError("Cause : " + ex.Message + " " + ex.StackTrace);
                Debug.LogError("Please write about it on e-mail(andre-orsk@yandex.ru) and I will help You");
                Debug.LogError("-----------------------------------");
                Disable();
            }
            UpdateTime = (DateTime.Now - start).TotalMilliseconds;//29ms
        }

        List<Renderer> lastVisibleRenderersList = new List<Renderer>();

        public int DymaicCount = 0;
        public int DymaicAddCount = 0;

        [ContextMenu("UpdateRenderersVisibleNewTest")]
        private void UpdateRenderersVisibleNewTest()
        {
            DateTime start = DateTime.Now;
            try
            {
                if (EnableCulling == false) return;
                if (IsFirstUpdate)
                {
                    IsFirstUpdate = false;
                    DisableRenderers();//隐藏所有相关物体 //3.3w个物体，11ms，设置renderer.enable的话需要4ms
                }
                else
                {
                    //DisableRenderers2();
                }

                List<Renderer> visibleRenderersList = new List<Renderer>();
                DisableRendersTime = (DateTime.Now - start).TotalMilliseconds;//11ms

                int i = 0;
                while (i < _cameras.Count)
                {
                    Camera cam = _cameras[i];
                    if (cam == null)
                    {
                        Debug.Log("StaticCulling::looks like camera was destroyed");
                        _cameras.RemoveAt(i);
                        if (!CheckCameras())
                            return;
                        continue;
                    }
                    //获取CameraInfo
                    CameraInfo info = null;
                    if (!cameraInfos.ContainsKey(cam))
                    {
                        info = new CameraInfo();
                        info.camera = cam;
                        cameraInfos.Add(cam, info);
                    }
                    else
                    {
                        info = cameraInfos[cam];
                    }

                    bool insideArea = false;
                    Vector3 pos = cam.transform.position;//摄像头坐标
                    for (int c = 0; c < _trees.Length; c++)
                    {
                        var tree = _trees[c];
                        if (new Bounds(tree.rootNode.center, tree.rootNode.size).Contains(pos))//找到包含该坐标的Tree
                        {
                            if (dynamicCulling != null)
                            {
                                dynamicCulling.SetCullingEnable(false);//采样范围内，使用静态遮挡剔除
                            }

                            DateTime start2 = DateTime.Now;
                            var node = tree.GetNode(pos);//根据坐标再找到找到TreeNode(Cell);
                            GetNodeTime = (DateTime.Now - start2).TotalMilliseconds; //0

                            DateTime start3 = DateTime.Now;
                            
                            //if (info.treeNode == node)
                            //{
                            //    UpdateTime = (DateTime.Now - start).TotalMilliseconds;//29ms
                            //    WithDynamicCulling(visibleRenderersList);//动态剔除做辅助作用。
                            //    return;//后面就不用再计算了
                            //}
                            if (info.treeNode!=null)
                            {
                                LastNodeTime = CurrentNodeTime;
                                CurrentNodeTime = start3;
                                LastNode = info.treeNode.name;
                                MoveNodeTime = (CurrentNodeTime - LastNodeTime).TotalMilliseconds;
                            }
                            info.treeNode = node;//保存摄像头所在的区域(Cell/TreeNode)
                            CurrentNode = node.name;

                            GetVisibleRenderers(cam, node.visibleRenderers, visibleRenderersList);//获得所有的可见物体，里面有视锥体剔除
                            //visibleRenderersList.AddRange(node.visibleRenderers);//记录可见的物体
                            insideArea = true;
                            EnableRendersTime = (DateTime.Now - start3).TotalMilliseconds; //18ms
                            break;
                        }
                    }
                    if (!insideArea)
                    {
                        AfterOutAreas();//不在采样区域内时则显示所有的模型-》采样范围外，使用动态遮挡剔除
                        return;
                    }
                    else
                    {

                    }
                    i++;
                }
                if (_trees.Length > 0 || _cameras.Count > 0)//cww:多个摄像机或者采用区域的情况下才有可能有重复的renderers
                {
                    visibleRenderersList = visibleRenderersList.Distinct().ToList();
                }//获得所有的可见物体

                //SetRenderersVisible(visibleRenderersList, true);//显示物体

                visibleRenderersList=WithDynamicCulling(visibleRenderersList);//动态剔除做辅助作用。

                EnableRenderers(visibleRenderersList);//这里按需要显示/隐藏模型。

                lastVisibleRenderersList.Clear();
                lastVisibleRenderersList = visibleRenderersList;

                VisibleCount = visibleRenderersList.Count;
            }
            catch (System.Exception ex)
            {
                Debug.LogError("StaticCulling::get exception");
                Debug.LogError("Cause : " + ex.Message + " " + ex.StackTrace);
                Debug.LogError("Please write about it on e-mail(andre-orsk@yandex.ru) and I will help You");
                Debug.LogError("-----------------------------------");
                Disable();
            }

            UpdateTime = (DateTime.Now - start).TotalMilliseconds;//29ms
        }


        //cww_add：主要解决，就算是静止不懂，也要DisableRenderers，3.3w个模型 需要4ms，33w个就是40ms了，而且显示部分也一样，随着模型数量增加而增加
        [ContextMenu("UpdateRenderersVisibleNew")]
        private void UpdateRenderersVisibleNew()
        {
            DateTime start = DateTime.Now;
            try
            {
                if (EnableCulling == false) return;
                if (IsFirstUpdate)
                {
                    IsFirstUpdate = false;
                    DisableRenderers();//隐藏所有相关物体 //3.3w个物体，11ms，设置renderer.enable的话需要4ms
                }
                else
                {
                    //DisableRenderers2();
                }

                List<Renderer> visibleRenderersList = new List<Renderer>();
                DisableRendersTime = (DateTime.Now - start).TotalMilliseconds;//11ms

                int i = 0;
                while (i < _cameras.Count)
                {
                    Camera cam = _cameras[i];
                    if (cam == null)
                    {
                        Debug.Log("StaticCulling::looks like camera was destroyed");
                        _cameras.RemoveAt(i);
                        if (!CheckCameras())
                            return;
                        continue;
                    }
                    //获取CameraInfo
                    CameraInfo info = null;
                    if (!cameraInfos.ContainsKey(cam))
                    {
                        info = new CameraInfo();
                        info.camera = cam;
                        cameraInfos.Add(cam, info);
                    }
                    else
                    {
                        info = cameraInfos[cam];
                    }

                    bool insideArea = false;
                    Vector3 pos = cam.transform.position;//摄像头坐标
                    for (int c = 0; c < _trees.Length; c++)
                    {
                        var tree = _trees[c];
                        if (new Bounds(tree.rootNode.center, tree.rootNode.size).Contains(pos))//找到包含该坐标的Tree
                        {
                            if (dynamicCulling != null)
                            {
                                dynamicCulling.SetCullingEnable(false);//采样范围内，使用静态遮挡剔除
                            }

                            DateTime start2 = DateTime.Now;
                            var node = tree.GetNode(pos);//根据坐标再找到找到TreeNode(Cell);
                            GetNodeTime = (DateTime.Now - start2).TotalMilliseconds; //0

                            DateTime start3 = DateTime.Now;
                            
                            //if (info.treeNode == node)
                            //{
                            //    UpdateTime = (DateTime.Now - start).TotalMilliseconds;//29ms
                            //    WithDynamicCulling(visibleRenderersList);//动态剔除做辅助作用。
                            //    return;//后面就不用再计算了
                            //}
                            if (info.treeNode!=null)
                            {
                                LastNodeTime = CurrentNodeTime;
                                CurrentNodeTime = start3;
                                LastNode = info.treeNode.name;
                                MoveNodeTime = (CurrentNodeTime - LastNodeTime).TotalMilliseconds;
                            }
                            info.treeNode = node;//保存摄像头所在的区域(Cell/TreeNode)
                            CurrentNode = node.name;

                            GetVisibleRenderers(cam, node.visibleRenderers, visibleRenderersList);//获得所有的可见物体，里面有视锥体剔除
                            //visibleRenderersList.AddRange(node.visibleRenderers);//记录可见的物体
                            insideArea = true;
                            EnableRendersTime = (DateTime.Now - start3).TotalMilliseconds; //18ms
                            break;
                        }
                    }
                    if (!insideArea)
                    {
                        AfterOutAreas();//不在采样区域内时则显示所有的模型-》采样范围外，使用动态遮挡剔除
                        return;
                    }
                    else
                    {

                    }
                    i++;
                }
                if (_trees.Length > 0 || _cameras.Count > 0)//cww:多个摄像机或者采用区域的情况下才有可能有重复的renderers
                {
                    visibleRenderersList = visibleRenderersList.Distinct().ToList();
                }//获得所有的可见物体

                //SetRenderersVisible(visibleRenderersList, true);//显示物体

                visibleRenderersList=WithDynamicCulling(visibleRenderersList);//动态剔除做辅助作用。

                EnableRenderers(visibleRenderersList);//这里按需要显示/隐藏模型。

                lastVisibleRenderersList.Clear();
                lastVisibleRenderersList = visibleRenderersList;

                VisibleCount = visibleRenderersList.Count;
            }
            catch (System.Exception ex)
            {
                Debug.LogError("StaticCulling::get exception");
                Debug.LogError("Cause : " + ex.Message + " " + ex.StackTrace);
                Debug.LogError("Please write about it on e-mail(andre-orsk@yandex.ru) and I will help You");
                Debug.LogError("-----------------------------------");
                Disable();
            }

            UpdateTime = (DateTime.Now - start).TotalMilliseconds;//29ms
        }

        public bool IsWithDynamicCulling = true;

        List<Renderer> WithDynamicCulling(List<Renderer> list)
        {
            if (dynamicCulling != null && IsWithDynamicCulling)
            {
                DateTime start4 = DateTime.Now;
                List<Renderer> newRenders = dynamicCulling.ComputeVisibility();//判断是否有其他的模型静态没有检测出来，比如静态为了提高烘培速度降低了精度的情况下
                                                                                   //EnableRenderers(newRenders);
                DymaicCount = newRenders.Count;
                int c1 = list.Count;
                list.AddRange(newRenders);
                list = list.Distinct().ToList();
                int c2 = list.Count;
                DymaicAddCount = c2 - c1;

                DynamicCullingTime = (DateTime.Now - start4).TotalMilliseconds; //18ms
            }
            return list;
        }

        private void AfterOutAreas()
        {
            
            if (dynamicCulling != null)
            {
                dynamicCulling.SetCullingEnable(true);
            }
            else
            {
                //Debug.LogError("DynamicCulling.Instance==null");
                //EnableRenderers();//不在采样区域内时则显示所有的模型
            }
        }

        public bool IsNew = true;

        public bool IsUpdateInfo=true;

        private void Update()
        {
            if(IsUpdateInfo)
            {
                if (IsNew)
                {
                    UpdateRenderersVisibleNew();
                }
                else
                {
                    UpdateRenderersVisible();
                }
            }

        }

        private void OnDrawGizmos()
        {
            //Debug.Log("OnDrawGizmos");
            if (!_drawCullingArea || _trees == null || _trees.Length == 0)
                return;

            //Debug.Log("OnDrawGizmos2:"+_trees.Length);
            foreach (var tree in _trees)
                BinaryTreeUtil.DrawGizmos(tree, Color.blue);
        }

        private void OnDrawGizmosSelected()
        {
            //Debug.Log("OnDrawGizmosSelected");
            if (_drawCullingArea || _trees == null || _trees.Length == 0)
                return;

            //Debug.Log("OnDrawGizmosSelected2:"+_trees.Length);
            Gizmos.color = Color.blue;
            foreach (var tree in _trees)
            {
                if (tree == null) continue;
                Gizmos.DrawWireCube(tree.rootNode.center, tree.rootNode.size*2);
            }
        }


        private bool CheckCameras()
        {
            if (_cameras == null || _cameras.Count == 0)
            {
                Debug.Log("StaticCulling::no cameras assigned");

                Disable();

                return false;
            }

            return true;
        }

        private Dictionary<Renderer,bool> hiddenRenderDict = new Dictionary<Renderer, bool>();

        private Dictionary<Renderer, bool> shownRenderDict = new Dictionary<Renderer, bool>();

        //private

        [ContextMenu("DisableRenderersEx")]
        private void DisableRenderersEx()
        {
            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].HideRenderer();//只显示阴影，不显示模型
                hiddenRenderDict.Add(_renderers[i], _renderers[i].enabled);
            }
            //disabledRenderList.AddRange(_renderers);//不直接隐藏全部物体

            //visibleRenderersList.Clear();
        }

        [ContextMenu("DisableRenderers")]
        private void DisableRenderers()
        {
            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].HideRenderer();//只显示阴影，不显示模型
                //hiddenRenderDict.Add(_rendererArray[i], _rendererArray[i].enabled);
            }
            //disabledRenderList.AddRange(_renderers);//不直接隐藏全部物体

            //visibleRenderersList.Clear();
        }

        [HideInInspector]
        public bool lastIsEnled;

        public int TestCount = 2;

        [ContextMenu("DisableRenderers2")]
        private void DisableRenderers2()
        {
            for (int j = 0; j < TestCount; j++)
            {
                for (int i = 0; i < _renderers.Length; i++)//4ms左右，无论做什么
                {
                    if(hiddenRenderDict.ContainsKey(_renderers[i]))
                    {

                    }
                    //var render = _renderers[i];
                    //lastIsEnled = render.enabled;
                    //if(_renderers[i].enabled==true)
                    //{
                    //    _renderers[i].HideRenderer();//只显示阴影，不显示模型
                    //}
                }
            }
            //disabledRenderList.AddRange(_renderers);//不直接隐藏全部物体

            //visibleRenderersList.Clear();
        }

        /// <summary>
        /// 显示所有物体
        /// </summary>
        [ContextMenu("EnableRenderers")]
        private void EnableRenderers()
        {
            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].ShowRenderer();//显示阴影和模型
            }
            VisibleCount = _renderers.Length;
        }

        private void EnableRenderers(List<Renderer> renderers)
        {
            //foreach (var item in renderers)
            //{
            //    item.ShowRenderer();//显示 这次新显示的列表
            //}

            Dictionary<Renderer, bool> showToHideDict = new Dictionary<Renderer, bool>();
            foreach (var item in shownRenderDict.Keys)
            {
                showToHideDict.Add(item, item.enabled);
            }
            //先拷贝一下已经显示的模型列表

            Dictionary<Renderer, bool> shownRenderDictNew = new Dictionary<Renderer, bool>();//新的显示列表

            for (int i = 0; i < renderers.Count; i++)
            {
                var render = renderers[i];
                render.ShowRenderer();

                if (showToHideDict.ContainsKey(render))//已经显示了
                {
                    showToHideDict.Remove(render);//删除掉，最后剩下的就算需要隐藏的
                }
                else
                {
                    shownRenderDictNew.Add(render, render.enabled);//新的要显示的

                }
            }

            ShowToHideCount = showToHideDict.Count;
            foreach (var item in showToHideDict.Keys)
            {
                item.HideRenderer();//隐藏 上次显示的 而这次没有显示的 模型
                shownRenderDict.Remove(item);//从已经显示列表删除
                hiddenRenderDict.Add(item, item.enabled);
            }

            HideToShowCount = shownRenderDictNew.Count;
            foreach (var item in shownRenderDictNew.Keys)
            {
                item.ShowRenderer();//显示 这次新显示的列表
                shownRenderDict.Add(item, item.enabled);//加到已经显示列表
                hiddenRenderDict.Remove(item);//从已经隐藏的列表中删除
            }
        }

        public int ShowToHideCount = 0;
        public int HideToShowCount = 0;

        private void SetRenderersVisible(List<MeshRenderer> renderers,bool isVisible)
        {
            if(isVisible)
            {
                for (int i = 0; i < renderers.Count; i++)
                {
                    renderers[i].ShowRenderer();
                }
            }
            else
            {
                for (int i = 0; i < renderers.Count; i++)
                {
                    renderers[i].HideRenderer();
                }
            }
        }

        private void EnableRenderers(Camera camera, List<MeshRenderer> renderers)
        {
#if UNITY_EDITOR //为什么这个必须是在Editor下的
            Plane[] planes = null;
            if (_enableFrustum)
                planes = GeometryUtility.CalculateFrustumPlanes(camera);//视椎体剔除

            //从可见模型列表中剔除视锥体外的模型，只显示视锥体内的模型
            for (int i = 0; i < renderers.Count; i++)
                if (!_enableFrustum || GeometryUtility.TestPlanesAABB(planes, renderers[i].bounds))
                {
                    renderers[i].ShowRenderer();//核心
                }
#else
            for (int i = 0; i < renderers.Count; i++)
            {
                renderers[i].ShowRenderer();
            }
#endif
        }

        private void GetVisibleRenderers(Camera camera, List<Renderer> renderers, List<Renderer> visibleList)
        {
//#if UNITY_EDITOR //为什么这个必须是在Editor下的
            Plane[] planes = null;
            if (_enableFrustum)
                planes = GeometryUtility.CalculateFrustumPlanes(camera);//视椎体剔除

            //从可见模型列表中剔除视锥体外的模型，只显示视锥体内的模型
            for (int i = 0; i < renderers.Count; i++)
                if (!_enableFrustum || GeometryUtility.TestPlanesAABB(planes, renderers[i].bounds))
                {
                    //renderers[i].ShowRenderer();//核心
                    //if (!visibleRenderersList.Contains(_renderers[i]))//1290个Renderers，这个Contains函数存在的时候需要20ms，没有只需要1ms
                    //{
                    //    visibleRenderersList.Add(_renderers[i]);
                    //}

                    //if (!visibleRenderersDict.ContainsKey(renderers[i]))//1-2ms
                    //{
                    //    visibleRenderersDict.Add(renderers[i],renderers[i]);
                    //}

                    visibleList.Add(renderers[i]);//1ms
                }
//#else
//            for (int i = 0; i < renderers.Count; i++)
//            {
//                renderers[i].ShowRenderer();
//                //if(!visibleRenderers.Contains(renderers[i]))
//                //{
//                //    visibleRenderers.Add(renderers[i]);
//                //}
//            }

//            visibleList.AddRange(renderers);
//#endif
        }

        private void EnableRenderersEx(Camera camera, List<Renderer> renderers,List<Renderer> visibleList)
        {
#if UNITY_EDITOR //为什么这个必须是在Editor下的
            Plane[] planes = null;
            if (_enableFrustum)
                planes = GeometryUtility.CalculateFrustumPlanes(camera);//视椎体剔除

            //从可见模型列表中剔除视锥体外的模型，只显示视锥体内的模型
            for (int i = 0; i < renderers.Count; i++)
                if (!_enableFrustum || GeometryUtility.TestPlanesAABB(planes, renderers[i].bounds))
                {
                    renderers[i].ShowRenderer();//核心
                    //if (!visibleRenderersList.Contains(renderers[i]))//1290个Renderers，这个Contains函数存在的时候需要20ms，没有只需要1ms
                    //{
                    //    visibleRenderersList.Add(renderers[i]);
                    //}

                    //if (!visibleRenderersDict.ContainsKey(renderers[i]))//1-2ms
                    //{
                    //    visibleRenderersDict.Add(renderers[i],renderers[i]);
                    //}

                    visibleList.Add(renderers[i]);//1ms
                }
#else
            for (int i = 0; i < renderers.Count; i++)
            {
                renderers[i].ShowRenderer();
                //if(!visibleRenderers.Contains(renderers[i]))
                //{
                //    visibleRenderers.Add(renderers[i]);
                //}
            }

            visibleList.AddRange(renderers);
#endif
        }


        public void AddCamera(Camera camera)
        {
            if (!_cameras.Contains(camera))
                _cameras.Add(camera);

            if (!enabled)
                Enable();
        }

        public void RemoveCamera(Camera camera)
        {
            if (_cameras.Contains(camera))
                _cameras.Remove(camera);

            CheckCameras();
        }

        public void Enable()
        {
            enabled = true;

            CheckCameras();
        }

        public void Disable()
        {
            //EnableRenderers();

            enabled = false;
        }

        public List<BinaryTreeDataList> allTreeData = new List<BinaryTreeDataList>();

        public string FileName = "";

        [ContextMenu("SaveData")]
        public void SaveData()
        {
            // DateTime start = DateTime.Now;
            // BinaryTreeDataList treelist = ScriptableObject.CreateInstance<BinaryTreeDataList>();
            // for (int i = 0; i < _trees.Length; i++)
            // {
            //     BinaryTree tree = _trees[i];
            //     var data=tree.GetData();
            //     treelist.Add(data);
            // }
            // //SaveAsset(treelist, "Assets/Performance Tools/AdvancedCullingSystem/Resources/", FileName);
            // Debug.LogError("StaticCulling.SaveData:" + (DateTime.Now - start).ToString());

            // allTreeData.Clear();
            // allTreeData.Add(treelist);

            // //GameObject go = new GameObject(FileName);
            // //BinaryTreeDataComponent component=go.AddComponent<BinaryTreeDataComponent>();
            // //component.Data = treelist;

            // ProgressBarHelper.DisplayProgressBar("SaveData","Compress",0);
            // BinaryTreeDataListXml xml = new BinaryTreeDataListXml();
            // xml.Trees = treelist.Trees;
            // xml.Compress();
            // Debug.LogError("StaticCulling.Compress :" + (DateTime.Now - start).ToString());

            // ProgressBarHelper.DisplayProgressBar("SaveData","SaveXml",0);
            // SerializeHelper.Save(xml, $"E:\\{FileName}.xml");
            // Debug.LogError("StaticCulling.SaveXml :" + (DateTime.Now - start).ToString());

            // byte[] bytes = SerializeHelper.GetBytes(xml);
            
            // string file = $"E:\\{FileName}";
            // Debug.Log("bytes:" + bytes.Length+"|"+ file);
            // ProgressBarHelper.DisplayProgressBar("SaveData","WriteAllBytes",0);
            // File.WriteAllBytes(file, bytes);


            // Debug.LogError("StaticCulling.WriteAllBytes:" + (DateTime.Now - start).ToString());
            // ProgressBarHelper.ClearProgressBar();
        }

        public static void SaveAsset<T>(T asset, string path, string name) where T : ScriptableObject
        {
            Debug.Log("SaveAsset:" + path + name + ".asset");
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(asset, path + name + ".asset");
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }

        [ContextMenu("LoadData")]
        public void LoadData()
        {
            
            // string file = $"E:\\{FileName}.xml";
            // Debug.Log("LoadData:"+ file);
            // BinaryTreeDataListXml treelist = SerializeHelper.LoadFromFile<BinaryTreeDataListXml>(file);
            // if(treelist.IsCompressed)
            //     treelist.Decompress();
            // //BinaryTreeDataList treelist = Resources.Load<BinaryTreeDataList>(FileName);
            // if (treelist == null)
            // {
            //     Debug.LogError("StaticCulling.LoadData Data == null:" + FileName + ".asset");
            //     return;
            // }

            // if (_trees != null)
            // {
            //     foreach (var item in _trees)
            //     {
            //         if (item == null) continue;
            //         GameObject.DestroyImmediate(item.gameObject);
            //     }
            // }

            // _trees = new BinaryTree[treelist.Trees.Count];

            // for (int i = 0; i < treelist.Trees.Count; i++)
            // {
            //     BinaryTreeData item = (BinaryTreeData)treelist.Trees[i];
            //     GameObject treeGo = new GameObject("BinaryTree"+(i+1));
            //     treeGo.transform.SetParent(this.transform);
            //     BinaryTree tree=treeGo.AddComponent<BinaryTree>();
            //     tree.Data = item;
            //     tree.CreateTree();
            //     _trees[i] = tree;
            // }
        }

        [ContextMenu("CombineAllTree")]
        public void CombineAllTree()
        {
            if(allTreeData.Count==0)
            {
                Debug.LogError("CombineAllTree allTreeData.Count==0");
                return;
            }
            BinaryTreeDataList first = allTreeData[0];

            for (int i = 1; i < allTreeData.Count; i++)
            {
                first.Combine(allTreeData[i]);
            }


        }

        //[ContextMenu("LoadData")]
        //public void LoadData()
        //{
        //    if (DataFileName == "")
        //    {
        //        DataFileName = "BinaryTree" + TreeCount;
        //    }
        //    Data = Resources.Load<BinaryTreeData>(DataFileName + "");
        //    if (Data == null)
        //    {
        //        Debug.LogError("Data == null:"+ DataFileName + ".asset");
        //        return;
        //    }
        //}

        //[ContextMenu("CreateTree")]
        //public void CreateTree()
        //{
        //    for (int i = 0; i < _trees.Length; i++)
        //    {
        //        BinaryTree tree = _trees[i];
        //        tree.CreateTree();
        //    }
        //}
    }

    public class CameraInfo
    {
        public Camera camera;

        public Vector3 pos;

        public Vector3 rotation;

        public BinaryTreeNode treeNode;
    }
}