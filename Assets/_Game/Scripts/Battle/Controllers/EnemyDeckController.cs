using System.Collections.Generic;
using UnityEngine;

namespace BlockoutRoguelike.Battle.Controllers
{
    public class EnemyDeckController: MonoBehaviour
    {
        [Header("Setup")] 
        [SerializeField] private List<CardData> _deck;
        [Header("Deck")]
        [Space]
        [Header("Prefabs")]
        [SerializeField] private List<PrefabData> _prefabsDataBase;
    }
}