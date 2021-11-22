using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    [SerializeField] float ballSpeed;
    [SerializeField] float startRotateAngle;
    Rigidbody rgBody;

    bool canMove = false;

    private void Start()
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

    public void RotateBall(Vector3 _colliderPos)
    {
        //rotate ball in opposite direction
        transform.up = -_colliderPos + transform.position;
    }

    public void StartGame()
    {
        canMove = true;
        transform.parent = null;
        transform.localEulerAngles = new Vector3(0, 0, Random.Range(-startRotateAngle, startRotateAngle));
        //transform.localScale = new Vector3(1, 1, 1);
        rgBody.isKinematic = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if ball hits boundary
        if(collision.gameObject.CompareTag("Boundary"))
        {
            //bounce ball off wall
            RotateBall(collision.contacts[0].point);
        }
        //if ball hits brick
        if(collision.gameObject.CompareTag("Brick"))
        {
            //bounce ball off brick
            RotateBall(collision.contacts[0].point);
            collision.gameObject.GetComponent<BrickScript>().CollideWithBall();
        }
    }
}
