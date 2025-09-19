using System;
using BlockoutRoguelike.Enviroments;
using DG.Tweening;
using UnityEngine;

namespace _Game.Scripts.Modules
{
    [Serializable]
    public class LightObject
    {
        [Header("Setup")]
        public Light Light;

        [Header("ColorAnimation")] 
        public bool IsColorAnimation;
        public Color TargetColor;
        public float TimeAnimationColor;
        [Header("IntensityAnimation")]
        public bool IsIntensityAnimation;
        public float TargetIntensity;
        public float TimeAnimationIntensity;

        public Color StartColor { get; set; }
        public float StartIntensity { get; set; }

        public Tween ColorAnimTween { get; set; }
        public Tween IntensityAnimTween { get; set; }
    }
    
    public class LightTrigger: BaseInteractObject
    {
        [SerializeField] private LightObject[] _lights;

        private void Start()
        {
            foreach (var lightObject in _lights)
            {
                lightObject.StartColor = lightObject.Light.color;
                lightObject.StartIntensity = lightObject.Light.intensity;
            }
        }

        protected override void TriggerEnter()
        {
            foreach (var lightObject in _lights)
            {
                if (lightObject.IsColorAnimation)
                {
                    lightObject.ColorAnimTween = lightObject.Light.DOColor(lightObject.TargetColor, lightObject.TimeAnimationColor);
                }

                if (lightObject.IsIntensityAnimation)
                {
                    lightObject.IntensityAnimTween = lightObject.Light.DOIntensity(lightObject.TargetIntensity, lightObject.TimeAnimationIntensity);
                }
            }
        }

        protected override void ClearInteract()
        {
            base.ClearInteract();
            foreach (var lightObject in _lights)
            {
                if (lightObject.IsColorAnimation)
                {
                    lightObject.ColorAnimTween?.Kill();
                    lightObject.ColorAnimTween = lightObject.Light.DOColor(lightObject.StartColor, lightObject.TimeAnimationColor);
                }

                if (lightObject.IsIntensityAnimation)
                {
                    lightObject.IntensityAnimTween?.Kill();
                    lightObject.IntensityAnimTween = lightObject.Light.DOIntensity(lightObject.StartIntensity, lightObject.TimeAnimationIntensity);
                }
            }
        }
    }
}