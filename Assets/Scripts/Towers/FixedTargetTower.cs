using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FixedTargetTower : Tower
{
    private MySupremacyInc mySupremacyIncActions;
    private InputAction lMBClick;
    private GridGenerator levelGrid;
    [SerializeField] protected Vector3 currentTarget;
    [SerializeField] private LayerMask terrainLayerMask;
    [SerializeField] private bool isSelectingTarget = true;
    [SerializeField] protected Transform targetHighlight;

    private float terrainOffset;

    private void Awake()
    {
        //Initialise.
        mySupremacyIncActions = new MySupremacyInc();
        //Finds the object containing the GridGenerator script, and stores it.
        levelGrid = FindObjectOfType<GridGenerator>();

        //Binding the LMBClick action to the RayCastClick function.
        //mySupremacyIncActions.Camera.LMBClick.performed += ctx => ConfirmTarget();
    }

    private void OnEnable()
    {
        ////Enable LMBClick actions.
        //lMBClick = mySupremacyIncActions.Camera.LMBClick;
        //lMBClick.Enable();
    }

    private void OnDisable()
    {
        //Disable this action.
        lMBClick.Disable();
    }

    //Called every frame by a child script. 
    protected IEnumerator ManualUpdate()
    {
        //IF the player is selecting a target for the associated tower...
        if (isSelectingTarget)
        {
            //Prevent clicking on WorldInteraction script.
            WorldInteraction.isClickDisabled = true;

            //Conduct a raycast from the camera to where the player has clicked, and store it.
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            //[DEV] Draw the raycast.
            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.magenta);
            //IF the raycast intersects with an object on the terrainLayerMask layers...store collision information in hitInfo and proceed...
            if (Physics.Raycast(ray, out hitInfo, 10000, terrainLayerMask))
            {
                //Determines the required terrain Y axis offset.
                switch (hitInfo.collider.gameObject.tag)
                {
                    case "Height-0.0":
                        terrainOffset = 0f;
                        break;
                    case "Height-2.5":
                        terrainOffset = 2.5f;
                        break;
                    case "Height-5.0":
                        terrainOffset = 5f;
                        break;
                    default:
                        terrainOffset = 0f;
                        break;
                }

                //Ensures the target highlight object is set to active.
                targetHighlight.gameObject.SetActive(true);
                //Calls the GetNearestPointOnGrid function from the specified grid. Returns with the nearest point on the grid, and stores this point as checkPos.
                Vector3 checkPos = levelGrid.GetNearestPointOnGrid(hitInfo.point);
                //Moves the target highlight to the position the player is aiming at.
                targetHighlight.position = new(checkPos.x, checkPos.y + terrainOffset, checkPos.z);
                //Set currentTarget to the impact point of the raycast.
                currentTarget = targetHighlight.position;
                //Get the relative target direction and store it as targetDir.
                Vector3 targetDir = (currentTarget - turret.transform.position);
                //Create the rotation for targetDir, then limit the rotation to 1 axis (y) and store it as lookAtRotationLimitY.
                Quaternion lookAtRotation = Quaternion.LookRotation(targetDir);
                Quaternion lookAtRotationLimitY = Quaternion.Euler(turret.transform.rotation.eulerAngles.x, lookAtRotation.eulerAngles.y, turret.transform.rotation.eulerAngles.z);
                //Set the turret's rotation to lookAtRotationLimitY.
                turret.transform.rotation = lookAtRotationLimitY;
            }
        }

        //Gathers all colliders within the radius of the tower.
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, 15, colliders, towerLayerMask);
        //IF the number of colliders is greater than 0...then go through each collider, and find the IntelligenceCentre component.
        if (numColliders > 0)
        {
            for (int i = 0; i < numColliders; i++)
            {
                var nearbyIntelTower = colliders[i].GetComponent<IntelligenceCentre>();
                //IF the IntelligenceCentre component is true...check this tower's name and modify the coresponding stat IF it hasn't been modified previously.
                if (nearbyIntelTower != null)
                {
                    if (towerName == "Encampment")
                    {
                        if (range != defaultRange)
                        {
                            range = range * 1.4f;
                        }
                    }
                    else if (towerName == "ATEmplacement")
                    {
                        if (damage != defaultDamage)
                        {
                            damage = damage * 1.4f;
                        }
                    }
                    else if (towerName == "AAA")
                    {
                        if (rateOfFire != defaultRateOfFire)
                        {
                            rateOfFire = rateOfFire / 2;
                        }
                    }
                    else if (towerName == "Tank")
                    {
                        if (range != defaultRange)
                        {
                            range = range * 1.4f;
                        }
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
        WorldInteraction.isClickDisabled = false;
    }
}
