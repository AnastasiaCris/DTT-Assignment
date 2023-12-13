using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class RecursiveDivisionMaze : MonoBehaviour
{
    
    [SerializeField] private UIManager uiMan;
    
    //gen visualization
    [SerializeField] private float secUntilNextCell = 1;
    
    //walls
    [SerializeField] private GameObject horizontalWall, verticalWall;
    private List<GameObject> wallPathing = new List<GameObject>();//a series of walls made before a passage is carved through one of them
    
    //Grid
    public static int Width = 5;
    public static int Height = 5;
    private float startX, startY;

    
    private void SetUpGrid()
    {
        startX = -(float)Width / 2;
        startY = -(float)Height / 2;
        
        //instantiate outside walls
        for (int x = 0; x < Width; x++)
        {
            Instantiate(horizontalWall, new Vector3(startX + x, startY - 0.5f), Quaternion.identity, transform); //bottom
            Instantiate(horizontalWall, new Vector3(startX + x, startY + Height - 0.5f), Quaternion.identity, transform); //top
        }

        for (int y = 0; y < Height; y++)
        {
            Instantiate(verticalWall, new Vector3(startX - 0.5f, startY + y), Quaternion.identity, transform); //left
            Instantiate(verticalWall, new Vector3(startX + Width - 0.5f, startY + y), Quaternion.identity, transform); //right
        }
        
        uiMan.SetUpCamera(Width, Height);
        
        WaitForSeconds waitSec = new WaitForSeconds(secUntilNextCell);
        StartCoroutine(GenerateMaze(0,0, Width, Height, waitSec));
    }

    /// <summary>
    ///  A recursive coroutine that creates the core logic of the Recursive Division Algorithm
    /// </summary>
    /// <param name="x"> The x-coordinate of the top-left corner of the current chamber within the maze. </param>
    /// <param name="y"> The x-coordinate of the top-left corner of the current chamber within the maze. </param>
    /// <param name="width"> The width of the current chamber within the maze. </param>
    /// <param name="height"> The width of the current chamber within the maze. </param>
    /// <param name="waitSec"> How many sec to wait in between wall generations </param>
    /// <returns></returns>
    IEnumerator GenerateMaze(int x, int y, int width, int height, WaitForSeconds waitSec)
    {
        if (width < 2 || height < 2) // if the size of the chamber is too small stop the generation
        {
            yield break;
        }
        
        yield return waitSec;
        
        bool divideVertically = Random.Range(0, 2) == 0; //choose a random direction for creating a wall

        
        if (divideVertically)
        {
            int wallX = Random.Range(x + 1, x + width);
            CreateWall(wallX, y, wallX, y + height); // Create a wall along the division line
            yield return waitSec;
            StartCoroutine(GenerateMaze(x, y, wallX - x, height, waitSec)); //generate another wall for the first chamber created
            StartCoroutine(GenerateMaze(wallX, y, x + width - wallX, height, waitSec)); //generate another wall for the second chamber created
        }
        else
        {
            int wallY = Random.Range(y + 1, y + height);
            CreateWall(x, wallY, x + width, wallY); // Create a wall along the division line
            yield return waitSec;
            StartCoroutine(GenerateMaze(x, y, width, wallY - y, waitSec)); //generate another wall for the first chamber created
            StartCoroutine(GenerateMaze(x, wallY, width, y + height - wallY, waitSec)); //generate another wall for the second chamber created
        }
    }

    /// <summary>
    /// Creates a series of walls from the first pos (x1, y1) to the second pos(x2, y2) and creates a passage through the wall
    /// </summary>
    /// <param name="x1"> the x of the start position of the wall </param>
    /// <param name="y1"> the y of the start position of the wall </param>
    /// <param name="x2"> the x of the end position of the wall </param>
    /// <param name="y2"> the y of the end position of the wall </param>
    private void CreateWall(float x1, float y1, float x2, float y2)
    {
        bool horizontal = x1 == x2 ? false : true;

        Vector2 position;
        float length = Vector3.Distance(new Vector3(x1, 0, y1), new Vector3(x2, 0, y2));

        //for as many walls as there are
        for (int i = 0; i < length; i++)
        {
            position = new Vector2(horizontal ? startX + x1 + i : startX + x1  - 0.5f,horizontal ? startY + y1 - 0.5f  : startY + y1 + i );
            GameObject wall = Instantiate(horizontal ? horizontalWall : verticalWall, position, quaternion.identity, transform);
            wallPathing.Add(wall);
        }
        
        int randInt = Random.Range(0, wallPathing.Count);
        Destroy(wallPathing[randInt]); //make a passage
        wallPathing.Clear();
    }


    //--------------------------------Regeneration---------------------------------------------


    /// <summary>
    /// Start a new Wilson maze generation
    /// </summary>
    public void Regenerate()
    {
        for (int i = 0; i < transform.childCount; i++)//destroy all walls
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        SetUpGrid();
    }
   
    /// <summary>
    /// Destroys and resets the mazes properties
    /// </summary>
    public void DestroyMaze()
    {
        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)//destroy all walls
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}