﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------
public class FlagController : MonoBehaviour
//--------------------------------------------------
{
    //--------------------------------------------------
    private GameObject m_playerHead;
    //--------------------------------------------------

    //--------------------------------------------------
    private void Start()
    //--------------------------------------------------
    {
        m_playerHead = Camera.main.gameObject;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void Update()
    //--------------------------------------------------
    {
        this.transform.LookAt(m_playerHead.transform.position);
    }
    //--------------------------------------------------
}
//--------------------------------------------------
