using Modules.SceneLoader;
using UnityEngine;
using UnityEngine.UI;

namespace BlockoutRoguelike
{
    public class UIMain: MonoBehaviour
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _exitButton;

        private void Start()
        {
            _startButton.onClick.AddListener(() =>
            {
                SceneLoader.Instance.LoadSceneAsync(SceneType.GamePlay);
            });
            _exitButton.onClick.AddListener(Application.Quit);
        }
    }
}