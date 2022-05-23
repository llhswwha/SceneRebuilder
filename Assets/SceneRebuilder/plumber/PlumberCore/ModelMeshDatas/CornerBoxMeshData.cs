using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CornerBoxMeshData 
{
	public Vector3 forward;
	public Vector3 up;
	public float angle;
	public bool isMirror;

	public float length;
	public float width;
	public float height;

	public float cornerX;
	public float cornerY;

	public void RoundTo(int p)
    {
		length = length.RoundToEx(p);
		width = width.RoundToEx(p);
		height = height.RoundToEx(p);
		cornerX = cornerX.RoundToEx(p);
		cornerY = cornerY.RoundToEx(p);
	}
}
