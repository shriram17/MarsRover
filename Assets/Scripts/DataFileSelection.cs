using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//--------------------------------------------------
public class DataFileSelection : MonoBehaviour
//--------------------------------------------------
{
    //--------------------------------------------------
    private DataLoaderManager m_dataLoader = null;
    private AudioSource buttonClickSound;
    //--------------------------------------------------

    //--------------------------------------------------
    private void Start()
    //--------------------------------------------------
    {
        buttonClickSound = GetComponent<AudioSource>();
        m_dataLoader = GetComponentInParent<DataLoaderManager>();
        
        if (null != m_dataLoader)
        {
            m_dataLoader.AddDataFileSelection(this.GetComponent<Button>());
        }
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public void SelectDataFile()
    //--------------------------------------------------
    {
        if (null == m_dataLoader)
            return;

        string filename = GetComponentInChildren<Text>().text;
        m_dataLoader.SelectFile(this.GetComponent<Button>(), ref filename);
        buttonClickSound.Play();
    }
    //--------------------------------------------------
}
//--------------------------------------------------
