using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera camera;
    private Transform cameraTransform;
    [SerializeField]
    private int currentHeight = 5;

    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
        cameraTransform = camera.transform;
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            camera.transform.transform.localPosition += new Vector3(0, 0, 1f);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            camera.transform.transform.localPosition += (new Vector3(-1f, 0, 0));
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            camera.transform.transform.localPosition += (new Vector3(0, 0, -1f));
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            camera.transform.transform.localPosition += (new Vector3(1f, 0, 0));
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (currentHeight < 45)
            {
                currentHeight++;
                float x = camera.transform.position.x;
                float z = camera.transform.position.z;
                camera.transform.transform.position = new Vector3(x, currentHeight, z);
            }

        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (currentHeight > 0)
            {
                //Vector3 position = cameraTransform.position;
                currentHeight--;
                float x = camera.transform.position.x;
                float z = camera.transform.position.z;
                //Vector3 targetPosition = new Vector3(x,currentHeight,z);
                //camera.transform.transform.position = Vector3.Lerp(cameraTransform.position, targetPosition,1);
                camera.transform.transform.position = new Vector3(x, currentHeight, z);

            }
        }

    }
    // Update is called once per frame
    void Update()
    {
    }

}
