using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Luke
{
	public class WorldViewCamera : MonoBehaviour
	{
		[SerializeField]
		private Transform targetPlayer;

		[SerializeField]
		private float zAxisDistanceToPlayer;
			[SerializeField, Range(0,1)]
		private float cameraSpeed;
		private Transform cameraTransform;
		private Camera cam;
		
		[SerializeField]
		private Bounds bounds;
		private float minXBoundary;
		private float maxXBoundary;
		private float minYBoundary;
		private float maxYBoundary;

		// Start is called before the first frame update
		void Start()
		{
			cameraTransform = transform;
			cam = GetComponent<Camera>();
			Vector3 playerPosition = targetPlayer.position;
			cameraTransform.position = new Vector3(playerPosition.x, playerPosition.y,
				playerPosition.z - zAxisDistanceToPlayer);
			CalculateBoundaries();
		}

		// Update is called once per frame
		void FixedUpdate()
		{
			Vector3 playerPosition = targetPlayer.position;
			Vector3 cameraPosition = cameraTransform.position;
			float xTarget = Mathf.Lerp(cameraPosition.x, playerPosition.x, cameraSpeed);
			xTarget = Mathf.Clamp(xTarget, minXBoundary, maxXBoundary);
			float yTarget = Mathf.Lerp(cameraPosition.y, playerPosition.y, cameraSpeed);
			yTarget = Mathf.Clamp(yTarget, minYBoundary, maxYBoundary);
			cameraTransform.position = new Vector3(xTarget, yTarget,
				playerPosition.z - zAxisDistanceToPlayer);
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
			Gizmos.DrawCube(bounds.center, bounds.extents);
		}

		private void CalculateBoundaries()
		{
			float frustumHeight = zAxisDistanceToPlayer * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
			float frustumWidth = frustumHeight * cam.aspect;
			minXBoundary = bounds.center.x - bounds.extents.x/2 + frustumWidth;
			maxXBoundary = bounds.center.x + bounds.extents.x/2 - frustumWidth;
			minYBoundary = bounds.center.y - bounds.extents.y/2 + frustumHeight;
			maxYBoundary = bounds.center.y + bounds.extents.y/2 - frustumHeight;
		}
	}
}