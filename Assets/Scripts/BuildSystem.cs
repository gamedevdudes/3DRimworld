using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSystem : MonoBehaviour
{

    public WorldGrid worldGrid;
    public BuildItem Floor;
    public BuildTile EmptyTile;
    public Camera camera;
    public void BuildItem(BuildItem item) {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit)) {
            Transform objectHit = hit.transform;
            GameObject gameObject = objectHit.gameObject;
            WorldTile tile = gameObject.GetComponent<WorldTile>();
            Vector3 position = new Vector3(tile.getCoords().x, tile.getCoords().z - WorldGrid.levels/2+1.51f, tile.getCoords().y);
            BuildTile buildTile = Instantiate(EmptyTile,position, Quaternion.Euler(0,0,0));
            buildTile.gameObject.name = "BuiltTile at: " + tile.getCoords().ToString();
            BuildItem floor = Instantiate(Floor, position, Quaternion.Euler(0,0,0));
            buildTile.SetFloor(floor);
        }

    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.B)) {
            print("Trying to Build");
            BuildItem(Floor);
        }
    }
}
