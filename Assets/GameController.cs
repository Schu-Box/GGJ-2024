using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    
    private List<Bumper> bumperList = new List<Bumper>();

    private void Awake()
    {
        Instance = this;

        StartCoroutine(LateStart());
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForFixedUpdate();
        
        UpdateBumperVisuals();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
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

    public void UpdateBumperVisuals()
    {
        foreach (Bumper bumper in bumperList)
        {
            if(bumper.massValue <= Player.Instance.GetCurrentMass())
            {
                bumper.ShowBreakableVisuals();
            }
        }
    }
}
