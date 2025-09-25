using UnityEngine;

public class SimpleAreaFiller : MonoBehaviour
{
    [Header("Базовые настройки")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private Vector2Int gridSize = new Vector2Int(10, 10);
    [SerializeField] private float spacing = 0.5f;
    
    public void FillAreaWithCoins()
    {
        ClearArea();
        
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 position = new Vector3(
                    x * spacing - (gridSize.x * spacing) / 2,
                    0,
                    y * spacing - (gridSize.y * spacing) / 2
                ) + transform.position;
                
                Instantiate(coinPrefab, position, Quaternion.identity, transform);
            }
        }
    }
    
    public void ClearArea()
    {
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
    }
    
    [ContextMenu("Заполнить область")]
    private void FillAreaEditor()
    {
        FillAreaWithCoins();
    }
    
    [ContextMenu("Очистить область")]
    private void ClearAreaEditor()
    {
        ClearArea();
    }
}