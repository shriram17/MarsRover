using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------
public class DataSlicesManager : MonoBehaviour
//--------------------------------------------------
{
    #region PRIVATE VARIABLES
    //--------------------------------------------------
    #region PRIVATE VARIABLES: Serialize field
    //--------------------------------------------------
    [SerializeField] private Material m_defaultDataSliceMat;
    [SerializeField] private List<GameObject> m_dataSlices = 
        new List<GameObject>();
    //--------------------------------------------------
    #endregion PRIVATE VARIABLES: Serialize field

    private List<GameObject> m_activatedSlices =
        new List<GameObject>();
    private MaterialPropertyBlock m_propertyBlock;
    private bool m_renderingEnabled = true;
    //--------------------------------------------------
    #endregion PRIVATE VARIABLES


    #region MONOBEHAVIOUR
    //--------------------------------------------------
    public void Awake()
    //--------------------------------------------------
    {
        m_propertyBlock = new MaterialPropertyBlock();
    }
    //--------------------------------------------------
    #endregion MONOBEHAVIOUR


    #region PUBLIC METHODS
    //--------------------------------------------------
    public void TurnOffInitialRendering()
    //--------------------------------------------------
    {
        foreach (var slice in m_dataSlices)
        {
            slice.GetComponent<Renderer>().enabled = false;
        }
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public void RenderActiveData(bool enable)
    //--------------------------------------------------
    {
        m_renderingEnabled = enable;

        foreach (GameObject activeSlice in m_activatedSlices)
        {
            activeSlice.GetComponent<Renderer>().enabled = m_renderingEnabled;
        }
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public void AddSlice(GameObject slice)
    //--------------------------------------------------
    {
        m_dataSlices.Add(slice);
        slice.GetComponent<Renderer>().enabled = true;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public void SetDataSlicesState(List<SimulationRoverManager> rovers)
    //--------------------------------------------------
    {
        DeactivateSlices();
        //bool[] roverDataPresented = new bool[rovers.Count];

        foreach (GameObject slice in m_dataSlices)
        {
            for(int i = 0; i < rovers.Count; i++)
            {
                if (RoverInSlice(slice.transform, rovers[i].transform))
                {
                    if (!m_activatedSlices.Contains(slice))
                    {
                        RepresentData(slice, rovers[i]);                        
                        m_activatedSlices.Add(slice);
                    }
                }
            }

            //int numRoverDataPresented = 0;
            //for (int i = 0; i < roverDataPresented.Length; i++)
            //{
            //    if (roverDataPresented[i])
            //        numRoverDataPresented++;
            //}
            //if (numRoverDataPresented == roverDataPresented.Length)
            //    return;
        }
    }
    //--------------------------------------------------
    #endregion PUBLIC METHODS


    #region PRIVATE METHODS
    //--------------------------------------------------
    private void DeactivateSlices()
    //--------------------------------------------------
    {
        foreach (GameObject slice in m_activatedSlices)
        {
            Renderer renderer = slice.GetComponent<Renderer>();

            renderer.GetPropertyBlock(m_propertyBlock);
            m_propertyBlock.SetColor("_Color", m_defaultDataSliceMat.color);
            renderer.SetPropertyBlock(m_propertyBlock);
            slice.GetComponent<Renderer>().enabled = false;
        }

        m_activatedSlices.Clear();
    }
    //--------------------------------------------------

    //--------------------------------------------------
    /// <summary>
    /// Using area estimation ~CF 09/15/2017
    /// ToDo: Use more accurate distance check
    /// </summary>
    /// <param name="sliceTransform"></param>
    /// <param name="roverTransform"></param>
    /// <returns></returns>
    private bool RoverInSlice(Transform sliceTransform, Transform roverTransform)
    //--------------------------------------------------
    {
        const float ERROR_MARGIN = 2f;

        float deltaX = Mathf.Abs(
            roverTransform.localPosition.x - sliceTransform.localPosition.x);
        float deltaY = Mathf.Abs(
            roverTransform.localPosition.y - sliceTransform.localPosition.y);
        float deltaZ = Mathf.Abs(
            roverTransform.localPosition.z - sliceTransform.localPosition.z);

        if (deltaX < sliceTransform.localScale.x / 2 * ERROR_MARGIN
            && deltaY < sliceTransform.localScale.y / 2 * ERROR_MARGIN
            && deltaZ < sliceTransform.localScale.z / 2 * ERROR_MARGIN)
        {
            return true;
        }
        return false;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void RepresentData(GameObject curSlice, SimulationRoverManager curRover)
    //--------------------------------------------------
    {
        Renderer renderer = curSlice.GetComponent<Renderer>();
        renderer.GetPropertyBlock(m_propertyBlock);

        m_propertyBlock.Clear();

        float curOxygen = curRover.CurrentOxygenReading();
        Color dataColor = new Color(
            Color.blue.r,
            Color.blue.g, 
            Color.blue.b,
            curOxygen / 255f);

        m_propertyBlock.SetColor("_Color", dataColor);

        renderer.SetPropertyBlock(m_propertyBlock);

        renderer.enabled = m_renderingEnabled;
    }
    //--------------------------------------------------
    #endregion PRIVATE METHODS
}
//--------------------------------------------------
