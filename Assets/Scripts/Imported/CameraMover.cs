using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour {


    public Transform vrCameraRig;

	void Start()
    {

    }
	
	void Update()
    {
        // Warp to one end of the cave
		if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            vrCameraRig.transform.position = new Vector3(-68.099f, -6.535f, -5.684f);
            vrCameraRig.transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
        }

        // Warp to surface
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            vrCameraRig.transform.position = new Vector3(-28.52131f, 5.896158f, -11.92506f);
            vrCameraRig.transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
        }

        // Warp to other end of the cave
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            vrCameraRig.transform.position = new Vector3(11.31404f, 9.68243f, -0.4488343f);
            vrCameraRig.transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
        }


    }

}