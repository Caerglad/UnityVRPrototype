using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scenes.MainMenu
{
    public class SceneLoader : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            Debug.Log("Trying to load Scene with name: " + sceneName);
            SceneManager.LoadScene(sceneName);
        }
    }
}