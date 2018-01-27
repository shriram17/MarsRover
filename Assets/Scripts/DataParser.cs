using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//--------------------------------------------------
public class DataParser : MonoBehaviour
//--------------------------------------------------
{
    #region PROTECTED VARIABLES
    //--------------------------------------------------
    [SerializeField] protected string m_filename = "";

    //private FileInfo m_dataFile = null;
    protected string m_output;      // Value initialized to pass null check below
    protected List<string> m_data = new List<string>();
    protected bool m_dataParsed = false;
    //--------------------------------------------------
    #endregion PROTECTED VARIABLES


    #region PRIVATE VARIABLES
    //--------------------------------------------------
    private float m_timestamp;
    private float m_targetFPS = 150f;
    private float m_maximumTimePerFrame;
    //--------------------------------------------------
    #endregion PRIVATE VARIABLES


    #region PRIVATE METHODS
    //--------------------------------------------------
    private IEnumerator IE_ParseData()
    //--------------------------------------------------
    {
        using (StreamReader m_reader = File.OpenText(Application.dataPath + "/Resources/Data/" + m_filename))
        {
            m_timestamp = Time.realtimeSinceStartup;
            while ((m_output = m_reader.ReadLine()) != null)
            {
                if (m_output.Length <= 0)
                    continue;

                if ('#' == m_output[0])
                    continue;

                //Debug.Log(m_output);
                m_data.Add(m_output);

                if (SimulationManager.singleton.parseDataAsynch)
                {
                    if (Time.realtimeSinceStartup > m_timestamp + m_maximumTimePerFrame)
                    {
                        yield return null;
                        m_timestamp = Time.realtimeSinceStartup;
                    }
                }
            }
            //Debug.Log("End of data file reached");
        }
        m_dataParsed = true;
    }
    //--------------------------------------------------
    #endregion PRIVATE METHODS


    #region PUBLIC METHODS
    //--------------------------------------------------
    public void ParseDataFile(string filename = "")
    //--------------------------------------------------
    {
        //Debug.Log("Reading data for " + m_fileName);
        if ("" != filename)
        {
            // TODO: check proper filename
            m_filename = filename;
        }

        m_maximumTimePerFrame = 1.0f / m_targetFPS;
        StartCoroutine(IE_ParseData());
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public List<string> GetData()
    //--------------------------------------------------
    {
        if (!m_dataParsed)
            return new List<string>();

        return m_data;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public bool DataParsed()
    //--------------------------------------------------
    {
        return m_dataParsed;
    }
    //--------------------------------------------------
    #endregion PUBLIC METHODS
}
//--------------------------------------------------
