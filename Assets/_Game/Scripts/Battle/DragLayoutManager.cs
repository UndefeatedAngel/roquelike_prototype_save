using UnityEngine;
using UnityEngine.UI;

public class DragLayoutManager : MonoBehaviour
{
    public static DragLayoutManager Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Вызывается когда layout изменяется (добавление/удаление элементов)
    public void OnLayoutChanged()
    {
        // Обновляем позиции всех элементов
        foreach (Transform child in transform)
        {
            DragAndHoverUI dragScript = child.GetComponent<DragAndHoverUI>();
            if (dragScript != null)
            {
                dragScript.UpdateOriginalPosition();
            }
        }
    }

    // Принудительно возвращаем все элементы на места
    public void ResetAllItems()
    {
        foreach (Transform child in transform)
        {
            DragAndHoverUI dragScript = child.GetComponent<DragAndHoverUI>();
            if (dragScript != null)
            {
                dragScript.ForceReturnToPosition();
            }
        }
    }
}