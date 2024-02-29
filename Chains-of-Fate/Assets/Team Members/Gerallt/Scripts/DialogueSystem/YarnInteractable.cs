using System.Collections;
using System.Collections.Generic;
using ChainsOfFate.Gerallt;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.Events;
using FMODUnity;
using FMOD.Studio;

public class YarnInteractable : MonoBehaviour
{
    // internal properties exposed to editor
    [SerializeField] private string conversationStartNode;

    // internal properties not exposed to editor
    [SerializeField] private EventReference fmodEvent;
    private EventInstance instance;

    [SerializeField] private DialogueRunner dialogueRunner;
    public bool resetDialogWhenComplete = true;
    public bool interactable = true;
    private bool isCurrentConversation = false;
    public bool IsDialogueRunning => dialogueRunner.IsDialogueRunning;
    public UnityEvent DialogueOutcomes;

    public void StartConversation()
    {
        Debug.Log($"Started conversation with {name}.");
        isCurrentConversation = true;

        instance.start();                                           //FMOD start sound - currently at the beginning of the box, not the text

        dialogueRunner.StartDialogue(conversationStartNode);
    }

    public void EndConversation()
    {
        if (isCurrentConversation)
        {
            isCurrentConversation = false;
            Debug.Log($"Ended conversation with {name}.");

            dialogueRunner.Stop();

            instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);         //FMOD stop sound - currently at the end of the box, not the text
        }
    }

    public void OnViewRequestedInterrupt()
    {
        dialogueRunner.OnViewRequestedInterrupt();
    }

//    [YarnCommand("disable")]
    public void DisableConversation()
    {
        interactable = false;
    }
    
    private void NodeStart(string nodeName)
    {
        //DialogueSystemUI.Instance.NodeStart(nodeName);
        DialogueOutcomes.Invoke();
    }
    
    private void Start()
    {
 
        instance = RuntimeManager.CreateInstance(fmodEvent);

        if (dialogueRunner == null)
        {
            dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>();
        }
        dialogueRunner.onDialogueComplete.AddListener(EndConversation);
        dialogueRunner.onNodeStart.AddListener(NodeStart);
    }
}