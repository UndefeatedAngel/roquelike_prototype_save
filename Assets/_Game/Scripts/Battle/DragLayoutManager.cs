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

    public void OnLayoutChanged()
    {
        foreach (Transform child in transform)
        {
            DragAndHoverUI dragScript = child.GetComponent<DragAndHoverUI>();
            if (dragScript != null && dragScript.enabled)
            {
                dragScript.UpdateOriginalPosition();
            }
        }
    }

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