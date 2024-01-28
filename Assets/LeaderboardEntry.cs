using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardEntry : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;
    
    public void ShowScore(HighScoreData highScoreData)
    {
        // rankTextField.text = $"{highScoreData.rank}.";

        nameText.text = highScoreData.playerName;

        scoreText.text = highScoreData.playerScore.ToString();
    }
}
