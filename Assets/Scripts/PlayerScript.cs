using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Mirror;

public class PlayerScript : NetworkBehaviour
{
    [SerializeField] float paddleSpeed;
    [Tooltip("set x axis boundary for paddle so it can't move out of bounds")]
    [SerializeField] float xAxisBoundary;
    [Tooltip("the distance in height between each player that spawns")]
    [SerializeField] float playerHeightOffset;

    int playerNum;

    [SyncVar(hook = "SetBall")] GameObject ballObject;

    [SerializeField] GameObject ballPrefab;
    [SerializeField] GameObject ballSpawnPos;

    [SyncVar(hook = "SetCanStartGame")] bool canStartGame = true;

    private void Start()
    {
        //spawn ball on the network if is local
        if (isLocalPlayer)
        {
            SpawnBallCmd();
        }

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
                SpacePressedCmd();
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

    //get spawn position for ball
    public GameObject GetBallSpawnPos()
    {
        return ballSpawnPos;
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
    }
    //set reference to players ball
    public void SetCanStartGame(bool _oldActive, bool _active)
    {
        canStartGame = _active;
    }

    //Space Pressed (server to client)
    [ClientRpc]
    public void SpacePressedClnt()
    {
        SetCanStartGame(canStartGame, false);
        ballObject.GetComponent<BallScript>().StartGame();
    }
    //Space Pressed (client to server)
    [Command]
    public void SpacePressedCmd()
    {
        SpacePressedClnt();
    }
}
