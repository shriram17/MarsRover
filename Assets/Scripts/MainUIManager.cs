using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------
public class MainUIManager : MonoBehaviour
//--------------------------------------------------
{
    //--------------------------------------------------
    public enum UIMenu
    {
        ROVER,
        DATA
    }

    [SerializeField] private GameObject m_roverMenu;
    [SerializeField] private GameObject m_dataMenu;
    //--------------------------------------------------

    //--------------------------------------------------
    public void ActivateMenu(UIMenu menu)
    //--------------------------------------------------
    {
        m_roverMenu.SetActive(false);
        m_dataMenu.SetActive(false);

        switch (menu)
        {
            case UIMenu.ROVER:
                m_roverMenu.SetActive(true);
                break;
            case UIMenu.DATA:
                m_dataMenu.SetActive(true);
                break;
        }
    }
    //--------------------------------------------------
}
//--------------------------------------------------
