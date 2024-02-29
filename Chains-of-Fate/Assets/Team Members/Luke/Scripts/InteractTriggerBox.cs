using System;
using System.Collections;
using System.Collections.Generic;
using ChainsOfFate.Gerallt;
using UnityEngine;

public class InteractTriggerBox : MonoBehaviour
{
	public Action InteractEvent;

	[SerializeField] private Transform playerTransform;
	private Transform _transform;
	private Vector3 lastPos;

	void OnEnable()
	{
		_transform = GetComponent<Transform>();
		lastPos = playerTransform.position;
	}
	
	private void FixedUpdate()
	{
		if (playerTransform.position.x < lastPos.x)
		{
			_transform.localPosition = new Vector3(-1, 0, 0);
			_transform.localScale = new Vector3(1, 2, 1);
		}
		else if (playerTransform.position.x > lastPos.x)
		{
			_transform.localPosition = new Vector3(1, 0, 0);
			_transform.localScale = new Vector3(1, 2, 1);
		}
		
		if (playerTransform.position.y < lastPos.y)
		{
			_transform.localPosition = new Vector3(0, -1.5f, 0);
			_transform.localScale = new Vector3(1, 1, 1);
		}
		else if (playerTransform.position.y > lastPos.y)
		{
			_transform.localPosition = new Vector3(0, 1.5f, 0);
			_transform.localScale = new Vector3(1, 1, 1);
		}

		lastPos = playerTransform.position;
	}
}
