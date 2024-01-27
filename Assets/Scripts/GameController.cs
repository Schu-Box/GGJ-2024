using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    
    public float timeLimit = 60f;

    public TextMeshProUGUI speedText;
    public TextMeshProUGUI scoreText;
    public Slider timeSlider;
    public GameObject gameOverUI;

    private List<Bumper> bumperList = new List<Bumper>();
    
    private List<Bumper> possibleTargetList = new List<Bumper>();
    private Bumper lastTarget = null;

    private float timeRemaining = 0f;
    private int score = 0;

    private bool gameOver = false;

    private void Awake()
    {
        Instance = this;

        StartCoroutine(LateStart());
        
        gameOverUI.SetActive(false);
        timeRemaining = timeLimit;
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForFixedUpdate();
        
        SetRandomTarget();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }

        speedText.text = Player.Instance.GetCurrentSpeed().ToString("F1");

        if (!gameOver)
        {
            timeRemaining -= Time.deltaTime;
            timeSlider.value = timeRemaining / timeLimit;
            
            if (timeRemaining <= 0f)
            {
                GameOver();
            }
        }
        
        // foreach (Bumper bumper in bumperList)
        // {
        //     if(Player.Instance.GetCurrentMass() >= bumper.massRequiredToBreak && Player.Instance.GetCurrentSpeed() >= Player.Instance.requiredSpeedToBreakBumper)
        //     {
        //         bumper.SetBreakable(true);
        //     } 
        //     else
        //     {
        //         bumper.SetBreakable(false);
        //     }
        // }
    }

    public void GameOver()
    {
        gameOver = true;
        
        gameOverUI.SetActive(true);
    }

    public void AddBumper(Bumper bumper)
    {
        bumperList.Add(bumper);

        if (bumper.canBeTarget)
        {
            possibleTargetList.Add(bumper);
        }
    }

    public void RemoveBumper(Bumper bumper)
    {
        bumperList.Remove(bumper);
    }

    public void SetRandomTarget()
    {
        if (!gameOver)
        {
            Bumper newTarget = lastTarget;
            while (newTarget == lastTarget)
            {
                newTarget = possibleTargetList[Random.Range(0, possibleTargetList.Count)];
            }
        
            newTarget.SetAsTarget();
            lastTarget = newTarget;
        }
    }

    public void AddScore(int scoreEarned)
    {
        if (!gameOver)
        {
            score += scoreEarned;
        
            scoreText.text = score.ToString();  
        }
    }
}
