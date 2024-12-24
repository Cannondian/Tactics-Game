using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GridObject : MonoBehaviour
{
    public GridMap targetGrid;
    public Vector2Int positionOnGrid;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        positionOnGrid = targetGrid.GetGridPosition(transform.position);
        targetGrid.PlaceObject(positionOnGrid,this);
        Vector3 pos = targetGrid.GetWorldPosition(positionOnGrid.x,positionOnGrid.y,true);
        transform.position = pos;
    }
}
