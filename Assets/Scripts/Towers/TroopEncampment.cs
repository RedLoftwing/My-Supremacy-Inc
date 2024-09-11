using System;
using System.Collections;
using UnityEngine;

namespace Towers
{
    public class TroopEncampment : ProximityTower
    {
        public float maxAngle;
        public Transform[] firePoints;

        private float cosMaxAngle;
        
        private void Start()
        {
            //Sets necessary protected variables.
            TowerSpawned();

            cosMaxAngle = Mathf.Cos(maxAngle * Mathf.Deg2Rad);
        }

        private void Update()
        {
            UpdateList();

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
        }
        
        private void DetectTargetsInRange(Transform firePoint)
        {
            // Iterate through all potential targets
            foreach (var target in availableTargets)
            {
                // Check if the target is within range and angle of the fire point
                if (IsTargetInRange(firePoint, target.transform) && IsTargetInAngle(firePoint, target.transform))
                {
                    Debug.Log($"Target {target.name} is within range and angle of fire point {firePoint.name}");
                    // Add your logic here for what happens when a target is detected (e.g., attack the target)
                    Debug.DrawLine(firePoint.position, target.transform.position, Color.yellow);
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