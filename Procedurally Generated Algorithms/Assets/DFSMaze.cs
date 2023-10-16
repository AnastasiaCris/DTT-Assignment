using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DFSMaze : MonoBehaviour
{
    [SerializeField] private UIManager uiMan;
    
    //Grid
    public static int Width = 5;
    public static int Height = 5;
    private float startX, startY ;
    
    //Cell properties
    [SerializeField] private GameObject cellObject;
    private List<Cell> allCells = new List<Cell>();
    private Stack<Cell> cellStack = new Stack<Cell>();
    
    //Generation
    [SerializeField] private float secUntilNextCell;
    private int cellsVisited;
    private Cell currentCell;
    private Cell[,] gridArray;
    public static bool instantGen;

    
    /// <summary>
    /// Instantiate all cells in the grid
    /// Setup camera after
    /// </summary>
    private void InstantiateAllCells()
    {
        //create the grid
        gridArray = new Cell[Width, Height];

        startX = -Width/2;
        startY = -Height/2;
        
        // Instantiate all cells and set their gridX and gridY properties
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                GameObject cellClone = Instantiate(cellObject, new Vector3(startX + x, startY + y), Quaternion.identity, transform);
                Cell cell = cellClone.GetComponent<Cell>();
                cell.gridX = x;
                cell.gridY = y;
                gridArray[x, y] = cell;

                allCells.Add(cell);
            }
        }

        foreach (Cell stackItem in allCells)
        {
            stackItem.CalculateAdjacentCells(gridArray);
        }
        
        uiMan.SetUpCamera();
    }
    
    /// select a random cell to start from - and make it visited
    /// if current cell has adjacent cells that are not visited => choose one at random (break the wall between)
    /// else if all adjacent cells have been visited => go back to the previous cell
    /// do this until all cells have been visited
    private void Generate()
    {
        //from the list of cells select a random one
        int randomFirstCell = Random.Range(0, allCells.Count);

        currentCell = allCells[randomFirstCell];
        currentCell.visited = true;
        cellStack.Push(currentCell);
        
        //debug name
        currentCell.gameObject.name = "Cell " + cellsVisited;
        
        cellsVisited++;

        if (instantGen) //A choice to allow for slow generation (for better visualization)
        {
            while (cellsVisited < Width * Height) //while there are less cells visited then the total number of cells
            {
                GenCode();
            }
        }
        else
        {
            StartCoroutine(Generation());
        }
        
    }

    private void GenCode()
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
            currentCell.CellVisualization.color = currentCell.goingThroughCol;
        }
    }

    //A coroutine to visualize the generation slower
    private IEnumerator Generation()
    {
        WaitForSecondsRealtime waitSec = new WaitForSecondsRealtime(secUntilNextCell);
        
        while (cellsVisited < Width * Height) //while there are less cells visited then the total number of cells
        {
            GenCode();
            yield return waitSec;
        }
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
        Generate();
    }

}


