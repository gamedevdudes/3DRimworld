using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGrid : MonoBehaviour
{
    private Dictionary<Vector3, BuildTile> tiles;
    //public BuildTile BuildTile;
    public GameObject Floor;
    public GameObject Wall;
    public GameObject TableLike;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void addFloorToGrid() {
        for(int i = 0; i <3; i++) {
            for(int j = 0; j<3; j++) {
                 addToGrid(Floor, i,j);

            }
        }
    }
    /*
    NUR TEST METHODEN
     */
    public void addWallsToGrid(){
        addToGrid(Wall, 0,1,TilePosition.Left);
                addToGrid(Wall, 0,1,TilePosition.Back);

        addToGrid(Wall, 0,1,TilePosition.Right);
        addToGrid(Wall, 0,1,TilePosition.Front);

    }
    public void addToGrid(GameObject transform, int x, int y, TilePosition tilePosition) {
        Vector3 position = new Vector3(x,0.5f,y);
        Quaternion rotation = new Quaternion(0,0,0,0);
        switch(tilePosition) {
            case TilePosition.Left:rotation =Quaternion.Euler(0,180,0); position += new Vector3(-0.5f,0,0);  break;
            case TilePosition.Back:rotation =  Quaternion.Euler(0,270,0); position += new Vector3(0,0,0.5f);break;
            case TilePosition.Right:  rotation =Quaternion.Euler(0,0,0); position += new Vector3(0.5f,0,0);break;
            case TilePosition.Front: rotation = Quaternion.Euler(0,90,0); position += new Vector3(0,0,-0.5f);break;
        }
        GameObject objectToBeAdded = (Instantiate(transform, position,rotation));
        GameObject buildTileContainer = new GameObject("BuildTile" + x + y);
        objectToBeAdded.name = "Wall at " + x+y + tilePosition.ToString();
        BuildTile buildTile  = buildTileContainer.AddComponent<BuildTile>() as BuildTile;
        buildTile.setPosition(x,y);
        buildTile.SetWall(tilePosition, objectToBeAdded);
        objectToBeAdded.transform.SetParent(buildTileContainer.transform);
        tiles.Add(position, buildTile);

    }
    public void addToGrid(GameObject transform, int x, int y) {
        Vector3 position = new Vector3(x,0,y);
        GameObject objectToBeAdded = (Instantiate(transform, position, Quaternion.identity));
        GameObject buildTileContainer = new GameObject("BuildTile" + x + y);
        objectToBeAdded.name = "Floor at " + x+y;
        BuildTile buildTile  = buildTileContainer.AddComponent<BuildTile>() as BuildTile;
        buildTile.setPosition(x,y);
        buildTile.SetFloor(objectToBeAdded);
        objectToBeAdded.transform.SetParent(buildTileContainer.transform);
        tiles.Add(position, buildTile);

    }

}
public enum TilePosition {
    Left,
    Back,

    Right,
    Front

}
