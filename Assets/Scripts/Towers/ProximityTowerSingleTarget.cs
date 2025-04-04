using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Towers
{
    public class ProximityTowerSingleTarget : ProximityTower
    {
        [SerializeField] private ParticleSystem muzzlePlume;
        [SerializeField] private ParticleSystem backBlast;
        
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
                        Debug.Log("BACKBLAST!");
                        weaponFire.Play();
                        enemy.DecreaseHealth(Damage);
                        muzzlePlume.Play();
                        backBlast.Play();

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
                if (horizontalTurret != null)
                {
                    //Set the rotation speed over time value, and assigns it to singleStep.
                    float singleStep = scriptableObject.rotationSpeed * Time.deltaTime;
                    RotateTurret(horizontalTurret, singleStep, 'y');
                    
                    if (verticalTurret != null)
                    {
                        RotateTurret(verticalTurret, singleStep, 'x');
                    }
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