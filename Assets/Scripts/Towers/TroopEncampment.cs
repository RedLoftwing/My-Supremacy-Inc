using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Towers
{
    public class TroopEncampment : ProximityTowerMultiTarget
    {
        public float maxAngle;
        public Transform[] firePoints;
        [SerializeField] private ParticleSystem[] particleSystems;
        
        private float cosMaxAngle;

        private int test;
        
        private void Start()
        {
            // foreach (var pS in particleSystems)
            // {
            //     pS.Clear();
            //     pS.Stop();
            // }
            
            
            //Sets necessary protected variables.
            TowerSpawned();

            cosMaxAngle = Mathf.Cos(maxAngle * Mathf.Deg2Rad);
            
            StartCoroutine(Attack());
        }

        private void Update()
        {
            availableTargets.RemoveAll(target => target == null || !target.activeInHierarchy);

            foreach (var target in availableTargets)
            {
                // Vector3 dirToTarget = target.transform.position - transform.position;
                //
                // dirToTarget.Normalize();
                // float angle = Vector3.Angle(transform.forward, dirToTarget);
                // if (angle <= 45f)
                // {
                //     Debug.DrawLine(transform.position, target.transform.position, Color.red);
                // }
                
                
                
            }


            foreach (var firePoint in firePoints)
            {
                DetectTargetsInRange(firePoint);
            }
            
            //Debug.Log(particleSystems[11].particleCount);
        }

        private IEnumerator Attack()
        {
            for (int i = 0; i < firePoints.Length; i++)
            {
                var target = DetectSingleTarget(firePoints[i]);
                
                if (target != null)
                {
                    if (!particleSystems[i].isPlaying)
                    {
                        // var emission = particleSystems[i].emission;
                        // emission.enabled = true;
                        particleSystems[i].Play();
                    }
                    target.GetComponent<Enemies.Enemy>().DecreaseHealth(2);
                }
                else
                {
                    // var emission = particleSystems[i].emission;
                    // emission.enabled = false;
                    particleSystems[i].Stop();
                }
            }

            //Debug.Log(test++);
            yield return null;
            StartCoroutine(Attack());
        }

        private Transform DetectSingleTarget(Transform firePoint)
        {
            foreach (var target in availableTargets)
            {
                if (IsTargetInRange(firePoint, target.transform) && IsTargetInAngle(firePoint, target.transform))
                {
                    return target.transform;
                }
            }

            return null;
        }
        
        private void DetectTargetsInRange(Transform firePoint)
        {
            // Iterate through all potential targets
            foreach (var target in availableTargets)
            {
                // Check if the target is within range and angle of the fire point
                if (IsTargetInRange(firePoint, target.transform) && IsTargetInAngle(firePoint, target.transform))
                {
                    //Debug.Log($"Target {target.name} is within range and angle of fire point {firePoint.name}");
                    // Add your logic here for what happens when a target is detected (e.g., attack the target)
                    //Debug.DrawLine(firePoint.position, target.transform.position, Color.yellow);
                    
                }
            }
        }

        private bool IsTargetInRange(Transform firePoint, Transform target)
        {
            // Calculate distance between the fire point and the target
            float distance = Vector3.Distance(firePoint.position, target.position);

            // Calculate the distance from the center of the tower and subtract firePoint's offset
            var distanceBetweenCentreAndFirePoint = firePoint.position - transform.position;
            float trueTotalRadius = capsuleCollider.radius - distanceBetweenCentreAndFirePoint.magnitude;

            // Check if the target is within range (trueTotalRadius)
            return distance <= trueTotalRadius;
        }

        private bool IsTargetInAngle(Transform firePoint, Transform target)
        {
            // Calculate the direction to the target
            Vector3 directionToTarget = (target.position - firePoint.position).normalized;

            // Calculate the forward direction of the fire point
            Vector3 firePointForward = firePoint.forward;

            // Use the dot product to check if the target is within the max angle
            float dotProduct = Vector3.Dot(firePointForward, directionToTarget);

            // Compare the dot product with the precomputed cosine of the max angle
            return dotProduct >= cosMaxAngle;
        }
    }
}