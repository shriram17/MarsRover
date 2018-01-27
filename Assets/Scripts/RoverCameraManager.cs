using UnityEngine;

//--------------------------------------------------
public class RoverCameraManager : MonoBehaviour
//--------------------------------------------------
{
    //--------------------------------------------------
    private RaycastScan[] raycastScanArray;
	public Camera RoverCam;
	private float raycastAngle;
	public float raycastLength;
	public float rayMultiplier;//factor used to adjust raycast length to match the linerender lengths
    [SerializeField] float cameraFieldOfView;
    [SerializeField] float cameraDepthOfView;

	private float previousRaycastAngle;

	private CameraViewRay[] cameraViewRayArray;
	private float cameraViewRayAngle;
	private float previousCameraViewRayAngle;
    //--------------------------------------------------


    //--------------------------------------------------
    void Start ()
    //--------------------------------------------------
    {
        raycastScanArray = GetComponentsInChildren<RaycastScan> ();
		cameraViewRayArray = GetComponentsInChildren<CameraViewRay> ();
	}
    //--------------------------------------------------


    //--------------------------------------------------
    void Update ()
    //--------------------------------------------------
    {
        #region Setting raycast angle and lengths
        RoverCam.farClipPlane = cameraDepthOfView * transform.root.localScale.x * 0.5f;//arbitrary equation to get an input of "10" to equal to rough estimation of 10M in simulation
        RoverCam.fieldOfView = cameraFieldOfView;

        raycastAngle = RoverCam.fieldOfView / 3;//arbitrary equation to offset the raycast from the camera view boundaries
        raycastLength = RoverCam.farClipPlane / transform.root.localScale.x * 1.35f;//arbitrary equation to get raycast length to be 6.75
        //Debug.Log("ray length = " + raycastLength);


		//I got this equation above b/c when I set farclip to 10 on the camera, the length was about 22.5 to hit triangle
		//which meant when farclip is 10, then it's = farclipPlan * raycastAngle / 10 = 10*22.5/10, 
		//but farclip at 10 is too far, so I lowered it to 3 (visually, about 6 rover lengths, since rovers are 1.6M long and camera viewing length is 10M
		//that's why we got 3 * 22.5 /10 = 6.75 as the ray length
        //later I changed the equations but still using 6.75 as the ray length
		//I tested changing the farclip and camera angles, and the behaviors seem to hold

		if (raycastAngle != previousRaycastAngle) {
			for (int i = 0; i < raycastScanArray.Length; i++) {
				switch (i) {
				//Set 1 of raycasts -------------------------------------------------------
				case 0: //TopRight
					raycastScanArray[i].transform.localEulerAngles = new Vector3(-raycastAngle, raycastAngle, 0f);
					break;
				case 1: //BottomRight
					raycastScanArray[i].transform.localEulerAngles = new Vector3(raycastAngle, raycastAngle, 0f);
					break;
				case 2: //center
					raycastScanArray[i].transform.localEulerAngles = new Vector3(0f, 0f, 0f);
					break;
				case 3: //TopLeft
					raycastScanArray[i].transform.localEulerAngles = new Vector3(-raycastAngle, -raycastAngle, 0f);
					break;
				case 4: //BottomLeft
					raycastScanArray[i].transform.localEulerAngles = new Vector3(raycastAngle, -raycastAngle, 0f);
					break;
				//Set 2 of raycasts-----------------------------------------------------
				case 5: //Right
					raycastScanArray[i].transform.localEulerAngles = new Vector3(0f, -raycastAngle, 0f);
					break;
				case 6: //Top
					raycastScanArray[i].transform.localEulerAngles = new Vector3(raycastAngle, 0f, 0f);
					break;
				case 7: //Left
					raycastScanArray[i].transform.localEulerAngles = new Vector3(0f, raycastAngle, 0f);
					break;
				case 8: //Bottom
					raycastScanArray[i].transform.localEulerAngles = new Vector3(-raycastAngle, 0f, 0f);
					break;
				//Set 3 of raycasts-----------------------------------------------------
				case 9: //Right
					raycastScanArray[i].transform.localEulerAngles = new Vector3(-raycastAngle/2, raycastAngle/2, 0f);
					break;
				case 10: //Top
					raycastScanArray[i].transform.localEulerAngles = new Vector3(raycastAngle/2, raycastAngle/2, 0f);
					break;
				case 11: //Left
					raycastScanArray[i].transform.localEulerAngles = new Vector3(-raycastAngle/2, -raycastAngle/2, 0f);
					break;
				case 12: //Bottom
					raycastScanArray[i].transform.localEulerAngles = new Vector3(raycastAngle/2, -raycastAngle/2, 0f);
					break;
				//Set 4 of raycasts-----------------------------------------------------
				case 13: //Right
					raycastScanArray[i].transform.localEulerAngles = new Vector3(0f, -raycastAngle/2, 0f);
					break;
				case 14: //Top
					raycastScanArray[i].transform.localEulerAngles = new Vector3(raycastAngle/2, 0f, 0f);
					break;
				case 15: //Left
					raycastScanArray[i].transform.localEulerAngles = new Vector3(0f, raycastAngle/2, 0f);
					break;
				case 16: //Bottom
					raycastScanArray[i].transform.localEulerAngles = new Vector3(-raycastAngle/2, 0f, 0f);
					break;
				}
			}
		}
		previousRaycastAngle = raycastAngle;
		#endregion


		#region Setting Camera line renders
		cameraViewRayAngle = RoverCam.fieldOfView / 2;

		if (cameraViewRayAngle != previousCameraViewRayAngle) {
			for (int i = 0; i < cameraViewRayArray.Length; i++) {
				switch (i) {
				case 0: //TopRight
					cameraViewRayArray[i].transform.localEulerAngles = new Vector3(-cameraViewRayAngle, cameraViewRayAngle, 0f);
					break;
				case 1: //BottomRight
					cameraViewRayArray[i].transform.localEulerAngles = new Vector3(cameraViewRayAngle, cameraViewRayAngle, 0f);
					break;			
				case 2: //TopLeft
					cameraViewRayArray[i].transform.localEulerAngles = new Vector3(-cameraViewRayAngle, -cameraViewRayAngle, 0f);
					break;
				case 3: //BottomLeft
					cameraViewRayArray[i].transform.localEulerAngles = new Vector3(cameraViewRayAngle, -cameraViewRayAngle, 0f);
					break;
				}
			}
		}
		previousCameraViewRayAngle = cameraViewRayAngle;
		#endregion
	}
    //--------------------------------------------------

    //--------------------------------------------------
    public void SetCameraViewParameters (float angle, float depth)
    //--------------------------------------------------
    {
        cameraFieldOfView = angle;
        cameraDepthOfView = depth;
    }
    //--------------------------------------------------
}
//--------------------------------------------------
