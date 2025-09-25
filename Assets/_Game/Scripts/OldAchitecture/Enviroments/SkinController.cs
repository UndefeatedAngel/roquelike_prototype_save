using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BlockoutRoguelike
{
    public enum EnviromentType
    {
        Empty,
        First,
        Random,
        Boss
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
        [SerializeField] private SkinObjectEnv[] _skinObjects;

        private static int _lastRandomSkin = -1;

        public void Setup(EnviromentType type)
        {
            for (int i = 0; i < _skinObjects.Length; i++)
            {
                _skinObjects[i].SetID(i);
            }
            
            Debug.Log($"SkinType: {type.ToString()}");
            var skins = _skinObjects.Where(x => x.EnviromentType == type).ToList();
            if (type == EnviromentType.Random)
            {
                if (_lastRandomSkin>0)
                {
                    for (var i = 0; i < skins.Count; i++)
                    {
                        var skinFind = skins[i];
                        if (skinFind.Id!=_lastRandomSkin) continue;
                        
                        skins.RemoveAt(i);
                        break;
                    }
                }
            }

            var random = Random.Range(0, skins.Count);
            var skin = skins[random];
            if (type == EnviromentType.Random)
            {
                _lastRandomSkin = skin.Id;
            }
            _directionalLight.color = skin.DirectionalLightColors;
            _directionalLight.transform.eulerAngles = skin.DirectionalLightDirection;
            _floorRenderer.material = skin.FloorMaterials;
            _roadMaterial.color = skin.RoadColor;
            if (skin.ParticleSystem.Length>0)
            {
                var isParticleRandom = Random.Range(0, 3);
                if (isParticleRandom > 0)
                {
                    var rndP = Random.Range(0, skin.ParticleSystem.Length);
                    var skinPrefab = skin.ParticleSystem[rndP];
                    Instantiate(skinPrefab, null, true);
                }
            }

            if (skin.SkyboxMaterial != null)
            {
                RenderSettings.skybox = skin.SkyboxMaterial;
            }
            foreach (var treeMaterial in _treeMaterials)
            {
                treeMaterial.color = skin.TreeColor;
            }
        }
    }
}