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
    public GameObject up;
    public GameObject down;
    public GameObject left;
    public GameObject right;

    public List<Cell> adjacentCells;
    [SerializeField]private LayerMask LMaskCell;
    [SerializeField]private LayerMask LMaskWall;
    
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

    /// draw 4 raycasts to check if there are any cells adjacent to the current one
    /// add that cell to the adjacentCells list
    public void AddAdjacentCell()
    {
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };
        RaycastHit hit;

        for (int i = 0; i < directions.Length; i++)
        {
            if (Physics.Raycast(transform.position, directions[i], out hit, 1f, LMaskCell))
            {
                adjacentCells.Add(hit.transform.GetComponent<Cell>());
            }
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
    /// <param name="neighbouringCell"></param>
    public void DestroyWalls(Cell neighbouringCell)
    {
        RaycastHit[] hits;
        Vector3 dir = neighbouringCell.transform.position - transform.position;
        dir.Normalize();
        hits = Physics.RaycastAll(transform.position, dir, 1f, LMaskWall);

        foreach (RaycastHit hit in hits)
        {
            Destroy(hit.collider.gameObject);
        }
    }

}
