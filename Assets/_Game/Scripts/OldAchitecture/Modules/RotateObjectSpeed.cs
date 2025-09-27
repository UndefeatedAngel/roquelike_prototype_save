using UnityEngine;

namespace NothingAwaits.Modules
{
    public class RotateObjectSpeed: MonoBehaviour
    {
        [SerializeField] private Transform _objectToRotate;
        [SerializeField] private float _speed;

        private void Update()
        {
            _objectToRotate.Rotate(Vector3.up, _speed * Time.deltaTime);
        }
    }
}