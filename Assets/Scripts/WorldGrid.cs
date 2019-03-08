using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class WorldGrid : MonoBehaviour
{
    public BuildTile[,,] tiles = new BuildTile[width,height,levels];

    private WorldTile[] worldTiles = new WorldTile[width* height * levels];
    public const int width=300, height = 300, levels = 15;

    //public BuildTile BuildTile;
    public GameObject Floor;
    public GameObject worldTile;
    public GameObject Wall;
    public GameObject TableLike;
    
    public GameObject WorldContainer;

    // Start is called before the first frame update
    void Start()
    {
        tiles.Initialize();
        worldTiles.Initialize();
        this.WorldContainer = GameObject.Find("WorldContainer");
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
        float randomseed = UnityEngine.Random.Range(0,100000);
        print("seed: " + randomseed);
        generatePerlinWorld(randomseed);
        StaticBatchingUtility.Combine(WorldContainer);
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
        int factor = 8;
        for(int i = 0; i <width*factor; i+= 1 * factor) {
            for(int j = 0; j<height*factor; j+= 1 * factor ) {
                float xCoord = i/1000.0f +seed;
                float yCoord = j/1000.0f +seed;
                float sample = Mathf.PerlinNoise(xCoord,yCoord);
                float mappedValue = sample * levels -levels/2;
                float roundedValue = Mathf.Round(mappedValue);
                //mappedValue = Mathf.Round(mappedValue);
                //print("At: " +i+ "," + j + ": " + sample +"," + mappedValue);
                int x = (int) (i / factor);
                int y = (int) (j / factor);
                int z = (int) roundedValue;
                if(z == levels) z = levels -1;
                if(x == 4 || y == 4) {
                    //print("sample: " + sample + " i: "+ i + " j: "+ j + " seed: " + seed);
                }
                //print("x:" + x + " y: " + y + "," + z);
                if(x + width/2 < width  && y+height/2 < height && z +levels/2 < levels) {
                    //addToGrid(Floor, x, y,z);
                    addToTiles(x,y,z);
                }
            }
        }
        
        
    }
    private MeshFilter[] GetMeshFiltersFromWorldTiles() {
        MeshFilter[] filters = new MeshFilter[worldTiles.Length];
        filters.Initialize();
        print("Length: " + filters.Length);
        int i =0;
        foreach (WorldTile tile in worldTiles) {
            filters[i] = tile.GetMeshFilter();
        }
        return filters;
    }
    public void combineMeshes() {
        MeshFilter[] meshFilters = GetMeshFiltersFromWorldTiles();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while(i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);

            i++;
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);
    }
    public void addToTiles(int x,int y, int z) {
        Vector3 position = new Vector3(x,z,y);
        Quaternion rotation = new Quaternion(0,0,0,0);
        GameObject objectToBeAdded = (Instantiate(this.worldTile, position,rotation));
        objectToBeAdded.name = "WorldTile at " + x+y+z;
        worldTiles[x * width + y * height + z * levels] = objectToBeAdded.GetComponent<WorldTile>();
        objectToBeAdded.transform.SetParent(WorldContainer.transform);
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
        int zCoord = z + levels/2;
        return tiles[xCoord,yCoord,zCoord];
    }
    private void addBuildTile(BuildTile tile, int x, int z, int y) {
        //print("Eigentliche Parameter: " + x + "," + y +","+ z );
        int xCoord = x + width/2;
        int yCoord = y + height/2;
        int zCoord = z + levels/2;
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
