using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavTestInfo : MonoBehaviour {

    public UnityEngine.AI.NavMeshPath Path;

    public Vector3 Target;

    public float PathDistance;

    public float LineDistance;

    public NavMeshAgent agent;

    public void Start()
    {
        //Target = this.transform.position;

        if (agent != null)
        {
            var path = new NavMeshPath();
            agent.CalculatePath(transform.position, path);
            //Debug.Log(string.Format("{0},{1},{2}", this, path.status, path.corners.Length));
            PathDistance = NavMeshHelper.GetPathDistance(path);
            //Debug.Log(string.Format("{0},{1},{2},{3}", this.name, path.status, path.corners.Length, Distance));
            LineDistance = Vector3.Distance(transform.position, Target);
        }
    }

    [ContextMenu("ShowPath")]
    public void ShowPath()
    {
        //Path = agent.CalculatePath(Target);

        Path = new NavMeshPath();
        agent.CalculatePath(Target, Path);
        Debug.Log(string.Format("{0},{1}", Path.status, Path.corners.Length));

        foreach (var item in testPoints)
        {
            GameObject.Destroy(item);
        }
        //foreach (var item in Path.corners)
        //{
        //    GameObject tp = NavMeshHelper.CreatePoint(item, "" + item, 0.1f, Color.black);
        //    testPoints.Add(tp);
        //}

        PathDistance = NavMeshHelper.GetPathDistance(Path);
    }

    public List<GameObject> testPoints = new List<GameObject>();

    public void Update()
    {
        if (Path != null && Path.corners.Length > 0)
        {
            for (int i = 0; i < Path.corners.Length - 1; i++)
            {
                Debug.DrawLine(Path.corners[i], Path.corners[i + 1], Color.red);
            }
        }
    }
}
