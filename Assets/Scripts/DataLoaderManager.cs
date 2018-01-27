using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

//--------------------------------------------------
public class DataLoaderManager : MonoBehaviour
//--------------------------------------------------
{
    #region PRIVATE VARIABLES
    //--------------------------------------------------
    [SerializeField] private GameObject m_dataListWindow;
    [SerializeField] private GameObject m_dataListingPrefab;
    [SerializeField] private Color m_selectedButtonColor = Color.grey;

    private List<GameObject> m_dataListings = new List<GameObject>();
    private List<Button> m_dataFileSelectionList = new List<Button>();
    private string m_selectedFilename = "";
    private string m_directory;
    //--------------------------------------------------
    #endregion PRIVATE VARIABLES


    #region MONOBEHAVIOUR
    //--------------------------------------------------
    private void Start()
    //--------------------------------------------------
    {
        m_directory = Application.dataPath + "/Resources/Data/";

        ListDataFiles();
    }
    //--------------------------------------------------
    #endregion MONOBEHAVIOUR


    #region PUBLIC METHODS
    //--------------------------------------------------
    public void AddDataFileSelection(Button selection)
    //--------------------------------------------------
    {
        m_dataFileSelectionList.Add(selection);
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public void SelectFile(Button button, ref string filename)
    //--------------------------------------------------
    {
        m_selectedFilename = filename;

        ResetSelectionColor();

        ColorBlock colors = button.colors;
        colors.normalColor = m_selectedButtonColor;
        button.colors = colors;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public void LoadData()
    //--------------------------------------------------
    {
        if ("" == m_selectedFilename)
        {
            Debug.Log("Must select a data file before loading!");
            return;
        }

        SimulationManager.singleton.LoadDataFile(ref m_selectedFilename);
    }
    //--------------------------------------------------
    #endregion PUBLIC METHODS


    #region PRIVATE METHODS
    //--------------------------------------------------
    private void ResetSelectionColor()
    //--------------------------------------------------
    {
        foreach (Button curButton in m_dataFileSelectionList)
        {
            ColorBlock colors = curButton.colors;
            colors.normalColor = Color.white;
            curButton.colors = colors;
        }
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void ListDataFiles()
    //--------------------------------------------------
    {
        DirectoryInfo dir = new DirectoryInfo(m_directory);
        FileInfo[] filesArray = dir.GetFiles("*.txt");

        foreach (FileInfo file in filesArray)
        {
            GameObject curListing = Instantiate(m_dataListingPrefab, m_dataListWindow.transform);
            Text filenameText = curListing.GetComponentInChildren<Text>();
            filenameText.text = file.Name;

            m_dataListings.Add(curListing);
        }
    }
    //--------------------------------------------------
    #endregion PRIVATE METHODS
}
//--------------------------------------------------
