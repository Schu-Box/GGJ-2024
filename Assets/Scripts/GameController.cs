using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    
    [Header("Values)")]
    public float timeLimit = 60f;
    public int scorePerTarget = 100;
    public int scorePerPickup = 10;

    [Header("UI")]
    // public TextMeshProUGUI scoreText;
    public Text scoreText;
    public MMF_Player feedback_addScore;
    
    public Slider timeSlider;
    // public TextMeshProUGUI timerText;
    public Text timerText;
    public MMF_Player feedback_lowTime;
    public GameObject gameOverUI;

    [Header("Start/End Animation")]
    public Animator crumbleAnimator;
    public MMF_Player feedback_crumble;

    private List<Bumper> bumperList = new List<Bumper>();
    
    private List<Bumper> possibleTargetList = new List<Bumper>();
    private Bumper lastTarget = null;

    private float timeRemaining = 0f;
    public int score = 0;

    private bool gameOver = false;

    private float lowTimeThreshold = 10f;
    private float lowTimeDurationBetweenFeedback = 1f;
    private float lowTimeTimer = 0f;

    private void Awake()
    {
        Instance = this;

        StartCoroutine(LateStart());
        
        gameOverUI.SetActive(false);
        timeRemaining = timeLimit;

        crumbleAnimator.gameObject.SetActive(true);
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

        if (!gameOver)
        {
            timeRemaining -= Time.deltaTime;
            timeSlider.value = timeRemaining / timeLimit;
            timerText.text = (timeRemaining / timeLimit * timeLimit).ToString("F1");
            
            if (timeRemaining <= 0f)
            {
                GameOver();
            } 
            else if(timeRemaining <= lowTimeThreshold)
            {
                lowTimeTimer -= Time.deltaTime;
                
                if (lowTimeTimer <= 0)
                {
                    feedback_lowTime.PlayFeedbacks();
                    
                    lowTimeTimer = lowTimeDurationBetweenFeedback;
                }
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
        
        // crumbleAnimator.gameObject.SetActive(true);
        crumbleAnimator.Play("Grow");
        feedback_crumble.PlayFeedbacks();

        Player.Instance.StopMovement();

        Leaderboard.Instance.FinalizeScore("TEST NAME");
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
            
            feedback_addScore.PlayFeedbacks();
        }
    }
}
