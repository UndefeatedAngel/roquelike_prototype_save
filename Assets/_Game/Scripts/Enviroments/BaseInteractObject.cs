using BlockoutRoguelike.Core;
using UnityEngine;

namespace BlockoutRoguelike.Enviroments
{
    public abstract class BaseInteractObject: MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            var pc = other.GetComponent<OldPlayerController>();
            if (pc != null)
            {
                TriggerEnter();
                pc.Interact(Interact);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var pc = other.GetComponent<OldPlayerController>();
            if (pc != null)
            {
                ClearInteract();
                pc.ClearInteract();
            }
        }
        
        protected virtual void TriggerEnter()
        {
            
        }

        protected virtual void Interact()
        {
            
        }

        protected virtual void ClearInteract()
        {
            
        }
    }
}