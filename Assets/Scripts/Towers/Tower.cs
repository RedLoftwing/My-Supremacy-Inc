using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Enemies;

namespace Towers
{
    public class Tower : MonoBehaviour {
        [SerializeField] protected SO_Scripts.TowerInfo scriptableObject;
        
        // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=- Turret/FirePoint Variables -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
        protected enum TurretAxisTypes {
            Horizontal = 0,
            Vertical = 1,
            NonRotational = 2
        };
        [Serializable]
        protected class TurretAxis {
            public GameObject turretAxisPiece;
            public TurretAxisTypes turretAxisType;
            public float dot;
            public float turretRotationSpeed;
        }
        [Header("Turret/FirePoint Variables")]
        [SerializeField] protected TurretAxis[] turretAxes;
        [Serializable]
        public class FirePoint {
            public GameObject firePointPiece;
            public float timeSinceLastFired;
            public float dot;
            public float dotThreshold = 0.5f;
            public Enemy target;
            public ParticleSystem[] pS;
        }
        public FirePoint[] firePoints;
        private ParticleSystem[] _weaponParticleSystems;

        // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=- Target & Detection Related Variables -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
        [Header("Target Variables")]
        [SerializeField] protected List<GameObject> nearbyTargets = new ();
        public CapsuleCollider capsuleCollider;
        [SerializeField] public GameObject detectionRadiusCylinder;

        
        
        
        [Header("Range Values")]
        protected float Range;
        [Header("Damage Values")]
        protected float Damage;
        [Header("Rate of Fire Values")]
        protected float RateOfFire;

        private bool _isMultiTarget;
        private bool _isMultiPS;
        
        
        // //[SerializeField] protected GameObject[] horizontalTurret;
        // //[SerializeField] protected GameObject[] verticalTurret;
        //
        // //TODO: Change this from a string to a int perhaps.
        // [SerializeField] protected string[] validTargets;
        //
        // protected float RateOfFireTimer;
        //
        // //
        // protected readonly Collider[] Colliders = new Collider[10];
        //
        // [SerializeField] protected LayerMask towerLayerMask;
        //
        //
        //
        // //Used to allow/block attacks.
        // [SerializeField] protected bool isFireRunning;
        //
        // //Sounds.
        // [SerializeField] protected AudioSource weaponFire;
        //
        // //Explosives.
        // [SerializeField] protected bool hasExplosiveAmmo;
        // [SerializeField] protected GameObject explosiveEffect;

        private void Awake() {
            TowerSpawned();
        }

        protected void TowerSpawned() {
            TryGetComponent(out capsuleCollider);
            if (!capsuleCollider) { Debug.LogError("No capsule collider found!"); }

            // foreach (var firePoint in firePoints) {
            //     firePoint.firePointPiece.TryGetComponent(out firePoint.pS);
            // }
            
            // Set default stats.
            Damage = scriptableObject.defaultDamage;
            Range = scriptableObject.defaultRange;
            RateOfFire = scriptableObject.defaultRateOfFire;
            
            // TODO: Temp
            capsuleCollider.radius = Range;
            var newSize = new Vector3(Range * 2, 200, Range * 2);
            detectionRadiusCylinder.transform.localScale = newSize;
            
            _isMultiTarget = (firePoints.Length > 1);
            _isMultiPS = (firePoints[0].pS.Length > 1);
            
            // Start Coroutine for updating stats.
            StartCoroutine(UpdateStats());
        }

        protected virtual IEnumerator UpdateStats() {
            yield return null;
        }

        // TODO: Swap out nearbyTargets[0] from function. Placed to remove errors for now.
        // TODO: Might keep cause it does seem to prioritise the first enemy in the list...
        private void Update() {
            nearbyTargets.RemoveAll(target => !target || !target.activeInHierarchy);

            // if (nearbyTargets.Count <= 0) {
            //     foreach (var firePoint in firePoints) firePoint.particleSystem.Stop();
            // }
            
            if (!_isMultiPS) UpdateParticleSystem();
            CheckTurretRotations();
            if (_isMultiTarget) CheckFirePointsMultiTarget();
            else CheckFirePoints();
        }

        protected virtual void UpdateParticleSystem() {
            foreach (var firePoint in firePoints) {
                if (firePoint.target) {
                    if (!firePoint.pS[0].isPlaying) firePoint.pS[0].Play();
                }
                else {
                    if (firePoint.pS[0].isPlaying) firePoint.pS[0].Stop();
                }
            }
        }

        protected virtual void UpdateMultiParticleSystem() {
            foreach (var firePoint in firePoints) {
                foreach (var pS in firePoint.pS) pS.Play();
            }
        }

        private void CheckTurretRotations() {
            foreach (var turretAxis in turretAxes) {
                if (turretAxis.turretAxisType is not (TurretAxisTypes.Horizontal or TurretAxisTypes.Vertical)) continue;
                // Check if the turret axis piece is facing the target.
                var turretAxisToTargetDir = (nearbyTargets[0].transform.position - turretAxis.turretAxisPiece.transform.position).normalized;
                turretAxis.dot = Vector3.Dot(turretAxis.turretAxisPiece.transform.forward, turretAxisToTargetDir);
                if (turretAxis.dot < 1.0f) { RotateTurret(turretAxis, nearbyTargets[0]); }
            }
        }

        private void CheckFirePoints() {
            foreach (var firePoint in firePoints) {
                // Check if the fire point is facing in the general direction of the target, AND if there has been enough time since the fire point last fired.
                var firePointToTargetDir = (nearbyTargets[0].transform.position - firePoint.firePointPiece.transform.position).normalized;
                firePoint.dot = Vector3.Dot(firePoint.firePointPiece.transform.forward, firePointToTargetDir);
                if (firePoint.dot > firePoint.dotThreshold && firePoint.timeSinceLastFired > RateOfFire) {                         
                    TryGetComponent(out firePoint.target);
                    if (firePoint.target) FireWeapon(firePoint, firePoint.target);
                }
                firePoint.timeSinceLastFired += Time.deltaTime;
            }
        }
        
        private void CheckFirePointsMultiTarget() {
            foreach (var firePoint in firePoints) {
                if (firePoint.target) {
                    firePoint.dot = CalcDotProduct(firePoint.firePointPiece, firePoint.target.gameObject);
                    if (firePoint.dot <= firePoint.dotThreshold) {
                        firePoint.target = null;
                    }
                    else if (firePoint.timeSinceLastFired > RateOfFire) { FireWeapon(firePoint, firePoint.target); }
                }
                else {
                    UpdateParticleSystem();
                    foreach (var target in nearbyTargets) {
                        firePoint.dot = CalcDotProduct(firePoint.firePointPiece, target.gameObject);
                        if (firePoint.dot > firePoint.dotThreshold) {
                            target.TryGetComponent(out firePoint.target);
                            if (firePoint.target) {
                                if (firePoint.timeSinceLastFired > RateOfFire) {
                                    FireWeapon(firePoint, firePoint.target);
                                }
                            }
                        }
                    }
                }

                firePoint.timeSinceLastFired += Time.deltaTime;
            }
        }

        private static float CalcDotProduct(GameObject inDotFrom, GameObject inDotTo) {
            var toTargetDir = (inDotTo.transform.position - inDotFrom.transform.position).normalized;
            return Vector3.Dot(inDotFrom.transform.forward, toTargetDir);
        }

        private static void RotateTurret(TurretAxis inTurret, GameObject inCurrentTarget) {
            //Get the relative target direction and store it as targetDir.
            var targetDir = (inCurrentTarget.transform.position - inTurret.turretAxisPiece.transform.position).normalized;
            var targetRot = Quaternion.LookRotation(targetDir);
            var maxDeg = inTurret.turretRotationSpeed * Time.deltaTime;
            
            switch (inTurret.turretAxisType) {
                case TurretAxisTypes.Horizontal:
                    var lookAtRotationLimitY = Quaternion.Euler(inTurret.turretAxisPiece.transform.rotation.eulerAngles.x, targetRot.eulerAngles.y, inTurret.turretAxisPiece.transform.rotation.eulerAngles.z);
                    inTurret.turretAxisPiece.transform.rotation = Quaternion.RotateTowards(inTurret.turretAxisPiece.transform.rotation, lookAtRotationLimitY, maxDeg);
                    break;
                case TurretAxisTypes.Vertical:
                    var lookAtRotationLimitX = Quaternion.Euler(targetRot.eulerAngles.x, inTurret.turretAxisPiece.transform.rotation.eulerAngles.y, inTurret.turretAxisPiece.transform.rotation.eulerAngles.z);
                    inTurret.turretAxisPiece.transform.rotation = Quaternion.RotateTowards(inTurret.turretAxisPiece.transform.rotation, lookAtRotationLimitX, maxDeg);
                    break;
                case TurretAxisTypes.NonRotational:
                    Debug.LogError("RotateTurret called with a non-rotational turret.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void FireWeapon(FirePoint inFirePoint, Enemy inEnemy) {
            if (!inEnemy) {
                inFirePoint.pS.Stop();
                return;
            }
            inEnemy.DecreaseHealth(2);
            inFirePoint.timeSinceLastFired = 0f;
        }

        private void OnTriggerStay(Collider other) {
            // Checks if the other collider is a valid enemy type for this tower to target.
            other.TryGetComponent<Enemy>(out var enemyComp);
            if (!enemyComp) return;
            if (nearbyTargets.Contains(enemyComp.gameObject)) return;
            foreach (var targetType in scriptableObject.targetTypes) {
                if (other.gameObject.CompareTag(targetType.ToString())) {
                    nearbyTargets.Add(other.gameObject);
                }
            }
        }
        
        private void OnTriggerExit(Collider other) {
            // Checks if the other collider is an enemy in the list, then removes it.
            if (nearbyTargets.Contains(other.gameObject)) {
                nearbyTargets.Remove(other.gameObject);
            }

            // Ensures the other collider is not marked as the target for any fire points.
            foreach (var firePoint in firePoints) {
                if (!firePoint.target) continue;
                if (firePoint.target.gameObject == other.gameObject) firePoint.target = null;
            }
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            foreach (var firePoint in firePoints) {
                if (firePoint.target) {
                    Gizmos.DrawLine(firePoint.firePointPiece.transform.position, firePoint.target.transform.position);
                }
            }
        }
    }
}