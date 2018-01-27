using UnityEngine;

//--------------------------------------------------
public class CameraViewRay : MonoBehaviour
//--------------------------------------------------
{
    //--------------------------------------------------
    private LineRenderer lineRend;
	private Vector3 lineRendPos2 = new Vector3(0,0,0); 

	private SimulationRoverManager simulationRoverManager;
	private RoverCameraManager roverCameraManager;
    //--------------------------------------------------


    //--------------------------------------------------
    void Start ()
    //--------------------------------------------------
    {
        lineRend = GetComponent<LineRenderer> ();  
		simulationRoverManager = GetComponentInParent<SimulationRoverManager>();
		roverCameraManager = simulationRoverManager.GetComponent<RoverCameraManager> ();
	}
    //--------------------------------------------------

    //--------------------------------------------------
    private void Update()
    //--------------------------------------------------
    {
        lineRendPos2 = new Vector3 (0f, 0f, roverCameraManager.raycastLength);
		lineRend.SetPosition (1, lineRendPos2);
	}
    //--------------------------------------------------
}
//--------------------------------------------------
