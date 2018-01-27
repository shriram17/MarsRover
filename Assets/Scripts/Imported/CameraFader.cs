using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFader : MonoBehaviour {

	void Start()
    {
        //FlipNormals();
    }
	
	void Update()
    {
		
	}

    private void FlipNormals()
    {
        Debug.Log("flipping normals");

        Mesh mesh = this.GetComponent<MeshFilter>().mesh;

        Vector3[] normals = mesh.normals;

        Debug.Log(mesh.normals[0]);

        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = -normals[i];
        }

        mesh.normals = normals;
        Debug.Log(mesh.normals[0]);
    }
}
