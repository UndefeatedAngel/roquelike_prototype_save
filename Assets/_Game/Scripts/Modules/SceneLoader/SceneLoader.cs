using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using _Game.Scripts.Modules.SceneLoader;

public enum SceneType
{
    GamePlay,
    Battle,
    Cards,
    Chest
}

public class SceneLoader : MonoBehaviour
{
    [Header("Настройки загрузки")]
    [SerializeField] private float minLoadTime = 1.0f;
    [SerializeField] private bool showDebugLogs = true;
    
    [Header("Эффекты перехода")]
    [SerializeField] private UILoader _uiLoader;
    
    private SceneType currentActiveScene;
    private bool isLoading = false;
    
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
        
        currentActiveScene = Enum.Parse<SceneType>(SceneManager.GetActiveScene().name);
        _uiLoader = Instantiate(_uiLoader, transform);
    }
    
    public void LoadSceneAsync(SceneType sceneType, Action onComplete = null)
    {
        if (isLoading)
        {
            Debug.LogWarning("Загрузка уже в процессе!");
            return;
        }
        
        StartCoroutine(LoadSceneAsyncCoroutine(sceneType, onComplete));
    }
    
    private IEnumerator LoadSceneAsyncCoroutine(SceneType sceneType, Action onComplete)
    {
        isLoading = true;
        
        if (showDebugLogs) Debug.Log($"Начинаем загрузку сцены: {sceneType.ToString()}");
        
        // Показываем экран загрузки
        yield return StartCoroutine(_uiLoader.ShowLoadingScreen());
        
        // Сохраняем текущую активную сцену для удаления
        var sceneToUnload = currentActiveScene;
        
        // Асинхронная загрузка новой сцены
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneType.ToString(), LoadSceneMode.Single);
        loadOperation.allowSceneActivation = false;
        
        // Ждем минимум minLoadTime секунд для плавности
        float loadStartTime = Time.time;
        
        _uiLoader.ClearProgress();
        yield return new WaitForEndOfFrame();
        while (loadOperation.progress < 0.9f || Time.time - loadStartTime < minLoadTime)
        {
            // Целевой прогресс с учетом минимального времени
            yield return StartCoroutine(_uiLoader.UpdateSlider(loadOperation.progress));
            
            if (showDebugLogs) Debug.Log($"Прогресс загрузки: {loadOperation.progress:P0}");
            yield return null;
        }
        
        // Финальная анимация до 100%
        yield return StartCoroutine(_uiLoader.UpdateSlider(1f));
        
        // Активируем сцену
        loadOperation.allowSceneActivation = true;
        
        // Ждем полной загрузки
        while (!loadOperation.isDone)
        {
            yield return null;
        }
        
        // Скрываем экран загрузки
        yield return StartCoroutine(_uiLoader.HideLoadingScreen());
        
        isLoading = false;
        
        onComplete?.Invoke();
        
        if (showDebugLogs) Debug.Log($"Загрузка сцены {sceneType.ToString()} завершена!");
    }
    
    
}