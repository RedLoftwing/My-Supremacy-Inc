// using System;
// using System.Collections;
// using UnityEngine;
//
// namespace Towers
// {
//     public class ProximityTower : Tower
//     {
//         protected GameObject currentTarget;
//         
//         protected void TowerSpawned()
//         {
//             //Grabs the collider component to allow it to be adjusted in size. Uses the value of range to set the size. Collider used for detecting enemies within proximity.
//             capsuleCollider = GetComponent<CapsuleCollider>();
//             capsuleCollider.radius = Range * Cheats.Instance.variableTowerRangeSlider.value;
//             
//             Damage = scriptableObject.defaultDamage;
//             Range = scriptableObject.defaultRange;
//             RateOfFire = scriptableObject.defaultRateOfFire;
//             
//             //Start Coroutine for updating stats.
//             StartCoroutine(UpdateStats());
//         }
//
//         private IEnumerator UpdateStats()
//         {
//             //Updates the radius of the capsule collider when called. Multiplying the set range by the variableTowerRangeSlider value.
//             capsuleCollider.radius = Range * Cheats.Instance.variableTowerRangeSlider.value;
//             //Gathers all colliders within the radius of the tower.
//             int numColliders = Physics.OverlapSphereNonAlloc(transform.position, 15, Colliders, towerLayerMask);
//             //IF the number of colliders is greater than 0...then go through each collider, and find the IntelligenceCentre component.
//             if (numColliders > 0)
//             {
//                 for (int i = 0; i < numColliders; i++)
//                 {
//                     var nearbyIntelTower = Colliders[i].GetComponentInParent<IntelligenceCentre>();
//                     //IF the IntelligenceCentre component is true...check this tower's name and modify the corresponding stat IF it hasn't been modified previously. Update capsuleCollider radius if needed.
//                     if (nearbyIntelTower != null)
//                     {
//                         float difference = 0.001f;
//                         switch (scriptableObject.towerName)
//                         {
//                             case "Encampment":
//                                 if (Math.Abs(Range - scriptableObject.defaultRange) < difference)
//                                 {
//                                     Range *= 1.4f;
//                                     capsuleCollider.radius = Range * Cheats.Instance.variableTowerRangeSlider.value;
//                                 }
//                                 break;
//                             case "Anti-Tank Emplacement":
//                                 if (Math.Abs(Damage - scriptableObject.defaultDamage) < difference)
//                                 {
//                                     Damage *= 1.4f;
//                                 }
//                                 break;
//                             case "Anti-Aircraft Artillery":
//                                 if (Math.Abs(RateOfFire - scriptableObject.defaultRateOfFire) < difference)
//                                 {
//                                     RateOfFire /= 2;
//                                 }
//                                 break;
//                             case "Tank":
//                                 if (Math.Abs(Range - scriptableObject.defaultRange) < difference)
//                                 {
//                                     Range *= 1.4f;
//                                     capsuleCollider.radius = Range * Cheats.Instance.variableTowerRangeSlider.value;
//                                 }
//                                 break;
//                             default:
//                                 Debug.LogError("Is there meant to be another entry?");
//                                 break;
//                         }
//                     }
//                 }
//             }
//             
//             // Update the size of the detection radius based on the detection collider size.
//             if (detectionRadiusCylinder)
//             {
//                 detectionRadiusCylinder.transform.localScale = new Vector3(
//                     capsuleCollider.radius * 2, 
//                     detectionRadiusCylinder.transform.localScale.y, 
//                     capsuleCollider.radius * 2);
//             }
//
//             //Wait 5 seconds, and then start coroutine again.
//             yield return new WaitForSeconds(5);
//             StartCoroutine(UpdateStats());
//         }
//
//         protected void RotateTurret(GameObject inTurret, float inSingleStepValue, char inAxisToRotate, GameObject inCurrentTarget)
//         {
//             //Get the relative target direction and store it as targetDir.
//             Vector3 targetDir = inCurrentTarget.transform.position - inTurret.transform.position;
//             //Rotate the turret vector to targetDir over time using singleStep, and store the vector to newDir.
//             Vector3 newDir = Vector3.RotateTowards(inTurret.transform.forward, targetDir, inSingleStepValue, 0f);
//             //Rotate the turret to newDir.
//             Quaternion lookAtRotation = Quaternion.LookRotation(newDir);
//
//             switch (inAxisToRotate)
//             {
//                 case 'y':
//                 {
//                     Quaternion lookAtRotationLimitY = Quaternion.Euler(horizontalTurret[0].transform.rotation.eulerAngles.x, lookAtRotation.eulerAngles.y, horizontalTurret[0].transform.rotation.eulerAngles.z);
//                     //Set the turret's rotation to lookAtRotationLimitY.
//                     inTurret.transform.rotation = lookAtRotationLimitY;
//                     break;
//                 }
//                 case 'x':
//                 {
//                     Quaternion lookAtRotationLimitX = Quaternion.Euler(lookAtRotation.eulerAngles.x, horizontalTurret[0].transform.rotation.y, horizontalTurret[0].transform.rotation.z);
//                     inTurret.transform.localRotation = lookAtRotationLimitX;
//                     break;
//                 }
//             }
//         }
//     }
// }