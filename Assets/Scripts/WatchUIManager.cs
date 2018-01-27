using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

//--------------------------------------------------
public class WatchUIManager : MonoBehaviour
//--------------------------------------------------
{
    #region PRIVATE VARIABLES
    [SerializeField] private Text m_dayText;
    [SerializeField] private Text m_timeText;
    [SerializeField] private Text m_playbackSpeedText;

    private float m_timeUpdateRate = 0.5f;
    private float m_delayUntilNextUpdate = 0f;
    #endregion // PRIVATE VARIABLES


    #region MONOBEHAVIOUR
    //--------------------------------------------------
    private void Start()
    //--------------------------------------------------
    {
        m_timeText.text = "00:00:00";
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void OnEnable()
    //--------------------------------------------------
    {
        UpdatePlaybackSpeed();
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void Update()
    //--------------------------------------------------
    {
        if (m_delayUntilNextUpdate > 0f)
        {
            m_delayUntilNextUpdate -= Time.deltaTime;
            return;
        }

        SetWatchTime();

        m_delayUntilNextUpdate = m_timeUpdateRate;
    }
    //--------------------------------------------------
    #endregion // MONOBEHAVIOUR


    #region PUBLIC METHODS
    //--------------------------------------------------
    public void UpdatePlaybackSpeed()
    //--------------------------------------------------
    {
        if (SimulationManager.singleton == null)
            return;

        // ex: "100x"
        m_playbackSpeedText.text =
            SimulationManager.singleton.PlaybackSpeed().ToString() + "x";
    }
    //--------------------------------------------------
    #endregion PUBLIC METHODS


    #region PRIVATE METHODS
    //--------------------------------------------------
    private void SetWatchTime()
    //--------------------------------------------------
    {
        float totalTime = SimulationManager.singleton.CurrentTime();
        if (totalTime < 0f)
            totalTime = 0f;

        int curDay = (int)Mathf.Floor(totalTime / (60*60*24)) + 1;

        float timeInDay = totalTime / curDay;

        TimeSpan displayTime = TimeSpan.FromSeconds(timeInDay);

        m_dayText.text = "Day " + curDay.ToString();
        m_timeText.text = displayTime.ToString();
    }
    //--------------------------------------------------
    #endregion PRIVATE METHODS
}
//--------------------------------------------------
