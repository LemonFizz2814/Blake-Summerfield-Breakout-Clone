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
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);

        UpdateText();
    }

    public void UpdateScore(int _points)
    {
        score += _points;
        UpdateText();
    }

    void UpdateText()
    {
        scoreText.text = "Score: " + score;
    }

    void SetScore(int _oldPoints, int _points)
    {
        score = _points;
        UpdateText();
        print("set");
    }
}
