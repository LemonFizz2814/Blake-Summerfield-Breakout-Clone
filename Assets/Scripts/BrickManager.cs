using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BrickManager : NetworkManager
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

    private IEnumerator GenerateBricks(float _waitTime)
    {
        yield return new WaitForSeconds(_waitTime);

        if (isBrickPrefab)
        {
            Instantiate(brickLayout[level], new Vector3(0, 0, 0), Quaternion.identity);
        }
        else
        {
            for (int i = 0; i < amountOfRows; i++)
            {
                for (int j = 0; j < bricksPerRow; j++)
                {
                    GameObject brickObj = Instantiate(brickPrefab, new Vector3(j * brickHorizSpacing + startingPos.x, i * rowVertSpacing + startingPos.y, 0), Quaternion.identity);
                    brickObj.transform.SetParent(transform);
                }
            }
        }
    }

    void CheckIfAllBricksDestroyed()
    {
        //check if there aren't any bricks left
        if(GameObject.FindGameObjectsWithTag("Brick").Length <= 1)
        {
            print("length " + GameObject.FindGameObjectsWithTag("Brick").Length);
            //spawn in a fresh set of bricks
            StartCoroutine(GenerateBricks(brickSpawnWait));
        }
    }

    public void BrickDestroyed(int _points)
    {
        CheckIfAllBricksDestroyed();
        uiManager.UpdateScore(_points);
    }
}
