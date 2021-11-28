using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BallScript : NetworkBehaviour
{
    [SerializeField] float ballSpeed;
    [Tooltip("rotation of ball when it first is fired")]
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
        if (playerScript != null)
        {
            SetPlayersBall();
        }
    }

    void Update()
    {
        //move ball forward direction
        if (canMove)
        {
            rgBody.velocity = transform.up * ballSpeed;
        }
        else
        {
            //if is server then update position
            if (isServer && playerScript != null)
            {
                transform.position = playerScript.GetBallSpawnPos().transform.position;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //ball exited into red zone
        if (other.gameObject.CompareTag("OutOfBounds"))
        {
            RespawnPosition();
            //playerScript.RespawnBallCmd();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if ball hits object to bounce off
        if (collision.gameObject.CompareTag("Boundary") || collision.gameObject.CompareTag("Ball"))
        {
            //bounce ball off object
            BounceOffObject(collision.contacts[0]);
        }
        //if ball hits brick
        if (collision.gameObject.CompareTag("Brick"))
        {
            //bounce ball off brick and destroy it
            BounceOffObject(collision.contacts[0]);
            collision.gameObject.GetComponent<BrickScript>().CollideWithBall();
        }
    }

    void RespawnPosition()
    {
        FreezeBall(true);
        transform.localPosition = playerScript.GetBallSpawnPos().transform.localPosition;
        playerScript.SetCanStartGame(true, true);
    }

    //set player scripts reference to this script
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
        FreezeBall(false);
        transform.SetParent(null);
        //pick a random angle on start
        SetStartAngle();
        transform.localEulerAngles = new Vector3(0, 0, startRot);
    }

    //set starting angle of ball when ball is fired
    public void SetStartAngle()
    {
        startRot = Random.Range(-startRotateAngle, startRotateAngle);
    }

    //prevent ball from moving
    public void FreezeBall(bool _active)
    {
        canMove = !_active;
        rgBody.isKinematic = _active;
    }

    //set reference to player script
    public void SetPlayerScript(PlayerScript _oldPlayerScript, PlayerScript _playerScript)
    {
        playerScript = _playerScript;
    }
}