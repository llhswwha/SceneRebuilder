using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoECSTesting : MonoBehaviour
{
    public GameObject Prefab;
    public int Count = 100;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Count; i++)
        {
            GameObject obj = GameObject.Instantiate(Prefab);
            obj.transform.position = new Vector3(Random.Range(-8f, 8f), Random.Range(-5f, 5f), Random.Range(-1f, 1f));
            //Objs.Add(obj);
            obj.AddComponent<Mover>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //for (int i = 0; i < Count; i++)
        //{
        //    GameObject obj = Objs[i];
        //    float speed = Speed[i];
        //}
    }
}
