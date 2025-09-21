using UnityEngine;
using System.Collections.Generic;

public class CoinGridSpawner : MonoBehaviour
{
    [Header("Настройки монет")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int coinsX = 10;
    [SerializeField] private int coinsZ = 10;
    [SerializeField] private float spacing = 0.5f;
    
    [Header("Область спавна")]
    [SerializeField] private Vector3 areaSize = new Vector3(5f, 0f, 3f);
    [SerializeField] private bool showGizmos = true;
    
    [Header("Вариации")]
    [SerializeField] private float heightVariation = 0.1f;
    [SerializeField] private float rotationVariation = 15f;
    
    private List<GameObject> spawnedCoins = new List<GameObject>();
    
    public void SpawnCoinsGrid()
    {
        ClearExistingCoins();
        
        if (coinPrefab == null)
        {
            Debug.LogError("Coin prefab is not assigned!", this);
            return;
        }
        
        // Рассчитываем начальную позицию
        Vector3 startPos = transform.position - new Vector3(areaSize.x / 2, 0, areaSize.z / 2);
        
        for (int x = 0; x < coinsX; x++)
        {
            for (int z = 0; z < coinsZ; z++)
            {
                Vector3 position = CalculateGridPosition(startPos, x, z);
                Quaternion rotation = CalculateRandomRotation();
                
                GameObject coin = Instantiate(coinPrefab, position, rotation);
                spawnedCoins.Add(coin);
                
                // Делаем монету дочерним объектом для удобства
                coin.transform.SetParent(transform);
            }
        }
        
        Debug.Log($"Создано {spawnedCoins.Count} монет в сетке {coinsX}x{coinsZ}");
    }
    
    private Vector3 CalculateGridPosition(Vector3 startPos, int x, int z)
    {
        float posX = startPos.x + (x * spacing) + (spacing / 2);
        float posZ = startPos.z + (z * spacing) + (spacing / 2);
        float posY = transform.position.y + Random.Range(-heightVariation, heightVariation);
        
        return new Vector3(posX, posY, posZ);
    }
    
    private Quaternion CalculateRandomRotation()
    {
        return Quaternion.Euler(
            Random.Range(-rotationVariation, rotationVariation),
            Random.Range(0f, 360f),
            Random.Range(-rotationVariation, rotationVariation)
        );
    }
    
    public void ClearExistingCoins()
    {
        foreach (GameObject coin in spawnedCoins)
        {
            if (coin != null)
            {
                DestroyImmediate(coin);
            }
        }
        spawnedCoins.Clear();
    }
    
    private void OnDrawGizmos()
    {
        if (!showGizmos) return;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSize.x, 0.1f, areaSize.z));
        
        // Показываем сетку спавна
        Gizmos.color = Color.green;
        Vector3 startPos = transform.position - new Vector3(areaSize.x / 2, 0, areaSize.z / 2);
        
        for (int x = 0; x <= coinsX; x++)
        {
            for (int z = 0; z <= coinsZ; z++)
            {
                Vector3 pos = startPos + new Vector3(x * spacing, 0, z * spacing);
                Gizmos.DrawWireSphere(pos, 0.05f);
            }
        }
    }
    
    [ContextMenu("Спавн монет сеткой")]
    private void SpawnGridEditor()
    {
        SpawnCoinsGrid();
    }
    
    [ContextMenu("Очистить монеты")]
    private void ClearCoinsEditor()
    {
        ClearExistingCoins();
    }
}