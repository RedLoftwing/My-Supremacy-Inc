using System;
using System.Collections;
using UnityEngine;

namespace Towers
{
    public class ProximityTower : Tower
    {
        private Cheats _cheatsScript;
        [SerializeField] private GameObject currentTarget;

        protected void TowerSpawned()
        {
            //Grab cheats component and store it as cheatsScript.
            _cheatsScript = GameObject.Find("Manager").GetComponent<Cheats>();
            //Grabs the collider component to allow it to be adjusted in size. Uses the value of range to set the size. Collider used for detecting enemies within proximity.
            CapsuleCollider = GetComponent<CapsuleCollider>();
            CapsuleCollider.radius = Range * _cheatsScript.variableTowerRangeSlider.value;
            
            Damage = scriptableObject.defaultDamage;
            Range = scriptableObject.defaultRange;
            RateOfFire = scriptableObject.defaultRateOfFire;

            //Start Coroutine for updating stats.
            StartCoroutine(UpdateStats());
        }

        private IEnumerator UpdateStats()
        {
            //Updates the radius of the capsule collider when called. Multiplying the set range by the variableTowerRangeSlider value.
            CapsuleCollider.radius = Range * _cheatsScript.variableTowerRangeSlider.value;
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
                                    CapsuleCollider.radius = Range * _cheatsScript.variableTowerRangeSlider.value;
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
                                    CapsuleCollider.radius = Range * _cheatsScript.variableTowerRangeSlider.value;
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

        private IEnumerator FireTurret()
        {
            if (!isFireRunning)
            {
                //Ensures that there is a currentTarget before proceeding.
                if (currentTarget)
                {
                    //Sets isFireRunning to true. Prevents constant execution of FireTurret.
                    isFireRunning = true;
                    //Grabs the Enemy component from the current target, and assigns it to enemy.
                    Enemies.Enemy enemy = currentTarget.GetComponent<Enemies.Enemy>();
                    //If enemy is true...Go through each string within the validTargets array, and check if the enemy tag matches any...
                    if (enemy)
                    {
                        weaponFire.Play();
                        enemy.DecreaseHealth(Damage);

                        //IF hasExplosiveAmmo is true...Instantiate the explosive effect at the current target.
                        if (hasExplosiveAmmo)
                        {
                            Instantiate(explosiveEffect, currentTarget.transform.position, Quaternion.identity);
                        }
                    }

                    //Wait allocated number of seconds and then set isFireRunning to false, thus enabling the execution of FireTurret again.
                    yield return new WaitForSeconds(RateOfFire);
                    isFireRunning = false;
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            //IF there is no current target...Go through each string within the validTargets array, and check if the collision matches any...
            if (currentTarget == null)
            {
                foreach (var validTarget in validTargets)
                {
                    //IF the collision matches a string from the validTargets array...set the current target to the collision.
                    if (other.CompareTag(validTarget))
                    {
                        currentTarget = other.gameObject;
                    }
                }
            }
            else
            {
                //ELSE IF...there is a current target AND there is a turret...rotate the turret towards the target.
                if (turret != null)
                {
                    //Get the relative target direction and store it as targetDir.
                    Vector3 targetDir = currentTarget.transform.position - turret.transform.position;
                    //Set the rotation speed over time value, and assigns it to singleStep.
                    float singleStep = scriptableObject.rotationSpeed * Time.deltaTime;
                    //Rotate the turret vector to targetDir over time using singleStep, and store the vector to newDir.
                    Vector3 newDir = Vector3.RotateTowards(turret.transform.forward, targetDir, singleStep, 0f);
                    //Rotate the turret to newDir.
                    Quaternion lookAtRotation = Quaternion.LookRotation(newDir);
                    Quaternion lookAtRotationLimitY = Quaternion.Euler(turret.transform.rotation.eulerAngles.x, lookAtRotation.eulerAngles.y, turret.transform.rotation.eulerAngles.z);
                    //Set the turret's rotation to lookAtRotationLimitY.
                    turret.transform.rotation = lookAtRotationLimitY;
                }
            }

            //IF isFireRunning is false...start the FireTurret coroutine.
            if (!isFireRunning)
            {
                StartCoroutine(FireTurret());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            //IF there is a current target...set the target value to null, stop the FireTurret coroutine.
            if (currentTarget != null)
            {
                currentTarget = null;
                StopCoroutine(FireTurret());
            }
        }
    }
}