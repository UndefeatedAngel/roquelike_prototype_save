using System;
using DG.Tweening;
using UnityEngine;

namespace NothingAwaits.Cards
{
    public class AudioController: MonoBehaviour
    {
        [SerializeField] private AudioSource _winAudioSource;
        [SerializeField] private AudioSource _leaveAudioSource;
        [SerializeField] private AudioSource _returnAudioSource;

        private float _musicVolume = 0.2f;
        
        public static AudioController Instance{ get; private set; }
        
        private Tween _leaveFadeTween;
        private Tween _returnFadeTween;

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
            // Останавливаем все звуки перед воспроизведением победы
            StopAllSounds();
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
            // Останавливаем возврат звука и плавно включаем уход
            StopReturnSound();
            FadeInLeaveSound();
        }
        
        [ContextMenu("Test PlayReturnSound")]
        public void PlayReturnSound()
        {
            // Останавливаем звук ухода и плавно включаем возврат
            StopLeaveSound();
            FadeInReturnSound();
        }
        
        public void HideReturnSound()
        {
            StopReturnSound();
        }

        public void HideLeaveSound()
        {
            StopLeaveSound();
        }

        private void FadeInLeaveSound()
        {
            // Останавливаем предыдущую анимацию
            _leaveFadeTween?.Kill();
            
            // Начинаем с 0 и плавно увеличиваем громкость
            _leaveAudioSource.volume = 0f;
            _leaveAudioSource.Play();
            
            _leaveFadeTween = DOTween.To(() => _leaveAudioSource.volume, 
                x => _leaveAudioSource.volume = x, 
                _musicVolume, 
                0.5f);
        }

        private void FadeInReturnSound()
        {
            // Останавливаем предыдущую анимацию
            _returnFadeTween?.Kill();
            
            // Начинаем с 0 и плавно увеличиваем громкость
            _returnAudioSource.volume = 0f;
            _returnAudioSource.Play();
            
            _returnFadeTween = DOTween.To(() => _returnAudioSource.volume, 
                x => _returnAudioSource.volume = x, 
                _musicVolume, 
                0.5f);
        }

        private void StopLeaveSound()
        {
            _leaveFadeTween?.Kill();
            
            if (_leaveAudioSource.isPlaying)
            {
                // Плавно уменьшаем громкость и останавливаем
                _leaveFadeTween = DOTween.To(() => _leaveAudioSource.volume, 
                    x => _leaveAudioSource.volume = x, 
                    0f, 
                    0.3f)
                    .OnComplete(() => _leaveAudioSource.Stop());
            }
        }

        private void StopReturnSound()
        {
            _returnFadeTween?.Kill();
            
            if (_returnAudioSource.isPlaying)
            {
                // Плавно уменьшаем громкость и останавливаем
                _returnFadeTween = DOTween.To(() => _returnAudioSource.volume, 
                    x => _returnAudioSource.volume = x, 
                    0f, 
                    0.3f)
                    .OnComplete(() => _returnAudioSource.Stop());
            }
        }

        private void StopAllSounds()
        {
            StopLeaveSound();
            StopReturnSound();
            
            _winAudioSource.Stop();
        }

        private void FadeOut(AudioSource audioSource, float duration = 1f)
        {
            DOTween.To(() => audioSource.volume, 
                x => audioSource.volume = x, 
                0f, 
                duration)
                .OnComplete(() => audioSource.Stop());
        }
    }
}