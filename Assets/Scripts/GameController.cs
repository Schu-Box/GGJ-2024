using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public TextMeshProUGUI speedText;
    public TextMeshProUGUI scoreText;

    private List<Bumper> bumperList = new List<Bumper>();
    
    private List<Bumper> possibleTargetList = new List<Bumper>();
    private Bumper lastTarget = null;

    private int score = 0;

    private void Awake()
    {
        Instance = this;

        StartCoroutine(LateStart());
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

        foreach (Bumper bumper in bumperList)
        {
            if(Player.Instance.GetCurrentMass() >= bumper.massRequiredToBreak && Player.Instance.GetCurrentSpeed() >= Player.Instance.requiredSpeedToBreakBumper)
            {
                bumper.SetBreakable(true);
            } 
            else
            {
                bumper.SetBreakable(false);
            }
        }
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
        Bumper newTarget = lastTarget;
        while (newTarget == lastTarget)
        {
            newTarget = possibleTargetList[Random.Range(0, possibleTargetList.Count)];
        }
        
        newTarget.SetAsTarget();
        lastTarget = newTarget;
    }

    public void AddScore(int scoreEarned)
    {
        score += scoreEarned;
        
        scoreText.text = score.ToString();
    }
}
