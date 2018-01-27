using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------
public class DataManager : MonoBehaviour
//--------------------------------------------------
{
    #region PRIVATE VARIABLES
    //--------------------------------------------------
    private float m_xLowBound = 0f;
    private float m_xHighBound = 0f;
    private float m_yLowBound = 0f;
    private float m_yHighBound = 0f;
    private float m_zLowBound = 0f;
    private float m_zHighBound = 0f;

    private float m_maxDistanceX = 0f;
    private float m_maxDistanceY = 0f;
    private float m_maxDistanceZ = 0f;

    private Vector3 m_positionImportOffset = new Vector3(140f, 3.8f, -110f);
    private Vector3 m_rotationImportOffset = new Vector3(0f, 0f, Mathf.PI);

    private MainDataParser m_mainDataParser = null;
    private bool m_dataLoaded = false;

    private float m_timestamp;
    private float m_targetFPS = 150f;
    private float m_maximumTimePerFrame;

    #region PRIVATE VARIABLES: Rover class
    //--------------------------------------------------
    private class Rover
    //--------------------------------------------------
    {
        public int m_roverNumber = -1;
        public List<float> m_timestamps = new List<float>();
        public Dictionary<float, Vector3> m_positionsThroughTime = 
            new Dictionary<float, Vector3>();
        public Dictionary<float, Quaternion> m_rotationsThroughTime = 
            new Dictionary<float, Quaternion>();

        public Rover(int roverNumber)
        {
            m_roverNumber = roverNumber;
        }        
    }

    private List<int> m_roverIdentities = new List<int>();
    private Dictionary<int, Rover> m_rovers = new Dictionary<int, Rover>();
    //--------------------------------------------------
    #endregion PRIVATE VARIABLES: Rover class

    private const string ROVER_SEPARATOR = "Rover";
    //--------------------------------------------------
    #endregion PRIVATE VARIABLES


    #region MONOBEHAVIOUR
    //--------------------------------------------------
    private void Awake()
    //--------------------------------------------------
    {
        m_mainDataParser = GetComponentInChildren<MainDataParser>();
    }
    //--------------------------------------------------
    #endregion MONOBEHAVIOUR


    #region PUBLIC METHODS
    //--------------------------------------------------
    public void LoadDataFile(ref string filename)
    //--------------------------------------------------
    {
        if (null == m_mainDataParser)
        {
            this.gameObject.AddComponent<MainDataParser>();
            m_mainDataParser = this.GetComponent<MainDataParser>();
        }

        m_mainDataParser.ParseDataFile(filename);
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public bool InitializeData()
    //--------------------------------------------------
    {
        if (null == m_mainDataParser)
        {
            Debug.Log("No DataParser found");
            return false;
        }
        else
        {
            StartCoroutine("IE_InitializeRoverData");
            return true;
        }
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public bool DataLoaded()
    //--------------------------------------------------
    {
        return m_dataLoaded;
    }
    //--------------------------------------------------
    #endregion PUBLIC METHODS


    #region PRIVATE METHODS
    //--------------------------------------------------
    private IEnumerator IE_InitializeRoverData()
    //--------------------------------------------------
    {
        while (!m_mainDataParser.DataParsed())
        {
            yield return null;
        }

        List<string> data = m_mainDataParser.GetData();
        int curRoverNumber = -1;
        Rover curRover = null;

        m_maximumTimePerFrame = 1 / m_targetFPS;
        m_timestamp = Time.realtimeSinceStartup;
        foreach (string curString in data)
        {
            if (SwitchRover(curString, ref curRoverNumber))     // if multiple rover data on same data file
            {
                if (!m_rovers.ContainsKey(curRoverNumber))
                {
                    curRover = new Rover(curRoverNumber);
                    //Debug.Log("Rover number " + curRoverNumber + " created");
                    m_roverIdentities.Add(curRoverNumber);
                    m_rovers.Add(curRoverNumber, curRover);
                }
                else
                {
                    Debug.Log("Additional data for processed rover found. " +
                        "For best results, combine positional data into one file");
                    curRover = m_rovers[curRoverNumber];
                }

                BoundingValuesCheckpoint();
                continue;
            }
            else
            {
                Vector3 position = Vector3.zero;
                Vector3 eulerAngles = Vector3.zero;

                float timestamp = 0f;
                GetPositionAndRotationData(curString, ref timestamp, ref position, ref eulerAngles);

                if (null != curRover)
                {
                    curRover.m_timestamps.Add(timestamp);
                    curRover.m_positionsThroughTime[timestamp] = position;
                    curRover.m_rotationsThroughTime[timestamp] = Quaternion.Euler(eulerAngles);
                }

                if (SimulationManager.singleton.loadDataAsynch)
                {
                    if (Time.realtimeSinceStartup > m_timestamp + m_maximumTimePerFrame)
                    {
                        yield return null;
                        m_timestamp = Time.realtimeSinceStartup;
                    }
                }
            }

            BoundingValuesCheckpoint();
        }

        InitializeRovers();
        SimulationManager.singleton.SummarizeData();
        m_dataLoaded = true;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private bool SwitchRover(string dataString, ref int roverNumber)
    //--------------------------------------------------
    {
        if (dataString.Contains(ROVER_SEPARATOR))
        {
            int index = ROVER_SEPARATOR.Length;
            string roverNumText = "";

            while (index < dataString.Length)
            {
                roverNumText += dataString[index++];
            }

            roverNumber = int.Parse(roverNumText);
            return true;
        }
        else return false;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void GetPositionAndRotationData(string dataString, ref float timestamp, ref Vector3 position, ref Vector3 eulerAngles)
    //--------------------------------------------------
    {
        float xPosition, yPosition, zPosition;
        float xEulerAngle, yEulerAngle, zEulerAngle;

        xPosition = yPosition = zPosition = xEulerAngle = yEulerAngle = zEulerAngle = 0f;

        int curIndex = 0;

        // Values must be set in this order
        DetermineTimestamp(ref dataString, ref timestamp, ref curIndex);
        SetValue(ref dataString, ref zPosition, ref curIndex);
        SetValue(ref dataString, ref xPosition, ref curIndex);
        SetValue(ref dataString, ref yPosition, ref curIndex);
        SetValue(ref dataString, ref zEulerAngle, ref curIndex);
        SetValue(ref dataString, ref xEulerAngle, ref curIndex);
        SetValue(ref dataString, ref yEulerAngle, ref curIndex);

        position = new Vector3(xPosition, yPosition, zPosition) + m_positionImportOffset;
        UpdateBoundaries(position.x, position.y, position.z);

        eulerAngles = new Vector3(xEulerAngle, yEulerAngle, zEulerAngle) + m_rotationImportOffset;
        eulerAngles *= Mathf.Rad2Deg;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void DetermineTimestamp(ref string dataString, ref float timestamp, ref int index)
    //--------------------------------------------------
    {
        string timeText = "";
        index = 0;

        while (' ' != dataString[index])
        {
            timeText += dataString[index++];
        }

        timestamp = float.Parse(timeText);
        //Debug.Log(timestamp);
        index++;
        
        if (-1f == SimulationManager.singleton.initialTimestamp ||
            timestamp < SimulationManager.singleton.initialTimestamp)
        {
            SimulationManager.singleton.initialTimestamp = timestamp;
        }

        if (-1f == SimulationManager.singleton.finalTimestamp ||
            timestamp > SimulationManager.singleton.finalTimestamp)
        {
            SimulationManager.singleton.finalTimestamp = timestamp;
        }
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void SetValue(ref string dataString, ref float value, ref int index)
    //--------------------------------------------------
    {
        //Debug.Log("DataManager.SetValue");

        if (index >= dataString.Length)
            return;

        string stringVal = "";
        while (' ' != dataString[index])
        {
            stringVal += dataString[index++];

            if (index >= dataString.Length)
            {
                break;
            }
        }
        //Debug.Log("String parsed: " + stringVal);
        value = float.Parse(stringVal);
        //Debug.Log("Position parsed: " + position);
        index++;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void UpdateBoundaries(float x, float y, float z)
    //--------------------------------------------------
    {
        if (x < m_xLowBound)
            m_xLowBound = x;
        else if (x > m_xHighBound)
            m_xHighBound = x;

        if (y < m_yLowBound)
            m_yLowBound = y;
        else if (y > m_yHighBound)
            m_yHighBound = y;

        if (z < m_zLowBound)
            m_zLowBound = z;
        else if (z > m_zHighBound)
            m_zHighBound = z;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    /// <summary>
    /// Maintain max distance between farthest points 
    /// to determine appropriate scaling to translate
    /// data into local space
    /// </summary>
    private void BoundingValuesCheckpoint()
    //--------------------------------------------------
    {
        float curDistanceX = Mathf.Abs(m_xHighBound - m_xLowBound);
        float curDistanceY = Mathf.Abs(m_yHighBound - m_yLowBound);
        float curDistanceZ = Mathf.Abs(m_zHighBound - m_zLowBound);

        if (curDistanceX > m_maxDistanceX)
            m_maxDistanceX = curDistanceX;
        if (curDistanceY > m_maxDistanceY)
            m_maxDistanceY = curDistanceY;
        if (curDistanceZ > m_maxDistanceZ)
            m_maxDistanceZ = curDistanceZ;

        m_xLowBound = m_xHighBound =
        m_yLowBound = m_yHighBound =
        m_zLowBound = m_zHighBound = 0f;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void InitializeRovers()
    //--------------------------------------------------
    {
        foreach (int roverId in m_roverIdentities)
        {
            Rover curRover = m_rovers[roverId];
            GameObject instantiatedRover = 
                SimulationManager.singleton.InstantiateRover(
                    curRover.m_roverNumber,
                    ref curRover.m_timestamps,
                    ref curRover.m_positionsThroughTime,
                    ref curRover.m_rotationsThroughTime);
        }
    }
    //--------------------------------------------------
    #endregion PRIVATE METHODS
}
//--------------------------------------------------
