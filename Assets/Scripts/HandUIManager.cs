using UnityEngine;
using UnityEngine.UI;

//--------------------------------------------------
public class HandUIManager : MonoBehaviour
//--------------------------------------------------
{
    #region PUBLIC VARIABLES
    //--------------------------------------------------
    public enum HandUI
    {
        DATA_SELECTOR,
        LOADING,
        MAIN_MENU
    }
    //--------------------------------------------------
    #endregion PUBLIC VARIABLES


    #region PRIVATE VARIABLES
    //--------------------------------------------------
    #region PRIVATE VARIABLES: Serialize field
    //--------------------------------------------------
    [SerializeField] private GameObject m_dataSelectorUI;
    [SerializeField] private GameObject m_loadingUI;
    [SerializeField] private GameObject m_mainUI;
    //--------------------------------------------------
    #endregion PRIVATE VARIABLES: Serialize field

    private bool m_dataListInitialized = false;
    //--------------------------------------------------
    #endregion PRIVATE VARIABLES


    #region MONOBEHAVIOUR
    //--------------------------------------------------
    private void OnEnable()
    //--------------------------------------------------
    {
        if (SimulationManager.singleton == null)
            return;

        if (SimulationManager.GameState.PRE_DATA_LOAD ==
            SimulationManager.singleton.currentGameState)
        {
            if (SimulationManager.singleton.tunnelScanCompleted)
            {
                SwitchHandUI(HandUI.DATA_SELECTOR);
            }
            else
            {
                SwitchHandUI(HandUI.LOADING);
            }
        }
        else if (SimulationManager.GameState.LOADING ==
            SimulationManager.singleton.currentGameState)
        {
            SwitchHandUI(HandUI.LOADING);
        }
    }
    //--------------------------------------------------
    #endregion MONOBEHAVIOUR


    #region PUBLIC METHODS
    //--------------------------------------------------
    public void SwitchHandUI(HandUI uiElement)
    //--------------------------------------------------
    {
        m_dataSelectorUI.SetActive(false);
        m_loadingUI.SetActive(false);
        m_mainUI.SetActive(false);

        switch (uiElement)
        {
            case HandUI.DATA_SELECTOR:
                m_dataSelectorUI.SetActive(true);
                if (!m_dataListInitialized)
                {
                    // First time data list is initialized,
                    // unable to select buttons until UI is
                    // turned off and on again.
                    GetComponentInParent<UIActivator>().
                         RefreshUI();
                    m_dataListInitialized = true;
                }
                break;
            case HandUI.LOADING:
                m_loadingUI.SetActive(true);
                break;
            case HandUI.MAIN_MENU:
                m_mainUI.SetActive(true);
                break;
            default:
                break;
        }
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public void SetLoadingText(string pText)
    //--------------------------------------------------
    {
        Text loadingText = m_loadingUI.GetComponentInChildren<Text>();
        loadingText.text = pText;
    }
    //--------------------------------------------------
    #endregion PUBLIC METHODS
}
//--------------------------------------------------
