using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ContinuousPulseButton : MonoBehaviour
{
    [Header("Pulse Settings")]
    public float pulseDuration = 1f;
    public float minScale = 0.9f;
    public float maxScale = 1.1f;
    public bool startOnAwake = true;
    
    private Button button;
    private Tween pulseTween;

    void Start()
    {
        button = GetComponent<Button>();
        
        if (startOnAwake)
        {
            StartPulsating();
        }
        
        button.onClick.AddListener(OnButtonClick);
    }

    public void StartPulsating()
    {
        // Создаем бесконечную пульсацию с ping-pong эффектом
        pulseTween = transform.DOScale(maxScale, pulseDuration)
            .From(minScale)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void StopPulsating()
    {
        pulseTween?.Kill();
        transform.DOScale(1f, 0.2f); // Плавно возвращаем к нормальному размеру
    }

    void OnButtonClick()
    {
        // Дополнительный эффект при клике
        transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 1, 0.5f);
    }

    void OnDestroy()
    {
        pulseTween?.Kill();
    }
}