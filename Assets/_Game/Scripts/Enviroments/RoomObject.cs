using System;
using BlockoutRoguelike.Core.PlayerController;
using BlockoutRoguelike.Enviroments;
using BlockoutRoguelike.Inventory.Services;
using TMPro;
using UnityEngine;

namespace BlockoutRoguelike.Modules
{
    public class RoomObject : BaseInteractObject
    {
        [SerializeField] private string _itemIdLock = "1";
        [SerializeField] private TMP_Text[] _nameTexts;
        [SerializeField] private TMP_Text _questText;
        private RoomEnum roomEnum;

        private int _maxBottle;

        private Action<SceneType> _sceneLoadAction;
        
        private InventoryService _inventoryService;

        protected override void TriggerEnter()
        {
            base.TriggerEnter();
            UpdateQuestText($"{CountBottles()}/{_maxBottle} bottles");
        }

        protected override void Interact()
        {
            if (CountBottles() < _maxBottle) return;

            //TODO: SpendInventory
            BattleController.Type = roomEnum;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (roomEnum == RoomEnum.Enemy || roomEnum == RoomEnum.EliteEnemy)
            {
                SceneLoader.Instance.LoadSceneAsync(SceneType.Cards, () =>
                {
                    
                });
            }
            else if (roomEnum == RoomEnum.Chest)
            {
                SceneLoader.Instance.LoadSceneAsync(SceneType.Chest, () =>
                {
                    
                });
            }
            else
            {
                SceneLoader.Instance.LoadSceneAsync(SceneType.Battle, () =>
                {
                    
                });
            }
        }

        public void Setup(RoomEnum type, int maxBottle, InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
            
            foreach (var nameText in _nameTexts)
            {
                nameText.text = type.ToString();
            }
            roomEnum = type;
            _maxBottle = maxBottle;
            
            UpdateQuestText($"{CountBottles()}/{_maxBottle} bottles");
        }

        private int CountBottles()
        {
            if (_inventoryService!=null && _inventoryService.HasItem(_itemIdLock))
            {
                return _inventoryService.GetTotalItemCount(_itemIdLock);
            }
            return 0;
        }

        public void UpdateQuestText(string text)
        {
            _questText.text = text;
        }
    }
}
