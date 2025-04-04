using Towers;
using UnityEngine;
using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(TroopEncampment))]
    public class IsFiringVisualiser : UnityEditor.Editor
    {
        public void OnSceneGUI()
        {
            var troopTower = (TroopEncampment)target;
            
            foreach (var pS in troopTower.particleSystems)
            {
                Handles.color = Color.white;
                var labelText = $"IsPlaying: {pS.isPlaying.ToString()}";
                Handles.Label(pS.transform.position, labelText);
            }
            
            Handles.color = Color.yellow;
            Handles.Label(troopTower.transform.position + Vector3.up * 2, "THIS IS A TROOP TOWER!!!!!");
        }
    }
}