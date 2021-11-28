using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Networking;
using Mirror;

public class PlayerScript : NetworkBehaviour
{
    [SerializeField] float paddleSpeed;
    [SerializeField] float xAxisBoundary;
    [SerializeField] float playerHeightOffset;

    int playerNum;

    [SyncVar(hook = "SetBall")] GameObject ballObject;

    [SerializeField] GameObject ballPrefab;
    [SerializeField] GameObject ballSpawnPos;

    [SyncVar] bool canStartGame = true;

    private void Start()
    {
        print("PLAYER JOINED");

        SpawnBallCmd();

        playerNum = GameObject.FindGameObjectsWithTag("Player").Length - 1;
        transform.position = new Vector3(transform.position.x, transform.position.y + (playerNum * playerHeightOffset), transform.position.z);
    }
    private void Update()
    {
        if (isLocalPlayer)
        {
            //movement of paddle
            transform.Translate((Input.GetAxis("Horizontal") * paddleSpeed) * Time.deltaTime, 0, 0);
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -xAxisBoundary, xAxisBoundary), transform.position.y, transform.position.z);

            //when space pressed, start game and move ball
            if (Input.GetKeyDown(KeyCode.Space) && canStartGame)
            {
                print("space pressed");
                SpacePressedCmd();
            }

            //when R pressed, restart scene
            if (Input.GetKeyDown(KeyCode.R))
            {
                //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //if ball collides with paddle
        if (other.CompareTag("Ball"))
        {
            other.GetComponent<BallScript>().BounceOffPaddle(gameObject.transform.position);
        }
    }

    //spawn ball (on server)
    [Command]
    void SpawnBallCmd()
    {
        //spawn ball over network
        GameObject _obj = Instantiate(ballPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        NetworkServer.Spawn(_obj);

        //set properties of ball
        SetBall(_obj, _obj);
        ballObject.GetComponent<BallScript>().SetPlayerScript(null, this);
    }

    //set reference to players ball
    public void SetBall(GameObject _oldBallObject, GameObject _ballObject)
    {
        ballObject = _ballObject;
        //RespawnBallCmd();
    }

    //Space Pressed (server to client)
    [ClientRpc]
    public void SpacePressedClnt()
    {
        canStartGame = false;
        ballObject.GetComponent<BallScript>().StartGame();
    }
    //Space Pressed (client to server)
    [Command]
    public void SpacePressedCmd()
    {
        SpacePressedClnt();
    }

    [Command]
    public void RespawnBallCmd()
    {
        RespawnBallClnt();
    }


    [ClientRpc]
    public void RespawnBallClnt()
    {
        //print("respawn");
        ballObject.GetComponent<BallScript>().FreezeBall(true);
        ballObject.transform.localPosition = ballSpawnPos.transform.localPosition;
        //ballObject.transform.SetParent(transform, false);

        canStartGame = true;
    }
}
