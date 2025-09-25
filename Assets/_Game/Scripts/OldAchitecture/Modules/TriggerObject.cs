using System;
using UnityEngine;

namespace _Game.Scripts.Modules
{
    public class TriggerObject: MonoBehaviour
    {
        public Action OnEnter;
        public Action OnExit;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OnEnter?.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OnExit?.Invoke();
            }
        }
    }
}