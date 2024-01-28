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
    
    [Header("Music")]
    public FMODUnity.StudioEventEmitter musicEmitter;
    public float musicDuringMenus = 1f;
    public float musicDuringGameplay = 0.5f;
    
    [Header("Values)")]
    public float timeLimit = 60f;
    public int scorePerTarget = 100;
    public int scorePerPickup = 10;
    
    [Header("Cursors")]
    public Texture2D cursorWhiteArrow;
    public Texture2D cursorRedArrow;
    public Texture2D cursorGreenArrow;

    [Header("Start Menu")] 
    public GameObject startUI;
    public TMP_InputField nameInputField;

    [Header("UI")] 
    public GameObject gameUI;
    public TextMeshProUGUI scoreText;
    // public Text scoreText;
    public MMF_Player feedback_addScore;
    
    public TextMeshProUGUI timerText;
    // public Text timerText;
    public MMF_Player feedback_lowTime;
    public GameObject gameOverUI;

    [Header("Start/End Animation")]
    public Animator crumbleAnimator;
    public MMF_Player feedback_start;
    public MMF_Player feedback_end;

    private List<Bumper> bumperList = new List<Bumper>();
    
    private List<Bumper> possibleTargetList = new List<Bumper>();
    private Bumper lastTarget = null;

    private float timeRemaining = 0f;
    public int score = 0;

    public bool gameStarted = false;
    private bool firstLaunch = false;
    private bool gameOver = false;

    private float lowTimeThreshold = 10f;
    private float lowTimeDurationBetweenFeedback = 1f;
    private float lowTimeTimer = 0f;

    private string currentName = "";

    private void Awake()
    {
        Instance = this;

        StartCoroutine(LateStart());
        
        // musicEmitter.SetParameter("Parameter 1", musicDuringMenus);
        musicEmitter.EventInstance.setVolume(musicDuringMenus);
        
        crumbleAnimator.gameObject.SetActive(true);
        gameOverUI.SetActive(false);
        
        timeRemaining = timeLimit;

        string savedName = PlayerPrefs.GetString("savedName");
        
        Cursor.SetCursor(cursorWhiteArrow, Vector2.zero, CursorMode.ForceSoftware);
        
        if (savedName != "")
        {
            currentName = savedName;
            StartGame();
        }
        else
        {
            startUI.SetActive(true);
        }
        
        PlayerPrefs.SetString("savedName", "");
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForFixedUpdate();
        
        SetRandomTarget();
    }

    public void StartGame()
    {
        // musicEmitter.SetParameter("Parameter 1", musicDuringGameplay);
        musicEmitter.EventInstance.setVolume(musicDuringGameplay);
        
        feedback_start.PlayFeedbacks();
        crumbleAnimator.Play("Shrink");
        PlaySound("event:/Paper");

        gameStarted = true;

        if (currentName == "")
        {
            currentName = nameInputField.text;
        }
        
        startUI.SetActive(false);
        gameUI.SetActive(true);
        
        Cursor.SetCursor(cursorGreenArrow, Vector2.zero, CursorMode.ForceSoftware);
    }

    public void FirstLaunch()
    {
        if (!firstLaunch)
        {
            firstLaunch = true;
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SceneManager.LoadScene(0);
        }

        if (!gameOver && firstLaunch)
        {
            timeRemaining -= Time.deltaTime;
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
                    PlaySound("event:/Warning");
                    
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
        
        // musicEmitter.SetParameter("Parameter 1", musicDuringMenus);
        musicEmitter.EventInstance.setVolume(musicDuringMenus);
        
        //TODO: Delay on gameOverUI?
        
        feedback_end.PlayFeedbacks();
        crumbleAnimator.Play("Grow");
        PlaySound("event:/Paper");

        Player.Instance.StopMovement();

        StartCoroutine(DelayThenDisplayGameOverUI());
    }

    private IEnumerator DelayThenDisplayGameOverUI()
    {
        yield return new WaitForSeconds(0.5f);
        
        gameOverUI.SetActive(true);
        gameUI.SetActive(false);
        
        Leaderboard.Instance.FinalizeScore(currentName);
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

    private FMOD.Studio.EventInstance fmodStudioEvent;

    public void PlaySound(string sound)
    {
        fmodStudioEvent = FMODUnity.RuntimeManager.CreateInstance(sound);
        fmodStudioEvent.start();
        fmodStudioEvent.release();
    }

    //TODO: RETRY IS NOT WORKING!
    
    public void Retry()
    {
        PlayerPrefs.SetString("savedName", currentName);
        
        SceneManager.LoadScene(0);
    }

    public void NewPlayer()
    {
        SceneManager.LoadScene(0);
    }
}
