using System;
using System.Collections;
using UnityEngine;

namespace Towers
{
    public class ProximityTower : Tower
    {
        private Cheats _cheatsScript;
        [SerializeField] protected GameObject currentTarget;
        
        protected void TowerSpawned()
        {
            //Grab cheats component and store it as cheatsScript.
            _cheatsScript = GameObject.Find("Manager").GetComponent<Cheats>();
            //Grabs the collider component to allow it to be adjusted in size. Uses the value of range to set the size. Collider used for detecting enemies within proximity.
            capsuleCollider = GetComponent<CapsuleCollider>();
            capsuleCollider.radius = Range * _cheatsScript.variableTowerRangeSlider.value;
            
            Damage = scriptableObject.defaultDamage;
            Range = scriptableObject.defaultRange;
            RateOfFire = scriptableObject.defaultRateOfFire;

            //Start Coroutine for updating stats.
            StartCoroutine(UpdateStats());
        }

        private IEnumerator UpdateStats()
        {
            //Updates the radius of the capsule collider when called. Multiplying the set range by the variableTowerRangeSlider value.
            capsuleCollider.radius = Range * _cheatsScript.variableTowerRangeSlider.value;
            //Gathers all colliders within the radius of the tower.
            int numColliders = Physics.OverlapSphereNonAlloc(transform.position, 15, Colliders, towerLayerMask);
            //IF the number of colliders is greater than 0...then go through each collider, and find the IntelligenceCentre component.
            if (numColliders > 0)
            {
                for (int i = 0; i < numColliders; i++)
                {
                    var nearbyIntelTower = Colliders[i].GetComponentInParent<IntelligenceCentre>();
                    //IF the IntelligenceCentre component is true...check this tower's name and modify the corresponding stat IF it hasn't been modified previously. Update capsuleCollider radius if needed.
                    if (nearbyIntelTower != null)
                    {
                        float difference = 0.001f;
                        
                        switch (scriptableObject.towerName)
                        {
                            case "Encampment":
                                if (Math.Abs(Range - scriptableObject.defaultRange) < difference)
                                {
                                    Range = Range * 1.4f;
                                    capsuleCollider.radius = Range * _cheatsScript.variableTowerRangeSlider.value;
                                }
                                break;
                            case "ATEmplacement":
                                if (Math.Abs(Damage - scriptableObject.defaultDamage) < difference)
                                {
                                    Damage = Damage * 1.4f;
                                }
                                break;
                            case "AAA":
                                if (Math.Abs(RateOfFire - scriptableObject.defaultRateOfFire) < difference)
                                {
                                    RateOfFire = RateOfFire / 2;
                                }
                                break;
                            case "Tank":
                                if (Math.Abs(Range - scriptableObject.defaultRange) < difference)
                                {
                                    Range = Range * 1.4f;
                                    capsuleCollider.radius = Range * _cheatsScript.variableTowerRangeSlider.value;
                                }
                                break;
                            default:
                                Debug.LogError("Is there meant to be another entry?");
                                break;
                        }
                    }
                }
            }

            //Wait 5 seconds, and then start coroutine again.
            yield return new WaitForSeconds(5);
            StartCoroutine(UpdateStats());
        }



        

        protected void RotateTurret(GameObject turret, float singleStepValue, char axisToRotate)
        {
            //Get the relative target direction and store it as targetDir.
            Vector3 targetDir = currentTarget.transform.position - turret.transform.position;
            //Rotate the turret vector to targetDir over time using singleStep, and store the vector to newDir.
            Vector3 newDir = Vector3.RotateTowards(turret.transform.forward, targetDir, singleStepValue, 0f);
            //Rotate the turret to newDir.
            Quaternion lookAtRotation = Quaternion.LookRotation(newDir);

            switch (axisToRotate)
            {
                case 'y':
                {
                    Quaternion lookAtRotationLimitY = Quaternion.Euler(horizontalTurret.transform.rotation.eulerAngles.x, lookAtRotation.eulerAngles.y, horizontalTurret.transform.rotation.eulerAngles.z);
                    //Set the turret's rotation to lookAtRotationLimitY.
                    turret.transform.rotation = lookAtRotationLimitY;
                    break;
                }
                case 'x':
                {
                    Quaternion lookAtRotationLimitX = Quaternion.Euler(lookAtRotation.eulerAngles.x, horizontalTurret.transform.rotation.y, horizontalTurret.transform.rotation.z);
                    turret.transform.localRotation = lookAtRotationLimitX;
                    break;
                }
            }
        }
        

    }
}