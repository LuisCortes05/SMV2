using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
/// <summary>
/// Todo lo aqui escrito se asigna desde el componente button
/// </summary>
public class SceneManager : MonoBehaviour
{
    public GameObject UI1;
    public GameObject UI2;
    public GameObject pauseCanvas;

    [Header("Gamepad UI Focus")]
    [SerializeField] private GameObject firstButtonUI1; // Botón inicial del Main Menu
    [SerializeField] private GameObject firstButtonUI2; // Botón Regresar en Créditos
    [SerializeField] private GameObject firstPauseButton;

    private bool isPaused = false;

    private void Start()
    {
        if (UI1 != null && UI2 != null)
        {
            UI1.SetActive(true);
            UI2.SetActive(false);
            SelectButton(firstButtonUI1);
        }

        if (pauseCanvas != null)
        {
            pauseCanvas.SetActive(false);
            isPaused = false;
        }
    }

    private void Update()
    {
        // Solo escucha el botón de pausa en escenas que tengan asignado un pauseCanvas (como Arena)
        if (pauseCanvas == null) return;

        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            if (Gamepad.all[i].startButton.wasPressedThisFrame)
            {
                TogglePause();
                break; // Evita que se ejecute dos veces en el mismo fotograma
            }
        }
    }
    public void TogglePause()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }
    public void ChangeScene(int sceneNum)
    {
        isPaused = false;
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneNum);
    }

    public void ResetRounds()
    {
        isPaused = false;
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Arena");
    }

    public void Pause()
    {
        if (pauseCanvas == null) return;

        isPaused = true;
        Time.timeScale = 0f;
        pauseCanvas.SetActive(true);
        SelectButton(firstPauseButton);
    }

    public void Resume()
    {
        if (pauseCanvas == null) return;

        isPaused = false;
        Time.timeScale = 1f;
        pauseCanvas.SetActive(false);

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ChangeUI1()
    {
        UI1.SetActive(true);
        UI2.SetActive(false);
        SelectButton(firstButtonUI1);
    }

    public void ChangeUI2()
    {
        UI1.SetActive(false);
        UI2.SetActive(true);
        SelectButton(firstButtonUI2);
    }

    private void SelectButton(GameObject buttonObj)
    {
        if (buttonObj == null || EventSystem.current == null) return;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(buttonObj);
    }
}
