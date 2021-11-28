using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class UIManager : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;

    [SyncVar(hook = "SetScore")] int score = 0;

    private void Start()
    {
        //set resolution
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);

        UpdateScoreText();
    }

    //update score value
    public void UpdateScore(int _points)
    {
        score += _points;
        UpdateScoreText();
    }

    //update score ui text
    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }

    //hook for score
    void SetScore(int _oldPoints, int _points)
    {
        score = _points;
        UpdateScoreText();
    }
}
