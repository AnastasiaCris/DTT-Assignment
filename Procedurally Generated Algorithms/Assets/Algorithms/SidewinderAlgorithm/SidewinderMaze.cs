using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidewinderMaze : MonoBehaviour
{
    [SerializeField] private UIManager uiMan;

   //Grid
   public static int Width = 5;
   public static int Height = 5;
   private float startX, startY; 
   private SidewinderCell[,] gridArray;
   
   //Generation
   [SerializeField] private GameObject cellObject;
   [SerializeField] private float secUntilNextCell;
   private SidewinderCell currentCell;
   private List<SidewinderCell> allCells = new List<SidewinderCell>();
   private List<SidewinderCell> unvisitedCells = new List<SidewinderCell>();
   private List<SidewinderCell> cellCarvingPath = new List<SidewinderCell>();

   //--------------------------------Generation---------------------------------------------

   
   /// <summary>
   /// Instantiate all cells in the grid
   /// Setup camera after
   /// </summary>
   private void InstantiateAllCells()
   {
      //create the grid
      gridArray = new SidewinderCell[Width, Height];

      startX = -Width/2;
      startY = -Height/2;
        
      // Instantiate all cells and set their gridX and gridY properties
      for (int x = 0; x < Width; x++)
      {
         for (int y = 0; y < Height; y++)
         {
            GameObject cellClone = Instantiate(cellObject, new Vector3(startX + x, startY + y), Quaternion.identity, transform);
            SidewinderCell sidewinderCell = cellClone.GetComponent<SidewinderCell>();
            sidewinderCell.gridX = x;
            sidewinderCell.gridY = y;
            gridArray[x, y] = sidewinderCell;
            
            allCells.Add(sidewinderCell);
            unvisitedCells.Add(sidewinderCell);
         }
      }

      foreach (SidewinderCell stackItem in allCells)
      {
         stackItem.CalculateAdjacentCells(gridArray);
      }
        
      uiMan.SetUpCamera();
   }
   
   /// <summary>
   /// 1. First row is a single passage
   /// 2. Second row start with the current cell choosing to go right or up and add the cell to the cellCarvingPath list
   /// 3. if it chooses to go right destroy the wall in-between and add the cell to the cellCarvingPath list
   /// 4. if it chooses to go up make the next cell the current cell > if it was the furthest cell east go to a new row
   ///      a. choose one of the cells in the list to make a passage up
   /// 5. do this until there are no unvisited cells
   /// </summary>
   private IEnumerator Generation() //Made it a coroutine for visualization purposes
   {
      WaitForSeconds waitSec = new WaitForSeconds(secUntilNextCell);
      int x = 0; //value to keep track in which column the current cell is
      
      //1. make a passage in the first row
      for (int i = 0; i < Width; i++) 
      {
         currentCell = gridArray[i, Height - 1];
         currentCell.gameObject.name = "First Row " + i;
         unvisitedCells.Remove(currentCell);
         
         //visualization
         currentCell.CellVisualization.color = currentCell.visitedCol;
         
         if(i < Width - 1)
            currentCell.DestroyWalls(Vector2.right, gridArray);
         
         yield return waitSec;
      }

      while (unvisitedCells.Count > 0) //until all cells have been visited
      {
         for (int h = 2; h <= Height; h++) //whenever a new row starts
         {
            currentCell = gridArray[0, Height - h];
            cellCarvingPath.Add(currentCell);
            unvisitedCells.Remove(currentCell);
            
            x = currentCell.gridX;
            //visualization
            currentCell.CellVisualization.color = currentCell.makingPathCol;
            
            yield return waitSec;
            
            while (x < Width) //do this until a row is finished
            {
               //2. choose the next 'direction' of the current cell
               SidewinderCell nextDirCell = currentCell.ChooseRandomNeighbouringCell();
               
               //visualization
               nextDirCell.CellVisualization.color = nextDirCell.nextCellCol;
               
               yield return waitSec;

               if (currentCell.DirectionBetweenCells(nextDirCell) == Vector2.right) //3. if next cell is to the right
               {
                  //add to list and make it the next current cell
                  cellCarvingPath.Add(nextDirCell);
                  currentCell = nextDirCell;
                  x = currentCell.gridX;
                  currentCell.CellVisualization.color = currentCell.makingPathCol;
               }
               else if (currentCell.DirectionBetweenCells(nextDirCell) == Vector2.up) //4. if the next cell is on top
               {
                  //visualization
                  nextDirCell.CellVisualization.color = nextDirCell.visitedCol;
                     
                  //a. carve a passage up from a random cell in the list and empty the list
                  int randomInt = Random.Range(0, cellCarvingPath.Count - 1);
                  cellCarvingPath[randomInt].DestroyWalls(Vector2.up, gridArray);
                  
                  for (int i = 0; i < cellCarvingPath.Count; i++)
                  {
                     if(i<cellCarvingPath.Count - 1)
                        cellCarvingPath[i].DestroyWalls(Vector2.right, gridArray); //destroy the walls in the list
                     cellCarvingPath[i].CellVisualization.color = cellCarvingPath[i].visitedCol;
                     unvisitedCells.Remove(cellCarvingPath[i]);
                  }
                  cellCarvingPath.Clear();

                  // make the next cell the current cell if it's not the end of the row
                  if (currentCell.gridX + 1 < Width)
                  {
                     currentCell = gridArray[currentCell.gridX + 1, Height - h];
                     cellCarvingPath.Add(currentCell);
                     x = currentCell.gridX;
                     
                     //visualization
                     currentCell.CellVisualization.color = currentCell.makingPathCol;
                  }
                  else
                  {
                     x++;
                  }
               }
            }
            yield return waitSec;
         }
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
      cellCarvingPath.Clear();

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
      }
   }
   
}
