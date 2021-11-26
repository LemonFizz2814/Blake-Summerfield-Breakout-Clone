using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
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

        //print("playerNum " + playerNum);
        playerNum = GameObject.FindGameObjectsWithTag("Player").Length;
        transform.position = new Vector3(transform.position.x, transform.position.y + (playerNum * playerHeightOffset), transform.position.z);

        if (ballObject == null)
        {
            print("ballObject is null-------------------");
        }
        else
        {
            print("ballObject populated");
        }
    }

    [Command]
    void SpawnBallCmd()
    {
        print("spawn");
        GameObject _obj = Instantiate(ballPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        NetworkServer.Spawn(_obj);
        SetBall(_obj, _obj);
        ballObject.GetComponent<BallScript>().SetPlayerScript(null, this);
    }

    public void SetBall(GameObject _oldBallObject, GameObject _ballObject)
    {
        //print("set ball");
        ballObject = _ballObject;
        //RespawnBallCmd();
    }

    void SetPlayerNum(int _oldNum, int _newNum)
    {
        playerNum = _newNum;
    }

    public int GetPlayerNum()
    {
        return playerNum;
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
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
    [ClientRpc]
    public void SpacePressedClnt()
    {
        canStartGame = false;
        ballObject.GetComponent<BallScript>().StartGame();
    }
    [Command]
    public void SpacePressedCmd()
    {
        print("space command");
        SpacePressedClnt();
    }

    private void OnTriggerStay(Collider other)
    {
        //if ball collides with paddle
        if(other.CompareTag("Ball"))
        {
            other.GetComponent<BallScript>().BounceOffPaddle(gameObject.transform.position);
        }
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
