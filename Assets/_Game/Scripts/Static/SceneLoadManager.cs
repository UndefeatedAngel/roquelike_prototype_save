using UnityEngine.SceneManagement;

namespace BlockoutRoguelike.Static
{
    public enum SceneName
    {
        GamePlay,
        Battle,
        Cards
    }
    
    public static class SceneLoadManager
    {
        public static void LoadScene(SceneName sceneName)
        {
            SceneManager.LoadScene(sceneName.ToString());
        }
    }
}