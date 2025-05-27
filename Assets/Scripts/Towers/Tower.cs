using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Enemies;

namespace Towers {
    public class Tower : MonoBehaviour {
        [SerializeField] protected SO_Scripts.TowerInfo scriptableObject;
        
        // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=- Turret/AimPoint Variables -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
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
        [Header("Turret/AimPoint Variables")]
        [SerializeField] protected TurretAxis[] turretAxes;
        [Serializable]
        public class AimPoint {
            public GameObject aimPointPiece;
            public float timeSinceLastFired;
            public float dot;
            public float dotThreshold = 0.5f;
            public Enemy target;
            public ParticleSystem[] pS;
        }
        public AimPoint[] aimPoints;
        private ParticleSystem[] _weaponParticleSystems;

        // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=- Target & Detection Related Variables -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
        [Header("Target Variables")]
        public CapsuleCollider capsuleCollider;
        [SerializeField] public GameObject detectionRadiusCylinder;
        [SerializeField] protected List<GameObject> nearbyTargets = new ();

        
        
        
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

        protected virtual void TowerSpawned() {
            // foreach (var firePoint in aimPoints) {
            //     firePoint.firePointPiece.TryGetComponent(out firePoint.pS);
            // }
            
            // Set default stats.
            Damage = scriptableObject.defaultDamage;
            Range = scriptableObject.defaultRange;
            RateOfFire = scriptableObject.defaultRateOfFire;
            
            // TODO: Temp
            capsuleCollider.radius = Range;
            if (detectionRadiusCylinder) {
                var newSize = new Vector3(Range * 2, 200, Range * 2);
                detectionRadiusCylinder.transform.localScale = newSize;
            }

            _isMultiTarget = (aimPoints.Length > 1);
            _isMultiPS = (aimPoints[0].pS.Length > 1);
            
            // Start Coroutine for updating stats.
            StartCoroutine(UpdateStats());
        }

        protected virtual IEnumerator UpdateStats() { yield return null; }

        private void Update() { UpdateSystems(); }

        protected virtual void UpdateSystems() {
            nearbyTargets.RemoveAll(target => !target || !target.activeInHierarchy);

            if (!_isMultiPS) UpdateParticleSystem();
            if (turretAxes.Length > 0) CheckTurretRotations();
            if (_isMultiTarget) CheckAimPointsMultiTarget();
            else CheckAimPoints();
        }

        protected virtual void UpdateParticleSystem() {
            foreach (var aimPoint in aimPoints) {
                if (aimPoint.target) {
                    if (!aimPoint.pS[0].isPlaying) aimPoint.pS[0].Play();
                }
                else {
                    if (aimPoint.pS[0].isPlaying) aimPoint.pS[0].Stop();
                }
            }
        }

        protected virtual void UpdateMultiParticleSystem() {
            foreach (var aimPoint in aimPoints) {
                foreach (var pS in aimPoint.pS) pS.Play();
            }
        }

        protected virtual void CheckTurretRotations() {
            if (nearbyTargets.Count < 1) return;
            foreach (var turretAxis in turretAxes) {
                if (turretAxis.turretAxisType is not (TurretAxisTypes.Horizontal or TurretAxisTypes.Vertical)) continue;
                // Check if the turret axis piece is facing the target.
                var turretAxisToTargetDir = (nearbyTargets[0].transform.position - turretAxis.turretAxisPiece.transform.position).normalized;
                turretAxis.dot = Vector3.Dot(turretAxis.turretAxisPiece.transform.forward, turretAxisToTargetDir);
                if (turretAxis.dot < 1.0f) { RotateTurret(turretAxis, nearbyTargets[0]); }
            }
        }

        private void CheckAimPoints() {
            foreach (var aimPoint in aimPoints) {
                aimPoint.timeSinceLastFired += Time.deltaTime;
                if (nearbyTargets.Count < 1) return;
                
                // Check if the fire point is facing in the general direction of the target, AND if there has been enough time since the fire point last fired.
                var aimPointToTargetDir = (nearbyTargets[0].transform.position - aimPoint.aimPointPiece.transform.position).normalized;
                aimPoint.dot = Vector3.Dot(aimPoint.aimPointPiece.transform.forward, aimPointToTargetDir);
                if (aimPoint.dot > aimPoint.dotThreshold && aimPoint.timeSinceLastFired > RateOfFire) {
                    nearbyTargets[0].TryGetComponent(out aimPoint.target);
                    if (aimPoint.target) FireWeapon(aimPoint, aimPoint.target.gameObject);
                }
            }
        }
        
        private void CheckAimPointsMultiTarget() {
            foreach (var aimPoint in aimPoints) {
                if (aimPoint.target) {
                    aimPoint.dot = CalcDotProduct(aimPoint.aimPointPiece, aimPoint.target.gameObject);
                    if (aimPoint.dot <= aimPoint.dotThreshold) {
                        aimPoint.target = null;
                    }
                    else if (aimPoint.timeSinceLastFired > RateOfFire) { FireWeapon(aimPoint, aimPoint.target.gameObject); }
                }
                else {
                    UpdateParticleSystem();
                    foreach (var target in nearbyTargets) {
                        aimPoint.dot = CalcDotProduct(aimPoint.aimPointPiece, target.gameObject);
                        if (aimPoint.dot > aimPoint.dotThreshold) {
                            target.TryGetComponent(out aimPoint.target);
                            if (aimPoint.target) {
                                if (aimPoint.timeSinceLastFired > RateOfFire) {
                                    FireWeapon(aimPoint, aimPoint.target.gameObject);
                                }
                            }
                        }
                    }
                }
                aimPoint.timeSinceLastFired += Time.deltaTime;
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

        protected virtual void FireWeapon(AimPoint inAimPoint, GameObject inGO) {
            if (!inGO) {
                return;
            }
            inGO.TryGetComponent(out Enemy enemy);
            UpdateMultiParticleSystem();
            enemy.DecreaseHealth(2);
            inAimPoint.timeSinceLastFired = 0f;
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

            // Ensures the other collider is not marked as the target for any aim points.
            foreach (var aimPoint in aimPoints) {
                if (!aimPoint.target) continue;
                if (aimPoint.target.gameObject == other.gameObject) aimPoint.target = null;
            }
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            foreach (var aimPoint in aimPoints) {
                if (aimPoint.target) {
                    Gizmos.DrawLine(aimPoint.aimPointPiece.transform.position, aimPoint.target.transform.position);
                }
            }
        }
    }
}