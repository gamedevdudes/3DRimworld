using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunRotation : MonoBehaviour
{
    private bool rotating =false;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void setRotating() {
        rotating = !rotating;
    }
    // Update is called once per frame
    void Update()
    {
        if(rotating) {
            transform.RotateAround(new Vector3(0,0,0), new Vector3(0,1,0), 1);

        }
    }
}
