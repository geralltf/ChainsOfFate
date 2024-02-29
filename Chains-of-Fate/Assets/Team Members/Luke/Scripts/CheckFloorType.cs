using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CheckFloorType : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

	    RaycastHit2D hitInfo2D = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.right);

	    Debug.Log(hitInfo2D.collider.name);
	    
	    if (Physics.Raycast(transform.position, Vector3.forward, out RaycastHit hitInfo))
	    {
		    Debug.Log("Are those Freakin' Sharks?");
		    Sprite tile = hitInfo.collider.GetComponent<Sprite>();
		    if (tile != null) Debug.Log(tile.name);
	    }
    }
}
