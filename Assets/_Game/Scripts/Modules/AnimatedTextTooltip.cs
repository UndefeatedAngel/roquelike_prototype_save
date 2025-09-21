using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using TMPro;

public class AnimatedTextTooltip : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text textComponent;

    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float displayTime = 3f;

    private Tween currentFadeTween;
    private Tween currentTypingTween;
    private Coroutine currentDisplayCoroutine;

    private string fullText = "";
    private string currentText = "";
    private bool isShowing = false;

    void Awake()
    {
        // Автоматически находим компоненты если не установлены
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        
        if (textComponent == null)
            textComponent = GetComponentInChildren<TMP_Text>();

        // Скрываем при старте
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        if (textComponent != null)
            textComponent.text = "";
    }

    public void ShowText(string text)
    {
        // Останавливаем все текущие анимации
        KillAllTweens();
        if (currentDisplayCoroutine != null)
            StopCoroutine(currentDisplayCoroutine);

        fullText = text;
        currentText = "";

        // Немедленно показываем с прозрачностью
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }

        // Запускаем анимацию печати
        StartTypingAnimation();
        
        // Запускаем таймер скрытия
        currentDisplayCoroutine = StartCoroutine(HideAfterDelay());
    }

    private void StartTypingAnimation()
    {
        if (textComponent == null) return;

        // Останавливаем предыдущую анимацию печати
        if (currentTypingTween != null && currentTypingTween.IsActive())
            currentTypingTween.Kill();

        // Сбрасываем текст
        textComponent.text = "";
        currentText = "";

        // Создаем анимацию печати с правильным синтаксисом
        float duration = fullText.Length * typingSpeed;
        int charCount = 0;
        
        currentTypingTween = DOTween.To(() => charCount, x => charCount = x, fullText.Length, duration)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                currentText = fullText.Substring(0, charCount);
                textComponent.text = currentText;
            });
    }

    private IEnumerator HideAfterDelay()
    {
        isShowing = true;
        yield return new WaitForSeconds(displayTime);
        Hide();
    }

    public void Hide()
    {
        // Останавливаем анимацию печати
        if (currentTypingTween != null && currentTypingTween.IsActive())
            currentTypingTween.Kill();

        // Останавливаем корутину
        if (currentDisplayCoroutine != null)
            StopCoroutine(currentDisplayCoroutine);

        // Анимация исчезновения
        if (canvasGroup != null)
        {
            // Останавливаем предыдущую анимацию fade
            if (currentFadeTween != null && currentFadeTween.IsActive())
                currentFadeTween.Kill();

            currentFadeTween = canvasGroup.DOFade(0f, fadeDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    canvasGroup.blocksRaycasts = false;
                    canvasGroup.interactable = false;
                    isShowing = false;
                    
                    // Очищаем текст после скрытия
                    if (textComponent != null)
                        textComponent.text = "";
                });
        }
        else
        {
            // Если нет CanvasGroup, просто очищаем текст
            if (textComponent != null)
                textComponent.text = "";
            
            isShowing = false;
        }
    }

    public void ForceHide()
    {
        // Резкое скрытие без анимации
        KillAllTweens();
        
        if (currentDisplayCoroutine != null)
            StopCoroutine(currentDisplayCoroutine);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        if (textComponent != null)
            textComponent.text = "";

        isShowing = false;
    }

    private void KillAllTweens()
    {
        if (currentFadeTween != null && currentFadeTween.IsActive())
            currentFadeTween.Kill();

        if (currentTypingTween != null && currentTypingTween.IsActive())
            currentTypingTween.Kill();
    }

    public bool IsShowing()
    {
        return isShowing;
    }

    // Метод для быстрой настройки параметров
    public void SetParameters(float newFadeDuration, float newTypingSpeed, float newDisplayTime)
    {
        fadeDuration = newFadeDuration;
        typingSpeed = newTypingSpeed;
        displayTime = newDisplayTime;
    }

    void OnDestroy()
    {
        // Очищаем твины при уничтожении объекта
        KillAllTweens();
    }
}