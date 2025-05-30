//using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
//using Vector2 = UnityEngine.Vector2;

namespace Player.Camera
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance { get; private set; }
        
        public Transform camTransform;
        public float zoomDirectionScroll;

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
        
        private void Awake()
        {
            if (Instance == null) { Instance = this; }
            else { Destroy(gameObject); }
        }
        
        private void Start()
        {
            //Sets default values.
            newPos = transform.position;
            newRot = transform.rotation;
            normalSpeed = camSensitivity;
            fastSpeed = camSensitivity * 2;
            newZoom = camTransform.localPosition;
        }

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
            camSensitivity = (value > 0) ? fastSpeed : normalSpeed;
        }

        public void OnCameraRotate(InputValue inValue)
        {
            rotateValue = inValue.Get<float>();
        }

        public void OnCameraZoom(InputValue inValue)
        {
            zoomValue = inValue.Get<float>();
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
}