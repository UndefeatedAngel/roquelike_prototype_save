using BlockoutRoguelike;
using UnityEngine;

namespace _Game.Scripts
{
    public class Bottle: MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            Player.AddBottle(1);
            Destroy(gameObject);
        }
    }
}