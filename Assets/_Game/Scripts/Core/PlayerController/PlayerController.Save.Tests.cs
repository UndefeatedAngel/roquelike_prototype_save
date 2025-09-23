using UnityEngine;

namespace BlockoutRoguelike.Core.PlayerController
{
    public partial class PlayerController
    {
#if UNITY_EDITOR
        [ContextMenu("Save/Force Save")]
        private void ForceSave()
        {
            SaveData();
            Debug.Log("Player data saved!");
        }
        
        [ContextMenu("Save/Force Load")]
        private void ForceLoad()
        {
            _dataManager?.Load();
            Debug.Log("Player data loaded!");
        }
        
        [ContextMenu("Save/Delete Save Data")]
        private void DeleteSaveData()
        {
            _dataManager?.Delete();
            Debug.Log("Save data deleted!");
        }
#endif
    }
}