using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    Node[,] grid;
    public int width = 25;
    public int length = 25;
    [SerializeField] float cellSize = 1f;
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] LayerMask terrainLayer;

    // Pathfinding -20, GridObject -10 in Script Execution Order
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

    public bool CheckBoundary(int posX, int posY)
    {
        if (posX < 0 || posX  >= length)
        {
            return false;
        }
        if (posY < 0 || posY >= width)
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

        CalculateElevation();
        CheckPassableTerrain();
    }

    private void CalculateElevation()
    {
        for (int y = 0; y < width; y++)
        {
            for (int x = 0; x < length; x++)
            {
                Ray ray = new Ray(GetWorldPosition(x,y) + Vector3.up * 100f, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, float.MaxValue, terrainLayer))
                {
                    grid[x,y].elevation = hit.point.y;
                }
            }
        }
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
                grid[x,y].passable = passable;
            }
        }
    }

    public bool CheckWalkable(int pos_x, int pos_y)
    {
        return grid[pos_x, pos_y].passable;
    }

    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        Vector2Int positionOnGrid = new Vector2Int((int)(worldPosition.x / cellSize), (int)(worldPosition.z / cellSize));
        return positionOnGrid;
    }

    private void OnDrawGizmos()
    {
        if (grid == null) 
        {
            for (int y = 0; y < width; y++)
            {
                for (int x = 0; x < length; x++)
                {
                    Vector3 pos = GetWorldPosition(x,y);
                    Gizmos.DrawCube(pos, Vector3.one/4);
                }
            }

        }
        else
        {
            for (int y = 0; y < width; y++)
            {
                for (int x = 0; x < length; x++)
                {
                    Vector3 pos = GetWorldPosition(x,y, true);
                    Gizmos.color = grid[x,y].passable ? Color.white : Color.red;
                    Gizmos.DrawCube(pos, Vector3.one/4);
                }
            }
        }
        
    }

    public Vector3 GetWorldPosition(int x, int y, bool elevation = false)
    {
        return new Vector3(x * cellSize, elevation == true ? grid[x,y].elevation : 0f, y * cellSize);
    }
}
