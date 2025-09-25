using UnityEngine;

namespace BlockoutRoguelike
{
    public class ProjectController: MonoBehaviour
    {
        private static ProjectController _instance;

        private void Awake()
        {
            if (_instance == null)
            {
                PlayerPrefs.DeleteAll();
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}