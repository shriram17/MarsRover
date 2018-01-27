using UnityEngine;

//--------------------------------------------------
public class ResetColor : MonoBehaviour
//--------------------------------------------------
{
    //--------------------------------------------------
    void Start ()
    //--------------------------------------------------
    {
        MeshFilter filter = GetComponent<MeshFilter> ();
		if (filter == null) {
			Debug.Log ("mesh filter not found");
			return;
		}
		Mesh mesh = filter.mesh;
		Vector3[] vertices = mesh.vertices;
		Color[] colors = new Color[vertices.Length];

		for (int i = 0; i < colors.Length; i++) {
			colors [i] = Color.white;
			colors [i].a = 0f;
		}
		mesh.colors = colors;
	}
    //--------------------------------------------------
}
//--------------------------------------------------
