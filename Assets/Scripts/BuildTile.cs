﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTile : MonoBehaviour
{
    private GameObject floor;

    private GameObject ceiling;

    private GameObject tableLike;

    private GameObject item;

    private int x,y;

    private GameObject wallLeft,wallRight,wallBack,wallFront;


    public void SetFloor(GameObject floor) {
        this.floor = floor;
    }
    public void SetTableLike(GameObject tableLike) {
        this.tableLike = tableLike;
    }
    public void SetItem(GameObject item) {
        this.item = item;
    }
    public void SetWall(TilePosition position, GameObject wall) {
        switch(position){
            case TilePosition.Left: wallLeft = wall; break;
            case TilePosition.Back: wallBack = wall; break;
            case TilePosition.Right: wallRight = wall; break;
            case TilePosition.Front: wallFront = wall; break;

        }
    }
    public void setPosition(int x, int y) {
        this.x = x;
        this.y = y;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}