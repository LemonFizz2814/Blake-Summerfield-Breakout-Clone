using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] float paddleSpeed;
    [SerializeField] float xAxisBoundary;

    [SerializeField] GameObject ballPrefab;
    GameObject ballObject;

    [SerializeField] GameObject ballSpawnPos;

    private void Start()
    {
        RespawnBall();
    }

    private void Update()
    {
        transform.Translate((Input.GetAxis("Horizontal") * paddleSpeed) * Time.deltaTime, 0, 0);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -xAxisBoundary, xAxisBoundary), transform.position.y, transform.position.z);

        //when space pressed, start game and move ball
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ballObject.GetComponent<BallScript>().StartGame();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //if ball collides with paddle
        if(other.CompareTag("Ball"))
        {
            other.GetComponent<BallScript>().RotateBall(gameObject.transform.position);
        }
    }

    public void RespawnBall()
    {
        ballObject = Instantiate(ballPrefab, ballSpawnPos.transform.position, Quaternion.identity);
        ballObject.transform.SetParent(transform);
    }
}
