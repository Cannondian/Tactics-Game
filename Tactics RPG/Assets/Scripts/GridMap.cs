using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    Node[,] grid;
    [SerializeField] int width = 25;
    [SerializeField] int length = 25;
    [SerializeField] float cellSize = 1f;
    [SerializeField] LayerMask obstacleLayer;

    private void Awake()
    {
        GenerateGrid();
    }

    public void PlaceObject(Vector2Int positionOnGrid, GridObject gridObject)
    {
        if (CheckBoundary(positionOnGrid) == true)
        {
            grid[positionOnGrid.x, positionOnGrid.y].gridObject = gridObject;
        }
        else
        {
            Debug.Log("Object has been placed out of bounds");
        }
    }

    public bool CheckBoundary(Vector2Int positionOnGrid)
    {
        if (positionOnGrid.x < 0 || positionOnGrid.x >= length)
        {
            return false;
        }
        if (positionOnGrid.y < 0 || positionOnGrid.y >= width)
        {
            return false;
        }
        return true;
    }

    private void GenerateGrid()
    {
        grid = new Node[length,width];

        for (int y = 0; y < width; y++)
        {
            for (int x = 0; x < length; x++)
            {
                grid[x,y] = new Node();
            }
        }

        CheckPassableTerrain();
    }

    public GridObject GetPlacedObject(Vector2Int gridPosition)
    {
        if (CheckBoundary(gridPosition) == true)
        {
            GridObject gridObject = grid[gridPosition.x, gridPosition.y].gridObject;
            return gridObject;
        }
        return null;
        
    }

    private void CheckPassableTerrain()
    {
        for (int y = 0; y < width; y++)
        {
            for (int x = 0; x < length; x++)
            {
                Vector3 worldPosition = GetWorldPosition(x,y);
                bool passable = !Physics.CheckBox(worldPosition, Vector3.one / 2 * cellSize, Quaternion.identity,obstacleLayer);
                grid[x,y] = new Node();
                grid[x,y].passable = passable;
            }
        }
    }

    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        worldPosition -= transform.position;
        Vector2Int positionOnGrid = new Vector2Int((int)(worldPosition.x / cellSize), (int)(worldPosition.z / cellSize));
        return positionOnGrid;
    }

    private void OnDrawGizmos()
    {
        if (grid == null) { return; }
        for (int y = 0; y < width; y++)
        {
            for (int x = 0; x < length; x++)
            {
                Vector3 pos = GetWorldPosition(x,y);
                Gizmos.color = grid[x,y].passable ? Color.white : Color.red;
                Gizmos.DrawCube(pos, Vector3.one/4);
            }
        }
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(transform.position.x + (x * cellSize), 0f, transform.position.z + (y * cellSize));
    }
}
