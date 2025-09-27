using UnityEngine;
using UnityEngine.UI;

namespace BlockoutRoguelike.Core.Inventory.InventorySystem
{
    public class TooltipSystemController : MonoBehaviour
    {
        public static TooltipSystemController Instance { get; private set; }
    
        [SerializeField] private GameObject tooltipPanel;
        [SerializeField] private Text titleText;
        [SerializeField] private Text descriptionText;
    
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        
            tooltipPanel.SetActive(false);
        }
    
        public void ShowTooltip(string title, string description)
        {
            titleText.text = title;
            descriptionText.text = description;
            tooltipPanel.SetActive(true);
        
            tooltipPanel.transform.position = Input.mousePosition + new Vector3(20, -20, 0);
        }
    
        public void HideTooltip()
        {
            tooltipPanel.SetActive(false);
        }
    }
}