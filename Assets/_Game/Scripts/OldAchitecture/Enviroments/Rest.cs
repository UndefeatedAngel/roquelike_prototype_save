using NothingAwaits.Enviroments;
using UnityEngine;

namespace _Game.Scripts
{
    public class Rest: BaseInteractObject
    {
        protected override void Interact()
        {
            Destroy(gameObject);
        }
    }
}