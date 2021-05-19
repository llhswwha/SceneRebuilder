using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    public float Speed = 1;
    void Update()
    {
        float x = this.transform.position.x;
        float z = this.transform.position.z;
        float y= this.transform.position.y + Speed * Time.deltaTime;

        if (y > 5)
        {
            Speed = -Mathf.Abs(Speed);
        }
        if (y < -5)
        {
            Speed = Mathf.Abs(Speed);
        }

        this.transform.position = new Vector3(x, y, z);
    }
}
