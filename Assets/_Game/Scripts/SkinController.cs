using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Game.Scripts
{
    [Serializable]
    public struct SkinObject
    {
        public Color DirectionalLightColors;
        public Vector3 DirectionalLightDirection;
        public Material FloorMaterials;
        public Color RoadColor;
        public Color TreeColor;
    }
    
    public class SkinController: MonoBehaviour
    {
        [SerializeField] private int _testIndex = -1;
        [Header("Objects")]
        [SerializeField] private Light _directionalLight;
        [SerializeField] private MeshRenderer _floorRenderer;
        [SerializeField] private Material _roadMaterial;
        [SerializeField] private Material[] _treeMaterials;
        
        [Header("GDSetup")]
        [SerializeField] private SkinObject[] _skinObjects;

        private void Start()
        {
            var random = Random.Range(0, _skinObjects.Length);
            if (_testIndex >= 0)
            {
                random = _testIndex;
            }

            var skin = _skinObjects[random];
            _directionalLight.color = skin.DirectionalLightColors;
            _directionalLight.transform.eulerAngles = skin.DirectionalLightDirection;
            _floorRenderer.material = skin.FloorMaterials;
            _roadMaterial.color = skin.RoadColor;
            foreach (var treeMaterial in _treeMaterials)
            {
                treeMaterial.color = skin.TreeColor;
            }
        }
    }
}