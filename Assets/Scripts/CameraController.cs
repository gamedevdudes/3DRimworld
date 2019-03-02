using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera camera;
    [SerializeField]
    private int currentHeight = 5;

    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            camera.transform.transform.localPosition +=new Vector3(0,0,0.1f);
        }
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            camera.transform.transform.localPosition+=(new Vector3(-0.1f,0,0));
        }
        if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            camera.transform.transform.localPosition+=(new Vector3(0,0,-0.1f));
        }
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            camera.transform.transform.localPosition+=(new Vector3(0.1f,0,0));
        }
    }
}
