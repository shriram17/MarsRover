using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------
public class ControllerTrigger : MonoBehaviour
//--------------------------------------------------
{
    private bool m_mapIntersected = false;


    #region MONOBEHAVIOUR
    //--------------------------------------------------
    private void OnTriggerEnter(Collider other)
    //--------------------------------------------------
    {
        if (other.tag == "Tunnel")
        {
            m_mapIntersected = true;
        }
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void OnTriggerExit(Collider other)
    //--------------------------------------------------
    {
        if (other.tag == "Tunnel")
        {
            m_mapIntersected = false;
        }
    }
    //--------------------------------------------------
    #endregion


    #region PUBLIC METHODS
    //--------------------------------------------------
    public bool IntersectingMapModel()
    {
        return m_mapIntersected;
    }
    #endregion
}
//--------------------------------------------------
