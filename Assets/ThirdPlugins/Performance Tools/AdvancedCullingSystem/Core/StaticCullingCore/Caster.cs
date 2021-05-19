using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;

namespace AdvancedCullingSystem.StaticCullingCore
{
    /// <summary>
    /// 投射器
    /// </summary>
    public class Caster : MonoBehaviour
    {
        [SerializeField]
        private List<MeshRenderer> _visibleRenderers;
        public List<MeshRenderer> visibleRenderers
        {
            get
            {
                return _visibleRenderers;
            }
        }

        private NativeList<int> _visibleRenderersIDs;

        public NativeList<int> visibleRenderersIDs
        {
            get
            {
                return _visibleRenderersIDs;
            }
        }

        /// <summary>
        /// 投射数据容器
        /// </summary>
        private CasterDataContainer _dataContainer;

        private NativeList<RaycastCommand> _commands;
        private NativeArray<RaycastHit> _hitsResult;
        private NativeList<int> _hittedObjects;


        public void Initialize(CasterDataContainer dataContainer)
        {
            _dataContainer = dataContainer;

            _visibleRenderers = new List<MeshRenderer>();

            _visibleRenderersIDs = new NativeList<int>(Allocator.TempJob);
        }

        public JobHandle CreateRayCommands(NativeList<int> targets, 
            NativeList<float3> points, NativeList<int2> pointers)
        {
            NativeArray<int> targets2=new NativeArray<int>(targets.Length, Allocator.Persistent);
            for (int i = 0; i < targets.Length; i++)
            {
                targets2[i]=targets[i];
            }

            _commands = new NativeList<RaycastCommand>(Allocator.TempJob);
            return new CreateRayCommandsJob
            {
                visibleRenderersIDs = _visibleRenderersIDs,

                origin = transform.position,
                layerMask = _dataContainer.layerMask,

                //targets = targets,
                targets = targets2,
                pointers = pointers,
                points = points,

                commands = _commands

            }.Schedule();
        }

        public void CreateRayCommands(NativeList<float3> directions)
        {
            _commands = new NativeList<RaycastCommand>(Allocator.TempJob);
            for (int i = 0; i < directions.Length; i++)
            {
                // _commands.Add(new RaycastCommand(transform.position, 
                //     transform.TransformDirection(directions[i]), 
                //     layerMask : _dataContainer.layerMask));

                _commands.Add(new RaycastCommand(transform.position, 
                    transform.TransformDirection(directions[i])));
            }
        }

        public JobHandle CastRays()
        {
            _hitsResult = new NativeArray<RaycastHit>(_commands.Length, Allocator.TempJob);

            return RaycastCommand.ScheduleBatch(_commands, _hitsResult, 1, default);
        }

        public JobHandle ComputeHitsResult()
        {
            _commands.Dispose();

            _hittedObjects = new NativeList<int>(Allocator.TempJob);

            List<Collider> occluders = _dataContainer.occluders;
            for (int i = 0; i < _hitsResult.Length; i++)
            {
                Collider collider = _hitsResult[i].collider;

                if (collider != null && !occluders.Contains(collider))
                {
                    if(!_dataContainer.iDByCollider.ContainsKey(collider))
                    {
                        MeshRenderer renderer=collider.GetComponent<MeshRenderer>();
                        _dataContainer.InitRenderer(renderer,collider);
                    }
                    _hittedObjects.Add(
                        _dataContainer.iDByCollider[collider]
                );
                }
                    
                    
            }

            _hitsResult.Dispose();

            return new ComputeHitsResultJob()
            {

                hittedObjects = _hittedObjects,
                visibleRenderersIDs = _visibleRenderersIDs

            }.Schedule();
        }

        public void PushData()
        {
            if (_commands.IsCreated) _commands.Dispose();
            if (_hitsResult.IsCreated) _hitsResult.Dispose();
            if (_hittedObjects.IsCreated) _hittedObjects.Dispose();

            for (int i = 0; i < _visibleRenderersIDs.Length; i++)
            {
                MeshRenderer renderer = _dataContainer.rendererByID[_visibleRenderersIDs[i]];

                if (!_visibleRenderers.Contains(renderer))
                    _visibleRenderers.Add(renderer);
            }
        }

        public void Dispose()
        {
            if (_commands.IsCreated) _commands.Dispose();
            if (_hitsResult.IsCreated) _hitsResult.Dispose();
            if (_hittedObjects.IsCreated) _hittedObjects.Dispose();
            if (_visibleRenderersIDs.IsCreated) _visibleRenderersIDs.Dispose();
        }


        public float WireSphereScale=0.1f;
        private void OnDrawGizmosSelected()
        {
            if (_visibleRenderers == null || _visibleRenderers.Count == 0)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, WireSphereScale);

            Gizmos.color = Color.red;
            for (int i = 0; i < _visibleRenderers.Count; i++)
            {
                MeshRenderer renderer = _visibleRenderers[i];

                Gizmos.DrawWireCube(renderer.bounds.center, renderer.bounds.size);
            }
        }
    }

    [BurstCompile]
    public struct CreateRayCommandsJob : IJob
    {
        [ReadOnly] public NativeList<int> visibleRenderersIDs;
        /// <summary>
        /// 射线的起点
        /// </summary>
        [ReadOnly] public float3 origin;
        [ReadOnly] public int layerMask;
        /// <summary>
        /// 目标对象(ID)列表
        /// </summary>
        //[NativeDisableParallelForRestriction]
        [ReadOnly] public NativeArray<int> targets;
        [ReadOnly] public NativeList<int2> pointers;
        /// <summary>
        /// 射线的终点
        /// </summary>
        [ReadOnly] public NativeList<float3> points;
        //[NativeDisableParallelForRestriction]
        [WriteOnly] public NativeList<RaycastCommand> commands;
        public void Execute()
        {
            for (int i = 0; i < targets.Length; i++)
            {
                if (visibleRenderersIDs.Contains(targets[i]))
                    continue;
                int startIndex = pointers[i].x;
                int endIndex = pointers[i].y;
                for (int c = startIndex; c < endIndex; c++)
                    commands.Add(
                        //new RaycastCommand(origin, points[c] - origin, layerMask : layerMask)
                        new RaycastCommand(origin, points[c] - origin)
                        );
            }
        }
    }

    [BurstCompile]
    public struct ComputeHitsResultJob : IJob
    {
        [ReadOnly]
        public NativeList<int> hittedObjects;
        public NativeList<int> visibleRenderersIDs;

        public void Execute()
        {
            //将碰撞到的物体ID放到可见物体列表中
            for (int i = 0; i < hittedObjects.Length; i++)
                if (!visibleRenderersIDs.Contains(hittedObjects[i]))
                    visibleRenderersIDs.Add(hittedObjects[i]);
        }
    }

    /// <summary>
    /// Cast投射，投射数据容器，记录renderer的各种信息，关联ID到Dictionary
    /// </summary>

    public class CasterDataContainer
    {
        private NativeList<int> _renderersIDs;

        public NativeList<int> renderersIDs
        {
            get
            {
                return _renderersIDs;
            }
        }

        /// <summary>
        ///  遮蔽体
        /// </summary>
        public List<Collider> occluders { get; private set; }

        public int layerMask { get; private set; }

        public Dictionary<int, MeshRenderer> rendererByID { get; private set; }
        public Dictionary<Collider, MeshRenderer> rendererByCollider { get; private set; }

        public Dictionary<MeshRenderer, int> iDByRenderer { get; private set; }
        public Dictionary<Collider, int> iDByCollider { get; private set; }

        public CasterDataContainer(MeshRenderer[] renderers, MeshCollider[] renderersColliders,
            Collider[] occluders, string layerName)
        {
            _renderersIDs = new NativeList<int>(Allocator.TempJob);
            this.occluders = new List<Collider>(occluders);

            layerMask = LayerMask.GetMask(layerName);

            rendererByID = new Dictionary<int, MeshRenderer>();
            rendererByCollider = new Dictionary<Collider, MeshRenderer>();

            iDByRenderer = new Dictionary<MeshRenderer, int>();
            iDByCollider = new Dictionary<Collider, int>();

            //记录renderer的各种信息，关联ID到Dictionary
            for (int i = 0; i < renderers.Length; i++)
            {
                // int ID = renderers[i].GetInstanceID();

                // rendererByID.Add(ID, renderers[i]);
                // rendererByCollider.Add(renderersColliders[i], renderers[i]);

                // iDByRenderer.Add(renderers[i], ID);
                // iDByCollider.Add(renderersColliders[i], ID);

                // _renderersIDs.Add(ID);

                InitRenderer(renderers[i],renderersColliders[i]);
            }
        }

        public void InitRenderer(MeshRenderer renderer,Collider collider)
        {
                           int ID = renderer.GetInstanceID();

                rendererByID.Add(ID, renderer);
                rendererByCollider.Add(collider, renderer);

                iDByRenderer.Add(renderer, ID);
                iDByCollider.Add(collider, ID);

                _renderersIDs.Add(ID);
        }

        public void Dispose()
        {
            if(_renderersIDs.IsCreated)
                _renderersIDs.Dispose();
        }
    }
}
