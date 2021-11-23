using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;

    int score = 0;

    private void Start()
    {
        UpdateScore(0);
    }

    public void UpdateScore(int _points)
    {
        score += _points;
        scoreText.text = "Score: " + score;
    }
}
