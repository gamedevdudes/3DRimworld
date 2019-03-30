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
        public float WaterLevel = 0.3f;
        public BuildTile[,,] tiles;

        private WorldTile[,,] worldTiles;
        public int width = 150, height = 150, levels = 20;
        public int factor;
        public Slider slider;
        public bool grass = false;
        private float heightOffset = 0.5f;
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
        public Mesh planeMesh;
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
        /// <summary> 
        /// Starts the coroutine for generating a world with the seed 0. This will also call <see cref="flattenLandscape"/>
        /// <summary/>
        public void generateWorldWithoutRandomSeed()
        {
            worldGeneration = generatePerlinWorld(0);
            StartCoroutine(worldGeneration);
            //generatePerlinWorld(randomseed);
        }
        /// <summary> 
        /// Starts the coroutine for generatig a world with a random seed. For this you need to have set the  <see cref="width"/>, the <see cref="height"/> (depth), the <see cref="factor"/> and the <see cref="levels"/>.
        /// The factor sets the amount of height differences that occur.
        /// Levels sets the maximum difference between lowest and highest height.
        /// Height sets the amount of block in the z axis
        /// Width sets the amount of blocks in the x axis
        /// This will also call <see cref="flattenLandscape"/>
        /// <summary/>
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
        /// <summary>
        /// Finishes the world generation in disabling the visibility of the slider, stopping the navmesh generation and combining the generated mesh with the <see cref="StaticBatchingUtility"/>
        /// <summary/>
        private void finishGeneration()
        {
            if (grass)
            {
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
        /// <summary>
        /// Generates bunches of Grass Objects. Currently not used. Is toggled whe <see cref="grass"/> = true.
        /// <summary/>
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
        /// <summary>
        /// Smoothens the landscape by adding diagonal Tiles to the world. To generate the Tiles it used <see cref="addDiagonalCorners"/> and <see cref="addFlattening"/>
        /// <summary/>
        IEnumerator flattenLandscape()
        {
            slider.maxValue = levels;
            slider.value = 0;
            for (int z = 0; z < levels - 1; z++)
            {
                //depth
                for (int y = 1; y < height - 1; y++)
                {
                    // width 
                    for (int x = 1; x < width - 1; x++)

                    {
                        WorldTile tile = worldTiles[x, y, z];
                        if (tile != null)
                        {
                            if (worldTiles[x + 1, y, z] != null && worldTiles[x - 1, y, z] != null && worldTiles[x, y + 1, z] != null && worldTiles[x, y - 1, z] != null)
                            {
                                tile.GetComponent<MeshRenderer>().shadowCastingMode = 0;
                                tile.GetMeshFilter().mesh = planeMesh;
                                tile.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                                tile.transform.position += new Vector3(0, 0.5f, 0);
                            }
                            else
                            {
                                //Rechts + Hinten
                                addFlattening(x, y, z);
                            }

                        } else
                        {
                            if(z < WaterLevel)
                            {
                                var groundTile = addToTiles(GroundWorldTile, x, y, z - 1, Vector3.zero, Quaternion.identity);

                            }
                        }
                    }
                }

                slider.value = z;
                yield return null;
            }
            slider.maxValue = levels;
            slider.value = 0;
            for (int z = 0; z < levels - 1; z++)

            {
                for (int y = 1; y < height - 1; y++)
                {
                    for (int x = 1; x < width - 1; x++)

                    {
                        WorldTile tile = worldTiles[x, y, z];
                        if (tile == null)
                        {
                            addDiagonalCorners(x, y, z);

                        }
                    }
                }

                slider.value = z;
                yield return null;
            }
            print("Flattening done");
            finishGeneration();

        }
        /// <summary>
        /// Adds CornerStones to further smoothen edges. It determines if there already are Diagonal tiles nearby and adds a corner Stone if thats the case.
        /// <summary/>
        private void addDiagonalCorners(int x, int y, int z)
        {
            WorldTile tile = null;
            const float startRotation = 180.0f;
            if (z + 1 < levels) // && worldTiles[x, y, z] != null && worldTiles[x, y, z].gameObject.tag != "Diagonal" && worldTiles[x, y, z].gameObject.tag != "DiagonalCorner")
            {


                //Rechts & Hinten
                if (worldTiles[x + 1, y, z + 1] != null && worldTiles[x, y + 1, z + 1] != null && (worldTiles[x + 1, y, z + 1].tag == "Diagonal" || worldTiles[x, y + 1, z + 1].tag == "Diagonal"))
                {
                    tile = addToTiles(DiagonalCornerTile, x, y, z + 1, Vector3.zero, Quaternion.Euler(-90, startRotation, 0), "DiagonalCorner i+1 j+1");

                }
                else
                    //Rechts und Vorne
                    if (worldTiles[x + 1, y, z + 1] != null && worldTiles[x + 1, y, z + 1].tag == "Diagonal" && worldTiles[x, y - 1, z + 1] != null && worldTiles[x, y - 1, z + 1].tag == "Diagonal")
                {
                    tile = addToTiles(DiagonalCornerTile, x, y, z + 1, Vector3.zero, Quaternion.Euler(-90, startRotation + 90 % 360, 0), "DiagonalCorner i+1 j-1");

                }
                else
                    //Links und Vorne
                    if (worldTiles[x - 1, y, z + 1] != null && worldTiles[x - 1, y, z + 1].tag == "Diagonal" && worldTiles[x, y - 1, z + 1] != null && worldTiles[x, y - 1, z + 1].tag == "Diagonal")
                {
                    tile = addToTiles(DiagonalCornerTile, x, y, z + 1, Vector3.zero, Quaternion.Euler(-90, startRotation + 180 % 360, 0), "DiagonalCorner i-1 j-1");

                }
                else
                //Links und Hinten
                if (worldTiles[x - 1, y, z + 1] != null && worldTiles[x - 1, y, z + 1].tag == "Diagonal" && worldTiles[x, y + 1, z + 1] != null && worldTiles[x, y + 1, z + 1].tag == "Diagonal")
                {
                    tile = addToTiles(DiagonalCornerTile, x, y, z + 1, Vector3.zero, Quaternion.Euler(-90, startRotation + 270 % 360, 0), "DiagonalCorner i-1 j+1");
                }
                tile?.setCoords(x, y, z + 1);
            }
        }
        /// <summary>
        /// Adds smoothening DiagonalTiles to make the Landscape better. 
        /// <summary/>
        /// <param name="x"> The x coordinate <param/>
        /// <param name="y"> The y coordinate <param/>
        /// <param name="z"> The z coordinate <param/>
        private void addFlattening(int x, int y, int z)
        {
            WorldTile tile = null;
            if (x - 1 <= 0 || x + 1 >= width || y - 1 <= 0 || y + 1 >= height || z - 1 < 0 || z + 1 >= levels)
            {
                return;
            }
            else
            {
                tile = addDiagonalGround(x, y, z, tile);
                if (tile == null)
                {
                    addCornerCubeDoubleSided(x, y, z);
                }
            }
            tile?.setCoords(x, y, z + 2);

        }
        /// <summary>
        /// Man gibt die Koordinaten eines WorldTiles an um herauszufinden, ob es sich auf einer Steigung befindet. 
        /// Wenn es das tut, wird eine Rotation zurück gegeben, mit der man berechnen kann in welche Richtung die Steigung ist.
        /// Gibt die nötige Rotation zurück um an eine Diagonale einzufügen.
        /// </summary>
        /// <param name="x"> Die X Koordinate des WorldTiles</param>
        /// <param name="y">Die Y Koordinate des WorldTiles</param>
        /// <param name="z">Die Z Koordinate des WorldTiles</param>
        /// <returns> Ein Int zwischen 0 und 3 zum Multiplizieren mit 90Grad. Wenn -1 zurückgegeben wird gab es einen Fehler. </returns>
        private int DifferentiateWorldTiles(int x,int y, int z)
        {
            if (worldTiles[x + 1, y, z + 1] != null && worldTiles[x, y, z] != null && worldTiles[x + 1, y, z + 1].gameObject.tag != "Diagonal")
            {
                return 3;
            } else if(worldTiles[x, y+1, z + 1] != null && worldTiles[x, y, z] != null && worldTiles[x, y+1, z + 1].gameObject.tag != "Diagonal")
            {
                return 2;
            } else if(worldTiles[x - 1, y, z + 1] != null && worldTiles[x, y, z] != null && worldTiles[x - 1, y, z + 1].gameObject.tag != "Diagonal")
            {
                return 1;
            } else if(worldTiles[x, y-1, z + 1] != null && worldTiles[x, y, z] != null && worldTiles[x, y-1, z + 1].gameObject.tag != "Diagonal")
            {
                return 0;
            }
            return -1;

        }

        private WorldTile addDiagonalGround(int x, int y, int z, WorldTile tile)
        {
            if(z+1 < levels)
            {
                float rotator = DifferentiateWorldTiles(x, y, z + 1);
                if (rotator > -1)
                {
                    tile = addToTiles(DiagonalWorldTile, x, y, z + 2, Vector3.zero, Quaternion.Euler(0, rotator * 90.0f, 0), rotator + "");

                }
            }
            

            /**
            if (worldTiles[x + 1, y, z + 1] != null && worldTiles[x - 1, y, z] != null && worldTiles[x + 1, y, z + 1].gameObject.tag != "Diagonal" && worldTiles[x - 1, y, z].gameObject.tag != "Diagonal")
            {
                tile = addToTiles(DiagonalWorldTile, x, y, z + 2, Vector3.zero, Quaternion.Euler(0, 270.0f, 0), "Orientation x+1,x-1");
            }
            else
            if (worldTiles[x, y - 1, z + 1] != null && worldTiles[x, y + 1, z] != null && worldTiles[x, y - 1, z + 1].gameObject.tag != "Diagonal" && worldTiles[x, y + 1, z].gameObject.tag != "Diagonal")
            {
                tile = addToTiles(DiagonalWorldTile, x, y, z + 2, Vector3.zero, Quaternion.Euler(0, startRotation + 90 % 360, 0), "Orientation y-1,y+1");
            }
            else
            if (worldTiles[x - 1, y, z + 1] != null && worldTiles[x + 1, y, z] != null && worldTiles[x - 1, y, z + 1].gameObject.tag != "Diagonal" && worldTiles[x + 1, y, z].gameObject.tag != "Diagonal")
            {
                tile = addToTiles(DiagonalWorldTile, x, y, z + 2, Vector3.zero, Quaternion.Euler(0, startRotation + 180 % 360, 0), "Orientation x-1,x+1");
            }
            else
            if (worldTiles[x, y + 1, z + 1] != null && worldTiles[x, y - 1, z] != null && worldTiles[x, y + 1, z + 1].gameObject.tag != "Diagonal" && worldTiles[x, y - 1, z].gameObject.tag != "Diagonal")
            {
                tile = addToTiles(DiagonalWorldTile, x, y, z + 2, Vector3.zero, Quaternion.Euler(0, startRotation + 270 % 360, 0), "Orientation y+1,y-1");
            }
            */
            return tile;
        }

        public void clearChildren()
        {
            foreach (Transform t in WorldContainer.transform)
            {
                GameObject.Destroy(t.gameObject);
            }
        }
        private bool addCornerCubeDoubleSided(int x, int y, int z)
        {
            const float startRotation = 270.0f;
            bool used = false;
            z++;
            if (worldTiles[x + 1, y, z] != null && !(worldTiles[x + 1, y, z].gameObject.tag == "Diagonal" || worldTiles[x + 1, y, z].gameObject.tag == "DiagonalCorner") && worldTiles[x, y - 1, z] != null && !(worldTiles[x, y - 1, z].gameObject.tag == "Diagonal" || worldTiles[x, y - 1, z].gameObject.tag == "DiagonalCorner"))
            {
                addToTiles(CornerCube, x, y, z + 1, Vector3.zero, Quaternion.Euler(0, startRotation, 0), "Orientation x+1,y-1");
                used = true;
            }
            //Hinten + Links
            else
            if (worldTiles[x, y - 1, z] != null && !(worldTiles[x, y - 1, z].gameObject.tag == "Diagonal" || worldTiles[x, y - 1, z].gameObject.tag == "DiagonalCorner") && worldTiles[x - 1, y, z] != null && !(worldTiles[x - 1, y, z].gameObject.tag == "Diagonal" || worldTiles[x - 1, y, z].gameObject.tag == "DiagonalCorner"))
            {
                addToTiles(CornerCube, x, y, z + 1, Vector3.zero, Quaternion.Euler(0, startRotation + 90 % 360, 0), "Orientation y-1, x-1");
                used = true;
            }
            //Links + Vorne
            else
            if (worldTiles[x - 1, y, z] != null && !(worldTiles[x - 1, y, z].gameObject.tag == "Diagonal" || worldTiles[x - 1, y, z].gameObject.tag == "DiagonalCorner") && worldTiles[x, y + 1, z] != null && !(worldTiles[x, y + 1, z].gameObject.tag != "Diagonal" || worldTiles[x, y + 1, z].gameObject.tag == "DiagonalCorner"))
            {
                addToTiles(CornerCube, x, y, z + 1, Vector3.zero, Quaternion.Euler(0, startRotation + 180 % 360, 0), "Orientation y+1, x-1");
                used = true;
            }
            // Vorne + Rechts
            else
            if (worldTiles[x, y + 1, z] != null && !(worldTiles[x, y + 1, z].gameObject.tag == "Diagonal" || worldTiles[x, y + 1, z].gameObject.tag == "DiagonalCorner") && worldTiles[x + 1, y, z] != null && !(worldTiles[x + 1, y, z].gameObject.tag == "Diagonal" || worldTiles[x + 1, y, z].gameObject.tag == "DiagonalCorner"))
            {
                addToTiles(CornerCube, x, y, z + 1, Vector3.zero, Quaternion.Euler(0, startRotation + 270 % 360, 0), "Orientation y+1,x+1");
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
            float mappedValue = value * levels;
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
                    if (x < width && y < height && z < levels)
                    {
                        var tile = addToTiles(GroundWorldTile, x, y, z, translation, rotation);
                        //tile?.GetComponent<WorldTile>().setCoords(x, y, z);
                        if (z > 0)
                        {
                            var groundTile = addToTiles(GroundWorldTile, x, y, z - 1, translation, rotation);
                            //groundTile?.GetComponent<WorldTile>().setCoords(x, y, z-1);
                        }
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
            if (z >= 0 && z < levels && worldTiles[x, y, z] != null)
            {
                return null;
            }
            if (z < WaterLevel * levels)
            {
                tileObject.landscapeType = LandscapeType.Water;
            }
            else if (z < 0.7f * levels)
            {
                tileObject.landscapeType = LandscapeType.Earth;
            }
            else
            {
                tileObject.landscapeType = LandscapeType.Stone;

            }
            Vector3 position = new Vector3(x, z, y);
            Quaternion rotation = rotate;
            GameObject objectToBeAdded = (Instantiate(tileObject.gameObject, position + translate, rotate));
            objectToBeAdded.name = tileObject.name + "WorldTile at " + x + "," + y + "," + z;
            try
            {
                WorldTile tile = objectToBeAdded.GetComponent<WorldTile>();
                worldTiles[x, y, z] = tile;
                tile.setCoords(x, y, z);
            }
            catch (Exception e)
            {
                if (e is NullReferenceException)
                {
                    print(e);
                }
                print("Exception at:" + x + ", " + y + ", " + z);
            }
            objectToBeAdded.GetComponent<Renderer>().material = MaterialProvider.GetWorldTileMaterialByType(tileObject.landscapeType);

            objectToBeAdded.transform.SetParent(WorldContainer.transform);
            return objectToBeAdded.GetComponent<WorldTile>();
        }
        public WorldTile addToTiles(WorldTile tileObject, int x, int y, int z, Vector3 translate, Quaternion rotate, string orientation)
        {
            if (z >= 0 && z < levels && worldTiles[x, y, z] != null)
            {
                return null;
            }
            if (z < 0.3f * levels)
            {
                tileObject.landscapeType = LandscapeType.Water;
            }
            else if (z < 0.7f * levels)
            {
                tileObject.landscapeType = LandscapeType.Earth;
            }
            else
            {
                tileObject.landscapeType = LandscapeType.Stone;

            }
            Vector3 position = new Vector3(x, z, y);
            Quaternion rotation = rotate;
            GameObject objectToBeAdded = (Instantiate(tileObject.gameObject, position, rotate));
            objectToBeAdded.name = tileObject.name + "WorldTile at " + x + "," + y + "," + z;
            try
            {
                WorldTile tile = objectToBeAdded.GetComponent<WorldTile>();
                worldTiles[x, y, z] = tile;
                tile.Orientation = orientation;
                tile.setCoords(x, y, z);


            }
            catch (IndexOutOfRangeException e)
            {
                print("Exception at:" + x + ", " + y + ", " + z);
            }
            objectToBeAdded.GetComponent<Renderer>().material = MaterialProvider.GetWorldTileMaterialByType(tileObject.landscapeType);

            objectToBeAdded.transform.SetParent(WorldContainer.transform);
            return objectToBeAdded.GetComponent<WorldTile>();
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