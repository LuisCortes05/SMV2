using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Todo lo aqui escrito se asigna desde el componente button
/// </summary>
public class SceneManager : MonoBehaviour
{
    public GameObject UI1;
    public GameObject UI2;
    public GameObject pauseCanvas;
    private void Start()
    {
        if (UI1 == null || UI2 == null) return;
        else UI2.SetActive(false);

        if (pauseCanvas == null) return;
        else pauseCanvas.SetActive(false);
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
        pauseCanvas.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        pauseCanvas.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ChangeUI1()
    {
        UI1.SetActive(true);
        UI2.SetActive(false);
    }

    public void ChangeUI2()
    {
        UI1.SetActive(false);
        UI2.SetActive(true);
    }
}
