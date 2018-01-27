using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

//--------------------------------------------------
public delegate void TeleportCallback();
//--------------------------------------------------

//--------------------------------------------------
public class SimulationManager : MonoBehaviour
//--------------------------------------------------
{
    #region PUBLIC VARIABLES
    //--------------------------------------------------
    public static SimulationManager singleton = null;

    [Header("Debug toggles")]
    public bool parseDataAsynch = true;
    public bool loadDataAsynch = false;

    [Header("Time variables")]
    public float timeIncrement = 1f;        // Data rate at 1 per second
    public float initialTimestamp = -1f;
    public float finalTimestamp = -1f;

    [Header("Game state conditions")]
    public bool controllersTracked = false;
    public bool tunnelScanCompleted = false;

    public enum GameState
    {
        PRE_DATA_LOAD,
        LOADING,
        SIMULATION_LOADED
    }
    public GameState currentGameState = GameState.PRE_DATA_LOAD;

    public static int roverNum;
    //--------------------------------------------------
    #endregion PUBLIC VARIABLES


    #region PRIVATE VARIABLES
    //--------------------------------------------------
    #region PRIVATE VARIABLES: Serialize field
    //--------------------------------------------------
    [Header("References in Hierarchy")]
    [SerializeField] private DataManager m_dataManager;
    [SerializeField] private GameObject m_cameraFeed;
    [SerializeField] private Text m_largeFeedRoverInfo;
    [SerializeField] private Text m_largeFeedTimeInfo;
    [SerializeField] private GameObject m_controlRoom;
    [SerializeField] private Transform m_playerStartTransform;
    [SerializeField] private Transform m_mapStartTransform;
    [SerializeField] private GameObject m_mapInHierarchy;
    [SerializeField] private GameObject m_dataSlices;
    [SerializeField] private GameObject m_scientistPlayer;
    [SerializeField] private Text m_handUIInfoFeed;
    [SerializeField] private GameObject m_tunnelScanner;
    [SerializeField] private GameObject m_preLoadUI;

    [Header("Prefabs")]
    [SerializeField] private GameObject m_roverPrefab;

    [Header("Rover settings")]
    [SerializeField] private Color[] m_roverColors =
    {
        Color.blue,
        Color.red,
        Color.green,
        Color.yellow,
        Color.cyan,
        Color.magenta
    };
    [SerializeField] private float cameraFieldOfView = 45f;
    [SerializeField] private float cameraDepthOfView = 10f;
    //--------------------------------------------------
    #endregion PRIVATE VARIABLES: Serialize field

    private List<GameObject> m_rovers = new List<GameObject>();
    private List<SimulationRoverManager> m_roverManagers = 
        new List<SimulationRoverManager>();
    private int m_maxDataSampleIndex = 0;
    private int m_currentDataSampleIndex = 0;
    private int m_indexOfSelectedRover = -1;
    private bool m_playing = false;
    private float m_timeUntilNextUpdate = 0f;
    private int m_playbackSpeedIndex = 0;
    private int[] m_playbackSpeeds = 
        { 1, 4, 8, 20, 60, 120 };

    private Vector3 m_previousMapPosition;
    private Vector3 m_previousMapScale;

    private bool m_tunnelScanStarted = false;

    private TeleportCallback teleportCallback;
    //--------------------------------------------------
    #endregion PRIVATE VARIABLES


    #region MONOBEHAVIOUR
    //--------------------------------------------------
    private void Awake()
    //--------------------------------------------------
    {
        if (null != singleton)
        {
            DestroyImmediate(this.gameObject);
        }
        singleton = this;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void Start()
    //--------------------------------------------------
    {
        m_previousMapPosition = m_mapInHierarchy.transform.position;
        m_previousMapScale = m_mapInHierarchy.transform.localScale;
        m_cameraFeed.SetActive(false);
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void Update()
    //--------------------------------------------------
    {
        if (controllersTracked && !m_tunnelScanStarted)
        {
            m_tunnelScanner.GetComponent<TunnelSliceScanner>().
                  StartScan();
            m_tunnelScanStarted = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_playing = !m_playing;
        }

        if (m_maxDataSampleIndex > 0 && m_playing)
        {
            if (Time.time >= m_timeUntilNextUpdate)
            {
                PlaySimulation();
                m_timeUntilNextUpdate = Time.time + timeIncrement;
            }
        }

        if (m_indexOfSelectedRover > -1)
        {

            RoverInfoDisplay();
            CameraFeedDisplay();
        }
    }
    //--------------------------------------------------
    #endregion MONOBEHAVIOUR


    #region PUBLIC METHODS
    #region PUBLIC METHODS: Managing game objects
    //--------------------------------------------------
    public void LoadDataFile(ref string filename)
    //--------------------------------------------------
    {
        m_dataManager.LoadDataFile(ref filename);
        if (!m_dataManager.InitializeData())
        {
            Debug.Log("Unable to load selected data file");
            return;
        }

        StartCoroutine(IE_LoadingData());
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public void TunnelSliceScanComplete()
    //--------------------------------------------------
    {
        tunnelScanCompleted = true;

        HandUIManager handUI = m_scientistPlayer.GetComponentInChildren<HandUIManager>();
        handUI.SwitchHandUI(HandUIManager.HandUI.DATA_SELECTOR);
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public GameObject InstantiateRover(
                    int id,
                    ref List<float> timestamps,
                    ref Dictionary<float, Vector3> positions,
                    ref Dictionary<float, Quaternion> rotations)
    //--------------------------------------------------
    {
        GameObject curRover = Instantiate(m_roverPrefab, null);
        m_rovers.Add(curRover);

        SimulationRoverManager roverManager = 
            curRover.GetComponent<SimulationRoverManager>();
        m_roverManagers.Add(roverManager);

        int colorIndex = m_rovers.Count - 1;
        Color curRoverColor;
        if (colorIndex >= m_roverColors.Length)
        {
            curRoverColor = new Color(
                UnityEngine.Random.Range(0f, 255f),
                UnityEngine.Random.Range(0f, 255f),
                UnityEngine.Random.Range(0f, 255f));
        }
        else
        {
            curRoverColor = m_roverColors[colorIndex];
        }

        roverManager.InitializeVariables(
            id, ref timestamps, ref positions, ref rotations, ref curRoverColor);

        curRover.GetComponent<RoverCameraManager>().SetCameraViewParameters(cameraFieldOfView, cameraDepthOfView);

        return curRover;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public void TeleportToNextRover()
    //--------------------------------------------------
    {
        if (m_indexOfSelectedRover < 0)
            return;

        if (++m_indexOfSelectedRover >= m_roverManagers.Count)
        {
            m_indexOfSelectedRover = -1;
            ExitTunnel();
        }
        else
        {
            TeleportToRover();
        }
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public int SelectRover
    //--------------------------------------------------
    {
        set
        {
            m_indexOfSelectedRover = value - 1;
            SimulationManager.roverNum = m_indexOfSelectedRover;
            //Debug.Log("value = " + value);
        }
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public void TeleportToRover()
    //--------------------------------------------------
    {
        if (m_roverManagers.Count <= 0)
            return;

        teleportCallback = TeleportToRoverAfterFade;
        EventsManager.Teleport(teleportCallback);
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public void ExitTunnel()
    //--------------------------------------------------
    {
        if (null == m_scientistPlayer.transform.parent)     // player already in control room
            return;

        teleportCallback = ExitTunnelAfterFade;
        EventsManager.Teleport(teleportCallback);
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public void ExitTunnelAfterFade()
    //--------------------------------------------------
    {
        SwitchToControlRoomState();

        m_scientistPlayer.transform.SetParent(null);
        m_scientistPlayer.transform.rotation = m_playerStartTransform.rotation;
        m_scientistPlayer.transform.position = m_playerStartTransform.position;
        m_scientistPlayer.transform.localScale = Vector3.one;
    }

    //--------------------------------------------------
    public void RoverInfoDisplay()
    //--------------------------------------------------
    {
        Vector3 m_roverPosition = m_roverManagers[m_indexOfSelectedRover].transform.localPosition;
        //Debug.Log("Rover "+)
        m_handUIInfoFeed.text = "Pos X : " + m_roverPosition.x.ToString() + "\nPos Y : " + m_roverPosition.y.ToString() +
            "\nPos Z : " + m_roverPosition.z.ToString();
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public void CameraFeedDisplay()
    //--------------------------------------------------
    {
        Vector3 m_roverPosition = m_roverManagers[m_indexOfSelectedRover].transform.localPosition;
        Vector3 m_roverRotation = m_roverManagers[m_indexOfSelectedRover].transform.localRotation.eulerAngles;
        //Debug.Log("Rover "+)
        m_largeFeedRoverInfo.text = "Pos X : " + m_roverPosition.x.ToString()
            + "\nPos Y : " + m_roverPosition.y.ToString()
            + "\nPos Z : " + m_roverPosition.z.ToString()
            + "\nRot X : " + m_roverRotation.x.ToString()
            + "\nRot Y : " + m_roverRotation.y.ToString()
            + "\nRot Z : " + m_roverRotation.z.ToString();

        m_largeFeedTimeInfo.text = CurrentTimeDisplay();
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public void EnableMap(MapsManager.MapType map, bool enabled)
    //--------------------------------------------------
    {
        MapsManager mapsManager = m_mapInHierarchy.GetComponent<MapsManager>();
        mapsManager.EnableMap(map, enabled);
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public void ResetMapPosition()
    //--------------------------------------------------
    {
        m_mapInHierarchy.transform.position = m_mapStartTransform.position;
        m_mapInHierarchy.transform.rotation = m_mapStartTransform.rotation;
        m_mapInHierarchy.transform.localScale = m_mapStartTransform.localScale;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public List<SimulationRoverManager> GetRoverManagerList()
    //--------------------------------------------------
    {
        return m_roverManagers;
    }
    //--------------------------------------------------
    #endregion PUBLIC METHODS: Managing game objects

    #region PUBLIC METHODS: Playback and time manipulation
    //--------------------------------------------------
    public void SummarizeData()
    //--------------------------------------------------
    {
        m_maxDataSampleIndex =
            (int)((finalTimestamp - initialTimestamp) / timeIncrement);

        m_currentDataSampleIndex = 0;

        m_playing = true;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public void SetPlayState()
    //--------------------------------------------------
    {
        m_playing = !m_playing;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public bool IsPlaying()
    //--------------------------------------------------
    {
        return m_playing;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public void Rewind()
    //--------------------------------------------------
    {
        SetSimulationState(m_currentDataSampleIndex);

        //if (--m_currentDataSampleIndex < 0)
        //{
        //    m_currentDataSampleIndex = m_maxDataSampleIndex;
        //}
        m_currentDataSampleIndex -= m_playbackSpeeds[m_playbackSpeedIndex];

        if (m_currentDataSampleIndex <= 0)
        {
            m_currentDataSampleIndex = 0;
            return;
        }
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public void FastForward()
    //--------------------------------------------------
    {
        SetSimulationState(m_currentDataSampleIndex);

        //if (++m_currentDataSampleIndex > m_maxDataSampleIndex)
        //{
        //    m_currentDataSampleIndex = 0;
        //}
        m_currentDataSampleIndex += m_playbackSpeeds[m_playbackSpeedIndex];

        if (m_currentDataSampleIndex > m_maxDataSampleIndex)
        {
            m_currentDataSampleIndex = m_maxDataSampleIndex;
            return;
        }
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public void IncrementPlaybackSpeed()
    //--------------------------------------------------
    {
        if (++m_playbackSpeedIndex >= m_playbackSpeeds.Length)
            m_playbackSpeedIndex = m_playbackSpeeds.Length - 1;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public void DecrementPlaybackSpeed()
    //--------------------------------------------------
    {
        if (--m_playbackSpeedIndex < 0)
            m_playbackSpeedIndex = 0;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public int PlaybackSpeed()
    //--------------------------------------------------
    {
        return m_playbackSpeeds[m_playbackSpeedIndex];
    }
    //--------------------------------------------------

    //--------------------------------------------------
    /// <summary>
    /// Returns simulation time point in seconds
    /// </summary>
    /// <returns></returns>
    public float CurrentTime()
    //--------------------------------------------------
    {
        if (m_rovers.Count <= 0)
            return 0f;

        return initialTimestamp +
            m_currentDataSampleIndex * timeIncrement;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public float TimeIncreaseInSeconds
    //--------------------------------------------------
    {
        set
        {
            float increment = value / timeIncrement;
            m_currentDataSampleIndex += Mathf.FloorToInt(increment);
            if (m_currentDataSampleIndex > m_maxDataSampleIndex)
            {
                m_currentDataSampleIndex = m_maxDataSampleIndex;
            }

            SetSimulationState(m_currentDataSampleIndex);
        }
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public float TimeDecreaseInSeconds
    //--------------------------------------------------
    {
        set
        {
            //Debug.Log("Decreasing time by " + value);
            float decrement = value / timeIncrement;
            m_currentDataSampleIndex -= Mathf.FloorToInt(decrement);
            if (m_currentDataSampleIndex < 0)
            {
                m_currentDataSampleIndex = 0;
            }

            SetSimulationState(m_currentDataSampleIndex);
        }
    }
    //--------------------------------------------------
    #endregion PUBLIC METHODS: Playback and time manipulation
    #endregion PUBLIC METHODS


    #region PRIVATE METHODS
    //--------------------------------------------------
    private void TeleportToRoverAfterFade()
    //--------------------------------------------------
    {
        ScientistController scientistController = m_scientistPlayer.GetComponent<ScientistController>();
        if (scientistController.ScientistInControlRoom)
        {
            SwitchToTunnelState();
        }

        const float PLAYER_TUNNEL_SCALE = 0.25f;
        Transform teleportTransform = m_roverManagers[m_indexOfSelectedRover].TeleportTransform();

        m_scientistPlayer.transform.SetParent(m_mapInHierarchy.transform);
        m_scientistPlayer.transform.rotation = teleportTransform.rotation;
        m_scientistPlayer.transform.position = teleportTransform.position;
        m_scientistPlayer.transform.localScale = Vector3.one * PLAYER_TUNNEL_SCALE;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private float SampleIndex2Timestamp(int sampleIndex)
    //--------------------------------------------------
    {
        if (m_rovers.Count <= 0)
            return 0f;

        return initialTimestamp + 
            timeIncrement * sampleIndex;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void PlaySimulation()
    //--------------------------------------------------
    {
        //Debug.Log("State : " + m_currentDataSampleIndex);
        SetSimulationState(m_currentDataSampleIndex);

        m_currentDataSampleIndex += m_playbackSpeeds[m_playbackSpeedIndex];
        if (m_currentDataSampleIndex > m_maxDataSampleIndex)
        {
            m_currentDataSampleIndex = m_maxDataSampleIndex;
            //Debug.Log("Playback completed");
        }
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void SetSimulationState(int sampleIndex)
    //--------------------------------------------------
    {
        //Debug.Log("Setting state for " + sampleIndex);
        foreach (var roverManagers in m_roverManagers)
        {
            roverManagers.SetStateInTime(SampleIndex2Timestamp(sampleIndex));
        }

        m_dataSlices.GetComponent<DataSlicesManager>().
            SetDataSlicesState(
                new List<SimulationRoverManager>(m_roverManagers));
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void SwitchToTunnelState()
    //--------------------------------------------------
    {
        const float TUNNEL_UPSCALE = 4.0f;

        m_controlRoom.SetActive(false);
        m_previousMapPosition = m_mapInHierarchy.transform.position;
        m_previousMapScale = m_mapInHierarchy.transform.localScale;

        m_mapInHierarchy.transform.localScale = Vector3.one * TUNNEL_UPSCALE;
        m_scientistPlayer.GetComponent<ScientistController>().
            ScientistInControlRoom = false;
        //m_scientistPlayer.GetComponent<ScientistController>().EnableTeleportDevices(true);

        EventsManager.TeleportToTunnel();
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void SwitchToControlRoomState()
    //--------------------------------------------------
    {
        m_mapInHierarchy.transform.localScale = m_previousMapScale;
        m_mapInHierarchy.transform.position = m_previousMapPosition;
        m_controlRoom.SetActive(true);
        m_scientistPlayer.GetComponent<ScientistController>().
            ScientistInControlRoom = true;
        //m_scientistPlayer.GetComponent<ScientistController>().EnableTeleportDevices(false);

        EventsManager.TeleportToCommandCenter();
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private string CurrentTimeDisplay()
    //--------------------------------------------------
    {
        float totalTime = CurrentTime();
        if (totalTime < 0f)
            totalTime = 0f;

        int curDay = (int)Mathf.Floor(totalTime / (60 * 60 * 24)) + 1;

        float timeInDay = totalTime / curDay;

        TimeSpan displayTime = TimeSpan.FromSeconds(timeInDay);

        string daytext = "Day " + curDay.ToString();
        string timeText = displayTime.ToString();

        return (daytext + "\n" + timeText);
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void OnLoadMapIntializer()
    //--------------------------------------------------
    {
        currentGameState = GameState.SIMULATION_LOADED;

        m_dataSlices.GetComponent<DataSlicesManager>().TurnOffInitialRendering();
        m_preLoadUI.SetActive(false);
        m_cameraFeed.SetActive(true);
        m_cameraFeed.GetComponent<CameraFeedManager>().StartCameraFeeds();

        if (m_rovers.Count > 0)
        {
            EventsManager.SwitchRoverCam(m_indexOfSelectedRover = 0);
        }

        EnableMap(MapsManager.MapType.MODEL, true);
        EnableMap(MapsManager.MapType.SURFACES, true);
        EnableMap(MapsManager.MapType.DATA, false);
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private IEnumerator IE_LoadingData()
    //--------------------------------------------------
    {
        //Debug.Log("Loading data...");

        currentGameState = GameState.LOADING;

        HandUIManager handUI = m_scientistPlayer.GetComponentInChildren<HandUIManager>();
        handUI.SetLoadingText("Loading data...");
        handUI.SwitchHandUI(HandUIManager.HandUI.LOADING);

        while (!m_dataManager.DataLoaded())
        {
            yield return null;
        }

        //Debug.Log("Data loaded successfully!");
        handUI.SwitchHandUI(HandUIManager.HandUI.MAIN_MENU);
        OnLoadMapIntializer();
    }
    //--------------------------------------------------
    #endregion PRIVATE METHODS
}
//--------------------------------------------------
