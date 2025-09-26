using BlockoutRoguelike.Enviroments;
using UnityEngine;

namespace _Game.Scripts.Modules
{
    public class ActiveObjectTrigger: BaseInteractObject
    {
        [SerializeField] private GameObject[] _objects;

        private void Start()
        {
            foreach (var obj in _objects)
            {
                obj.SetActive(false);
            }
        }

        protected override void TriggerEnter()
        {
            foreach (var obj in _objects)
            {
                obj.SetActive(true);
            }
        }

        protected override void ClearInteract()
        {
            base.ClearInteract();
            foreach (var obj in _objects)
            {
                obj.SetActive(false);
            }
        }
    }
}