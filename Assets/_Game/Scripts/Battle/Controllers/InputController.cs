using System;
using BlockoutRoguelike.Battle.Cards;
using BlockoutRoguelike.Cards;
using UnityEngine;

namespace BlockoutRoguelike.Battle.Controllers
{
    public class InputController: MonoBehaviour
    {
        public static CardUI ActiveCard;
        public Action<CardUI, Slot> OnCardToSlot;

        private void Update()
        {
                    
            if (Input.GetMouseButtonUp(0))
            {
                if (ActiveCard == null) return;
                
                if (Camera.main == null) return;
            
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    var slot = hit.transform.GetComponent<Slot>();
                    if (slot == null) return;
                
                    OnCardToSlot?.Invoke(ActiveCard, slot);
                }
            }
        }
    }
}