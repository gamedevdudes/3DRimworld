using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using BuildItems;

namespace WorldGen
{
    public class WorldGrid : MonoBehaviour
    {
        public BuildTile[,,] tiles;

        private WorldTile[,,] worldTiles;
        public int width = 150, height = 150, levels = 20;
        public int factor;
        public Slider slider;
        public bool grass = false;

        //public BuildTile BuildTile;
        public WorldTile Floor;
        public WorldTile worldTile;
        public WorldTile WaterTile;
        public GameObject GrassObject;
        public GameObject Wall;
        public GameObject TableLike;
        public WorldTile GroundWorldTile;
        public WorldTile DiagonalCornerTile;
        public WorldTile DiagonalWorldTile;
        public GameObject WorldContainer;
        public WorldTile CornerCube;
        public LocalNavMeshBuilder navMeshBuilder;
        private IEnumerator worldGeneration;
        private IEnumerator flattening;
        private IEnumerator destroySlider;

        public WorldTileMaterilaProvider MaterialProvider;

        // Start is called before the first frame update
        void Start()
        {
            tiles = new BuildTile[width, height, levels];
            worldTiles = new WorldTile[width, height, levels];
            tiles.Initialize();
            worldTiles.Initialize();
            flattening = flattenLandscape();

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
            //generatePerlinWorld(randomseed);
        }
        public void generateWorldWithRandomSeed()
        {
            slider.gameObject.SetActive(true);
            float randomseed = UnityEngine.Random.Range(0, 100000);
            print("seed: " + randomseed);
            worldGeneration = generatePerlinWorld(randomseed);
            StartCoroutine(worldGeneration);
            flattening = flattenLandscape();
            //generatePerlinWorld(randomseed);
        }
        private void finishGeneration()
        {
            if(grass) {
            StartCoroutine(addGrass());

            }
            StaticBatchingUtility.Combine(WorldContainer);
            slider.gameObject.SetActive(false);
            navMeshBuilder.stillUpdating = false;
        }
        // Update is called once per frame
        void Update()
        {

        }
        public void setWidth(String input)
        {
            width = int.Parse(input);
            print("width: " + width);
        }
        public void setHeight(String input)
        {
            height = int.Parse(input);
            print("height: " + height);
        }
        public void setLevels(String input)
        {
            levels = int.Parse(input);
        }
        public void setFactor(String input)
        {
            factor = int.Parse(input);

        }
        public void addFloorToGrid()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    addToGrid(Floor.gameObject, i, j);

                }
            }
        }
        IEnumerator addGrass()
        {
            for (int k = 0; k < levels - 1; k++)

            {
                for (int j = 1; j < height - 1; j++)
                {
                    for (int i = 1; i < width - 1; i++)

                    {
                        if (worldTiles[i, j, k] != null)
                        {

                            if (worldTiles[i, j, k].landscapeType == LandscapeType.Earth)
                            {
                                float build = UnityEngine.Random.Range(0, 1.0f);
                                if (build > 0.5f)
                                {
                                    int grassDepth = UnityEngine.Random.Range(0, 15);
                                    for (int n = 0; n < grassDepth; n++)
                                    {
                                        float offsetX = UnityEngine.Random.Range(0, 0.5f), offsetY = UnityEngine.Random.Range(0, 0.5f);
                                        float offsetAxis = UnityEngine.Random.Range(0, 180.0f);
                                        var grass = GameObject.Instantiate(GrassObject, worldTiles[i, j, k].transform.position + new Vector3(offsetX, 0.75f, offsetY), Quaternion.Euler(0, offsetAxis, 90));
                                        grass.transform.parent = WorldContainer.transform;
                                    }

                                }

                            }

                        }
                    }
                    yield return null;
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
                                //Rechts + Hinten
                                addFlattening(k, j, i);
                            }

                        }
                    }
                }

                slider.value = k;
                yield return null;
            }
            slider.maxValue = levels;
            slider.value = 0;
            for (int k = 0; k < levels - 1; k++)
                
            {
                for (int j = 1; j < height - 1; j++)
                {
                    for (int i = 1; i < width - 1; i++)

                    {
                        WorldTile tile = worldTiles[i, j, k];
                        if (tile == null)
                        {
                            addDiagonalCorners(k, j, i);

                        }
                    }
                }

                slider.value = k;
                yield return null;
            }
            print("Flattening done");
            finishGeneration();

        }

        private void addDiagonalCorners(int k, int j, int i)
        {
            //Rechts & Hinten
            if (worldTiles[i + 1, j, k] != null && worldTiles[i + 1, j, k].tag == "Diagonal" && worldTiles[i, j + 1, k] != null && worldTiles[i, j + 1, k].tag == "Diagonal")
            {
                addToTiles(DiagonalCornerTile, i, j, k + 1 - levels / 2, Vector3.zero, Quaternion.Euler(-90, 180, 0), "DiagonalCorner i+1 j+1");

            }
            else
                //Rechts und Vorne
                if (worldTiles[i + 1, j, k] != null && worldTiles[i + 1, j, k].tag == "Diagonal" && worldTiles[i, j - 1, k] != null && worldTiles[i, j - 1, k].tag == "Diagonal")
            {
                addToTiles(DiagonalCornerTile, i, j, k + 1 - levels / 2, Vector3.zero, Quaternion.Euler(-90, 270, 0), "DiagonalCorner i+1 j-1");

            }
            else
                //Links und Vorne
                if (worldTiles[i - 1, j, k] != null && worldTiles[i - 1, j, k].tag == "Diagonal" && worldTiles[i, j - 1, k] != null && worldTiles[i, j - 1, k].tag == "Diagonal")
            {
                addToTiles(DiagonalCornerTile, i, j, k + 1 - levels / 2, Vector3.zero, Quaternion.Euler(-90, 0, 0), "DiagonalCorner i-1 j-1");

            }
            else


            //Links und Hinten
            if (worldTiles[i - 1, j, k] != null && worldTiles[i - 1, j, k].tag == "Diagonal" && worldTiles[i, j + 1, k] != null && worldTiles[i, j + 1, k].tag == "Diagonal")
            {
                addToTiles(DiagonalCornerTile, i, j, k + 1 - levels / 2, Vector3.zero, Quaternion.Euler(-90, 90, 0), "DiagonalCorner i-1 j+1");

            }
        }

        private void addFlattening(int k, int j, int i)
        {
            
            if(!checkDoubleSided(k, j, i))
            {

                //Ist Rechts einer höher
                if (worldTiles[i + 1, j, k + 1] != null && worldTiles[i - 1, j, k] != null && worldTiles[i + 1, j, k + 1].gameObject.tag != "Diagonal")
                {
                    //GameObject.Destroy(tile.GetComponent<WorldTile>());
                    //GameObject.DestroyImmediate(tile);
                    //GameObject.Destroy(tile.gameObject);
                    addToTiles(DiagonalWorldTile, i, j, k + 2 - levels / 2, Vector3.zero, Quaternion.Euler(0, 270, 0), "Orientation i+1");
                }
                else
                {
                    //Check ob hinten höher
                    if (worldTiles[i, j - 1, k + 1] != null && worldTiles[i, j + 1, k] != null && worldTiles[i, j - 1, k + 1].gameObject.tag != "Diagonal")
                    {
                        //GameObject.Destroy(tile.GetComponent<WorldTile>());
                        //GameObject.DestroyImmediate(tile);
                        //GameObject.Destroy(tile.gameObject);
                        addToTiles(DiagonalWorldTile, i, j, k + 2 - levels / 2, Vector3.zero, Quaternion.Euler(0, 0, 0), "Orientation j-1");
                    }
                    else
                    {
                        //Ist links höher
                        if (worldTiles[i - 1, j, k + 1] != null && worldTiles[i + 1, j, k] != null && worldTiles[i - 1, j, k + 1].gameObject.tag != "Diagonal")
                        {
                            //RICHTIG
                            //GameObject.Destroy(tile.GetComponent<WorldTile>());
                            //GameObject.DestroyImmediate(tile);
                            //GameObject.Destroy(tile.gameObject);
                            addToTiles(DiagonalWorldTile, i, j, k + 2 - levels / 2, Vector3.zero, Quaternion.Euler(0, 90, 0), "Orientation i-1");
                        }
                        else
                        {
                            //Ist vorne höher
                            if (worldTiles[i, j + 1, k + 1] != null && worldTiles[i, j - 1, k] != null && worldTiles[i, j + 1, k + 1].gameObject.tag != "Diagonal")
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
        public void clearChildren()
        {
            foreach(Transform t in WorldContainer.transform)
            {
                GameObject.Destroy(t.gameObject);
            }
        }
        private bool checkDoubleSided(int k, int j, int i)
        {
            if (k <= 0 || k >= width || j <= 0 || j>= height || i <= -levels/2 || i >= levels/2)
            {
                return false;
            }
            bool used = false;
            if (worldTiles[i + 1, j, k + 1] != null && worldTiles[i + 1, j, k + 1].gameObject.tag != "Diagonal" && worldTiles[i, j - 1, k + 1] != null && worldTiles[i, j - 1, k + 1].gameObject.tag != "Diagonal")
            {
                addToTiles(CornerCube, i, j, k + 2 - levels / 2, Vector3.zero, Quaternion.Euler(0, 270, 0), "Orientation i+1");
                used = true;
            }
            //Hinten + Links
            else
            if (worldTiles[i, j - 1, k + 1] != null && worldTiles[i, j - 1, k + 1].gameObject.tag != "Diagonal" && worldTiles[i - 1, j, k + 1] != null && worldTiles[i - 1, j, k + 1].gameObject.tag != "Diagonal")
            {
                addToTiles(CornerCube, i, j, k + 2 - levels / 2, Vector3.zero, Quaternion.Euler(0, 0, 0), "Orientation i+1");
                used = true;
            }
            //Links + Vorne
            else
            if (worldTiles[i - 1, j, k + 1] != null && worldTiles[i - 1, j, k + 1].gameObject.tag != "Diagonal" && worldTiles[i, j + 1, k + 1] != null && worldTiles[i, j + 1, k + 1].gameObject.tag != "Diagonal")
            {
                addToTiles(CornerCube, i, j, k + 2 - levels / 2, Vector3.zero, Quaternion.Euler(0, 90, 0), "Orientation i+1");
                used = true;
            }
            // Vorne + Rechts
            else
            if (worldTiles[i, j + 1, k + 1] != null && worldTiles[i, j + 1, k + 1].gameObject.tag != "Diagonal" && worldTiles[i + 1, j, k + 1] != null && worldTiles[i + 1, j, k + 1].gameObject.tag != "Diagonal")
            {
                addToTiles(CornerCube, i, j, k + 2 - levels / 2, Vector3.zero, Quaternion.Euler(0, 180, 0), "Orientation i+1");
                used = true;
            }
            return used;
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
        private int roundAndFlatHeight(float value)
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
                        var tile = addToTiles(WaterTile, x, y, z, translation, rotation);
                        // if (z < -levels / 2 + 2)
                        // {
                            
                        //     tile.landscapeType = LandscapeType.Water;
                        //     tile.GetComponent<MeshRenderer>().material = MaterialProvider.GetWorldTileMaterialByType(tile.GetComponent<WorldTile>().landscapeType);

                        // }
                        // else if (z < 0.7f * levels - levels / 2)
                        // {
                        //     var tile = addToTiles(GroundWorldTile, x, y, z, translation, rotation);
                        //     tile.landscapeType = LandscapeType.Earth;
                        //     tile.GetComponent<MeshRenderer>().material = MaterialProvider.GetWorldTileMaterialByType(tile.GetComponent<WorldTile>().landscapeType);

                        // }
                        // else
                        // {
                        //     var tile = addToTiles(Floor, x, y, z, translation, rotation);
                        //     tile.landscapeType = LandscapeType.Stone;
                        // }
                    }
                    slider.value = i;
                }
                yield return null;

            }
            StartCoroutine(flattening);
            print("Generating done");
        }
        public WorldTile addToTiles(WorldTile tileObject, int x, int y, int z, Vector3 translate, Quaternion rotate)
        {
            if(z < -levels /2 + 2) {
                tileObject.landscapeType = LandscapeType.Water;
            } else if(z < 0.7f * levels - levels/2) {
                tileObject.landscapeType = LandscapeType.Earth;
            } else  {
               tileObject.landscapeType = LandscapeType.Stone;

            }
            Vector3 position = new Vector3(x, z, y);
            Quaternion rotation = rotate;
            GameObject objectToBeAdded = (Instantiate(tileObject.gameObject, position + translate, rotate));
            objectToBeAdded.name = tileObject.name + "WorldTile at " + x + "," + y + "," + z;
            try
            {
                WorldTile tile = objectToBeAdded.GetComponent<WorldTile>();
                worldTiles[x, y, z + (levels / 2) - 1] = tile;
                tile.setCoords(x, y, z + (levels / 2) - 1);

            }
            catch (Exception e)
            {
                if(e is NullReferenceException) {
                    print(e);
                }
                print("Exception at:" + x + ", " + y + ", " + z);
            }
            objectToBeAdded.GetComponent<Renderer>().material = MaterialProvider.GetWorldTileMaterialByType(tileObject.landscapeType);

            objectToBeAdded.transform.SetParent(WorldContainer.transform);
            return objectToBeAdded.GetComponent<WorldTile>();
        }
        public void addToTiles(WorldTile tileObject, int x, int y, int z, Vector3 translate, Quaternion rotate, string orientation)
        {
            if(z < -levels /2 + 2) {
                tileObject.landscapeType = LandscapeType.Water;
            } else if(z < 0.7f * levels - levels/2) {
                tileObject.landscapeType = LandscapeType.Earth;
            } else  {
               tileObject.landscapeType = LandscapeType.Stone;

            }
            Vector3 position = new Vector3(x, z, y);
            Quaternion rotation = rotate;
            GameObject objectToBeAdded = (Instantiate(tileObject.gameObject, position + translate, rotate));
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
            objectToBeAdded.GetComponent<Renderer>().material = MaterialProvider.GetWorldTileMaterialByType(tileObject.landscapeType);

            objectToBeAdded.transform.SetParent(WorldContainer.transform);
        }
        public void addToTiles(int x, int y, int z)
        {
            Vector3 position = new Vector3(x, z, y);
            Quaternion rotation = new Quaternion(0, 0, 0, 0);
            GameObject objectToBeAdded = (Instantiate(this.worldTile.gameObject, position, rotation));
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
}