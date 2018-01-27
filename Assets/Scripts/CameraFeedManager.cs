using UnityEngine;
using UnityEngine.UI;

//--------------------------------------------------
public class CameraFeedManager : MonoBehaviour
//--------------------------------------------------
{
    //--------------------------------------------------
    public static CameraFeedManager instance;

    public RenderTexture[] rendTextArray;//holds references to render textures
    public Material noSignalMat;

    //public SimulationManager simManager;
    //public static int roverNum2; 
    //--------------------------------------------------


    //--------------------------------------------------
    [SerializeField] Material[] rendTextMatArray;
    [SerializeField] Text largeCamText;
    private int rendTextCount = -1;

    [SerializeField] GameObject videoFeedLarge;
    private Renderer videoFeedLargeRend;

    [SerializeField] GameObject[] videoFeeds;
    private Renderer[] videoFeedRend;

    private int cameraFeedNum = 1;
    //--------------------------------------------------


    //--------------------------------------------------
    private void Awake()
    //--------------------------------------------------
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void Start()
    //--------------------------------------------------
    {
        videoFeedLargeRend = videoFeedLarge.GetComponent<Renderer>();
        videoFeedLargeRend.material = noSignalMat;

        for (int i = 0; i < videoFeeds.Length; i++)
        {
            //videoFeedRend[i] = videoFeeds[i].GetComponent<Renderer>();
            videoFeeds[i].GetComponent<Renderer>().material = noSignalMat;
        }

        // videoFeedLargeRend.material = rendTextMatArray[0];

        //rendTextArray[0] = noSignalMat;

        //roverNum = simManager.roverNum;
        //Debug.Log(" Initial Rover Number " + roverNum2);
        largeCamText.text = "Rover " + cameraFeedNum;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void OnEnable()
    //--------------------------------------------------
    {
        EventsManager.OnSwitchRoverCamEvent += SwitchCam;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void OnDisable()
    //--------------------------------------------------
    {
        EventsManager.OnSwitchRoverCamEvent -= SwitchCam;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void SwitchCam(int index)
    //--------------------------------------------------
    {
        if (index > 4)
        {
            return;
        }

        cameraFeedNum = index + 1;
        largeCamText.text = "Rover " + cameraFeedNum;

        //Check if Rover exists first,not RenderTexture !!!!!!!!!!!!!!!!!!!
        if (rendTextMatArray[index] != null)
        {
            videoFeedLargeRend.material = rendTextMatArray[index];
        }
        else
        {
            videoFeedLargeRend.material = noSignalMat;
        }


        for (int i = 0; i < videoFeeds.Length; i++)
        {
            //Check if Rover Camera exists
            if (rendTextMatArray[i] != null)
                videoFeeds[i].GetComponent<Renderer>().material = rendTextMatArray[i];
            else
                videoFeeds[i].GetComponent<Renderer>().material = noSignalMat;

        }
    }
    //--------------------------------------------------

    //--------------------------------------------------
    //Rovers get their respective render textures at runtime 
    public RenderTexture GetRenderTexture ()
    //--------------------------------------------------
    {
        rendTextCount++;
        //Debug.Log("rend text count = " + rendTextCount);
        return rendTextArray[rendTextCount];        
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public void StartCameraFeeds()
    //--------------------------------------------------
    {
        for (int i = 0; i < videoFeeds.Length; i++)
        {
            //Check if Rover Camera exists
            if (rendTextMatArray[i] != null)
                videoFeeds[i].GetComponent<Renderer>().material = rendTextMatArray[i];
            else
                videoFeeds[i].GetComponent<Renderer>().material = noSignalMat;
        }
    }
    //--------------------------------------------------
}
//--------------------------------------------------
