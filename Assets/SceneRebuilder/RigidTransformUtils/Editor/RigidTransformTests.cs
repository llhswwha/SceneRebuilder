using NUnit.Framework;
using MeshJobs;
using UnityEngine;


public class RigidTransformTests
{

    [Test]
    public void Identity()
    {
        var id = RTTransform.Identity;
        Assert.AreEqual(Vector3.zero, id.Translation);
        Assert.AreEqual(Quaternion.identity, id.Rotation);
    }

    [Test]
    public void Translation()
    {
        var a = new RTTransform(new Vector3(1, 2, 3));
        var b = a * a;
        Assert.AreEqual(new Vector3(2, 4, 6), b.Translation);
        Assert.AreEqual(Quaternion.identity, b.Rotation);
    }

    [Test]
    public void Rotation()
    {
        var a = new RTTransform(Quaternion.Euler(90, 0, 0));
        var b = a * a;
        Assert.AreEqual(Vector3.zero, b.Translation);
        var q = Quaternion.Euler(180, 0, 0);
        Assert.True(q.Approximately(b.Rotation));
    }

    [Test]
    public void Transform()
    {
        var a = new RTTransform(Quaternion.Euler(0, 0, 90), new Vector3(1, 0, 0));
        var b = a * a;
        var v = new Vector3(1, 1, 0);
        Assert.True(v.Approximately(b.Translation));
        var q = Quaternion.Euler(0, 0, 180);
        Assert.True(q.Approximately(b.Rotation));
    }

    [Test]
    public void Inverse()
    {
        var a = new RTTransform(Quaternion.Euler(0, 0, 90), new Vector3(1, 0, 0));
        var b = a.Inverse();
        Assert.AreEqual(Quaternion.Euler(0, 0, -90), b.Rotation);
        Assert.True(new Vector3(0, 1, 0).Approximately(b.Translation));

        var i = a * b;
        Assert.True(RTTransform.Identity.Approximately(i));
    }

    [Test]
    public void Appy()
    {
        var a = new RTTransform(Quaternion.Euler(0, 0, 90), new Vector3(1, 0, 0));
        var c=a.Apply(new Vector3(1, 0, 0));

        Assert.True(new Vector3(1, 1, 0).Approximately(c));
    }

    /*
    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator RigidTransformTestsWithEnumeratorPasses() {
        // Use the Assert class to test conditions.
        // yield to skip a frame
        yield return null;
    }
    */
}
