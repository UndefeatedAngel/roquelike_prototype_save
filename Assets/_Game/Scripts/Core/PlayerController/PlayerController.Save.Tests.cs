using UnityEngine;
using BlockoutRoguelike.Adapters;

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
            _dataManager.LoadData();
            Debug.Log("Player data loaded!");
        }
        
        [ContextMenu("Save/Delete Save Data")]
        private void DeleteSaveData()
        {
            var dataService = new CoreDataServiceAdapter();
            dataService.DeletePlayerData();
            Debug.Log("Save data deleted!");
        }
#endif
    }
}