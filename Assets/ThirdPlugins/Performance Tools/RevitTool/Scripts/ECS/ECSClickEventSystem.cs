using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;
using UnityEngine;

public class ECSClickEventSystem : MonoBehaviour
{
    public static ECSClickEventSystem Instance;

    void Awake()
    {
        Instance = this;
    }


    public Camera Cam;
    const float RAYCAST_DISTANCE = 1000;
    PhysicsWorld physicsWorld => World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld;

    EntityManager entityManager => World.DefaultGameObjectInjectionWorld.EntityManager;
    // Start is called before the first frame update
    void Start()
    {
        if (Cam == null)
        {
            Cam = gameObject.GetComponent<Camera>();
        }
        if (Cam == null)
        {
            Cam = Camera.main;
        }
    }

    Entity? selectedEntity;
    // Update is called once per frame
    void LateUpdate()
    {
        if (!Input.GetMouseButtonDown(0) || Cam == null) return;
        print("click:" + Input.mousePosition);
        var screenPointToRay = Cam.ScreenPointToRay(Input.mousePosition);
        var rayInput = new RaycastInput
        {
            Start = screenPointToRay.origin,
            End = screenPointToRay.GetPoint(RAYCAST_DISTANCE),
            Filter = CollisionFilter.Default
        };
        if (!physicsWorld.CastRay(rayInput, out Unity.Physics.RaycastHit hit))
        {
            print("clearSelected:" + selectedEntity);
            RecoveryEntityColor();
            selectedEntity = null;
            return;
        }

        print("hit.RigidBodyIndex:" + hit.RigidBodyIndex);
        Entity entity = physicsWorld.Bodies[hit.RigidBodyIndex].Entity;
        print("hit:" + entity);
        print("isSame:" + (entity== selectedEntity));


        RecoveryEntityColor();
        entityColor = SetEntityColor(entity, SelectedColor);
        selectedEntity = entity;

        //var data = entityManager.GetComponentData<ModelDataComponent>(entity);
        //print("data:" + data);
    }

    private void RecoveryEntityColor()
    {
        if (selectedEntity != null)
        {
            SetEntityColor((Entity)selectedEntity, entityColor);
        }
    }

    private Color SetEntityColor(Entity entity,Color color)
    {
        print("entity:" + entity+",color:"+color);
        var renderMesh = entityManager.GetSharedComponentData<RenderMesh>(entity);
        var mat = new UnityEngine.Material(renderMesh.material);
        Color orgColor = mat.GetColor("_Color");
        //mat.SetColor("_Color", color);
        //renderMesh.material = mat;
        //entityManager.SetSharedComponentData(entity, renderMesh);
        return orgColor;
    }

    public Color entityColor;

    public Color SelectedColor = Color.green;
}
