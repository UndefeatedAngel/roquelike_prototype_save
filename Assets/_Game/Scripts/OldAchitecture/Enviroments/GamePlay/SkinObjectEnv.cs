using UnityEngine;

namespace NothingAwaits
{
    [CreateAssetMenu(fileName = "SkinObject", menuName = "BlockoutRoguelike/SkinObject")]
    public class SkinObjectEnv: ScriptableObject
    {
        private int _id;
        public int Id => _id;
        public EnviromentType EnviromentType;
        public Color DirectionalLightColors;
        public Vector3 DirectionalLightDirection;
        public Material FloorMaterials;
        public Color RoadColor;
        public Color TreeColor;
        public Material SkyboxMaterial;
        public GameObject[] ParticleSystem;

        public void SetID(int id)
        {
            _id = id;
        }
    }
}