using System;
using NothingAwaits.Battle.Cards;
using NothingAwaits.Cards;
using UnityEngine;

namespace NothingAwaits.Battle.Controllers
{
    public class InputController: MonoBehaviour
    {
        public static CardUI ActiveCard;
        public Action<CardUI, Slot> OnCardToSlot;

        private Slot _activeSlot;

        private void Update()
        {
            if (Input.GetMouseButtonUp(0) && _activeSlot != null)
            {
                OnCardToSlot?.Invoke(ActiveCard, _activeSlot);
            }
            if (_activeSlot != null)
            {
                _activeSlot.Highlight(false);
                _activeSlot = null;
            }
            if (ActiveCard == null) return;
            if (Camera.main == null) return;
            
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                var slot = hit.transform.GetComponent<Slot>();
                if (slot == null) return;
                _activeSlot = slot;
                slot.Highlight(true);
            }
        }
    }
}