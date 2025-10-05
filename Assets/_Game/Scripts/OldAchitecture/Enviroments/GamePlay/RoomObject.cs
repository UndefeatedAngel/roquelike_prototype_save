using System;
using System.Collections.Generic;
using Modules.SceneLoader;
using NothingAwaits.Core.Inventory.InventorySystem;
using NothingAwaits.Enviroments;
using TMPro;
using UnityEngine;

namespace NothingAwaits.Modules
{
    public class RoomObject : BaseInteractObject
    {
        public List<string> requiredKeyIds; // Должен совпадать с keyId ключа
        public string doorId; // Должен совпадать с doorId ключа
        public List<int> requiredKeyCounts; // Количество каждого ключа
        
        [SerializeField] private TMP_Text[] _nameTexts;
        [SerializeField] private TMP_Text _questText;
        private RoomEnum roomEnum;

        private int _maxBottle;
        private Dictionary<string, int> foundKeyCounts = new();
        
        public Action<SceneType> onSceneLoadAction;
        protected override void TriggerEnter()
        {
            base.TriggerEnter();
            CheckPlayerKeys();
            UpdateQuestText($"{GetKeyProgressText()} bottles");
        }
        
        private string GetKeyProgressText()
        {
            if (requiredKeyIds == null || requiredKeyCounts == null || requiredKeyIds.Count != requiredKeyCounts.Count)
            {
                return "0/0";
            }
            
            var foundCounts = 0;
            var requiredCounts = 0;
            for (var i = 0; i < requiredKeyIds.Count; i++)
            {
                var keyId = requiredKeyIds[i];
                requiredCounts+= requiredKeyCounts[i];
                foundCounts+= foundKeyCounts.ContainsKey(keyId) ? foundKeyCounts[keyId] : 0;
            }
            return $"{foundCounts}/{requiredCounts}";
        }
        
        private bool IsReadyToOpen()
        {
            var foundCounts = 0;
            var requiredCounts = 0;
            for (var i = 0; i < requiredKeyIds.Count; i++)
            {
                var keyId = requiredKeyIds[i];
                requiredCounts+= requiredKeyCounts[i];
                foundCounts+= foundKeyCounts.ContainsKey(keyId) ? foundKeyCounts[keyId] : 0;
            }
            return foundCounts == requiredCounts;
        }
        
        private void CheckPlayerKeys()
        {
            // Сбрасываем счётчики
            foreach (var keyId in requiredKeyIds)
            {
                foundKeyCounts[keyId] = 0;
            }
        
            // Проверяем все ключи в инвентаре
            var allKeys = InventorySystemController.Instance.FindAllKeysForDoor(doorId);
        
            foreach (var key in allKeys)
            {
                if (requiredKeyIds.Contains(key.KeyId))
                {
                    foundKeyCounts[key.KeyId] += key.CurrentStack;
                }
            }
        }

        protected override void Interact()
        {
            CheckPlayerKeys();
            if (!IsReadyToOpen()) return;
            
            BattleController.Type = roomEnum;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            switch (roomEnum)
            {
                case RoomEnum.Enemy:
                case RoomEnum.EliteEnemy:
                    onSceneLoadAction?.Invoke(SceneType.CardsBattle);
                    //SceneLoader.Instance.LoadSceneAsync(SceneType.CardsBattle);
                    break;
                case RoomEnum.Chest:
                    onSceneLoadAction?.Invoke(SceneType.Chest);
                    //SceneLoader.Instance.LoadSceneAsync(SceneType.Chest);
                    break;
                case RoomEnum.Upgradable:
                    onSceneLoadAction?.Invoke(SceneType.Upgradable);
                    //SceneLoader.Instance.LoadSceneAsync(SceneType.Upgradable);
                    break;
                case RoomEnum.Trade:
                    onSceneLoadAction?.Invoke(SceneType.Trade);
                    //SceneLoader.Instance.LoadSceneAsync(SceneType.Trade);
                    break;
                case RoomEnum.Unknown:
                    onSceneLoadAction?.Invoke(SceneType.Unknown);
                    //SceneLoader.Instance.LoadSceneAsync(SceneType.Unknown);
                    break;
                case RoomEnum.Rest:
                    onSceneLoadAction?.Invoke(SceneType.Rest);
                    //SceneLoader.Instance.LoadSceneAsync(SceneType.Rest);
                    break;
                default:
                    onSceneLoadAction?.Invoke(SceneType.Battle);
                    //SceneLoader.Instance.LoadSceneAsync(SceneType.Battle);
                    break;
            }
        }

        public void Setup(RoomEnum type, int maxBottle)
        {
            foreach (var nameText in _nameTexts)
            {
                nameText.text = type.ToString();
            }
            roomEnum = type;
            _maxBottle = maxBottle;
            
            // Инициализируем словарь найденных ключей
            foreach (var keyId in requiredKeyIds)
            {
                foundKeyCounts[keyId] = 0;
            }
        
            // Если количества не заданы, устанавливаем по умолчанию 1
            if (requiredKeyCounts == null || requiredKeyCounts.Count != requiredKeyIds.Count)
            {
                requiredKeyCounts = new List<int>();
                foreach (var keyId in requiredKeyIds)
                {
                    requiredKeyCounts.Add(_maxBottle);
                }
            }
            
            UpdateQuestText($"{GetKeyProgressText()} bottles");
        }

        public void UpdateQuestText(string text)
        {
            _questText.text = text;
        }
    }
}
