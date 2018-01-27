using UnityEngine;
using UnityEngine.UI;

//--------------------------------------------------
public class OptionsMenuManager : MonoBehaviour
//--------------------------------------------------
{
    #region PUBLIC AND PRIVATE VARIABLES
    //--------------------------------------------------
    //--------------------------------------------------
    [System.Serializable]
    public class DataSelection
    //--------------------------------------------------
    {
        public Image selector;
        public bool selected;
    }
    //--------------------------------------------------

    [SerializeField] private DataSelection[] m_dataSelections;

    private enum DataSelector
    {
        TUNNEL_MODEL,
        SURFACES_VIEWED,
        DATA_OVERLAY
    }

    private bool m_dataOverlayState;
    //--------------------------------------------------
    #endregion PUBLIC AND PRIVATE VARIABLES


    #region MONOBEHAVIOUR
    //--------------------------------------------------
    private void Awake()
    //--------------------------------------------------
    {
        for (int i = 0; i < m_dataSelections.Length; i++)
        {
            m_dataSelections[i].selector.enabled = true;
            m_dataSelections[i].selected = true;
            m_dataOverlayState = true;
        }

        DataSelection currentSelection =
            m_dataSelections[(int)DataSelector.DATA_OVERLAY];
        currentSelection.selected = !currentSelection.selected;
        currentSelection.selector.enabled = currentSelection.selected;

        EventsManager.OnTeleportToTunnel += HandleTeleportToTunnel;
        EventsManager.OnTeleportToCommandCenter += HandleTeleportToCommandCenter;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void OnDestroy()
    //--------------------------------------------------
    {
        EventsManager.OnTeleportToTunnel -= HandleTeleportToTunnel;
        EventsManager.OnTeleportToCommandCenter -= HandleTeleportToCommandCenter;
    }
    //--------------------------------------------------
    #endregion MONOBEHAVIOUR


    #region PUBLIC METHODS
    //--------------------------------------------------
    public void SelectTunnelModel()
    //--------------------------------------------------
    {
        DataSelection currentSelection = 
            m_dataSelections[(int)DataSelector.TUNNEL_MODEL];
        currentSelection.selected = !currentSelection.selected;
        currentSelection.selector.enabled = currentSelection.selected;

        SimulationManager.singleton.EnableMap(
            MapsManager.MapType.MODEL, currentSelection.selected);
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public void SelectSurfaceViewed()
    //--------------------------------------------------
    {
        DataSelection currentSelection = 
            m_dataSelections[(int)DataSelector.SURFACES_VIEWED];
        currentSelection.selected = !currentSelection.selected;
        currentSelection.selector.enabled = currentSelection.selected;

        SimulationManager.singleton.EnableMap(
            MapsManager.MapType.SURFACES, currentSelection.selected);
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public void SelectDataOverlay()
    //--------------------------------------------------
    {
        DataSelection currentSelection = 
            m_dataSelections[(int)DataSelector.DATA_OVERLAY];
        currentSelection.selected = !currentSelection.selected;
        currentSelection.selector.enabled = currentSelection.selected;

        SimulationManager.singleton.EnableMap(
            MapsManager.MapType.DATA, currentSelection.selected);
    }
    //--------------------------------------------------
    #endregion PUBLIC METHODS


    #region PRIVATE METHODS
    //--------------------------------------------------
    private void HandleTeleportToTunnel()
    //--------------------------------------------------
    {
        //Debug.Log("HandleTeleportToTunnel");
        DataSelection dataOverlayOption =
            m_dataSelections[(int)DataSelector.DATA_OVERLAY];

        m_dataOverlayState = dataOverlayOption.selected;
        dataOverlayOption.selected = false;
        dataOverlayOption.selector.enabled = false;

        SimulationManager.singleton.EnableMap(
            MapsManager.MapType.DATA, false);
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void HandleTeleportToCommandCenter()
    //--------------------------------------------------
    {
        //Debug.Log("HandleTeleportToCommandCenter");
        DataSelection dataOverlayOption =
            m_dataSelections[(int)DataSelector.DATA_OVERLAY];

        dataOverlayOption.selected = m_dataOverlayState;
        dataOverlayOption.selector.enabled = m_dataOverlayState;

        SimulationManager.singleton.EnableMap(
            MapsManager.MapType.DATA, m_dataOverlayState);
    }
    //--------------------------------------------------
    #endregion PRIVATE METHODS
}
//--------------------------------------------------
