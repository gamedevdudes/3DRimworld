using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGrid : MonoBehaviour
{
    private BuildTile[,,] tiles = new BuildTile[width,height,LEVELS];
    public const int width=100, height = 100;
    private const int LEVELS = 10;
    //public BuildTile BuildTile;
    public GameObject Floor;
    public GameObject Wall;
    public GameObject TableLike;
    // Start is called before the first frame update
    void Start()
    {
        tiles.Initialize();
        // print("Tile size: " + tiles.GetLength(0) + "," + tiles.GetLength(1) + "," +tiles.GetLength(2));
        // for(int i =0; i <width; i++) {
        //     for(int j =0; j< height; j++) {
        //         for(int k = 0; k < LEVELS; k++) {
        //             tiles[i,j,k] = new BuildTile();
        //         }
        //     }
        // }
        // tiles.Initialize();

    }
    public void generateWorldWithRandomSeed() {
        float randomseed = Random.Range(0,100000);
        print("seed: " + randomseed);
        generatePerlinWorld(randomseed);
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

    public void generatePerlinWorld(float seed) {
        int factor = 15;
        for(int i = 0; i <width*factor; i+= 1 * factor) {
            for(int j = 0; j<height*factor; j+= 1 * factor ) {
                float xCoord = i/1000.0f +seed;
                float yCoord = j/1000.0f +seed;
                float sample = Mathf.PerlinNoise(xCoord,yCoord);
                float mappedValue = sample * LEVELS -LEVELS/2;
                float roundedValue = Mathf.Round(mappedValue);
                //mappedValue = Mathf.Round(mappedValue);
                //print("At: " +i+ "," + j + ": " + sample +"," + mappedValue);
                int x = (int) (i / factor);
                int y = (int) (j / factor);
                int z = (int) roundedValue;
                if(z == LEVELS) z = LEVELS -1;
                if(x == 4 || y == 4) {
                    print("sample: " + sample + " i: "+ i + " j: "+ j + " seed: " + seed);
                }
                //print("x:" + x + " y: " + y + "," + z);
                if(x + width/2 < width  && y+height/2 < height && z +LEVELS/2 < LEVELS) {
                    addToGrid(Floor, x, y,z);
                }
            }
        }
        
        
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
        // BuildTile buildTile  = buildTileContainer.AddComponent<BuildTile>() as BuildTile;
        // buildTile.setPosition(x,y);
        // buildTile.SetWall(tilePosition, objectToBeAdded);
        // objectToBeAdded.transform.SetParent(buildTileContainer.transform);
        // addBuildTile(buildTile, (int) position.x, (int) position.y, (int) position.z);

    }
    public void addToGrid(GameObject transform, int x, int y) {
        Vector3 position = new Vector3(x,0,y);
        GameObject objectToBeAdded = (Instantiate(transform, position, Quaternion.identity));
        GameObject buildTileContainer = new GameObject("BuildTile" + x + y);
        objectToBeAdded.name = "Floor at " + x+y;
        // BuildTile buildTile  = buildTileContainer.AddComponent<BuildTile>() as BuildTile;
        // buildTile.setPosition(x,y);
        // buildTile.SetFloor(objectToBeAdded);
        // objectToBeAdded.transform.SetParent(buildTileContainer.transform);
        // addBuildTile(buildTile, (int) position.x, (int) position.y, (int) position.z);

    }
    public void addToGrid(GameObject transform, int x, int y, int z) {
        Vector3 position = new Vector3(x,z,y);
        GameObject objectToBeAdded = (Instantiate(transform, position, Quaternion.identity));
        objectToBeAdded.name = "Floor at " + x+ "," +y+","+z;
        BuildTile buildTile  = new BuildTile();
        buildTile.setPosition(x,y);
        buildTile.SetFloor(objectToBeAdded);
        //objectToBeAdded.transform.SetParent(this.transform);
        addBuildTile(buildTile, (int) position.x, (int) position.y, (int) position.z);
    }
    private BuildTile getBuildTile(int x, int y, int z) {
        int xCoord = x + width/2;
        int yCoord = y+ height/2;
        int zCoord = z + LEVELS/2;
        return tiles[xCoord,yCoord,zCoord];
    }
    private void addBuildTile(BuildTile tile, int x, int z, int y) {
        //print("Eigentliche Parameter: " + x + "," + y +","+ z );
        int xCoord = x + width/2;
        int yCoord = y + height/2;
        int zCoord = z + LEVELS/2;
        //print("Parameter: "+ xCoord +","+ yCoord +","+ zCoord);
        //print("Größe: " + tiles.GetLength(0) + "," + tiles.GetLength(1) + "," + tiles.GetLength(2));
        //TODO UMBEDINGT ÜBERSCHREIBEN VERHINDERN
        tiles[xCoord,yCoord,zCoord] = tile;
    }

}
public enum TilePosition {
    Left,
    Back,

    Right,
    Front

}
