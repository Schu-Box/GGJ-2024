using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public TextMeshProUGUI speedText;

    private List<Bumper> bumperList = new List<Bumper>();

    private void Awake()
    {
        Instance = this;

        StartCoroutine(LateStart());
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForFixedUpdate();
        
        // UpdateBumperVisuals();
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
    }

    public void RemoveBumper(Bumper bumper)
    {
        bumperList.Remove(bumper);
    }

    // public void UpdateBumperVisuals()
    // {
    //     foreach (Bumper bumper in bumperList)
    //     {
    //         if(bumper.massValue <= Player.Instance.GetCurrentMass())
    //         {
    //             bumper.ShowBreakableVisuals();
    //         }
    //     }
    // }
}
