using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class WorldTile : MonoBehaviour

{

    public MeshFilter GetMeshFilter() {
        return this.GetComponent<MeshFilter>();
    }
    public MeshRenderer GetMeshRenderer() {
        return this.GetComponent<MeshRenderer>();
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
