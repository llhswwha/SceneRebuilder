using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SteelModelLData
{
	public Vector3 forward;
	public Vector3 up;
	public float angle;
	public bool isMirror;

	public float length;
	public float width;
	public float height;

	public float sizePlane;
	public bool isSameSize;
	public float sizeDetail;

	public void SetSize(float height,float width)
    {
		List<float> hw = new List<float>() { height, width };
		hw.Sort();

		this.width = hw[0];
		this.height = hw[1];
        isSameSize = $"{this.width:F4}" == $"{this.height:F4}";
	}

	//public bool IsCenter;

	public SteelModelLData(float l)
	{
		forward = new Vector3(0, 0, 1);
		up = new Vector3(0, 1, 0);
		angle = 0;
		length = l;

		sizePlane = 0.2f;
		width = 2;
		height = 3;
		sizeDetail = 0.04f;

		isMirror = false;

		isSameSize = $"{width:F4}" == $"{height:F4}";

		//IsCenter = true;
	}

	//public bool GetScale()
	//{
	//	string sx = $"{sizeX:F4}";
	//	string sy = $"{sizeY:F4}";
	//	return sx == sy;
	//}

	public string GetId()
	{
		return $"L_{height:F4}_{width:F4}_{sizePlane:F4}_{sizeDetail:F4}";
	}

    public override string ToString()
    {
		return $"[sizeY:{height},sizeX:{width},sizePlane:{sizePlane},sizeDetail:{sizeDetail},length:{length},angle:{angle},isMirror:{isMirror}]";
	}

	public bool IsError(string tag)
    {
		if (length < height)
		{
			Debug.LogError($"GetLModelData length < sizeY name:{tag} lData:{this}");
			return true;
		}
		if (length < width)
		{
			Debug.LogError($"GetLModelData length < sizeX name:{tag} lData:{this}");
			return true;
		}
		if (length == 0)
		{
			Debug.LogError($"GetLModelData length == 0 name:{tag} lData:{this}");
			return true;
		}
		return false;
	}
}

[Serializable]
public struct SteelModelHData
{

}

[Serializable]
public struct SteelModelCData
{

}
