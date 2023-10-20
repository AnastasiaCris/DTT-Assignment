using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidewinderCell : MonoBehaviour
{
    //Walls
    public GameObject up;
    public GameObject down;
    public GameObject left;
    public GameObject right;
    
    //Grid
    public int gridX;
    public int gridY;
    private List<SidewinderCell> adjacentCells;
    
    //Visualization
    public SpriteRenderer CellVisualization;
    public Color visitedCol;
    public Color makingPathCol;
    public Color nextCellCol;

    /// <summary>
    /// Check if a cell's position is within the bounds of the grid
    /// </summary>
    private bool IsCellWithinGridBounds(int x, int y, SidewinderCell[,] grid)
    {
        return x >= 0 && y >= 0 && x < grid.GetLength(0) && y < grid.GetLength(1);
    }

    /// <summary>
    /// Calculate adjacent cells ONLY on the top and right side of the cell
    /// </summary>
    public void CalculateAdjacentCells(SidewinderCell[,] grid)
    {
        adjacentCells = new List<SidewinderCell>();

        // Check and add the top cell
        if (IsCellWithinGridBounds(gridX, gridY + 1, grid))
        {
            adjacentCells.Add(grid[gridX, gridY + 1]);
        }

        // Check and add the right cell
        if (IsCellWithinGridBounds(gridX + 1, gridY, grid))
        {
            adjacentCells.Add(grid[gridX + 1, gridY]);
        }
    }

    /// <summary>
    /// Chooses a random cell from the adjacent cells 
    /// </summary>
    public SidewinderCell ChooseRandomNeighbouringCell()
    {
        int randomNr = Random.Range(0,adjacentCells.Count);
        return adjacentCells[randomNr];
    }
    
    /// <summary>
    /// Gets the direction where the neighbouring cell is situated in relation to the current cell
    /// </summary>
    /// <param name="neighbouringCell"></param>
    /// <returns></returns>
    public Vector2 DirectionBetweenCells(SidewinderCell neighbouringCell)
    {
        Vector2 dir = new Vector2(neighbouringCell.gridX - gridX, neighbouringCell.gridY - gridY);
        return dir;
    }

    /// <summary>
    /// Destroy the wall between the current and neighbouring cell based on the direction
    /// </summary>
    public void DestroyWalls(Vector2 dir, SidewinderCell[,] grid)
    {
        SidewinderCell neighbouringCell = grid[gridX + (int)dir.x, gridY + (int)dir.y];
        
        //The algorithm checks only for cells to the right or on top, so do that
        
        // Check if the neighboring cell is to the right
        if (dir == Vector2.right)
        {
            Destroy(right);
            Destroy(neighbouringCell.left);
        }
        // Check if the neighboring cell is on top
        else if (dir == Vector2.up)
        {
            Destroy(up);
            Destroy(neighbouringCell.down);
        }
    }
}
