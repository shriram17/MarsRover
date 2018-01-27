using UnityEngine;
using UnityEngine.UI;

//--------------------------------------------------
public class UISelectorManager : MonoBehaviour
//--------------------------------------------------
{
    #region PRIVATE VARIABLES
    //--------------------------------------------------
    [SerializeField] private GameObject m_roverUIButton;
    [SerializeField] private GameObject m_dataUIButton;
    [SerializeField] private Color m_selectedTextColor;
    [SerializeField] private Color m_unselectedTextColor;

    private MainUIManager m_mainUIManager;
    //--------------------------------------------------
    #endregion PRIVATE VARIABLES


    #region MONOBEHAVIOUR
    //--------------------------------------------------
    private void Start()
    //--------------------------------------------------
    {
        m_mainUIManager = GetComponentInParent<MainUIManager>();
        SelectRoverMenu();
    }
    //--------------------------------------------------
    #endregion MONOBEHAVIOUR


    #region PUBLIC METHODS
    //--------------------------------------------------
    public void SelectRoverMenu()
    //--------------------------------------------------
    {
        ActivateMenuItem(MainUIManager.UIMenu.ROVER);
        m_mainUIManager.ActivateMenu(MainUIManager.UIMenu.ROVER);
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public void SelectDataMenu()
    //--------------------------------------------------
    {
        ActivateMenuItem(MainUIManager.UIMenu.DATA);
        m_mainUIManager.ActivateMenu(MainUIManager.UIMenu.DATA);
    }
    //--------------------------------------------------
    #endregion PUBLIC METHODS


    #region PRIVATE METHODS
    //--------------------------------------------------
    private void ActivateMenuItem(MainUIManager.UIMenu item)
    //--------------------------------------------------
    {
        Text roverMenuText = m_roverUIButton.GetComponentInChildren<Text>();
        Text dataMenuText = m_dataUIButton.GetComponentInChildren<Text>();

        roverMenuText.color = m_unselectedTextColor;
        dataMenuText.color = m_unselectedTextColor;

        switch (item)
        {
            case MainUIManager.UIMenu.ROVER:
                roverMenuText.color = m_selectedTextColor;
                break;
            case MainUIManager.UIMenu.DATA:
                dataMenuText.color = m_selectedTextColor;
                break;
        }
    }
    //--------------------------------------------------
    #endregion PRIVATE METHODS
}
//--------------------------------------------------
