using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Todo lo aqui escrito se asigna desde el componente button
/// </summary>
public class SceneManager : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(this);
    }
    public void ChangeScene(int sceneNum)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneNum);
    }

    public void ResetRounds()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Arena");
    }

    public void Pause()
    {
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
