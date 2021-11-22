using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickScript : MonoBehaviour
{
    BrickManager brickManager;
    SpriteRenderer spriteRenderer;
    Color32 brickColour;

    int brickPoints = 100;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = brickColour;
    }

    public void SetBrickManager(BrickManager _brickManager)
    {
        brickManager = _brickManager;
    }

    public void SetBrickColour(Color32 _color)
    {
        brickColour = _color;

        if(spriteRenderer != null)
        {
            spriteRenderer.color = brickColour;
        }
    }

    public void CollideWithBall()
    {
        brickManager.BrickDestroyed(brickPoints);
        Destroy(gameObject);
    }
}
