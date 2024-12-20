using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class PathNode
{
    public int pos_x;
    public int pos_y;

    public float gValue;
    public float hValue;
    public PathNode parentNode;

    public float fValue
    {
        get { return gValue + hValue; }
    }

    public PathNode(int xPos, int yPos)
    {
        pos_x = xPos;
        pos_y = yPos;
    }
}

[RequireComponent(typeof(GridMap))]

public class Pathfinding : MonoBehaviour
{
    GridMap gridMap;

    PathNode[,] pathNodes;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        if (gridMap == null) { gridMap = GetComponent<GridMap>(); }

        pathNodes = new PathNode[gridMap.length,gridMap.width];

        for (int x = 0; x < gridMap.length; x++)
        {
            for (int y = 0; y < gridMap.width; y++)
            {
                pathNodes[x,y] = new PathNode(x,y);
            }
        }
    }

    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = pathNodes[startX,startY];
        PathNode endNode = pathNodes[endX,endY];

        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        openList.Add(startNode);

        while(openList.Count > 0)
        {
            PathNode currentNode = openList[0];

            for (int i = 0; i < openList.Count; i++)
            {
                if (currentNode.fValue > openList[i].fValue)
                {
                    currentNode = openList[i];
                }

                if (currentNode.fValue == openList[i].fValue && currentNode.hValue > openList[i].hValue)
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == endNode)
            {
                return RetracePath(startNode,endNode);
            }

            // Look for neighbors in a 3x3 grid, ignoring the center node since it's the current node
            List<PathNode> neighborNodes = new List<PathNode>();
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (x == 0 && y == 0) { continue; }
                    if (gridMap.CheckBoundary(currentNode.pos_x + x, currentNode.pos_y + y) == false) { continue; }

                    neighborNodes.Add(pathNodes[currentNode.pos_x + x, currentNode.pos_y + y]);
                }
            }

            for (int i = 0; i < neighborNodes.Count; i++)
            {
                if (closedList.Contains(neighborNodes[i])) { continue; }
                if (gridMap.CheckWalkable(neighborNodes[i].pos_x, neighborNodes[i].pos_y) == false) { continue; }

                float movementCost = currentNode.gValue + CalculateDistance(currentNode, neighborNodes[i]);

                if (openList.Contains(neighborNodes[i]) == false
                    || movementCost < neighborNodes[i].gValue)
                {
                    neighborNodes[i].gValue = movementCost;
                    neighborNodes[i].hValue = CalculateDistance(neighborNodes[i], endNode);
                    neighborNodes[i].parentNode = currentNode;

                    if (openList.Contains(neighborNodes[i]) == false)
                    {
                        openList.Add(neighborNodes[i]);
                    }
                }
            }
        }

        return null;
    }

    private int CalculateDistance(PathNode currentNode, PathNode target)
    {
        int distX = Mathf.Abs(currentNode.pos_x - target.pos_x);
        int distY = Mathf.Abs(currentNode.pos_y - target.pos_y);

        if (distX > distY) { return 14 * distY + 10 * (distX - distY); }
        return 14 * distX + 10 * (distY - distX);
    }

    private List<PathNode> RetracePath(PathNode startNode, PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();

        PathNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parentNode;
        }
        path.Reverse();

        return path;
    }
}
