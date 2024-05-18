using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject mainCameraObject;
    [SerializeField] private Transform asteroidCameraPos;
    [SerializeField] private Transform trampolineCameraPos;

    private void Start()
    {
        mainCameraObject.transform.position = asteroidCameraPos.transform.position;
    }

    public void ChangeCameraPos(Transform cameraPos)
    {
        mainCameraObject.transform.position = cameraPos.position;
    }

    public void ChangeCameraProjectionSize()
    {
        mainCamera.orthographicSize = 50.0f;
    }

}
