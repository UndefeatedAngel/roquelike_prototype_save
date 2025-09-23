using BlockoutRoguelike;
using BlockoutRoguelike.Core.Interactable;
using BlockoutRoguelike.Enviroments;
using UnityEngine;

namespace _Game.Scripts
{
    public class Bottle: BaseInteractObject
    {
        [SerializeField] private CollectableItem _collectableItem;
        
        protected override void Interact()
        {
            _collectableItem.TryCollectItem();
        }
    }
}