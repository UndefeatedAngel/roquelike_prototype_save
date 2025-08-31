using BlockoutRoguelike;
using UnityEngine;

namespace _Game.Scripts
{
    public class Rest: MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            
            Player.AddHP(Player.MaxHp-Player.HP);
            Destroy(gameObject);
        }
    }
}