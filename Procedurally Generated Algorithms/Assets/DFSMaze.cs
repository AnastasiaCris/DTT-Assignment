using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class DFSMaze : MonoBehaviour
{
    //Grid
    [SerializeField] private int width = 5;
    [SerializeField] private int height = 5;
    private float startX, startY ;
    
    //Cell properties
    [SerializeField] private GameObject CellObject;
    private List<Cell> allCells = new List<Cell>();
    private Stack<Cell> cellStack = new Stack<Cell>();
    
    //Generation
    [SerializeField] private float genSpeed;
    private int cellsVisited;
    private Cell currentCell;
    private bool generating;
    
    
    /// <summary>
    /// Instantiate all cells in the grid
    /// Setup camera after
    /// </summary>
    private void InstantiateAllCells()
    {
        startX = -width/2;
        startY = -height/2;
        for (int i = 0; i < width * height; i++)
        {
            GameObject cellClone = Instantiate(CellObject, new Vector3(startX + i % width, startY + i / width), quaternion.identity, transform);
            allCells.Add(cellClone.GetComponent<Cell>());
        }

        foreach (Cell stackItem in allCells)
        {
            stackItem.AddAdjacentCell();
        }

        //Setting up the camera 
        SetUpCamera();
    }
    
    /// select a random cell to start from - and make it visited
    /// if current cell has adjacent cells that are not visited => choose one at random (break the wall between)
    /// else if all adjacent cells have been visited => go back to the previous cell
    /// do this until all cells have been visited
    private void TryGeneration()
    {
        generating = true;
        //from the list of cells select a random one
        int randomFirstCell = Random.Range(0, allCells.Count);

        currentCell = allCells[randomFirstCell];
        currentCell.visited = true;
        cellStack.Push(currentCell);
        
        //debug name
        currentCell.gameObject.name = "Cell " + cellsVisited;
        
        cellsVisited++;

        StartCoroutine(Generation());
    }

    private IEnumerator Generation()
    {
        while (cellsVisited < width * height) //while there are less cells visited then the total number of cells
        {
            if (HasUnvisitedNeighbourCells(currentCell))
            {

                Cell nextCell = currentCell.ChooseRandomUnvisitedCell();
                cellStack.Push(nextCell);

                //get rid of the walls in between the current and next cell
                currentCell.DestroyWalls(nextCell);

                currentCell = nextCell;
                currentCell.visited = true;

                //debug name
                currentCell.gameObject.name = "Cell " + cellsVisited;

                cellsVisited++;
            }
            else // if all have been visited go to the previous cell
            {
                currentCell = cellStack.Pop();
                currentCell.CellIMG.color = currentCell.goingThroughCol;
            }

            yield return new WaitForSeconds(genSpeed);
        }

        generating = false;
    }

    /// <summary>
    /// Check if the current cell has any unvisited cells
    /// </summary>
    bool HasUnvisitedNeighbourCells(Cell currCell)
    {
        for (int i = 0; i < currCell.adjacentCells.Count; i++)
        {
            if (!currCell.adjacentCells[i].visited)
                return true;
        }

        return false;
    }
    
    //---------------------------------------------UI-----------------------------------------------------------------

    /// <summary>
    /// Setting up camera position and size
    /// </summary>
    public void SetUpCamera()
    {
        Camera cam = Camera.main;
        cam.transform.position = new Vector3(transform.position.x, transform.position.y, -10); //set position
        
        int biggestNr = height > width ? height : width; //get the biggest nr between height and width
        float camSize = 2+ biggestNr / 2;
        if (camSize <= 5.5f) camSize = 5.5f; // size can't be smaller then 5.5
        
        cam.orthographicSize = camSize; //set the size
    }
    /// <summary>
    /// Start a new DFS maze generation
    /// </summary>
    public void Regenerate()
    {

        for (int i = 0; i < allCells.Count; i++)
        {
            Destroy(allCells[i].gameObject);
        }

        allCells.Clear();
        cellStack.Clear();
        cellsVisited = 0;

        InstantiateAllCells();
        TryGeneration();
        
    }
    
    /// <summary>
    /// Change Width
    /// </summary>
    public void OnWidthValueChanged(TMP_InputField textField)
    {
        int parseW = 0;
        if (int.TryParse(textField.text, out parseW))
        {
            parseW = Int32.Parse(textField.text);
        }
        if (parseW <= 2 && textField.text.Length > 0) //width can't be smaller then 2
        {
            parseW = 2;
            textField.text = parseW.ToString();
        }
        if (parseW >= 250) //width can't be bigger then 250
        {
            parseW = 250;
            textField.text = parseW.ToString();
        }

        width = parseW;
    }
    
    /// <summary>
    /// Change Height
    /// </summary>
    public void OnHeightValueChanged(TMP_InputField textField)
    {
        int parseH = 0;
        if (int.TryParse(textField.text, out parseH))
        {
            parseH = Int32.Parse(textField.text);
        }
        if (parseH <= 2 && textField.text.Length > 0) //height can't be smaller then 2
        {
            parseH = 2;
            textField.text = parseH.ToString();
        }
        if (parseH >= 250) //height can't be bigger then 250
        {
            parseH = 250;
            textField.text = parseH.ToString();
        }

        height = parseH;
    }

}


