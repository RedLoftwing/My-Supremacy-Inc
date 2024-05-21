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


    public Vector2 camInput;
    public float rotateValue;
    public float zoomValue;
    public float zoomSensitivity;

    public float test;

    private void Start()
    {
        //Sets default values.
        newPos = transform.position;
        newRot = transform.rotation;
        moveSpeed = normalSpeed;
        newZoom = camTransform.localPosition;
    }

    //newPos += (transform.forward * (moveSpeed * camSensitivitySlider.value));

    private void Update()
    {
        //--Camera positioning--
        newPos += (transform.TransformDirection(camInput.x, 0, camInput.y)) * moveSpeed;
        //newPos += (transform.forward * (moveSpeed * camSensitivitySlider.value));

        //Lerp the objects position from the current position to the newPos vector over time.
        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * moveTime);


        //--Rotation--
        //Read the X and Y value of the input, and then set the newRot quaternion accordingly.
        if (rotateValue > 0)
        {
            newRot *= Quaternion.Euler(Vector3.up * rotAmount);
        }
        if (rotateValue < 0)
        {
            newRot *= Quaternion.Euler(Vector3.up * -rotAmount);
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
            moveSpeed = fastSpeed;
        }
        else
        {
            moveSpeed = normalSpeed;
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



















    //// Start is called before the first frame update
    //void Start()
    //{
    //    //Sets default values.
    //    newPos = transform.position;
    //    newRot = transform.rotation;
    //    newZoom = camTransform.localPosition;
    //    moveSpeed = normalSpeed;
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    //Set moveDirection to the vector2 value from the moveCam action, and then call OnCameraMove with that value.
    //    //moveDirection = InputReader.moveCam.ReadValue<Vector2>();
    //    //OnCameraMove(moveDirection);
    //    //Set rotateDirection to the vector2 value from the rotateCam action, and then call OnCameraRotate with that value.
    //    rotateDirection = InputReader.rotateCam.ReadValue<Vector2>();
    //    OnCameraRotate(rotateDirection);
    //    //Set zoomDirection to the vector2 value from the zoomCam action, and then call OnCameraZoomKeys with that value.
    //    zoomDirectionKeys = InputReader.zoomCamKeys.ReadValue<Vector2>();
    //    OnCameraZoomKeys(zoomDirectionKeys);
    //    //Call OnCameraZoomScroll.
    //    OnCameraZoomScrool(zoomDirectionScroll);
    //}

    //private void OnCameraMove(Vector2 input)
    //{
    //    //Read the X and Y value of the input, and then set the newPos vector in appropriate directions.
    //    if (input.y > 0)
    //    {
    //        newPos += (transform.forward * (moveSpeed * camSensitivitySlider.value));
    //    }
    //    if (input.y < 0)
    //    {
    //        newPos += (transform.forward * (-moveSpeed * camSensitivitySlider.value));
    //    }
    //    if (input.x > 0)
    //    {
    //        newPos += (transform.right * (moveSpeed * camSensitivitySlider.value));
    //    }
    //    if (input.x < 0)
    //    {
    //        newPos += (transform.right * (-moveSpeed * camSensitivitySlider.value));
    //    }
    //    //Lerp the objects position from the current position to the newPos vector over time.
    //    transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * moveTime);
    //}

    //private void OnCameraRotate(Vector2 input)
    //{
    //    //Read the X and Y value of the input, and then set the newRot quaternion accordingly.
    //    if (input.x > 0)
    //    {
    //        newRot *= Quaternion.Euler(Vector3.up * rotAmount);
    //    }
    //    if (input.x < 0)
    //    {
    //        newRot *= Quaternion.Euler(Vector3.up * -rotAmount);
    //    }
    //    //Lerp the objects rotation from the current rotation to the newRot quaternion rotation over time. 
    //    transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime * moveTime);
    //}

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

    //private void OnCameraZoomScrool(float input)
    //{
    //    //Read the float value of the input, and then set the newZoom vector accordingly.
    //    if (input > 0)
    //    {
    //        newZoom += zoomScrollAmount;
    //    }
    //    if (input < 0)
    //    {
    //        newZoom -= zoomScrollAmount;
    //    }
    //    //Lerp the camera's local position from the current local position to the newZoom vector over time.
    //    camTransform.localPosition = Vector3.Lerp(camTransform.localPosition, newZoom, Time.deltaTime * moveTime);
    //}
}
