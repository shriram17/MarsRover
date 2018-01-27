using UnityEngine;

//--------------------------------------------------
public class AssignRenderTextures : MonoBehaviour
//--------------------------------------------------
{
    public Camera rendTextCamera;
    //public SimulationRoverManager simulationRoverManager;//was using this to access the rover #, no need for now

	private void Start () {
        rendTextCamera = GetComponent<Camera>();
        rendTextCamera.targetTexture = CameraFeedManager.instance.GetRenderTexture();
        //simulationRoverManager = GetComponentInParent<SimulationRoverManager>();
	}  
}
//--------------------------------------------------
