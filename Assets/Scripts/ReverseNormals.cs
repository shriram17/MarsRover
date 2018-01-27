using UnityEngine;

//--------------------------------------------------
public class ReverseNormals : MonoBehaviour
//--------------------------------------------------
{
    //--------------------------------------------------
    void Start()
    //--------------------------------------------------
    {
        MeshFilter filter = GetComponent<MeshFilter> ();
		if (filter == null) {
			Debug.Log ("mesh filter not found");
			return;
		}
        
        Mesh mesh = filter.mesh;

        Vector3[] normals = mesh.normals;
        for (int i = 0; i < normals.Length; i++)
            normals[i] = -normals[i];
        mesh.normals = normals;  

		int[] triangles = mesh.triangles;
		for (int i = 0; i < triangles.Length; i += 3)
		{
			int temp = triangles[i + 0];
			triangles[i + 0] = triangles[i + 1];
			triangles[i + 1] = temp;
		}
		mesh.triangles = triangles;

		// for submeshes-----------------------------
//            for (int m = 0; m < mesh.subMeshCount; m++)
//            {
//                //Debug.Log("submesh count = " + mesh.subMeshCount);
//                int[] triangles = mesh.GetTriangles(m);
//                for (int i = 0; i < triangles.Length; i += 3)
//                {
//                    int temp = triangles[i + 0];
//                    triangles[i + 0] = triangles[i + 1];
//                    triangles[i + 1] = temp;
//                }
//                mesh.SetTriangles(triangles, m);
//            }
        
    }
    //--------------------------------------------------
}
//--------------------------------------------------
