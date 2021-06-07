using UnityEngine;
using System.Collections;

public class TagInfo : MonoBehaviour
{

    public string tagName;

    public bool flag;

	// Use this for initialization
	void Start ()
	{
	    tagName = gameObject.tag;
	    flag = gameObject.CompareTag("Untagged");

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
