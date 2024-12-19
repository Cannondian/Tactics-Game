using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GridObject : MonoBehaviour
{
    [SerializeField] GridMap targetGrid;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        Vector2Int positionOnGrid = targetGrid.GetGridPosition(transform.position);
        targetGrid.PlaceObject(positionOnGrid,this);
    }
}
