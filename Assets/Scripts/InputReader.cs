using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class InputReader : MonoBehaviour
{
    private PlayerInput playerInput;
    private MySupremacyInc mySupremacyIncActions;

    //public static InputAction moveCam;
    //public static InputAction changeCamSpeed;
    //public static InputAction rotateCam;
    //public static InputAction zoomCamKeys;
    //public static InputAction zoomCamScroll;
    //private InputAction lMBClick;

    [SerializeField] private CameraController cameraController;
    [SerializeField] private WorldInteraction worldInteraction;
    [SerializeField] private FixedTargetTower fixedTargetTower;

    private void Awake()
    {
        //Initialise.
        mySupremacyIncActions = new MySupremacyInc();

        //Binding the actions to relevant functions.
        mySupremacyIncActions.Camera.ChangeCameraSpeed.performed += ctx => cameraController.moveSpeed = cameraController.fastSpeed;
        mySupremacyIncActions.Camera.ChangeCameraSpeed.canceled += ctx => cameraController.moveSpeed = cameraController.normalSpeed;
        //mySupremacyIncActions.Camera.ZoomCameraScroll.performed += context => cameraController.zoomDirectionScroll = context.ReadValue<float>();

        //Binding the LMBClick action to the RayCastClick function.
        //mySupremacyIncActions.Camera.LMBClick.performed += ctx => worldInteraction.RayCastClick();
    }

    private void OnEnable()
    {
        //---Camera Controller---
        //Enable Move actions.
        //moveCam = mySupremacyIncActions.Camera.Move;
        //moveCam.Enable();
        //Enable Rotate actions.
        //rotateCam = mySupremacyIncActions.Camera.RotateCamera;
        //rotateCam.Enable();
        //Enable Changing Camera Speed actions.
        //changeCamSpeed = mySupremacyIncActions.Camera.ChangeCameraSpeed;
        //changeCamSpeed.Enable();
        //Enable Zoom Camera Keys actions.
        //zoomCamKeys = mySupremacyIncActions.Camera.ZoomCamera;
        //zoomCamKeys.Enable();
        //Enable Zoom Camera Scrolling actions.
        //zoomCamScroll = mySupremacyIncActions.Camera.ZoomCameraScroll;
        //zoomCamScroll.Enable();

        //---World Interaction
        //Enable LMBClick actions.
        //lMBClick = mySupremacyIncActions.Camera.LMBClick;
        //lMBClick.Enable();
    }

    private void OnDisable()
    {
        //---Camera Controller---
        //Disable all actions.
        //moveCam.Disable();
        //rotateCam.Disable();
        //changeCamSpeed.Disable();
        //zoomCamKeys.Disable();
        //zoomCamScroll.Disable();

        //Disable this action.
        //lMBClick.Disable();
    }
}
