using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------
public class RaycastScan : MonoBehaviour
//--------------------------------------------------
{
    //--------------------------------------------------
    private bool pointingAtPaintable;
	private MeshCollider meshCollider;
	private Mesh mesh;
	private Vector3[] vertices;
	private int[] triangles;
	public static Color[] meshColors;
   

	private int roverID;
	private float rayLength = 1f;
	//[SerializeField] float rayMultiplyFactor = 1f;//moved to rovercameramanager
	private LineRenderer lineRend;
	private Vector3 lineRendPos2 = new Vector3(0,0,0);    
    private Color roverHighlightColor = new Color();
    private Color initialMeshColor = new Color();
    private bool initialColorSet = false;
    private float highLightAlpha = 0.5f;
    private List<Color> roverColorsList = new List<Color>();
    private List<SimulationRoverManager> roverManagerList;
	private int prevTriangleChecked;

    private SimulationRoverManager simulationRoverManager;
    private SimulationManager simulationManager;
	private RoverCameraManager roverCameraManager;

    public static int numTrianglesPainted { get; private set; }
    public static float percentTrianglesPainted { get; private set; }
    //--------------------------------------------------

    //--------------------------------------------------
    private void Start()
    //--------------------------------------------------
    {
        simulationRoverManager = GetComponentInParent<SimulationRoverManager>();
        simulationManager = GameObject.Find("SimulationManager").GetComponent<SimulationManager>();
        roverCameraManager = simulationRoverManager.GetComponent<RoverCameraManager>();
        roverManagerList = simulationManager.GetRoverManagerList();

        //grabbing colors from list of rovers
        for (int i = 0; i < roverManagerList.Count; i++)
        {
            roverColorsList.Add(roverManagerList[i].GetRoverColor());
        }

        //setting alpha value of all colors in the list
        for (int i = 0; i < roverColorsList.Count; i++)
        {
            Color tempColor = roverColorsList[i];
            tempColor.a = highLightAlpha;
            roverColorsList[i] = tempColor;
        }

        roverID = simulationRoverManager.GetRoverID();
        roverHighlightColor = simulationRoverManager.GetRoverColor();
        roverHighlightColor.a = highLightAlpha;

        lineRend = GetComponent<LineRenderer>();
        lineRend.material.SetColor("_TintColor", roverHighlightColor);
    }
    //--------------------------------------------------


    //--------------------------------------------------
    private void Update()
    //--------------------------------------------------
    {


        lineRendPos2 = new Vector3 (0f, 0f, roverCameraManager.raycastLength);
		lineRend.SetPosition (1, lineRendPos2);//sets the end point of the line rend

        Ray raycast = new Ray(transform.position, transform.forward);
        RaycastHit hit;
		rayLength = transform.root.localScale.x * roverCameraManager.raycastLength * roverCameraManager.rayMultiplier;//ties ray length to local scale and multiplier
		raycast.origin = raycast.GetPoint (rayLength);
        raycast.direction = -raycast.direction;
		bool bHit = Physics.Raycast(raycast, out hit, rayLength);

		//if raycast hit something tagged paintable
        if (bHit && hit.transform.CompareTag("Paintable"))
        {
			//grab mesh data for the first time
            if (!pointingAtPaintable)
            {
                GetMeshInfo(hit);
                pointingAtPaintable = true;
            }

			//if we already cycled through this triangle, return
			if (hit.triangleIndex == prevTriangleChecked) {
				return;
			} else {
				prevTriangleChecked = hit.triangleIndex;
			}

			//check if this triangle hit has any unpainted vertex
			if (CheckTriangleHasUnpaintedVertex(hit.triangleIndex))
            {
				//if so, add to triangles painted count, and paint the triangle
				numTrianglesPainted++;                
                percentTrianglesPainted = (float)numTrianglesPainted / (float)(mesh.triangles.Length / 3f);
                PaintTriangleVertices (hit.triangleIndex);
            }
            else
            {
				return;//return will be much more performant, but continue will get more accurate results
            }           

            //set the vertices that we want to compare to
            Vector3 v1 = vertices[triangles[hit.triangleIndex * 3 + 0]];
            Vector3 v2 = vertices[triangles[hit.triangleIndex * 3 + 1]];
            Vector3 v3 = vertices[triangles[hit.triangleIndex * 3 + 2]];

            for (int i = 0; i < triangles.Length; i++)
            {
				//checks each triangle int to see if it matches any of the vertex above
                Vector3 currentVertex = vertices[triangles[i]];
                if (currentVertex == v1 || currentVertex == v2 || currentVertex == v3)
                {					
					//if there's a match, grab the index of the triangle by dividing by 3
					//so if i = 300, index = 100, and if i = 299, index = 99 (rounds down)
					//we want this behavior b/c we want all triangles that share the vertex as any of its 3 points, not just the ones where the shared vertex is its first point
                    int triangleN = i / 3;

                    if (triangleN != hit.triangleIndex)
                    {
						if (CheckTriangleHasUnpaintedVertex(triangleN))
                        {
                            numTrianglesPainted++;
                            percentTrianglesPainted = (float)numTrianglesPainted / (float)(mesh.triangles.Length / 3f);
                            PaintTriangleVertices (triangleN);  
                        }
                        else
                        {
							continue;
                        }                                              
                    }
                }
            }
            mesh.colors = meshColors;            
        }
        else
        {
            pointingAtPaintable = false;
        }
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void GetMeshInfo (RaycastHit hit)
    //--------------------------------------------------
    {
        meshCollider = hit.collider as MeshCollider;
        mesh = meshCollider.gameObject.GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        triangles = mesh.triangles;
        //Debug.Log("number total triangles = " + triangles.Length / 3);
        meshColors = new Color[vertices.Length];
        meshColors = mesh.colors;
        if (!initialColorSet)
        {
            initialMeshColor = mesh.colors[0];
            initialColorSet = true;
        }
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private bool CheckTriangleHasUnpaintedVertex(int triangleIndex)
    //--------------------------------------------------
    {
        if (meshColors[triangles[triangleIndex * 3 + 0]] == initialMeshColor ||
            meshColors[triangles[triangleIndex * 3 + 1]] == initialMeshColor ||
            meshColors[triangles[triangleIndex * 3 + 2]] == initialMeshColor)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void PaintTriangleVertices (int triangleIndex)
    //--------------------------------------------------
    {
        meshColors[triangles[triangleIndex * 3 + 0]] = roverHighlightColor;
		meshColors[triangles[triangleIndex * 3 + 1]] = roverHighlightColor;
		meshColors[triangles[triangleIndex * 3 + 2]] = roverHighlightColor;
	}
    //--------------------------------------------------
}
//--------------------------------------------------



