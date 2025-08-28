using System;
using UnityEngine;

namespace BlockoutRoguelike.Cards
{
    public class InputController: MonoBehaviour
    {
        public static DraggableCard ActiveCard;

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
                    var slot = hit.transform.GetComponent<CreateMob>();
                    if (slot == null) return;
                
                    slot.MobCreate(ActiveCard.NumberCard);
                    Debug.Log(hit.transform.name);
                }
            }
        }
    }
}