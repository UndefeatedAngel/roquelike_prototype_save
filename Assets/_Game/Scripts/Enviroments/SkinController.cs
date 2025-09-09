using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Game.Scripts
{
    public enum EnviromentType
    {
        Empty,
        First,
        Random,
        Boss
    }
    
    [Serializable]
    public class SkinObject
    {
        public EnviromentType EnviromentType;
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

        private static SkinObject _lastRandomSkin;

        public void Setup(EnviromentType type)
        {
            Debug.Log($"SkinType: {type.ToString()}");
            var skins = _skinObjects.Where(x => x.EnviromentType == type).ToList();
            if (type == EnviromentType.Random)
            {
                if (_lastRandomSkin !=null)
                {
                    for (var i = 0; i < skins.Count; i++)
                    {
                        var skinFind = skins[i];
                        if (!skinFind.Equals(_lastRandomSkin)) continue;
                        
                        skins.RemoveAt(i);
                        break;
                    }
                }
            }

            var random = Random.Range(0, skins.Count);
            var skin = skins[random];
            if (type == EnviromentType.Random)
            {
                _lastRandomSkin = skin;
            }
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