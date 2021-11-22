using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickManager : MonoBehaviour
{
    [SerializeField] UIManager uiManager;
    [SerializeField] GameObject brickPrefab;

    [SerializeField] int bricksPerRow;
    [SerializeField] int amountOfRows;
    [SerializeField] float rowVertSpacing;
    [SerializeField] float brickHorizSpacing;
    [SerializeField] Vector2 startingPos;

    [SerializeField] List<Color32> colourList = new List<Color32>();

    private void Start()
    {
        //error checking
        if(colourList.Count != amountOfRows)
        {
            Debug.LogError("Warning! colour list length isn't equal to the amount of rows");
        }

        for(int i = 0; i < amountOfRows; i++)
        {
            for (int j = 0; j < bricksPerRow; j++)
            {
                GameObject brickObj = Instantiate(brickPrefab, new Vector3(j * brickHorizSpacing + startingPos.x, i * rowVertSpacing + startingPos.y, 0), Quaternion.identity);
                brickObj.transform.SetParent(transform);
                brickObj.GetComponent<BrickScript>().SetBrickColour(colourList[i]);
            }
        }
    }
}
