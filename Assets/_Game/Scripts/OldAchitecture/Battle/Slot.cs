using NothingAwaits.Cards.Mobs;
using UnityEngine;

namespace NothingAwaits.Cards
{
    public class Slot: MonoBehaviour, IDragAndReturnSelector
    {
        [SerializeField] private Color _slotColor;
        [SerializeField] private ParticleSystem _circlePartile;
        [SerializeField] private ParticleSystem _portalDustPartile;
        [SerializeField] private ParticleSystem _glowParticle;
        
        public int Team;
        public int NumberSlot;
        
        private MobUI _activeMob;
        public MobUI ActiveMob
        {
            get => _activeMob;
            set
            {
                ActiveEmptyParticle(value == null);
                _activeMob = value;
            }
        }

        private void Start()
        {
            Highlight(false);
        }

        private void ActiveEmptyParticle(bool state)
        {
            if (state)
            {
                _portalDustPartile.Play();
                _glowParticle.Play();
            }
            else
            {
                _portalDustPartile.Stop();
                _glowParticle.Stop();
            }
        }

        public void Highlight(bool state)
        {
            _circlePartile.startColor = state ? Color.yellow : _slotColor;
            _portalDustPartile.startColor = state ? Color.yellow : _slotColor;
            var color = state ? Color.yellow : _slotColor;
            color.a = 0.17f;
            _glowParticle.startColor = color;
        }

        public void Highlight()
        {
            Highlight(true);
        }

        public void Clear()
        {
            Highlight(false);
        }
    }
}