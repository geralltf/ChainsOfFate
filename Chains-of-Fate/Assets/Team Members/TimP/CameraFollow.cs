using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player;
    public Vector3 orthoOffset;
    public Vector3 perspectiveOffset;
    public float trackingSpeed = 2.0f;

    private Vector3 offset;
    private Camera _camera;
    private PlayerController playerController;
    
    public Vector3 GetCenterWorldPosition()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, _camera.nearClipPlane);

        Vector3 worldCenter = _camera.ScreenToWorldPoint(screenCenter);

        return worldCenter;
    }
    
    private void Awake()
    {
        //offset = transform.position - player.transform.position;

        // NEW offset based on positioning player on screen center.
        _camera = GetComponent<Camera>();
        
        offset = GetCenterWorldPosition();

        if (player == null)
        {
            player = ChainsOfFate.Gerallt.GameManager.Instance.GetPlayer().gameObject;
        }

        playerController = player.GetComponent<PlayerController>();
        playerController.OnReady += PlayerController_OnReady;
    }

    private void Start()
    {
        isometricCameraOffset = ChainsOfFate.Gerallt.GameManager.Instance.isometricCameraOffset;
    }

    private Vector3 GetOffset()
    {
        if (_camera.orthographic)
        {
            offset = orthoOffset;
        }
        else
        {
            offset = perspectiveOffset;
        }

        return offset;
    }
    
    private void PlayerController_OnReady()
    {
        transform.position = playerController.defaultSpawnLocation + GetOffset();
    }

    private ChainsOfFate.Gerallt.GameManager.CameraMode cameraMode = ChainsOfFate.Gerallt.GameManager.CameraMode.TopDown;
    private Vector3 isometricCameraOffset;
    
    public void ApplyOffsets()
    {
        Vector3 pos = playerController.transform.position;

        Vector3 offset = Vector3.zero;

        bool offsetChanged = false;
        if (isometricCameraOffset != ChainsOfFate.Gerallt.GameManager.Instance.isometricCameraOffset)
        {
            isometricCameraOffset = ChainsOfFate.Gerallt.GameManager.Instance.isometricCameraOffset;
            offsetChanged = true;
        }

        // Only apply offset if camera changes
        if (cameraMode != ChainsOfFate.Gerallt.GameManager.Instance.cameraMode || offsetChanged)
        {
            offset = ChainsOfFate.Gerallt.GameManager.Instance.isometricCameraOffset;
                
            if (ChainsOfFate.Gerallt.GameManager.Instance.cameraMode ==
                ChainsOfFate.Gerallt.GameManager.CameraMode.Isometric)
            {
                pos.x += offset.x;
                pos.y += offset.y;
            }
            else
            {
                pos.x -= offset.x;
                pos.y -= offset.y;
            }

            cameraMode = ChainsOfFate.Gerallt.GameManager.Instance.cameraMode;
        }
        
        pos.x += GetOffset().x;
        pos.y += GetOffset().y;
        pos.z += GetOffset().z;
        
        transform.position = pos;
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        ApplyOffsets();
        
        //transform.position = player.transform.position + offset;

        Vector3 pos = Vector3.Lerp(transform.position, player.transform.position, trackingSpeed * Time.fixedDeltaTime);

        pos.z = -14;
        transform.position = pos;
    }
}
