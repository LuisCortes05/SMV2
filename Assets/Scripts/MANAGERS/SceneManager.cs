using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Todo lo aqui escrito se asigna desde el componente button
/// </summary>
public class SceneManager : MonoBehaviour
{
    private MenuFocusHelper helper;
    public GameObject UI1;
    public GameObject UI2;
    private void Start()
    {
        if (UI1 == null || UI2 == null) return;
        else UI2.SetActive(false);
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

    public void ChangeUI1()
    {
        UI1.SetActive(true);
        UI2.SetActive(false);
    }

    public void ChangeUI2()
    {
        UI1.SetActive(false);
        UI2.SetActive(true);
        helper.FocusNewPanel();
    }
}
