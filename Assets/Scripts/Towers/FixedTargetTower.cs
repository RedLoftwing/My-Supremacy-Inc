using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Towers
{
    public class FixedTargetTower : Tower
    {
        //private MySupremacyInc mySupremacyIncActions;
        private InputAction _lMBClick;
        private GridGenerator _levelGrid;
        [SerializeField] protected Vector3 currentTarget;
        [SerializeField] private LayerMask terrainLayerMask;
        public bool isSelectingTarget = true;
        [SerializeField] protected Transform targetHighlight;

        private float _terrainOffset;

        private void Awake()
        {
            //Initialise.
            //mySupremacyIncActions = new MySupremacyInc();
            //Finds the object containing the GridGenerator script, and stores it.
            _levelGrid = FindObjectOfType<GridGenerator>();

            //Binding the LMBClick action to the RayCastClick function.
            //mySupremacyIncActions.Camera.LMBClick.performed += ctx => ConfirmTarget();
        }

        private void OnEnable()
        {
            ////Enable LMBClick actions.
            //lMBClick = mySupremacyIncActions.Camera.LMBClick;
            //lMBClick.Enable();
        }

        // private void OnDisable()
        // {
        //     //Disable this action.
        //     _lMBClick.Disable();
        // }

        //Called every frame by a child script. 
        protected IEnumerator ManualUpdate()
        {
            //IF the player is selecting a target for the associated tower...
            if (isSelectingTarget)
            {
                //Prevent clicking on WorldInteraction script.
                WorldInteraction.IsClickDisabled = true;

                //Conduct a raycast from the camera to where the player has clicked, and store it.
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                //[DEV] Draw the raycast.
                Debug.DrawRay(ray.origin, ray.direction * 1000, Color.magenta);
                //IF the raycast intersects with an object on the terrainLayerMask layers...store collision information in hitInfo and proceed...
                if (Physics.Raycast(ray, out RaycastHit hitInfo, 10000, terrainLayerMask))
                {
                    //Determines the required terrain Y axis offset.
                    switch (hitInfo.collider.gameObject.tag)
                    {
                        case "Height-0.0":
                            _terrainOffset = 0f;
                            break;
                        case "Height-2.5":
                            _terrainOffset = 2.5f;
                            break;
                        case "Height-5.0":
                            _terrainOffset = 5f;
                            break;
                        default:
                            _terrainOffset = 0f;
                            break;
                    }

                    //Ensures the target highlight object is set to active.
                    targetHighlight.gameObject.SetActive(true);
                    //Calls the GetNearestPointOnGrid function from the specified grid. Returns with the nearest point on the grid, and stores this point as checkPos.
                    Vector3 checkPos = _levelGrid.GetNearestPointOnGrid(hitInfo.point);
                    //Moves the target highlight to the position the player is aiming at.
                    targetHighlight.position = new(checkPos.x, checkPos.y + _terrainOffset, checkPos.z);
                    //Set currentTarget to the impact point of the raycast.
                    currentTarget = targetHighlight.position;
                    //Get the relative target direction and store it as targetDir.
                    Vector3 targetDir = (currentTarget - horizontalTurret[0].transform.position);
                    //Create the rotation for targetDir, then limit the rotation to 1 axis (y) and store it as lookAtRotationLimitY.
                    Quaternion lookAtRotation = Quaternion.LookRotation(targetDir);
                    Quaternion lookAtRotationLimitY = Quaternion.Euler(horizontalTurret[0].transform.rotation.eulerAngles.x, lookAtRotation.eulerAngles.y, horizontalTurret[0].transform.rotation.eulerAngles.z);
                    //Set the turret's rotation to lookAtRotationLimitY.
                    horizontalTurret[0].transform.rotation = lookAtRotationLimitY;
                }
            }

            //Gathers all colliders within the radius of the tower.
            int numColliders = Physics.OverlapSphereNonAlloc(transform.position, 15, Colliders, towerLayerMask);
            //IF the number of colliders is greater than 0...then go through each collider, and find the IntelligenceCentre component.
            if (numColliders > 0)
            {
                for (int i = 0; i < numColliders; i++)
                {
                    var nearbyIntelTower = Colliders[i].GetComponentInParent<IntelligenceCentre>();
                    //IF the IntelligenceCentre component is true...check this tower's name and modify the corresponding stat IF it hasn't been modified previously.
                    if (nearbyIntelTower != null)
                    {
                        float difference = 0.001f;
                        switch (scriptableObject.towerName)
                        {
                            case "Encampment":
                                if (Math.Abs(Range - scriptableObject.defaultRange) > difference)
                                {
                                    Range *= 1.4f;
                                }
                                break;
                            case "ATEmplacement":
                                if (Math.Abs(Damage - scriptableObject.defaultDamage) > difference)
                                {
                                    Damage *= 1.4f;
                                }
                                break;
                            case "AAA":
                                if (Math.Abs(RateOfFire - scriptableObject.defaultRateOfFire) > difference)
                                {
                                    RateOfFire /= 2;
                                }
                                break;
                            case "Tank":
                                if (Math.Abs(Range - scriptableObject.defaultRange) > difference)
                                {
                                    Range *= 1.4f;
                                }
                                break;
                            default:
                                Debug.LogError("Is there meant to be another entry?");
                                break;
                        }
                    }
                }
            }

            yield return null;
        }

        public void ConfirmTarget()
        {
            isSelectingTarget = false;
            //Enable clicking on WorldInteraction script.
            WorldInteraction.IsClickDisabled = false;
        }
    }
}