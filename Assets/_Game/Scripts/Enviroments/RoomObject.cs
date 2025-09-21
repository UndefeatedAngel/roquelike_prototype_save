using System;
using BlockoutRoguelike.Enviroments;
using TMPro;
using UnityEngine;

namespace BlockoutRoguelike.Modules
{
    public class RoomObject : BaseInteractObject
    {
        [SerializeField] private TMP_Text[] _nameTexts;
        [SerializeField] private TMP_Text _questText;
        private RoomEnum roomEnum;

        private int _maxBottle;

        private Action<SceneType> _sceneLoadAction;

        protected override void TriggerEnter()
        {
            base.TriggerEnter();
            UpdateQuestText($"{Player.Bottle}/{_maxBottle} bottles");
        }

        protected override void Interact()
        {
            if (Player.Bottle < _maxBottle) return;
            
            Player.AddBottle(-_maxBottle);
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

        public void Setup(RoomEnum type, int maxBottle)
        {
            foreach (var nameText in _nameTexts)
            {
                nameText.text = type.ToString();
            }
            roomEnum = type;
            _maxBottle = maxBottle;
            if (Player.Bottle < maxBottle)
            {
                UpdateQuestText($"{Player.Bottle}/{_maxBottle} bottles");
            }
        }

        public void UpdateQuestText(string text)
        {
            _questText.text = text;
        }
    }
}
