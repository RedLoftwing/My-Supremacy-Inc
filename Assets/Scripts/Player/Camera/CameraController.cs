using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
//using Vector2 = UnityEngine.Vector2;

public class CameraController : MonoBehaviour
{
    public Transform camTransform;

    private Vector2 moveDirection = Vector2.zero;
    private Vector2 rotateDirection = Vector2.zero;
    private Vector2 zoomDirectionKeys = Vector2.zero;
    public float zoomDirectionScroll = 0;

    public float normalSpeed;
    public float fastSpeed;
    public float moveSpeed;
    public float moveTime;
    public float rotAmount;
    public Vector3 zoomAmount;
    public Vector3 zoomScrollAmount;
    [SerializeField] private Slider camSensitivitySlider;

    public Vector3 newPos;
    public Quaternion newRot;
    public Vector3 newZoom;

    //New variables
    public Vector2 camInput;
    public float camSensitivity;
    public float rotateValue;
    public float rotateSensitivity;
    public float zoomValue;
    public float zoomSensitivity;

    public float test;

    private void Start()
    {
        //Sets default values.
        newPos = transform.position;
        newRot = transform.rotation;
        normalSpeed = camSensitivity;
        fastSpeed = camSensitivity * 2;
        newZoom = camTransform.localPosition;
    }

    //newPos += (transform.forward * (moveSpeed * camSensitivitySlider.value));

    private void Update()
    {
        //--Camera positioning--
        newPos += (transform.TransformDirection(camInput.x, 0, camInput.y)) * camSensitivity;

        //Lerp the objects position from the current position to the newPos vector over time.
        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * moveTime);


        //--Rotation--
        //Read the X and Y value of the input, and then set the newRot quaternion accordingly.
        if (rotateValue > 0)
        {
            newRot *= Quaternion.Euler(Vector3.up * rotateSensitivity);
        }
        if (rotateValue < 0)
        {
            newRot *= Quaternion.Euler(Vector3.up * -rotateSensitivity);
        }
        //Lerp the objects rotation from the current rotation to the newRot quaternion rotation over time. 
        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime * moveTime);

        //--Zoom--
        //Read the X and Y value of the input, and then set the newZoom vector accordingly.
        if (zoomValue > 0)
        {
            newZoom += (zoomAmount * zoomSensitivity);
        }
        if (zoomValue < 0)
        {
            newZoom -= (zoomAmount * zoomSensitivity);
        }
        //Lerp the camera's local position from the current local position to the newZoom vector over time.
        camTransform.localPosition = Vector3.Lerp(camTransform.localPosition, newZoom, Time.deltaTime * moveTime);
    }

    public void OnCameraMove(InputValue inValue)
    {
        camInput = inValue.Get<Vector2>();
    }

    public void OnChangeCameraSpeed(InputValue inValue)
    {
        var value = inValue.Get<float>();

        if (value > 0)
        {
            camSensitivity = fastSpeed;
        }
        else
        {
            camSensitivity = normalSpeed;
        }
    }

    public void OnCameraRotate(InputValue inValue)
    {
        rotateValue = inValue.Get<float>();
    }

    public void OnCameraZoom(InputValue inValue)
    {
        zoomValue = inValue.Get<float>();
    }

    public void OnLeftMouseButton(InputValue inValue)
    {
        test = inValue.Get<float>();
    }


    //private void OnCameraZoomKeys(Vector2 input)
    //{
    //    //Read the X and Y value of the input, and then set the newZoom vector accordingly.
    //    if(input.y > 0)
    //    {
    //        newZoom += zoomAmount;
    //    }
    //    if(input.y < 0)
    //    {
    //        newZoom -= zoomAmount;
    //    }
    //    //Lerp the camera's local position from the current local position to the newZoom vector over time.
    //    camTransform.localPosition = Vector3.Lerp(camTransform.localPosition, newZoom, Time.deltaTime * moveTime);
    //}
}
