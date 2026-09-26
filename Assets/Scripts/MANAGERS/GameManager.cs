using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
/// <summary> 
/// Notas de bugs:
/// Aun hay que corregir los rounds
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private float maxTime = 99f;
    private float currentTime;

    [Header("Roounds")]
    private int maxRounds = 2;
    private int currentRound;
    private bool endRound;

    [Header("Panels")]
    //[SerializeField] private GameObject EndGamePanel;

    [Header("References")]
    SceneManager scManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        endRound = false;
        currentTime = maxTime;
        timeText.text = currentTime.ToString();
        DontDestroyOnLoad(this);
        StartCoroutine(Timer());
    }

    private void Update()
    {
        if(endRound)
        {
            EndRound();
        }
    }

    private void EndRound()
    {
        currentRound += 1;
        //scManager.ResetRounds();
        Debug.Log($"Round {currentRound}");
        if (currentRound > maxRounds) EndGame();
    }

    private void EndGame()
    {
        Debug.Log($"Game over");
    }

    #region Corutines
    private IEnumerator Timer()
    {
        while(currentTime >= 0)
        {
            currentTime -= Time.deltaTime;
            
            timeText.text = currentTime.ToString("f0"); // "f0" hace que no se impriman decimales en el tiempo
            yield return new WaitForSeconds(1f);
            currentTime--;
        }
        if (currentTime < 0)
        {
            endRound = true;
            yield return new WaitForSeconds(5f);
            endRound = false;
        }
    }
    #endregion
}
