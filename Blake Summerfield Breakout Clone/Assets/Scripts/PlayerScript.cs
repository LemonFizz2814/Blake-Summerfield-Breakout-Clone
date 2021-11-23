using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] float paddleSpeed;
    [SerializeField] float xAxisBoundary;

    GameObject ballObject;

    [SerializeField] GameObject ballPrefab;
    [SerializeField] GameObject ballSpawnPos;

    bool canStartGame = true;

    private void Start()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);

        ballObject = Instantiate(ballPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        ballObject.GetComponent<BallScript>().SetPlayerScript(this);
        RespawnBall();
    }

    private void Update()
    {
        transform.Translate((Input.GetAxis("Horizontal") * paddleSpeed) * Time.deltaTime, 0, 0);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -xAxisBoundary, xAxisBoundary), transform.position.y, transform.position.z);

        //when space pressed, start game and move ball
        if(Input.GetKeyDown(KeyCode.Space) && canStartGame)
        {
            canStartGame = false;
            ballObject.GetComponent<BallScript>().StartGame();
        }

        //when R pressed, restart scene
        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //if ball collides with paddle
        if(other.CompareTag("Ball"))
        {
            other.GetComponent<BallScript>().BounceOffPaddle(gameObject.transform.position);
        }
    }

    public void RespawnBall()
    {
        ballObject.GetComponent<BallScript>().FreezeBall(true);
        ballObject.transform.position = ballSpawnPos.transform.localPosition;
        ballObject.transform.SetParent(transform, false);

        canStartGame = true;
    }
}
