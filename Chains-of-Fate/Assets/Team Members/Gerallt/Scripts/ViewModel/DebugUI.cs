using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChainsOfFate.Gerallt
{
    public class DebugUI : MonoBehaviour
    {
        [SerializeField] private GameObject viewCameraButtons;
        [SerializeField] private GameObject viewCameraButtonPrefab;
        [SerializeField] private float buttonSize;
        [SerializeField] private float buttonOffset;

        public void SelectCamera(Camera cam, GameManager.CameraMode cameraMode)
        {
            cam.gameObject.SetActive(true);

            // HACK: Hack to change Z-values of player, npcs, and enemies depending on camera mode
            if (cameraMode == GameManager.CameraMode.Isometric)
            {
                GameManager.Instance.spawnZ = GameManager.Instance.spawnOrthoZ;
            }
            else
            {
                GameManager.Instance.spawnZ = GameManager.Instance.spawnPerspectiveZ;
            }
            
            GameManager.Instance.cameraMode = cameraMode;

            WorldInfo[] worldInfos = FindObjectsOfType<WorldInfo>(true);
            foreach (WorldInfo worldInfo in worldInfos)
            {
                worldInfo.ChangeViewMode();
            }
        }
        
        public void PopulateCameras()
        {
            Camera[] cameras = FindObjectsOfType<Camera>(true);

            int i = 0;
            foreach (Camera cam in cameras)
            {
                GameObject go = Instantiate(viewCameraButtonPrefab, viewCameraButtons.transform);

                Vector3 pos = Vector3.zero;
                pos.x = (i * buttonSize) + buttonOffset;
                
                go.transform.localPosition = pos;
                
                Button cameraButton = go.GetComponent<Button>();
                
                cameraButton.GetComponentInChildren<TextMeshProUGUI>().text = cam.name;

                cameraButton.onClick.AddListener(() =>
                {
                    foreach (Camera camOther in cameras)
                    {
                        camOther.gameObject.SetActive(false);
                    }

                    GameManager.CameraMode cameraMode = GameManager.CameraMode.TopDown;

                    if (cam.orthographic)
                    {
                        cameraMode = GameManager.CameraMode.Isometric;
                    }
                    
                    SelectCamera(cam, cameraMode);
                });

                i++;
            }
        }
        
        private void Start()
        {
            PopulateCameras();
        }
    }
}