using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Method to load a scene by name
    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    // Example methods for specific scenes
    public void LoadMainMenu()
    {
        LoadScene("MainMenu");
    }

    public void LoadGame()
    {
        LoadScene("SampleScene");
    }

    public void LoadSettings()
    {
        LoadScene("Settings");
    }

    // Method to reload the current scene
    public void ReloadCurrentScene()
    {
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        LoadScene(currentSceneName);
    }

    // Method to quit the game
    public void QuitGame()
    {
        Application.Quit();
    }
}
