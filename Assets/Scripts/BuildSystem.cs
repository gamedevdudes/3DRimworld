using BuildItems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using WorldGen;

public class BuildSystem : MonoBehaviour
{

    public WorldGrid worldGrid;
    public BuildItem Floor;
    public GameObject agentModel;
    private NavMeshAgent agent;
    public BuildTile EmptyTile;
    public Camera camera;
    public void BuildItem(BuildItem item) {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit)) {
            Transform objectHit = hit.transform;
            GameObject gameObject = objectHit.gameObject;
            WorldTile tile = gameObject.GetComponent<WorldTile>();
            Vector3 position = new Vector3(tile.getCoords().x, tile.getCoords().z - worldGrid.levels/2+1.51f, tile.getCoords().y);
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
    public void BuildAgent() {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit)) {
            Transform objectHit = hit.transform;
            GameObject gameObject = objectHit.gameObject;
            var agentObject = GameObject.Instantiate(agentModel,gameObject.transform.position + new Vector3(0,0.5f,0), Quaternion.identity);
            agent = agentObject.GetComponent<NavMeshAgent>();

        }

    }
    public void SetOnTrail() {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit)) {
            Transform objectHit = hit.transform;
            GameObject gameObject = objectHit.gameObject;
            if(agent != null) {
                agent.SetDestination(gameObject.transform.position);
            }
        }

    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.B)) {
            print("Trying to Build");
            BuildItem(Floor);
        }
        if(Input.GetKeyDown(KeyCode.T))
         {
             print("Spawning Agent");
            BuildAgent();
         }
         if(Input.GetKeyDown(KeyCode.G)) {
             print("Target chosen");
                SetOnTrail();
         }  

    }
}
