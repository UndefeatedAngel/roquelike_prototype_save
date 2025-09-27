using System;
using DG.Tweening;
using UnityEngine;

namespace NothingAwaits.Cards
{
    public class AudioController: MonoBehaviour
    {
        [SerializeField] private AudioSource _winAudioSource;
        [SerializeField] private AudioSource _leaveAudioSource;

        private float _musicVolume = 0.2f;
        
        public static AudioController Instance{ get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        [ContextMenu("Test PlayWinSound")]
        public void PlayWinSound()
        {
            _winAudioSource.Stop();
            _winAudioSource.volume = _musicVolume;
            _winAudioSource.Play();
        }
        
        public void HideWinSound()
        {
            FadeOut(_winAudioSource);
        }
        
        [ContextMenu("Test PlayLeaveSound")]
        public void PlayLeaveSound()
        {
            _leaveAudioSource.Stop();
            _leaveAudioSource.volume = 0f;
            _leaveAudioSource.Play();
            FadeIn(_leaveAudioSource);
        }

        public void HideLeaveSound()
        {
            FadeOut(_leaveAudioSource);
        }

        private void FadeIn(AudioSource audioSource, float duration = 1f, Action onComplete = null)
        {
            DOTween.To(() => audioSource.volume, x => audioSource.volume = x, _musicVolume, duration).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }

        private void FadeOut(AudioSource audioSource, float duration = 1f)
        {
            DOTween.To(() => audioSource.volume, x => audioSource.volume = x, _musicVolume, duration)
                .OnComplete(() => audioSource.Stop());
        }
    }
}