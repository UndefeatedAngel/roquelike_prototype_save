using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using UnityEngine.Serialization;

namespace Modules.SceneLoader
{
    public class SceneLoader : MonoBehaviour
    {
        [Header("Настройки загрузки")] 
        [SerializeField] private float _minLoadTime = 2.0f;
        [SerializeField] private bool _showDebugLogs = true;

        [Header("Эффекты перехода")]
        [SerializeField] private UILoader _uiLoader;

        private bool _isLoading = false;

        public static SceneLoader Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                if (transform.parent != null)
                {
                    transform.SetParent(null);
                }

                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            _uiLoader = Instantiate(_uiLoader, transform);
        }

        public void LoadSceneAsync(SceneType sceneType, Action onComplete = null)
        {
            if (_isLoading)
            {
                Debug.LogWarning("Загрузка уже в процессе!");
                return;
            }

            StartCoroutine(LoadSceneAsyncCoroutine(sceneType, onComplete));
        }

        public void LoadScene(SceneType sceneType)
        {
            SceneManager.LoadScene(sceneType.ToString());
        }

        private IEnumerator LoadSceneAsyncCoroutine(SceneType sceneType, Action onComplete)
        {
            _isLoading = true;

            if (_showDebugLogs) Debug.Log($"Начинаем загрузку сцены: {sceneType.ToString()}");

            yield return StartCoroutine(_uiLoader.ShowLoadingScreen());

            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneType.ToString(), LoadSceneMode.Single);
            loadOperation.allowSceneActivation = false;

            var loadStartTime = Time.time;

            _uiLoader.ClearProgress();
            yield return new WaitForEndOfFrame();
            while (loadOperation.progress < 0.9f || Time.time - loadStartTime < _minLoadTime)
            {
                yield return StartCoroutine(_uiLoader.UpdateSlider(loadOperation.progress));

                if (_showDebugLogs) Debug.Log($"Прогресс загрузки: {loadOperation.progress:P0}");
                yield return null;
            }

            yield return StartCoroutine(_uiLoader.UpdateSlider(1f));
            yield return StartCoroutine(_uiLoader.FullSlider());
            _uiLoader.ActivateSpace();
            yield return StartCoroutine(_uiLoader.SpaceCondition());

            loadOperation.allowSceneActivation = true;

            while (!loadOperation.isDone)
            {
                yield return null;
            }

            yield return StartCoroutine(_uiLoader.HideLoadingScreen());

            _isLoading = false;

            onComplete?.Invoke();

            if (_showDebugLogs) Debug.Log($"Загрузка сцены {sceneType.ToString()} завершена!");
        }
    }
}