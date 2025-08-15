using TMPro;
using UnityEngine;

namespace BlockoutRoguelike.Modules
{
    public class ObjectTextShow: MonoBehaviour
    {
        [SerializeField] private string _textToShow;
        [SerializeField] private TMP_Text[] _texts;
        [SerializeField] private GameObject _textObject;

        private void Start()
        {
            _textObject.SetActive(false);
            foreach (var text in _texts)
            {
                text.text = _textToShow;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _textObject.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _textObject.SetActive(false);
            }
        }
    }
}