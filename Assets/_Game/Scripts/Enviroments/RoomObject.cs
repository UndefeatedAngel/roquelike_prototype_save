using BlockoutRoguelike.Static;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BlockoutRoguelike.Modules
{
    public class RoomObject : MonoBehaviour
    {
        [SerializeField] private TMP_Text[] _nameTexts;
        [SerializeField] private TMP_Text _questText;
        private RoomEnum roomEnum;

        private int _maxBottle;

        public void OnTriggerEnter(Collider other)
        {
            Debug.Log("OnTriggerEnter:" + other.name);
            if(!other.CompareTag("Player")) return;
            
            if (Player.Bottle >= _maxBottle)
            {
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
            else
            {
                UpdateQuestText($"{Player.Bottle}/{_maxBottle} bottles");
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
