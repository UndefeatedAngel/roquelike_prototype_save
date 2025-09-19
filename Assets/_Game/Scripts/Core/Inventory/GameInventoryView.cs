using System;
using UnityEngine;

namespace BlockoutRoguelike.Core.Inventory
{
    [Serializable]
    public class GameItemView
    {
        public Transform ItemParent;
    }
    
    public class GameInventoryView: MonoBehaviour
    {
        [SerializeField] private GameItemView[] _openItems;
        [SerializeField] private GameItemView[] _applyItems;

        public void AddItem()
        {
            
        }
    }
}