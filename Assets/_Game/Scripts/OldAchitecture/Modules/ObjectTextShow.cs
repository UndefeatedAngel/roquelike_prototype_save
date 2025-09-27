using NothingAwaits.Enviroments;
using TMPro;
using UnityEngine;

namespace NothingAwaits.Modules
{
    public class ObjectTextShow: BaseInteractObject
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

        protected override void TriggerEnter()
        {
            base.TriggerEnter();
            _textObject.SetActive(true);
        }

        protected override void ClearInteract()
        {
            base.ClearInteract();
            _textObject.SetActive(false);
        }
    }
}