using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BrickManager : NetworkBehaviour
{
    int level = 0;

    [Header("Script References")]
    [SerializeField] UIManager uiManager;
    [SerializeField] GameObject brickPrefab;

    [Header("Variables")]
    [SerializeField] float brickSpawnWait;
    [SerializeField] Vector2 startingPos;
    [SerializeField] GameObject[] brickLayout;

    [Header("Custom Brick Layout")]
    [Tooltip("if true then brick manager uses prefab brick layouts instead")]
    [SerializeField] bool isBrickPrefab;

    [SerializeField] int bricksPerRow;
    [SerializeField] int amountOfRows;

    [SerializeField] float rowVertSpacing;
    [SerializeField] float brickHorizSpacing;

    private void Start()
    {
        StartCoroutine(GenerateBricks(0));
    }

    //Spawn in bricks
    private IEnumerator GenerateBricks(float _waitTime)
    {
        yield return new WaitForSeconds(_waitTime);

        //if using prefab
        if (isBrickPrefab)
        {
            Instantiate(brickLayout[level], new Vector3(0, 0, 0), Quaternion.identity);
        }
        else //if using auto generator
        {
            //loop through rows and columns of bricks
            for (int i = 0; i < amountOfRows; i++)
            {
                for (int j = 0; j < bricksPerRow; j++)
                {
                    //generate bricks
                    GameObject brickObj = Instantiate(brickPrefab, new Vector3(j * brickHorizSpacing + startingPos.x, i * rowVertSpacing + startingPos.y, 0), Quaternion.identity);
                    brickObj.transform.SetParent(transform);
                }
            }
        }
    }

    //check if all bricks in scene have been destroyed
    void CheckIfAllBricksDestroyed()
    {
        //check if there aren't any bricks left
        if(GameObject.FindGameObjectsWithTag("Brick").Length <= 1)
        {
            //spawn in a fresh set of bricks
            StartCoroutine(GenerateBricks(brickSpawnWait));
        }
    }

    //When brick object is destroyed
    public void BrickDestroyed(int _points)
    {
        CheckIfAllBricksDestroyed();
        uiManager.UpdateScore(_points);
    }
}
