using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyFollow : MonoBehaviour
{
    public GameObject player;

    public int followDistance;
    public List<Vector3> storedPositions;

    public Animator thisAnimator;
    public Animator mcAnimator;
    public List<float> horAnimVectors;
    public List<float> vertAnimVectors;

    public GameObject PlayerScript;
    private PlayerController moveCheck;

    public GameObject combatPrefab;

    void Awake()
    {
        storedPositions = new List<Vector3>();
        horAnimVectors = new List<float>();
        vertAnimVectors = new List<float>();
    }

    void Start()
    {
        moveCheck = PlayerScript.GetComponent<PlayerController>();
    }

    void Update()
    {
        if (storedPositions.Count == 0)
        {
            storedPositions.Add(player.transform.position);
            horAnimVectors.Add(mcAnimator.GetFloat("Horizontal"));
            vertAnimVectors.Add(mcAnimator.GetFloat("Vertical"));
            return;
        }
        else if (storedPositions[storedPositions.Count - 1] != player.transform.position)
        {
            storedPositions.Add(player.transform.position);
            horAnimVectors.Add(mcAnimator.GetFloat("Horizontal"));
            vertAnimVectors.Add(mcAnimator.GetFloat("Vertical"));
        }
        if(moveCheck.playerMoving == true)
        {
            transform.position = storedPositions[0];

            thisAnimator.SetFloat("Horizontal", horAnimVectors[0]);
            thisAnimator.SetFloat("Vertical", vertAnimVectors[0]);
            thisAnimator.SetBool("isMoving", true);
            
            if (storedPositions.Count > followDistance)
            {
                storedPositions.RemoveAt(0);
                horAnimVectors.RemoveAt(0);
                vertAnimVectors.RemoveAt(0);
            }
        }
        if (moveCheck.playerMoving == false)
        {
            thisAnimator.SetBool("isMoving", false);
        }
    }
}
