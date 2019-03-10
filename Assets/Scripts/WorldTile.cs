using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class WorldTile : MonoBehaviour

{

    public string Orientation= "";
    public MeshFilter GetMeshFilter() {
        return this.GetComponent<MeshFilter>();
    }
    public MeshRenderer GetMeshRenderer() {
        return this.GetComponent<MeshRenderer>();
    }
    public int x,y,z;
    public void setCoords(int x, int y , int z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public Vector3 getCoords() {
        return new Vector3(x,y,z);
    }
    // Start is called before the first frame update
    void Start()
    {
        this.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
