using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private List<Cards> allCards;
    private Cards flippedCard;
    private bool isFlipping;

    [SerializeField] private Slider timeOutSlider;
    [SerializeField] private TextMeshProUGUI timeOutText;
    [SerializeField] private float timeLimit = 60f;
    private float currentTime;

    [SerializeField] private GameObject pausePanel;
    private bool isPaused = false;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI btnGameOverText;
    private bool isGameOver = false;

    [SerializeField] private TextMeshProUGUI scoreText;
    private int scoreCorrectMatches, scoreWrongMatches = 0;

    private int totalMatches = 10;
    private int matchesFound = 0;

    


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        Board board = FindAnyObjectByType<Board>();
        allCards = board.GetCards();

        currentTime = timeLimit;
        SetCurrentTimeText();

        StartCoroutine(FlippAllCardsRoutine());
    }

    void SetCurrentTimeText()
    {
        int timeSec = Mathf.CeilToInt(currentTime);
        timeOutText.SetText(timeSec.ToString());
    }

IEnumerator FlippAllCardsRoutine()
    {
        isFlipping = true;

        yield return new WaitForSeconds(0.5f);
        FlippAllCards();
        yield return new WaitForSeconds(2f);
        FlippAllCards();
        yield return new WaitForSeconds(0.5f);

        isFlipping = false;

        yield return StartCoroutine("CountDownTimerRoutine");
    }

    IEnumerator CountDownTimerRoutine()
    {
        while(currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            timeOutSlider.value = currentTime / timeLimit;

            SetCurrentTimeText();

            yield return null;
        }

        GameOver(false);
    }

    void FlippAllCards()
    {
        foreach (Cards card in allCards)
        {
            card.FilpCard();
        }
    }

    public void CardClicked(Cards card)
    {
        if (isFlipping || isGameOver || isPaused)
        {
            return;
        }

        card.FilpCard();

        if (flippedCard == null)
        {
            flippedCard = card;
            
        }
        else
        {
            StartCoroutine(CheckMatchRoutine(flippedCard, card));
        }
    }

    IEnumerator CheckMatchRoutine(Cards card1, Cards card2)
    {
        isFlipping = true;
        if (card1.cardID == card2.cardID)
        {
            card1.SetMatched();
            card2.SetMatched();

            matchesFound ++;

            scoreCorrectMatches ++;

            if(matchesFound == totalMatches)
            {
                GameOver(true);
            }

        }
        else
        {
            yield return new WaitForSeconds(0.6f);

            card1.FilpCard();
            card2.FilpCard();
           
            scoreWrongMatches ++;

            yield return new WaitForSeconds(0.4f);
        }

        flippedCard = null;
        isFlipping = false;
    }

    void GameOver(bool success)
    {
        if(!isGameOver)
        {
            isGameOver = true;
            StopCoroutine("CountDownTimerRoutine");

            if (success)
            {
                gameOverText.SetText("Great Job");
                btnGameOverText.SetText("Play");
            
            }
            else
            {
                gameOverText.SetText("Try Again");
                btnGameOverText.SetText("Retry");
                
            }

            ScoreCalculate();

            Invoke("ShowGameOverPanel",0.5f);
        }
        
        
    }

    void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
        
    }

    void ScoreCalculate()
    {
        float completionFactor = scoreCorrectMatches / 10f;
        float completionScore = completionFactor * 70;

        float timeBonus = (currentTime / timeLimit) * 50;  
        float penalty = scoreWrongMatches * 2; 

        float rawScore = (completionScore + timeBonus) - penalty;
        int finalScore = (int)Mathf.Round(Mathf.Clamp(rawScore, 0, 100));

        scoreText.SetText("Score: " + finalScore.ToString());
    }


    public void PauseGame()
    {
        if (isFlipping || isGameOver || isPaused) {return;}

        Time.timeScale = 0f;
        DOTween.PauseAll();
        pausePanel.SetActive(true);

        isPaused = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        DOTween.PlayAll();
        pausePanel.SetActive(false);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        StopAllCoroutines();
        DOTween.KillAll();
        SceneManager.LoadScene("GameScene");
    }

    public void LoadHome()
    {
        Time.timeScale = 1f;
        StopAllCoroutines();
        DOTween.KillAll();
        SceneManager.LoadScene("HomeScene");
    }


}
