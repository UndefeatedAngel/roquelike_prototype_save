using BlockoutRoguelike;
using BlockoutRoguelike.Enviroments;

namespace _Game.Scripts
{
    public class Rest: BaseInteractObject
    {
        protected override void Interact()
        {
            Player.AddHP(Player.MaxHp-Player.HP);
            Destroy(gameObject);
        }
    }
}