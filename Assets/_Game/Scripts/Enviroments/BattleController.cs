using System.Collections;
using BlockoutRoguelike.Core.PlayerController;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace BlockoutRoguelike
{
    public class BattleController: MonoBehaviour
    {
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private Sprite[] _iconBoss;
        public static RoomEnum Type;

        private int _enemyStep;

        private int _hp;
        private int _maxHp;

        public int HP
        {
            get { return _hp; }
            set
            {
                _hp = value;
                _imageHPEnemy.fillAmount = (float)_hp/_maxHp;
            }
        }
        private int _attack;
        
        private int _enemyHp = 10;
        private int _eliteEnemyHp = 15;
        private int _bossHp = 21;
        private int _enemyAttack = 1;
        private int _eliteEnemyAttack = 2;
        private int _bossAttack = 3;

        [Header("UI")] 
        [SerializeField] private Button _buttonAttack;
        [SerializeField] private Image _enemyIconImage;
        [SerializeField] private Image _imageHPEnemy;
        [SerializeField] private GameObject _enemyUI;
        
        [SerializeField] private GameObject _narrativeUI;
        [SerializeField] private TMP_Text _narrativeText;
        [SerializeField] private Button _narrativeButton;


        private void Start()
        {
            _buttonAttack.onClick.AddListener(ButtonAttackClick);
            _buttonAttack.gameObject.SetActive(false);
            SetupFight();
        }

        private void SetupFight()
        {
            switch (Type)
            {
                case RoomEnum.Unknown:
                    UnknownStart();
                    break;
                case RoomEnum.Enemy:
                    EnemyStart();
                    break;
                case RoomEnum.EliteEnemy:
                    EliteEnemyStart();
                    break;
                case RoomEnum.Chest:
                    _narrativeText.text = "В сундуке ты получаешь сокровища!";
                    _playerController.AddCoins(50);
                    ChestStart();
                    break;
                case RoomEnum.Trade:
                    _narrativeText.text = "У торговца ты покупаешь что-то полезное!";
                    if (_playerController.TrySpendCoins(50))
                    {
                        
                    }
                    TradeStart();
                    break;
                case RoomEnum.Boss:
                    BossStart();
                    break;
            }
        }

        private void EndGame()
        {
            SceneLoader.Instance.LoadSceneAsync(SceneType.GamePlay, () =>
            {
                
            });
        }

        private void UnknownStart()
        {
            //Narrative
            var random = Random.Range(0, 2);
            if (random == 0)
            {
                EnemyStart();
            }
            else
            {
                _narrativeText.text = "В сундуке ты получаешь сокровища!";
                _playerController.AddCoins(50);
                ChestStart();
            }
        }
        private void EnemyStart()
        {
            _enemyUI.gameObject.SetActive(true);
            _enemyIconImage.sprite = _iconBoss[0];
            HP = _maxHp = _enemyHp;
            _attack = _enemyAttack;
            StartEnemyGame();
        }
        private void EliteEnemyStart()
        {
            _enemyUI.gameObject.SetActive(true);
            _enemyIconImage.sprite = _iconBoss[1];
            HP = _maxHp = _eliteEnemyHp;
            _attack = _enemyAttack;
            StartEnemyGame();
        }
        
        private void ChestStart()
        {
            _narrativeUI.gameObject.SetActive(true);
            _narrativeButton.onClick.AddListener(EndGame);
        }
        private void TradeStart()
        {
            //Narrative
            ChestStart();
        }
        
        private void BossStart()
        {
            _enemyUI.gameObject.SetActive(true);
            _enemyIconImage.sprite = _iconBoss[2];
            HP = _maxHp = _bossHp;
            _attack = _bossAttack;
            StartEnemyGame();
        }

        private void StartEnemyGame()
        {
            _enemyUI.gameObject.SetActive(true);
            _buttonAttack.gameObject.SetActive(false);
            _playerController.TakeDamage(_attack);
            if(_playerController.IsDead())
            {
                _gameOverPanel.gameObject.SetActive(true);
                EndGame();
            }
            else
            {
                StartCoroutine(WaitStartAttack());
            }
        }

        private IEnumerator WaitStartAttack()
        {
            yield return new WaitForSeconds(0.5f);
            _buttonAttack.gameObject.SetActive(true);
        }
        
        private void ButtonAttackClick()
        {
            HP -= 3;
            if (HP <= 0)
            {
                HP = 0;
                EndGame();
            }
            else
            {
                StartEnemyGame();
            }
        }
        
    }
}