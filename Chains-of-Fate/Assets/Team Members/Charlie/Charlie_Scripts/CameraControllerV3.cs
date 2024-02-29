using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]

public class CameraControllerV3 : MonoBehaviour {

    public Transform target;

    public Tilemap theMap;

    private Vector3 botLeftLimit;
    private Vector3 topRighLimit;

    private float halfHeight;
    private float halfWidth;

    private static CameraControllerV3 instance;

    // Start is called before the first frame update
    void Start()
    {
        target = FindObjectOfType<PlayerController>().transform;

        halfHeight = Camera.main.orthographicSize;
        halfWidth = halfHeight * Camera.main.aspect;

        botLeftLimit = theMap.localBounds.min + new Vector3(halfWidth, halfHeight, 0f);
        topRighLimit = theMap.localBounds.max + new Vector3(-halfWidth, -halfHeight, 0f);

        if(theMap == null)
        {
            Destroy(gameObject);
        }

        //DontDestroyOnLoad(gameObject);
    }

    // LateUpdate is called once per frame after Update
    void LateUpdate()
    {
        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);

        //keep camera inside bounds
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, botLeftLimit.x, topRighLimit.x), Mathf.Clamp(transform.position.y, botLeftLimit.y, topRighLimit.y), transform.position.z);

        if (theMap == null)
        {
            Destroy(gameObject);
        }
    }
}
