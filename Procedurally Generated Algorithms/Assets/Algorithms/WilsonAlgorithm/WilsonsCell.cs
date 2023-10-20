using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// This cell is very familiar with the DFS cell, the only difference is that it has to store a direction, I just wanted to keep the scripts separate for separate algorithms
/// </summary>
public class WilsonsCell : MonoBehaviour
{
    //Walls
    public GameObject up;
    public GameObject down;
    public GameObject left;
    public GameObject right;
    
    //Grid
    public int gridX;
    public int gridY;
    private List<WilsonsCell> adjacentCells;
    public Vector2 direction;
    public bool visited;
    public bool currentlyInPathFinding;
    
    //Visualization
    public SpriteRenderer CellVisualization;
    public Color unvisitedCol;
    public Color visitedCol;
    public Color makingPathCol;
    public Color currentCellCol;

    /// <summary>
    /// Check if a cell's position is within the bounds of the grid
    /// </summary>
    private bool IsCellWithinGridBounds(int x, int y, WilsonsCell[,] grid)
    {
        return x >= 0 && y >= 0 && x < grid.GetLength(0) && y < grid.GetLength(1);
    }

    /// <summary>
    /// Calculate adjacent cells for this cell within the grid and add them to the adjacentCells list
    /// </summary>
    public void CalculateAdjacentCells(WilsonsCell[,] grid)
    {
        adjacentCells = new List<WilsonsCell>();

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
    /// Chooses a random cell from the adjacent cells that is not currently making a path
    /// </summary>
    public WilsonsCell ChooseRandomNeighbouringCell()
    {
        List<WilsonsCell> notMakingPath = new List<WilsonsCell>();
        foreach (WilsonsCell cell in adjacentCells)
        {
            if (!cell.currentlyInPathFinding)//if the cells are not in path finding and the cells are not surrounded by cells in path finding 
            {
                notMakingPath.Add(cell);
            }
        }
        
        if (notMakingPath.Count == 0) //if all neighbouring cells are part of the current path then choose one at random
        {
            int randomNr = Random.Range(0,adjacentCells.Count);
            return adjacentCells[randomNr];
        }
        else
        {
           int randomNr = Random.Range(0,notMakingPath.Count);
           return notMakingPath[randomNr]; 
        }
    }

    /// <summary>
    /// Sets the direction where the neighbouring cell is situated in relation to the current cell
    /// </summary>
    /// <param name="neighbouringCell"></param>
    /// <returns></returns>
    public Vector2 SetDirectionBetweenCells(WilsonsCell neighbouringCell)
    {
        Vector2 dir = new Vector2(neighbouringCell.gridX - gridX, neighbouringCell.gridY - gridY);
        direction = dir;
        return dir;
    }

    /// <summary>
    /// Destroy the wall between the current cell and the neighbouring cell according the direction the current cell has
    /// </summary>
    public void DestroyWalls(WilsonsCell[,] grid)
    {
        WilsonsCell neighbouringWilsonCell = grid[gridX + (int)direction.x, gridY + (int)direction.y];

        // Check if the neighboring cell is to the right
        if (direction == Vector2.right)
        {
            Destroy(right);
            Destroy(neighbouringWilsonCell.left);
        }
        // Check if the neighboring cell is to the left
        else if (direction == Vector2.left)
        {
            Destroy(left);
            Destroy(neighbouringWilsonCell.right);
        }
        // Check if the neighboring cell is above
        else if (direction == Vector2.up)
        {
            Destroy(up);
            Destroy(neighbouringWilsonCell.down);
        }
        // Check if the neighboring cell is below
        else if (direction == Vector2.down)
        {
            Destroy(down);
            Destroy(neighbouringWilsonCell.up);
        }
    }
}
