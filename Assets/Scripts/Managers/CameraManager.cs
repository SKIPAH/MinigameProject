using Cinemachine;
using UnityEngine;
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineVirtualCamera cinemachineCam;
    [SerializeField] private GameObject mainCameraObject;
    [SerializeField] private Transform asteroidCameraPos;
    [SerializeField] private Transform lionGameCameraPos;
    [SerializeField] private GameObject cinemachineGameObject;
    [SerializeField] private Bananas bananas;
    private void Start()
    {
        Instance = this;
        bananas.OnBananaHit += Bananas_OnBananaHit;
        PlayerMonkey.Instance.OnCoconutThrown += Instance_OnCoconutThrown;
        mainCameraObject.transform.position = asteroidCameraPos.transform.position;
        ChangeCameraProjectionSizeDefault();
    }

    private void Instance_OnCoconutThrown(object sender, System.EventArgs e)
    {
        
    }

    private void Bananas_OnBananaHit(object sender, System.EventArgs e)
    {
        ChangeCameraPos(lionGameCameraPos);
        ChangeCameraProjectionSizeLionGame();
    }
    public void ChangeCameraPos(Transform cameraPos)
    {
        mainCameraObject.transform.position = cameraPos.position;
    }
    public void ChangeCameraProjectionSize()
    {
        cinemachineCam.m_Lens.OrthographicSize = 15.0f;
        cinemachineGameObject.SetActive(true);
    }
    public void ChangeCameraProjectionSizeDefault()
    {
        cinemachineGameObject.SetActive(false);
        mainCamera.orthographicSize = 10.0f;
    }
    public void ChangeCameraProjectionSizeLionGame()
    {
        cinemachineGameObject.SetActive(false);
        mainCamera.orthographicSize = 20.0f;
    }
}
