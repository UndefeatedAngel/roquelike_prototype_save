using UnityEngine;

public class BillboardUICamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera targetCamera;
    public bool useMainCamera = true;
    
    [Header("Rotation Settings")]
    public bool billboard = true; // Всегда поворачивать к камере
    public bool verticalBillboard = false; // Только горизонтальный поворот
    public bool reverseFace = false; // Развернуть на 180 градусов
    
    [Header("Axis Constraints")]
    public bool freezeX = false;
    public bool freezeY = false;
    public bool freezeZ = false;
    
    [Header("Smoothing")]
    public bool useSmoothing = false;
    public float smoothSpeed = 5f;
    
    private Quaternion originalRotation;
    private Vector3 originalEulerAngles;

    void Start()
    {
        InitializeCamera();
        StoreOriginalRotation();
    }

    void LateUpdate()
    {
        if (billboard && targetCamera != null)
        {
            UpdateBillboardRotation();
        }
    }

    private void InitializeCamera()
    {
        if (useMainCamera)
        {
            targetCamera = Camera.main;
        }
        
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
            if (targetCamera == null)
            {
                Debug.LogWarning("BillboardUI: No camera assigned and no MainCamera found!", this);
            }
        }
    }

    private void StoreOriginalRotation()
    {
        originalRotation = transform.localRotation;
        originalEulerAngles = transform.localEulerAngles;
    }

    private void UpdateBillboardRotation()
    {
        Vector3 targetPosition = transform.position;
        Vector3 cameraPosition = targetCamera.transform.position;
        
        // Вычисляем направление к камере
        Vector3 directionToCamera = cameraPosition - targetPosition;
        
        if (verticalBillboard)
        {
            // Только горизонтальный поворот (Y axis)
            directionToCamera.y = 0;
        }
        
        if (directionToCamera.sqrMagnitude > 0.001f)
        {
            // Вычисляем целевой поворот
            Quaternion targetRotation = Quaternion.LookRotation(reverseFace ? -directionToCamera : directionToCamera);
            
            // Применяем ограничения осей
            targetRotation = ApplyAxisConstraints(targetRotation);
            
            // Применяем сглаживание или мгновенный поворот
            if (useSmoothing)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);
            }
            else
            {
                transform.rotation = targetRotation;
            }
        }
    }

    private Quaternion ApplyAxisConstraints(Quaternion rotation)
    {
        Vector3 eulerAngles = rotation.eulerAngles;
        
        if (freezeX) eulerAngles.x = originalEulerAngles.x;
        if (freezeY) eulerAngles.y = originalEulerAngles.y;
        if (freezeZ) eulerAngles.z = originalEulerAngles.z;
        
        return Quaternion.Euler(eulerAngles);
    }

    // Методы для динамического управления
    public void SetBillboard(bool enabled)
    {
        billboard = enabled;
        if (!enabled)
        {
            ResetToOriginalRotation();
        }
    }

    public void SetVerticalBillboard(bool enabled)
    {
        verticalBillboard = enabled;
    }

    public void SetTargetCamera(Camera newCamera)
    {
        targetCamera = newCamera;
    }

    public void ResetToOriginalRotation()
    {
        transform.localRotation = originalRotation;
    }

    // Для отладки
    void OnDrawGizmosSelected()
    {
        if (billboard && targetCamera != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, targetCamera.transform.position);
            Gizmos.DrawWireSphere(transform.position, 0.1f);
        }
    }
}