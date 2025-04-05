using System.Collections;
using UnityEngine;

namespace Towers
{
    public class TroopEncampment : ProximityTowerMultiTarget
    {
        public float maxAngle;
        public Transform[] firePoints;
        private float[] _firePointCooldowns;
        public ParticleSystem[] particleSystems;
        private float _cosMaxAngle;
        private Coroutine _attackCoroutine;
        
        [Header("DebuggingGUI")]
        public bool showFireAngles;
        public bool showParticleSystems;
        
        private void Start()
        {
            _firePointCooldowns = new float[firePoints.Length];
            //Sets necessary protected variables.
            TowerSpawned();

            _cosMaxAngle = Mathf.Cos(maxAngle * Mathf.Deg2Rad);
        }

        private void Update()
        {
            availableTargets.RemoveAll(target => target == null || !target.activeInHierarchy);

            for (int i = 0; i < _firePointCooldowns.Length; i++)
            {
                _firePointCooldowns[i] -= Time.deltaTime;
            }

            switch (availableTargets.Count)
            {
                case > 0:
                    _attackCoroutine = StartCoroutine(Attack());
                    break;
                case <= 0 when _attackCoroutine != null:
                    StopCoroutine(_attackCoroutine);
                    _attackCoroutine = null;
                    foreach (var pS in particleSystems) { pS.Stop(); }
                    break;
            }
        }

        private IEnumerator Attack()
        {
            for (int i = 0; i < firePoints.Length; i++)
            {
                var target = DetectSingleTarget(firePoints[i]);

                if (target != null)
                {
                    if (!(_firePointCooldowns[i] < 0)) continue;
                    if (!particleSystems[i].isPlaying) { particleSystems[i].Play(); }
                    target.GetComponent<Enemies.Enemy>().DecreaseHealth(2);
                    _firePointCooldowns[i] = scriptableObject.defaultRateOfFire;
                }
                else
                {
                    particleSystems[i].Stop();
                }
            }
            yield return null;
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
            return dotProduct >= _cosMaxAngle;
        }
    }
}