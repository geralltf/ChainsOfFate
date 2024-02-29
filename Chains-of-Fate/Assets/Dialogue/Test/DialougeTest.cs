using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class DialougeTest : MonoBehaviour
{ 
  public Rigidbody2D rb;
  private bool isGreeting = false;
  private bool inDialogue = false;
  private bool canExitDialogue = false;
  private bool canEnterDialogue = true;
  public YarnInteractable yarnInteractable;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (InputSystem.GetDevice<Keyboard>().eKey.isPressed) // TODO: Use proper input system action event
        {
            if (!inDialogue && canEnterDialogue)
            {
                //StartCoroutine(StartDialogueCoroutine());
                yarnInteractable.StartConversation();
            }
            else if (inDialogue && canExitDialogue)
            {
                //StartCoroutine(EndDialogueCoroutine());
            }
        }
    }

}
