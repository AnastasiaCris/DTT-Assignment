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
    private float startX, startY;
    private DFSCell[,] gridArray;

    //Generation
    [SerializeField] private GameObject cellObject;
    private float secUntilNextCell = 0;
    private int cellsVisited;
    private DFSCell currentDfsCell;
    private List<DFSCell> allCells = new List<DFSCell>();
    private Stack<DFSCell> cellStack = new Stack<DFSCell>();

    //--------------------------------Generation---------------------------------------------

    /// <summary>
    /// Instantiate all cells in the grid
    /// Setup camera after
    /// </summary>
    private void InstantiateAllCells()
    {
        //create the grid
        gridArray = new DFSCell[Width, Height];

        startX = -Width/2;
        startY = -Height/2;
        
        // Instantiate all cells and set their gridX and gridY properties
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                GameObject cellClone = Instantiate(cellObject, new Vector3(startX + x, startY + y), Quaternion.identity, transform);
                DFSCell dfsCell = cellClone.GetComponent<DFSCell>();
                dfsCell.gridX = x;
                dfsCell.gridY = y;
                gridArray[x, y] = dfsCell;

                allCells.Add(dfsCell);
            }
        }

        foreach (DFSCell stackItem in allCells)
        {
            stackItem.CalculateAdjacentCells(gridArray);
        }
        
        uiMan.SetUpCamera();
    }

    /// 1. select a random cell to start from - and make it visited
    ///   a. if current cell has adjacent cells that are not visited => choose one at random (break the wall between)
    ///   b. else if all adjacent cells have been visited => go back to the previous cell
    /// 2. do this until all cells have been visited
    private IEnumerator Generate() //Made it a coroutine for visualization purposes
    {
        WaitForSeconds waitSec = new WaitForSeconds(secUntilNextCell);

        //from the list of cells select a random one
        int randomFirstCell = Random.Range(0, allCells.Count);

        currentDfsCell = allCells[randomFirstCell];
        currentDfsCell.visited = true;
        cellStack.Push(currentDfsCell);

        cellsVisited++;

        yield return waitSec;
        while (cellsVisited < Width * Height || cellStack.Count > 0) //while there are still unvisited cells and the stack is empty
        {
            if (HasUnvisitedNeighbourCells(currentDfsCell))
            {
                DFSCell nextDfsCell = currentDfsCell.ChooseRandomUnvisitedCell();
                cellStack.Push(nextDfsCell);

                //get rid of the walls in between the current and next cell
                currentDfsCell.DestroyWalls(nextDfsCell);
                currentDfsCell = nextDfsCell;
                currentDfsCell.visited = true;

                cellsVisited++;
            }
            else // if all neighbouring cells have been visited go to the previous cell
            {
                currentDfsCell = cellStack.Pop();
                currentDfsCell.CellVisualization.color = currentDfsCell.goingThroughCol;
            }
            
            yield return waitSec;
        }
    }

    /// <summary>
    /// Check if the current cell has any unvisited cells
    /// </summary>
    bool HasUnvisitedNeighbourCells(DFSCell currDfsCell)
    {
        for (int i = 0; i < currDfsCell.adjacentCells.Count; i++)
        {
            if (!currDfsCell.adjacentCells[i].visited)
                return true;
        }

        return false;
    }
    
    //--------------------------------Regeneration---------------------------------------------

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
        StartCoroutine(Generate());
    }

    /// <summary>
    /// Destroys and resets the mazes properties
    /// </summary>
    public void DestroyMaze()
    {
        if (allCells.Count > 0)
        {
            for (int i = 0; i < allCells.Count; i++)
            {
                Destroy(allCells[i].gameObject);
            }

            allCells.Clear();
            cellStack.Clear();
            cellsVisited = 0;
        }
    }

}


