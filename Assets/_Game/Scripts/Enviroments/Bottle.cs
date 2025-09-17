using BlockoutRoguelike;
using BlockoutRoguelike.Enviroments;

namespace _Game.Scripts
{
    public class Bottle: BaseInteractObject
    {
        protected override void Interact()
        {
            Player.AddBottle(1);
            Destroy(gameObject);
        }
    }
}