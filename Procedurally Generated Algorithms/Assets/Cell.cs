using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Cell : MonoBehaviour
{
    public bool visited;
    
    //Walls
    public GameObject up;
    public GameObject down;
    public GameObject left;
    public GameObject right;
    
    //Grid
    public int gridX;
    public int gridY;
    public List<Cell> adjacentCells;
    
    //Visualization
    public Image CellIMG;
    [SerializeField]private Color unvisitedCol;
    [SerializeField]private Color visitedCol;
    public Color goingThroughCol;

    private void Start()
    {
        StartCoroutine(ShowVisited());
    }

    IEnumerator ShowVisited()
    {
        CellIMG.color = unvisitedCol;
        yield return new WaitUntil(() => visited);
        CellIMG.color = visitedCol;
    }
    
    /// Check if a cell's position is within the bounds of the grid
    private bool IsCellWithinGridBounds(int x, int y, Cell[,] grid)
    {
        return x >= 0 && y >= 0 && x < grid.GetLength(0) && y < grid.GetLength(1);
    }

    /// Calculate adjacent cells for this cell within the grid and add them to the adjacentCells list
    public void CalculateAdjacentCells(Cell[,] grid)
    {
        adjacentCells = new List<Cell>();

        // Check and add the top cell
        if (IsCellWithinGridBounds(gridX, gridY + 1, grid))
        {
            adjacentCells.Add(grid[gridX, gridY + 1]);
        }

        // Check and add the bottom cell 
        if (IsCellWithinGridBounds(gridX, gridY - 1, grid))
        {
            adjacentCells.Add(grid[gridX, gridY - 1]);
        }

        // Check and add the left cell
        if (IsCellWithinGridBounds(gridX - 1, gridY, grid))
        {
            adjacentCells.Add(grid[gridX - 1, gridY]);
        }

        // Check and add the right cell
        if (IsCellWithinGridBounds(gridX + 1, gridY, grid))
        {
            adjacentCells.Add(grid[gridX + 1, gridY]);
        }
    }

    /// <summary>
    /// Chooses a random unvisitedCell from the adjacent cells
    /// </summary>
    /// <returns></returns>
    public Cell ChooseRandomUnvisitedCell()
    {
        List<Cell> unvisistedCells = new List<Cell>();

        foreach (Cell cell in adjacentCells)
        {
            if (!cell.visited)
            {
                unvisistedCells.Add(cell);
            }
        }
        
        int randomNr = Random.Range(0,unvisistedCells.Count);
        return unvisistedCells[randomNr];
    }

    /// <summary>
    /// Destroy the wall between the current cell and the neighbouring cell
    /// </summary>
    public void DestroyWalls(Cell neighbouringCell)
    {
        // Calculate the relative positions of the two cells
        int dx = neighbouringCell.gridX - gridX;
        int dy = neighbouringCell.gridY - gridY;

        // Check if the neighboring cell is to the right
        if (dx == 1)
        {
            Destroy(right);
            Destroy(neighbouringCell.left);
        }
        // Check if the neighboring cell is to the left
        else if (dx == -1)
        {
            Destroy(left);
            Destroy(neighbouringCell.right);
        }
        // Check if the neighboring cell is above
        else if (dy == 1)
        {
            Destroy(up);
            Destroy(neighbouringCell.down);
        }
        // Check if the neighboring cell is below
        else if (dy == -1)
        {
            Destroy(down);
            Destroy(neighbouringCell.up);
        }
    }

}
