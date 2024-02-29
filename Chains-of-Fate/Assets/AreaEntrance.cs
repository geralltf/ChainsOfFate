using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEntrance : MonoBehaviour
{
    public string transitionName;

    // Start is called before the first frame update
    void Start()
    {
        if(transitionName == PlayerController.instance.areaTransitionName)
        {
            PlayerController.instance.transform.position = transform.position;
            foreach (PartyFollow t in PlayerController.instance.GetComponentsInChildren<PartyFollow>())
            {
                t.transform.localPosition = Vector3.zero;
                t.storedPositions.Clear();
                t.horAnimVectors.Clear();
                t.vertAnimVectors.Clear();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
