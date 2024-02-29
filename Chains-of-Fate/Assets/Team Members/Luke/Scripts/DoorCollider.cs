using System;
using System.Collections;
using System.Collections.Generic;
using ChainsOfFate.Gerallt;
using FMODUnity;
using FMOD.Studio;
using Unity.Collections;
using UnityEngine;

public enum DoorType
{
	Metal,
	Wooden,
	TrapDoor
}

public class DoorCollider : MonoBehaviour
{
	private InteractTriggerBox _interactBox;
	
	private EventInstance instance;

	[SerializeField] private EventReference fmodEvent;
	
	[SerializeField] private DoorType type;

	void Start()
	{
		instance = RuntimeManager.CreateInstance(fmodEvent);
	}
	
	private void OnTriggerEnter2D(Collider2D other)
	{
		InteractTriggerBox interactBox = other.GetComponent<InteractTriggerBox>();
		if (interactBox != null)
		{
			_interactBox = interactBox;
			interactBox.InteractEvent += UseDoor;
		}
	}
	
	private void OnTriggerExit2D(Collider2D other)
	{
		InteractTriggerBox interactBox = other.GetComponent<InteractTriggerBox>();
		if (interactBox != null)
		{
			interactBox.InteractEvent -= UseDoor;
		}
	}

	private void OnDisable()
	{
		if (_interactBox != null) _interactBox.InteractEvent -= UseDoor;
	}
	
	private void OnDestroy()
	{
		if (_interactBox != null) _interactBox.InteractEvent -= UseDoor;
	}

	private void UseDoor()
	{
		instance.setParameterByName("DoorType", (int) type);
		instance.start();
	}
}
