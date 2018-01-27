using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------
public class PlayButtonManager : WatchButtonManager
//--------------------------------------------------
{
    //--------------------------------------------------
    [SerializeField] private GameObject m_playButton;
    [SerializeField] private GameObject m_pauseButton;
    //--------------------------------------------------

    //--------------------------------------------------
    protected override void OnEnable()
    //--------------------------------------------------
    {
        base.OnEnable();
        SwitchButton();
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public void SwitchButton()
    //--------------------------------------------------
    {
        if (SimulationManager.singleton == null)
            return;

        if (SimulationManager.singleton.IsPlaying())
        {
            m_pauseButton.SetActive(true);
            m_playButton.SetActive(false);
        }
        else
        {
            m_playButton.SetActive(true);
            m_pauseButton.SetActive(false);
        }
    }
    //--------------------------------------------------
}
//--------------------------------------------------
