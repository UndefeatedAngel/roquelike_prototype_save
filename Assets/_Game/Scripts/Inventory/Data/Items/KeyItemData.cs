using UnityEngine;

namespace BlockoutRoguelike.Core.Inventory.Data.Items
{
    [CreateAssetMenu(fileName = "New Key", menuName = "Roguelike/Items/Key")]
    public class KeyItemData : BaseItemData
    {
        [Header("Key Settings")]
        [SerializeField] private string _doorId = "door_01";
        [SerializeField] private bool _isMasterKey = false;

        public string DoorId => _doorId;
        public bool IsMasterKey => _isMasterKey;

        public override string GetDescription()
        {
            string keyType = _isMasterKey ? "Master Key" : "Key";
            return base.GetDescription() + $"\n{keyType} for door: {_doorId}";
        }
    }
}