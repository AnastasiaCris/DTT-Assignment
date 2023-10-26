using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WilsonMaze : MonoBehaviour
{
   [SerializeField] private UIManager uiMan;

   //Grid
   public static int Width = 5;
   public static int Height = 5;
   private float startX, startY; 
   private WilsonsCell[,] gridArray;
   
   //Generation
   [SerializeField] private GameObject cellObject;
   private float secUntilNextCell = 0;
   private WilsonsCell currentCell;
   private List<WilsonsCell> allCells = new List<WilsonsCell>();
   private List<WilsonsCell> unvisitedCells = new List<WilsonsCell>();
   private List<WilsonsCell> cellPath = new List<WilsonsCell>();

   //--------------------------------Generation---------------------------------------------

   /// <summary>
   /// Instantiate all cells in the grid
   /// Setup camera after
   /// </summary>
   private void InstantiateAllCells()
   {
      //create the grid
      gridArray = new WilsonsCell[Width, Height];

      startX = -Width/2;
      startY = -Height/2;
        
      // Instantiate all cells and set their gridX and gridY properties
      for (int x = 0; x < Width; x++)
      {
         for (int y = 0; y < Height; y++)
         {
            GameObject cellClone = Instantiate(cellObject, new Vector3(startX + x, startY + y), Quaternion.identity, transform);
            WilsonsCell wilsonCell = cellClone.GetComponent<WilsonsCell>();
            wilsonCell.gridX = x;
            wilsonCell.gridY = y;
            gridArray[x, y] = wilsonCell;
            
            allCells.Add(wilsonCell);
            unvisitedCells.Add(wilsonCell);
         }
      }

      foreach (WilsonsCell stackItem in allCells)
      {
         stackItem.CalculateAdjacentCells(gridArray);
      }
        
      uiMan.SetUpCamera(Width, Height);
   }
   
   /// <summary>
   /// 1. Choose a random cell and add it to the visited list
   /// 2. Choose another random cell, which is now the current cell
   /// 3. Choose a random cell that is adjacent to the current cell and save the direction traveled in the last cell.
   /// This is your new current cell. -> if the new random cell is part of the path => delete all cells until the current cell and continue the path
   /// 4. If the current cell is not in the visited cells list -> Go to 3
   /// 5. Else -> Starting from the first cell in 2. remove all walls between the cells to complete the path and make the cells visited
   /// 6. If all cells have not been visited -> Go to 2
   /// </summary>
   private IEnumerator Generation() //Made it a coroutine for visualization purposes
   {
      WaitForSecondsRealtime waitSec = new WaitForSecondsRealtime(secUntilNextCell);
      
      // SELECT one random cell and mark it as visited
      int randomInt = Random.Range(0, unvisitedCells.Count);
      
      unvisitedCells[randomInt].visited = true; 
      
      //visualization
      unvisitedCells[randomInt].CellVisualization.color = unvisitedCells[randomInt].visitedCol;
      
      unvisitedCells.Remove(unvisitedCells[randomInt]);

      yield return waitSec; 
      
      //SELECT another random cell which is now the current cell and add it to the path finding list
      randomInt = Random.Range(0, unvisitedCells.Count);
      currentCell = unvisitedCells[randomInt]; 
      cellPath.Add(currentCell);
      
      //visualization
      currentCell.currentlyInPathFinding = true;
      currentCell.CellVisualization.color = currentCell.makingPathCol;
      currentCell.CellVisualization.color = currentCell.currentCellCol;

      yield return waitSec;

      while (unvisitedCells.Count > 0) //while there's still unvisited cells
      {
         if (!currentCell.visited) //if current cell is not visited
         {
            //SELECT random cell adjacent to current cell
            WilsonsCell nextCell = currentCell.ChooseRandomNeighbouringCell(); 

            if (nextCell.currentlyInPathFinding) // if the next cell would end up looping with a cell from the path - delete the loop and start from next cell
            {
               int until = cellPath.IndexOf(nextCell);
               for (int i = cellPath.Count - 1; i > until;)
               {
                  cellPath[i].currentlyInPathFinding = false;
                  cellPath[i].CellVisualization.color = cellPath[i].unvisitedCol;
                  cellPath[i].direction = Vector2.zero;
                  cellPath.Remove(cellPath[i]);
                  i--;
               }
            }
            
            //save direction between cells
            currentCell.SetDirectionBetweenCells(nextCell); 
            
            //visualization
            if (cellPath.Contains(currentCell))
            {
               currentCell.CellVisualization.color = currentCell.makingPathCol;
            }

            //set the new current cell
            currentCell = nextCell; 

            if (!currentCell.visited) //check if the next/current cell is not visited
            {
               cellPath.Add(currentCell);

               //visualization
               currentCell.currentlyInPathFinding = true;
               currentCell.CellVisualization.color = currentCell.makingPathCol;
               currentCell.CellVisualization.color = currentCell.currentCellCol;
            }

         }
         else // If the current cell is visited -> remove all walls in the path
         {
            for (int i = 0; i < cellPath.Count; i++)
            {
               cellPath[i].DestroyWalls(gridArray);
               cellPath[i].visited = true;
               cellPath[i].currentlyInPathFinding = false;
               unvisitedCells.Remove(cellPath[i]);
               
               //Visualization
               cellPath[i].CellVisualization.color = cellPath[i].visitedCol;
            }
            cellPath.Clear();
            
            if (unvisitedCells.Count > 0) //if there's still unvisited cells -> select another random cell 
            {
               randomInt = Random.Range(0, unvisitedCells.Count);
               currentCell = unvisitedCells[randomInt];
               cellPath.Add(currentCell);

               //visualization
               currentCell.currentlyInPathFinding = true;
               currentCell.CellVisualization.color = currentCell.makingPathCol;
               currentCell.CellVisualization.color = currentCell.currentCellCol;

            }
         }
         yield return waitSec;
      }
   }
   
   //--------------------------------Regeneration---------------------------------------------


   /// <summary>
   /// Start a new Wilson maze generation
   /// </summary>
   public void Regenerate()
   {
      for (int i = 0; i < allCells.Count; i++)
      {
         Destroy(allCells[i].gameObject);
      }

      allCells.Clear();
      unvisitedCells.Clear();
      cellPath.Clear();

      InstantiateAllCells();
      StartCoroutine(Generation());
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
         unvisitedCells.Clear();
         cellPath.Clear();
      }
   }
}

