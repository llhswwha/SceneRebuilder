using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using MeshJobs;

public class RigidTransformTest : MonoBehaviour
{
    public Transform fromGo;

    public Transform toGo;

    public Transform testGo1;

    public Transform testGo2;

    public RigidTransform LocalFrom1(RigidTransform rt1,RigidTransform rt2)
    {
        return math.mul(math.inverse(rt1) , rt2);
    }

    [ContextMenu("Test1")]
    public void Test1()
    {
        RigidTransform rigidTransform1=new RigidTransform(fromGo.rotation,fromGo.position);
        RigidTransform rigidTransform2=new RigidTransform(toGo.rotation,toGo.position);
        RigidTransform rigidTransform3=LocalFrom1(rigidTransform1,rigidTransform2);

        // fromGo.position.PrintVector3("fromGo.position");
        // toGo.position.PrintVector3("toGo.position");
        // testGo.position.PrintVector3("testGo.position");

        var p=math.transform(rigidTransform3,fromGo.position);
        Debug.Log(fromGo.position);
        Debug.Log(toGo.position);
        Debug.Log(p);
        // p.PrintVector3("p");
    }

    [ContextMenu("Test2")]
    public void Test2()
    {
        RigidTransform rigidTransform1=new RigidTransform(fromGo.rotation,fromGo.position);
        RigidTransform rigidTransform2=new RigidTransform(toGo.rotation,toGo.position);
        RigidTransform rigidTransform3=LocalFrom1(rigidTransform2,rigidTransform1);//rigidTransform2/rigidTransform1

       var p=math.transform(rigidTransform3,fromGo.position);
        Debug.Log(fromGo.position);
        Debug.Log(toGo.position);
        Debug.Log(p);
    }

    public RigidTransform LocalFrom2(RigidTransform rt1,RigidTransform rt2)
    {
        return math.mul(rt2,math.inverse(rt1));
    }

    [ContextMenu("Test3")]
    public void Test3()
    {
        RigidTransform rigidTransform1=new RigidTransform(fromGo.rotation,fromGo.position);
        RigidTransform rigidTransform2=new RigidTransform(toGo.rotation,toGo.position);
        RigidTransform rigidTransform3=LocalFrom2(rigidTransform1,rigidTransform2);

        // fromGo.position.PrintVector3("fromGo.position");
        // toGo.position.PrintVector3("toGo.position");
        // testGo.position.PrintVector3("testGo.position");

        var p=math.transform(rigidTransform3,fromGo.position);
        Debug.Log(fromGo.position);
        Debug.Log(toGo.position);
        Debug.Log(p);
        // p.PrintVector3("p");
    }

    [ContextMenu("Test4")]
    public void Test4()
    {
        RigidTransform rigidTransform1=new RigidTransform(fromGo.rotation,fromGo.position);
        RigidTransform rigidTransform2=new RigidTransform(toGo.rotation,toGo.position);
        RigidTransform rigidTransform3=LocalFrom2(rigidTransform2,rigidTransform1);//rigidTransform2/rigidTransform1

       var p=math.transform(rigidTransform3,fromGo.position);
        Debug.Log(fromGo.position);
        Debug.Log(toGo.position);
        Debug.Log(p);
    }

    [ContextMenu("Test5")]
    public void Test5()
    {
        var rigidTransform1=new MeshJobs.RTTransform(fromGo.rotation,fromGo.position);
        var rigidTransform2=new MeshJobs.RTTransform(toGo.rotation,toGo.position);
        var rigidTransform3=rigidTransform1.LocalTo(rigidTransform2);

        var p=rigidTransform3.Apply(fromGo.position);
        Debug.Log(fromGo.position);
        Debug.Log(toGo.position);
        Debug.Log(p);
    }

    [ContextMenu("Test6")]
    public void Test6()
    {
        var tFrom=new MeshJobs.RTTransform(fromGo.rotation,fromGo.position);
        var tTo=new MeshJobs.RTTransform(toGo.rotation,toGo.position);
        //var rtTransform=tTo.LocalTo(tFrom);//可以
        var rtTransform=MeshJobs.RTTransform.FormTo(tFrom,tTo);

        var p=rtTransform.Apply(fromGo.position);

        Debug.Log(fromGo.position);
        Debug.Log(toGo.position);
        Debug.Log(p);

       rtTransform.ApplyLocal(testGo1);

       rtTransform.ApplyWorld(testGo2);
    }
}
