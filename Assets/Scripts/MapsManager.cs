using UnityEngine;

//--------------------------------------------------
public class MapsManager : MonoBehaviour
//--------------------------------------------------
{
    #region PRIVATE VARIABLES
    //--------------------------------------------------
    [SerializeField] private GameObject m_tunnelModelMap;
    [SerializeField] private GameObject m_surfacesViewedMap;
    [SerializeField] private GameObject m_dataOverlayMap;
    [SerializeField] private Transform[] m_scaleReferences;
    //--------------------------------------------------
    #endregion PRIVATE VARIABLES


    #region PUBLIC VARIABLES
    //--------------------------------------------------
    public enum MapType
    //--------------------------------------------------
    {
        MODEL,
        SURFACES,
        DATA
    }
    //--------------------------------------------------
    #endregion PUBLIC VARIABLES


    #region PUBLIC METHODS
    //--------------------------------------------------
    public void EnableMap(MapType map, bool enabled)
    //--------------------------------------------------
    {
        switch (map)
        {
            case MapType.MODEL:
                RenderTunnelModel(enabled);
                break;
            case MapType.SURFACES:
                RenderSurfacesViewed(enabled);
                break;
            case MapType.DATA:
                RenderDataOverlayMap(enabled);
                break;
        }
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public Vector3 FindClosestScaleReference(Vector3 scaleOrigin)
    //--------------------------------------------------
    {
        Vector3 closestVector = Vector3.zero;
        float distance = Mathf.Infinity;
        for (int i = 0; i < m_scaleReferences.Length; i++)
        {
            float curDistance = Vector3.Distance(m_scaleReferences[i].position, scaleOrigin);
            if (curDistance < distance)
            {
                distance = curDistance;
                closestVector = m_scaleReferences[i].position;
            }
        }

        return closestVector;
    }
    //--------------------------------------------------
    #endregion PUBLIC METHODS


    #region PRIVATE METHODS
    //--------------------------------------------------
    private void RenderTunnelModel(bool renderOn)
    //--------------------------------------------------
    {
        Renderer[] mapRenderers = m_tunnelModelMap.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < mapRenderers.Length; i++)
        {
            mapRenderers[i].enabled = renderOn;
        }
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void RenderSurfacesViewed(bool renderOn)
    //--------------------------------------------------
    {
        Renderer[] mapRenderers = m_surfacesViewedMap.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < mapRenderers.Length; i++)
        {
            mapRenderers[i].enabled = renderOn;
        }
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void RenderDataOverlayMap(bool renderOn)
    //--------------------------------------------------
    {
        m_dataOverlayMap.GetComponent<DataSlicesManager>().
            RenderActiveData(renderOn);
    }
    //--------------------------------------------------
    #endregion PRIVATE METHODS
}
//--------------------------------------------------
