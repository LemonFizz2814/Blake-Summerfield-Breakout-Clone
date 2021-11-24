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
    [SyncVar] PlayerScript playerScript;

    bool canMove = false;

    [SyncVar] int ballNum;

    private void Awake()
    {
        rgBody = GetComponent<Rigidbody>();
        rgBody.isKinematic = true;
    }

    private void Start()
    {
        ballNum = System.Array.IndexOf(GameObject.FindGameObjectsWithTag("Ball"), gameObject);

        /*foreach(GameObject playerObj in GameObject.FindGameObjectsWithTag("Player"))
        {
            if(playerObj.GetComponent<PlayerScript>().GetPlayerNum() == ballNum)
            {
                print(playerObj.GetComponent<PlayerScript>().GetPlayerNum() + " with " + ballNum);
                SetPlayerScript(playerObj.GetComponent<PlayerScript>());
            }
        }*/

        if(playerScript == null)
        {
            print("COMMMMMEEEE ONNNNNNNNNNNNNN");
        }
    }

    void Update()
    {
        if (canMove)
        {
            //rgBody.velocity = transform.up * ballSpeed;
        }

        if(!isServer && !canMove)
        {
            //transform.position = playerScript.transform.position;
        }
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
        FreezeBall(false);
        transform.SetParent(null);
        SetStartAngle();
        //pick a random angle on start
        transform.localEulerAngles = new Vector3(0, 0, startRot);
    }

    void SetStartAngle()
    {
        if (isServer) return;
        startRot = 0;// Random.Range(-startRotateAngle, startRotateAngle);
    }

    public void FreezeBall(bool _active)
    {
        canMove = !_active;
        rgBody.isKinematic = _active;
    }

    [ClientRpc]
    public void SetPlayerScript(PlayerScript _playerScript)
    {
        print("command");
        playerScript = _playerScript;
        playerScript.SetBall(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        //ball exited into red zone
        if(other.gameObject.CompareTag("OutOfBounds"))
        {
            playerScript.RespawnBallCmd();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if ball hits boundary
        if(collision.gameObject.CompareTag("Boundary"))
        {
            //bounce ball off wall
            BounceOffObject(collision.contacts[0]);
        }
        //if ball hits brick
        if(collision.gameObject.CompareTag("Brick"))
        {
            //bounce ball off brick
            BounceOffObject(collision.contacts[0]);
            collision.gameObject.GetComponent<BrickScript>().CollideWithBall();
        }
    }
}