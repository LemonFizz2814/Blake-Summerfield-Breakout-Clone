using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    [SerializeField] float ballSpeed;
    [SerializeField] float startRotateAngle;
    Rigidbody rgBody;
    PlayerScript playerScript;

    bool canMove = false;

    private void Awake()
    {
        rgBody = GetComponent<Rigidbody>();
        rgBody.isKinematic = true;
    }

    void Update()
    {
        if (canMove)
        {
            rgBody.velocity = transform.up * ballSpeed;
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

    public void StartGame()
    {
        FreezeBall(false);
        transform.SetParent(null);
        transform.localEulerAngles = new Vector3(0, 0, Random.Range(-startRotateAngle, startRotateAngle));
    }

    public void FreezeBall(bool _active)
    {
        canMove = !_active;
        rgBody.isKinematic = _active;
    }

    public void SetPlayerScript(PlayerScript _playerScript)
    {
        playerScript = _playerScript;
    }

    private void OnTriggerEnter(Collider other)
    {
        //ball exited into red zone
        if(other.gameObject.CompareTag("OutOfBounds"))
        {
            playerScript.RespawnBall();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if ball hits boundary
        if(collision.gameObject.CompareTag("Boundary"))
        {
            //bounce ball off wall
            //RotateBall(collision.contacts[0].point);
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