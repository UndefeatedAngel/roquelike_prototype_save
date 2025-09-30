using NothingAwaits.Core.Cards.Data;
using UnityEngine;

namespace _Game.Scripts.OldAchitecture.Modules
{
    public class ModelRotatorActivate: MonoBehaviour
    {
        [SerializeField] private GameObject[] _gameObjects;

        public void Activate(CardType type)
        {
            for (var i = 0; i < _gameObjects.Length; i++)
            {
                _gameObjects[i].SetActive((int) type == i);
            }
        }
    }
}