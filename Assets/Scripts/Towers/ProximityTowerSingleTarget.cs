using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Towers
{
    public class ProximityTowerSingleTarget : ProximityTower
    {
        [SerializeField] private ParticleSystem[] smokePlumes;
        
        // private IEnumerator FireTurret()
        // {
        //     //Ensures that it is not already firing...AND that there is a currentTarget before proceeding.
        //     if (isFireRunning) yield break;
        //     if (!CurrentTarget) yield break;
        //
        //     //Sets isFireRunning to true. Prevents constant execution of FireTurret.
        //     isFireRunning = true;
        //     //Grabs the Enemy component from the current target, and assigns it to enemy.
        //     var enemy = CurrentTarget.GetComponent<Enemies.Enemy>();
        //     //If enemy is true...Go through each string within the validTargets array, and check if the enemy tag matches any...
        //     if (enemy)
        //     {
        //         weaponFire.Play();
        //         enemy.DecreaseHealth(Damage);
        //         foreach (var smokePlume in smokePlumes)
        //         {
        //             smokePlume.Play();
        //         }
        //         
        //         //IF hasExplosiveAmmo is true...Instantiate the explosive effect at the current target.
        //         if (hasExplosiveAmmo)
        //         {
        //             Instantiate(explosiveEffect, CurrentTarget.transform.position, Quaternion.identity);
        //         }
        //     }
        //
        //     //Wait allocated number of seconds and then set isFireRunning to false, thus enabling the execution of FireTurret again.
        //     yield return new WaitForSeconds(RateOfFire);
        //     isFireRunning = false;
        // }
        
        // private void OnTriggerStay(Collider other)
        // {
        //     //IF there is no current target...Go through each string within the validTargets array, and check if the collision matches any...
        //     if (CurrentTarget == null)
        //     {
        //         foreach (var validTarget in validTargets)
        //         {
        //             //IF the collision matches a string from the validTargets array...set the current target to the collision.
        //             if (other.CompareTag(validTarget))
        //             {
        //                 CurrentTarget = other.gameObject;
        //             }
        //         }
        //     }
        //     else
        //     {
        //         //ELSE IF...there is a current target AND there is a turret...rotate the turret towards the target.
        //         if (horizontalTurret.Length > 0)
        //         {
        //             //Set the rotation speed over time value, and assigns it to singleStep.
        //             float singleStep = scriptableObject.rotationSpeed * Time.deltaTime;
        //             RotateTurret(horizontalTurret[0], singleStep, 'y', CurrentTarget);
        //             
        //             if (verticalTurret.Length > 0)
        //             {
        //                 RotateTurret(verticalTurret[0], singleStep, 'x', CurrentTarget);
        //             }
        //         }
        //     }
        //
        //     //IF isFireRunning is false...start the FireTurret coroutine.
        //     if (!isFireRunning)
        //     {
        //         StartCoroutine(FireTurret());
        //     }
        // }
        
        // private void OnTriggerExit(Collider other)
        // {
        //     //IF there is a current target...set the target value to null, stop the FireTurret coroutine.
        //     if (CurrentTarget != null)
        //     {
        //         CurrentTarget = null;
        //         StopCoroutine(FireTurret());
        //     }
        // }
    }
}