using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class WorldGrid : MonoBehaviour
{
    public BuildTile[,,] tiles = new BuildTile[width, height, levels];

    private WorldTile[,,] worldTiles = new WorldTile[width, height, levels];
    [SerializeField]
    public static int width = 50, height = 50, levels = 20;

    public Slider slider;

    //public BuildTile BuildTile;
    public GameObject Floor;
    public GameObject worldTile;
    public GameObject Wall;
    public GameObject TableLike;
    public GameObject GroundWorldTile;
    public GameObject DiagonalWorldTile;
    public GameObject WorldContainer;
    public int factor;
    private IEnumerator worldGeneration;
    private IEnumerator flattening;
    private IEnumerator destroySlider;

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
    public void generateWorldWithoutRandomSeed()
    {
        worldGeneration = generatePerlinWorld(0);
        StartCoroutine(worldGeneration);
        flattening = flattenLandscape();
        //generatePerlinWorld(randomseed);
    }
    public void generateWorldWithRandomSeed()
    {
        float randomseed = UnityEngine.Random.Range(0, 100000);
        print("seed: " + randomseed);
        worldGeneration = generatePerlinWorld(randomseed);
        StartCoroutine(worldGeneration);
        flattening = flattenLandscape();
        //generatePerlinWorld(randomseed);
    }
    private void finishGeneration()
    {
        StaticBatchingUtility.Combine(WorldContainer);
        GameObject.Destroy(slider.gameObject);
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void addFloorToGrid()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                addToGrid(Floor, i, j);

            }
        }
    }

    IEnumerator flattenLandscape()
    {
        slider.maxValue = levels;
        slider.value = 0;
        for (int k = 0; k < levels - 1; k++)

        {
            for (int j = 1; j < height - 1; j++)
            {
                for (int i = 1; i < width - 1; i++)

                {
                    WorldTile tile = worldTiles[i, j, k];
                    if (tile != null)
                    {
                        if (worldTiles[i + 1, j, k] != null && worldTiles[i - 1, j, k] != null && worldTiles[i, j + 1, k] != null && worldTiles[i, j - 1, k] != null)
                        {
                            tile.GetComponent<MeshRenderer>().shadowCastingMode = 0;
                        }
                        else
                        {
                            //Ist Rechts einer höher
                            if (worldTiles[i + 1, j, k + 1] != null && worldTiles[i - 1, j, k] != null)
                            {
                                //GameObject.Destroy(tile.GetComponent<WorldTile>());
                                //GameObject.DestroyImmediate(tile);
                                //GameObject.Destroy(tile.gameObject);
                                addToTiles(DiagonalWorldTile, i, j, k + 2 - levels / 2, Vector3.zero, Quaternion.Euler(0, 270, 0), "Orientation i+1");
                            }
                            else
                            {
                                //Check ob hinten höher
                                if (worldTiles[i, j - 1, k + 1] != null  && worldTiles[i , j+1, k] != null && worldTiles[i,j-1, k+1].gameObject.tag != "Diagonal")
                                {
                                    //GameObject.Destroy(tile.GetComponent<WorldTile>());
                                    //GameObject.DestroyImmediate(tile);
                                    //GameObject.Destroy(tile.gameObject);
                                    addToTiles(DiagonalWorldTile, i, j, k + 2 - levels / 2, Vector3.zero, Quaternion.Euler(0, 0, 0), "Orientation j-1");
                                }
                                else
                                {
                                    //Ist vorne höher
                                    if (worldTiles[i - 1, j, k + 1] != null  && worldTiles[i + 1, j, k ] != null)
                                    {
                                        //RICHTIG
                                        //GameObject.Destroy(tile.GetComponent<WorldTile>());
                                        //GameObject.DestroyImmediate(tile);
                                        //GameObject.Destroy(tile.gameObject);
                                        addToTiles(DiagonalWorldTile, i, j, k + 2 - levels / 2, Vector3.zero, Quaternion.Euler(0, 90, 0), "Orientation i-1");
                                    }
                                    else
                                    {
                                        //Ist rechts höher
                                        if (worldTiles[i, j + 1, k + 1] != null  && worldTiles[i , j-1, k ] != null)
                                        {
                                            //GameObject.Destroy(tile.GetComponent<WorldTile>());
                                            //GameObject.DestroyImmediate(tile);
                                            //GameObject.Destroy(tile.gameObject);
                                            addToTiles(DiagonalWorldTile, i, j, k + 2 - levels / 2, Vector3.zero, Quaternion.Euler(0, 180, 0), "Orientation j+1");
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
            }
            slider.value = k;
            yield return null;
        }
        print("Flattening done");
        finishGeneration();

    }
    /*
    NUR TEST METHODEN
     */
    public void addWallsToGrid()
    {
        addToGrid(Wall, 0, 1, TilePosition.Left);
        addToGrid(Wall, 0, 1, TilePosition.Back);

        addToGrid(Wall, 0, 1, TilePosition.Right);
        addToGrid(Wall, 0, 1, TilePosition.Front);

    }
    private static int roundAndFlatHeight(float value)
    {
        float mappedValue = value * levels - levels / 2;
        float roundedValue = Mathf.Round(mappedValue);
        return (int)roundedValue;

    }
    private static float calcCoord(int value, float seed)
    {
        return value / 1000.0f + seed;
    }
    IEnumerator generatePerlinWorld(float seed)
    {
        slider.maxValue = width * factor;
        for (int i = 0; i < width * factor; i += 1 * factor)
        {
            for (int j = 0; j < height * factor; j += 1 * factor)
            {
                float xCoord = calcCoord(i, seed);
                float yCoord = calcCoord(j, seed);
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                Vector3 translation = new Vector3(0, 0, 0);
                Quaternion rotation = new Quaternion(0, 0, 0, 0);
                int x = (int)(i / factor);
                int y = (int)(j / factor);
                int z = roundAndFlatHeight(sample);
                if (z == levels) z = levels - 1;
                //print("x:" + x + " y: " + y + "," + z);
                if (x < width && y < height && z + levels / 2 < levels)
                {
                    //addToGrid(Floor, x, y,z);
                    if (z < 0.3f * levels)
                    {
                        addToTiles(GroundWorldTile, x, y, z, translation, rotation);

                    }
                    else
                    {
                        addToTiles(Floor, x, y, z, translation, rotation);

                    }
                }
                slider.value = i;

            }
            yield return null;

        }
        StartCoroutine(flattening);
        print("Generating done");
    }
    public void addToTiles(GameObject tileObject, int x, int y, int z, Vector3 translate, Quaternion rotate)
    {
        Vector3 position = new Vector3(x, z, y);
        Quaternion rotation = rotate;
        GameObject objectToBeAdded = (Instantiate(tileObject, position + translate, rotate));
        objectToBeAdded.name = tileObject.name + "WorldTile at " + x + "," + y + "," + z;
        try
        {
            WorldTile tile = objectToBeAdded.GetComponent<WorldTile>();
            worldTiles[x, y, z + (levels / 2) - 1] = tile;
            tile.setCoords(x,y,z + (levels /2) -1);

        }
        catch (IndexOutOfRangeException e)
        {
            print("Exception at:" + x + ", " + y + ", " + z);
        }
        objectToBeAdded.transform.SetParent(WorldContainer.transform);
    }
    public void addToTiles(GameObject tileObject, int x, int y, int z, Vector3 translate, Quaternion rotate, string orientation)
    {
        Vector3 position = new Vector3(x, z, y);
        Quaternion rotation = rotate;
        GameObject objectToBeAdded = (Instantiate(tileObject, position + translate, rotate));
        objectToBeAdded.name = tileObject.name + "WorldTile at " + x + "," + y + "," + z;
        try
        {
            WorldTile tile = objectToBeAdded.GetComponent<WorldTile>();
            worldTiles[x, y, z + (levels / 2) - 1] = tile;
            tile.Orientation = orientation;

        }
        catch (IndexOutOfRangeException e)
        {
            print("Exception at:" + x + ", " + y + ", " + z);
        }
        objectToBeAdded.transform.SetParent(WorldContainer.transform);
    }
    public void addToTiles(int x, int y, int z)
    {
        Vector3 position = new Vector3(x, z, y);
        Quaternion rotation = new Quaternion(0, 0, 0, 0);
        GameObject objectToBeAdded = (Instantiate(this.worldTile, position, rotation));
        objectToBeAdded.name = "WorldTile at " + x + y + z;
        //worldTiles[x * width + y * height + z * levels] = objectToBeAdded.GetComponent<WorldTile>();
        objectToBeAdded.transform.SetParent(WorldContainer.transform);
    }
    public void addToGrid(GameObject transform, int x, int y, TilePosition tilePosition)
    {
        Vector3 position = new Vector3(x, 0.5f, y);
        Quaternion rotation = new Quaternion(0, 0, 0, 0);
        switch (tilePosition)
        {
            case TilePosition.Left: rotation = Quaternion.Euler(0, 180, 0); position += new Vector3(-0.5f, 0, 0); break;
            case TilePosition.Back: rotation = Quaternion.Euler(0, 270, 0); position += new Vector3(0, 0, 0.5f); break;
            case TilePosition.Right: rotation = Quaternion.Euler(0, 0, 0); position += new Vector3(0.5f, 0, 0); break;
            case TilePosition.Front: rotation = Quaternion.Euler(0, 90, 0); position += new Vector3(0, 0, -0.5f); break;
        }
        GameObject objectToBeAdded = (Instantiate(transform, position, rotation));
        GameObject buildTileContainer = new GameObject("BuildTile" + x + y);
        objectToBeAdded.name = "Wall at " + x + y + tilePosition.ToString();
        // BuildTile buildTile  = buildTileContainer.AddComponent<BuildTile>() as BuildTile;
        // buildTile.setPosition(x,y);
        // buildTile.SetWall(tilePosition, objectToBeAdded);
        // objectToBeAdded.transform.SetParent(buildTileContainer.transform);
        // addBuildTile(buildTile, (int) position.x, (int) position.y, (int) position.z);

    }
    public void addToGrid(GameObject transform, int x, int y)
    {
        Vector3 position = new Vector3(x, 0, y);
        GameObject objectToBeAdded = (Instantiate(transform, position, Quaternion.identity));
        GameObject buildTileContainer = new GameObject("BuildTile" + x + y);
        objectToBeAdded.name = "Floor at " + x + y;
        // BuildTile buildTile  = buildTileContainer.AddComponent<BuildTile>() as BuildTile;
        // buildTile.setPosition(x,y);
        // buildTile.SetFloor(objectToBeAdded);
        // objectToBeAdded.transform.SetParent(buildTileContainer.transform);
        // addBuildTile(buildTile, (int) position.x, (int) position.y, (int) position.z);

    }
    // public void addToGrid(GameObject transform, int x, int y, int z)
    // {
    //     Vector3 position = new Vector3(x, z, y);
    //     GameObject objectToBeAdded = (Instantiate(transform, position, Quaternion.identity));
    //     objectToBeAdded.name = "Floor at " + x + "," + y + "," + z;
    //     BuildTile buildTile = new BuildTile();
    //     buildTile.setPosition(x, y);
    //     buildTile.SetFloor(objectToBeAdded);
    //     //objectToBeAdded.transform.SetParent(this.transform);
    //     addBuildTile(buildTile, (int)position.x, (int)position.y, (int)position.z);
    // }
    private BuildTile getBuildTile(int x, int y, int z)
    {
        int xCoord = x + width / 2;
        int yCoord = y + height / 2;
        int zCoord = z + levels / 2;
        return tiles[xCoord, yCoord, zCoord];
    }
    private void addBuildTile(BuildTile tile, int x, int z, int y)
    {
        //print("Eigentliche Parameter: " + x + "," + y +","+ z );
        int xCoord = x + width / 2;
        int yCoord = y + height / 2;
        int zCoord = z + levels / 2;
        //print("Parameter: "+ xCoord +","+ yCoord +","+ zCoord);
        //print("Größe: " + tiles.GetLength(0) + "," + tiles.GetLength(1) + "," + tiles.GetLength(2));
        //TODO UMBEDINGT ÜBERSCHREIBEN VERHINDERN
        tiles[xCoord, yCoord, zCoord] = tile;
    }
    public WorldTile[,,] GetTiles()
    {
        return worldTiles;
    }

    public BuildTile[,,] GetBuildTiles()
    {
        return tiles;
    }
}
public enum TilePosition
{
    Left,
    Back,

    Right,
    Front

}
