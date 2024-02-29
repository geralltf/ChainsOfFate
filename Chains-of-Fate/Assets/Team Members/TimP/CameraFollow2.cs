using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow2 : MonoBehaviour
{
    public Transform followTransform;
    public BoxCollider mapBounds;
    private float xMin, xMax, yMin, yMax, zMin, zMax;
    private float camY,camX,camZ;
    private float camOrthsize;
    private float cameraRatio;
    private Camera mainCam;

    private void Start()
    {
        if (!mapBounds)
        {
            xMin = mapBounds.bounds.min.x;
            xMax = mapBounds.bounds.max.x;
            yMin = mapBounds.bounds.min.y;
            yMax = mapBounds.bounds.max.y;
            zMin = mapBounds.bounds.min.z;
            zMax = mapBounds.bounds.max.z;
        }
        mainCam = GetComponent<Camera>();
        camOrthsize = mainCam.orthographicSize;
        cameraRatio = (xMax + camOrthsize) / 7.0f;
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        camY = Mathf.Clamp(followTransform.position.y, yMin + cameraRatio, yMax - cameraRatio);
        camX = Mathf.Clamp(followTransform.position.x, xMin + cameraRatio, xMax - cameraRatio);
        camZ = Mathf.Clamp(followTransform.position.z, zMin + camOrthsize, zMax - camOrthsize);
        this.transform.position = new Vector3(camX, camY, camZ);
    }
}
