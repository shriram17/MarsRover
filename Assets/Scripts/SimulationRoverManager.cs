using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------
public class SimulationRoverManager : MonoBehaviour
//--------------------------------------------------
{
    #region PRIVATE VARIABLES
    //--------------------------------------------------
    #region SERIALIZE FIELD
    //--------------------------------------------------
    [SerializeField] private GameObject m_teleportPoint = null;
    [SerializeField] private GameObject m_flag = null;
    [SerializeField] private AudioClip roverMoveClip;
    [SerializeField] private AudioClip roverTurnClip;
    //--------------------------------------------------
    #endregion SERIALIZE FIELD

    private int m_roverId = -1;
    private List<float> m_timestamps = null;
    private Dictionary<float, Vector3> m_positionsThroughTime = null;
    private Dictionary<float, Quaternion> m_rotationsThroughTime = null;
    private Dictionary<float, float> m_oxygenLevelsThroughTime =
        new Dictionary<float, float>();
    private Vector3 m_normalScale = Vector3.one;

    private GameObject m_mapInHierarchy = null;
    private Color m_colorId = Color.grey;
    private float m_currentTimestamp = 0f;

    private AudioSource audioRover;
    private Vector3 prevRoverPos;
    private Vector3 prevRoverRot;

    private bool isInTunnel = false;
    //--------------------------------------------------
    #endregion PRIVATE VARIABLES


    #region MONOBEHAVIOUR
    //--------------------------------------------------
    private void Awake()
    //--------------------------------------------------
    {
        m_mapInHierarchy = GameObject.FindGameObjectWithTag("Map");
        m_normalScale = this.transform.localScale;
        audioRover = GetComponent<AudioSource>();

        //Events to check if user is in the tunnel or not
        EventsManager.OnTeleportToTunnel += TunnelCheck;
        EventsManager.OnTeleportToCommandCenter += CommandCenterCheck;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void Start()
    //--------------------------------------------------
    {
        if (null != m_mapInHierarchy)
        {
            this.transform.SetParent(m_mapInHierarchy.transform);
            this.transform.localScale = m_normalScale;

        }
    }
    //--------------------------------------------------
    #endregion MONOBEHAVIOUR


    #region PUBLIC METHODS
    //--------------------------------------------------
    public void InitializeVariables(
                    int id,
                    ref List<float> timestamps,
                    ref Dictionary<float, Vector3> positions,
                    ref Dictionary<float, Quaternion> rotations,
                    ref Color color)
    //--------------------------------------------------
    {
        m_roverId = id;

        m_timestamps = new List<float>(timestamps);
        m_timestamps.Sort();

        m_positionsThroughTime = new Dictionary<float, Vector3>(positions);
        m_rotationsThroughTime = new Dictionary<float, Quaternion>(rotations);

        m_colorId = color;
        SetFlag((id + 1), color);

        InitializeOxygenLevels();

        SetStartPosition();
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public void SetStateInTime(float time)
    //--------------------------------------------------
    {
        while (!m_positionsThroughTime.ContainsKey(time))
        {
            if (time < m_timestamps[0])
            {
                time = m_timestamps[0];
            }
            else if (time > m_timestamps[m_timestamps.Count - 1])
            {
                time = m_timestamps[m_timestamps.Count - 1];
            }

            time--;
        }

        m_currentTimestamp = time;
        this.transform.localPosition = m_positionsThroughTime[time];
        this.transform.localRotation = m_rotationsThroughTime[time];

        PosRotChange(prevRoverPos, this.transform.localPosition, prevRoverRot, this.transform.localRotation.eulerAngles);
        prevRoverPos = this.transform.localPosition;
        prevRoverRot = this.transform.localRotation.eulerAngles;

    }
    //--------------------------------------------------

    //--------------------------------------------------
    public Transform TeleportTransform()
    //--------------------------------------------------
    {
        return m_teleportPoint.transform;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public int GetRoverID()
    //--------------------------------------------------
    {
        return m_roverId;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public Color GetRoverColor()
    //--------------------------------------------------
    {
        return m_colorId;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public float CurrentOxygenReading()
    //--------------------------------------------------
    {
        return m_oxygenLevelsThroughTime[m_currentTimestamp];
    }
    //--------------------------------------------------
    #endregion PUBLIC METHODS


    #region PRIVATE METHODS
    //--------------------------------------------------
    private void CommandCenterCheck()
    //--------------------------------------------------
    {
        isInTunnel =false;
    }

    //--------------------------------------------------
    private void TunnelCheck()
    //--------------------------------------------------
    {
        isInTunnel = true;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    /// <summary>
    /// Using arbitrary numbers as of 09/18/2017
    /// </summary>
    private void InitializeOxygenLevels()
    //--------------------------------------------------
    {
        float previousOxygenLevel = 255 / 2f;
        foreach (float timepoint in m_timestamps)
        {
            m_oxygenLevelsThroughTime[timepoint] = 
                Mathf.Clamp(previousOxygenLevel + Random.Range(-10f, 10f), 50f, 200f);

            previousOxygenLevel = m_oxygenLevelsThroughTime[timepoint];
            //Debug.Log(m_oxygenLevelsThroughTime[timepoint]);
        }
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void SetStartPosition()
    //--------------------------------------------------
    {
        this.transform.localPosition = m_positionsThroughTime[m_timestamps[0]];
        this.transform.localRotation = 
            m_rotationsThroughTime[m_timestamps[0]];
        prevRoverPos = this.transform.localPosition;
        prevRoverRot = this.transform.localRotation.eulerAngles;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void SetFlag(int roverNum, Color color)
    //--------------------------------------------------
    {
        if (null == m_flag)
            return;

        TextMesh text = m_flag.GetComponentInChildren<TextMesh>();
        text.color = color;
        text.text = "Rover " + roverNum.ToString();
    }
    //--------------------------------------------------

    //--------------------------------------------------
    //Check for Changes in Position & Rotation, update prev position and rotation acco.
    private void PosRotChange(Vector3 prevPos,Vector3 currentPos, Vector3 prevRot, Vector3 currentRot)
    //--------------------------------------------------
    {
        //if (audioRover == null || audioRover.enabled == false)
        //    Debug.Log("No audio source found on Rover");

        float diffX = currentPos.x - prevPos.x;
        float diffZ = currentPos.z - prevPos.z;
        //Find Change in postion and rotation
        if(isInTunnel)
        {
            audioRover.enabled = true;
            if (diffX > 0.001f || diffZ > 0.001f)
            {
                //Play Rover Moving Sound
                //Debug.Log("Rover Moving");
                audioRover.clip = roverMoveClip;
                if (!audioRover.isPlaying)
                    audioRover.Play();
            }
            //else if((prevRot - currentRot).sqrMagnitude < 0.2f)
            //{
            //    Debug.Log("Rover Turning");
            //    audioRover.Stop();
            //    audioRover.clip = roverTurnClip;
            //    audioRover.Play();
            //}
            else
            {
                audioRover.Stop();
            }
        }
        else
        {
            audioRover.enabled = false;
        }  
    }
    //--------------------------------------------------
    #endregion PRIVATE METHODS
}