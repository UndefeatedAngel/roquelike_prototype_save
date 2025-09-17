using BlockoutRoguelike.Enviroments;
using BlockoutRoguelike.Static;
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
                SceneLoadManager.LoadScene(SceneName.Cards);
            }
            else
            {
                SceneLoadManager.LoadScene(SceneName.Battle);
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
