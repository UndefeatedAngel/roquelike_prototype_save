using System;
using BlockoutRoguelike.Core.Inventory;
using TMPro;
using UnityEngine;

namespace BlockoutRoguelike.Core
{
    public class PlayerController: MonoBehaviour
    {
        [SerializeField] private TMP_Text _interactText;
        private Action _interact;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                _interact?.Invoke();
                _interactText.text = "";
            }
        }

        public void Interact(Action endAction)
        {
            _interactText.text = "F";
            _interact = endAction;
        }

        public void ClearInteract()
        {
            _interactText.text = "";
            _interact = null;
        }
    }
}