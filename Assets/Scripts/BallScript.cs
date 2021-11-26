using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BallScript : NetworkBehaviour
{
    [SerializeField] float ballSpeed;
    [SerializeField] float startRotateAngle;

    [SyncVar] float startRot;
    Rigidbody rgBody;
    [SyncVar (hook = "SetPlayerScript")] PlayerScript playerScript;

    bool canMove = false;


    private void Awake()
    {
        rgBody = GetComponent<Rigidbody>();
        rgBody.isKinematic = true;
    }

    private void Start()
    {
        /*foreach(GameObject playerObj in GameObject.FindGameObjectsWithTag("Player"))
        {
            if(playerObj.GetComponent<PlayerScript>().GetPlayerNum() == ballNum)
            {
                print(playerObj.GetComponent<PlayerScript>().GetPlayerNum() + " with " + ballNum);
                SetPlayerScript(playerObj.GetComponent<PlayerScript>());
            }
        }*/

        if (playerScript == null)
        {
            print("playerScript is null-------------------");
        }
        else
        {
            print("playerscript populated");
            SetPlayersBall();
        }
    }


    void Update()
    {
        if (canMove)
        {
            rgBody.velocity = transform.up * ballSpeed;
        }

        if(isServer && !canMove && playerScript != null)
        {
            transform.position = playerScript.transform.GetChild(0).position;
        }
    }

    void SetPlayersBall()
    {
        playerScript.SetBall(gameObject, gameObject);
    }

    //when ball bounces of paddle
    public void BounceOffPaddle(Vector3 _colliderPos)
    {
        //ball bounces in opposite direction to the centre of paddle so player can aim ball
        transform.up = -_colliderPos + transform.position;
    }

    //when ball bounces off boundary or brick or other objects
    public void BounceOffObject(ContactPoint _contact)
    {
        //reflect at an appropriate angle 
        transform.up = Vector3.Reflect(transform.up, _contact.normal);
    }

    //when play presses space
    public void StartGame()
    {
        print("start game");
        FreezeBall(false);
        transform.SetParent(null);
        SetStartAngle();
        //pick a random angle on start
        transform.localEulerAngles = new Vector3(0, 0, startRot);
    }

    void SetStartAngle()
    {
        if (isServer) return;
        startRot = Random.Range(-startRotateAngle, startRotateAngle);
    }

    public void FreezeBall(bool _active)
    {
        canMove = !_active;
        rgBody.isKinematic = _active;
    }

    public void SetPlayerScript(PlayerScript _oldPlayerScript, PlayerScript _playerScript)
    {
        print("set playerscript");
        playerScript = _playerScript;
    }

    private void OnTriggerEnter(Collider other)
    {
        //ball exited into red zone
        if(other.gameObject.CompareTag("OutOfBounds"))
        {
            print("out of bounds!!!!");
            playerScript.RespawnBallCmd();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if ball hits object to bounce off
        if(collision.gameObject.CompareTag("Boundary") || collision.gameObject.CompareTag("Ball"))
        {
            //bounce ball off object
            BounceOffObject(collision.contacts[0]);
        }
        //if ball hits brick
        if(collision.gameObject.CompareTag("Brick"))
        {
            //bounce ball off brick and destroy it
            BounceOffObject(collision.contacts[0]);
            collision.gameObject.GetComponent<BrickScript>().CollideWithBall();
        }
    }
}