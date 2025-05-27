// using System;
// using System.Collections;
// using System.Collections.Generic;
// using Unity.VisualScripting;
// using UnityEngine;
//
// namespace Towers
// {
//     public class ProximityTowerMultiTarget : ProximityTower
//     {
//         public List<GameObject> availableTargets = new List<GameObject>();
//         private float _timer;
//         
//         private void OnTriggerStay(Collider other)
//         {
//             if (!availableTargets.Contains(other.gameObject))
//             {
//                 foreach (var validTargetType in validTargets)
//                 {
//                     if (other.gameObject.CompareTag(validTargetType))
//                     {
//                         availableTargets.Add(other.gameObject);
//                     }
//                 }
//             }
//         }
//
//         private void OnTriggerExit(Collider other)
//         {
//             if (availableTargets.Contains(other.gameObject))
//             {
//                 availableTargets.Remove(other.gameObject);
//             }
//         }
//     }
// }