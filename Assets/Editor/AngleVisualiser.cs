using Towers;
using UnityEngine;
using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(TroopEncampment))]
    public class AngleVisualiser : UnityEditor.Editor
    {
        // Custom Scene GUI for AngleVisualizer
        void OnSceneGUI()
        {
            var troopTower = (TroopEncampment)target;
            var transform = troopTower.transform;

            if (troopTower.showFireAngles)
            {
                foreach (var firePoint in troopTower.firePoints)
                {
                    var firePointTransform = firePoint.transform;
                    var distanceBetweenCentreAndFirePoint = firePointTransform.position - transform.position;
                    var trueTotalRadius = troopTower.capsuleCollider.radius - distanceBetweenCentreAndFirePoint.magnitude;

                    // Draw the range circle around the object (in scene view)
                    Handles.color = Color.blue;
                    Handles.DrawWireDisc(firePointTransform.position, Vector3.up, trueTotalRadius);

                    // Draw the angle arc
                    Handles.color = new Color(0, 1, 0, 0.2f);

                    // Starting angle direction (transform's forward direction)
                    Vector3 forward = firePointTransform.forward;

                    // Draw the arc representing the max angle
                    Handles.DrawSolidArc(firePointTransform.position, Vector3.up, Quaternion.Euler(0, -troopTower.maxAngle, 0) * forward, troopTower.maxAngle * 2, trueTotalRadius);

                    // Optionally, draw lines to represent the boundaries of the angle
                    Vector3 leftBoundary = Quaternion.Euler(0, -troopTower.maxAngle, 0) * forward * trueTotalRadius;
                    Vector3 rightBoundary = Quaternion.Euler(0, troopTower.maxAngle, 0) * forward * trueTotalRadius;

                    Handles.color = Color.red;
                    Handles.DrawLine(firePointTransform.position, firePointTransform.position + leftBoundary);
                    Handles.DrawLine(firePointTransform.position, firePointTransform.position + rightBoundary);

                    // Draw labels for angle and range
                    Handles.Label(firePointTransform.position + forward * trueTotalRadius, $"Range: {trueTotalRadius}", new GUIStyle() { fontSize = 15, normal = new GUIStyleState() { textColor = Color.white } });
                    Handles.Label(firePointTransform.position + leftBoundary / 2, $"Max Angle: {troopTower.maxAngle}°", new GUIStyle() { fontSize = 15, normal = new GUIStyleState() { textColor = Color.white } });
                }
            }

            if (troopTower.showParticleSystems)
            {
                foreach (var pS in troopTower.particleSystems)
                {
                    Handles.color = Color.white;
                    var labelText = $"IsPlaying: {pS.isPlaying.ToString()}. ParticlesCount: {pS.particleCount}";
                    Handles.Label(pS.transform.position, labelText);
                }
            }
            
            // Force the scene view to repaint
            SceneView.RepaintAll();
        }
    }
}