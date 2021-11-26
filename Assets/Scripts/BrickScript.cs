using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BrickScript : NetworkBehaviour
{
    BrickManager brickManager;

    int brickPoints = 100;

    [SyncVar (hook = "SetVisible")] bool isActive = true;

    private void Start()
    {
        SetBrickManager(GameObject.FindGameObjectWithTag("BrickManager").GetComponent<BrickManager>());
        //spriteRenderer.color = brickColour;
        gameObject.SetActive(isActive);
    }

    public void SetBrickManager(BrickManager _brickManager)
    {
        brickManager = _brickManager;
    }

    public void CollideWithBall()
    {
        brickManager.BrickDestroyed(brickPoints);
        SetVisible(isActive, false);
    }

    public void SetVisible(bool _old, bool _new)
    {
        isActive = _new;
        gameObject.SetActive(isActive);
    }
}
