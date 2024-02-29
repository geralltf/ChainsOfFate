using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBounds : MonoBehaviour
{
	public Bounds buildingBounds;
	public string buildingScene;

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1, 1, 1, 0.5f);
		Gizmos.DrawCube(buildingBounds.center, buildingBounds.extents);
	}
}
