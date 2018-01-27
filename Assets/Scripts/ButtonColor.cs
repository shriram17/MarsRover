using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


//--------------------------------------------------
public class ButtonColor : MonoBehaviour
//--------------------------------------------------
{
    [SerializeField] private Button[] roverButtons;

    //--------------------------------------------------
    private void OnEnable()
    //--------------------------------------------------
    {
        EventsManager.OnSwitchRoverCamEvent += SwitchButtonColor;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void OnDisable()
    //--------------------------------------------------
    {
        EventsManager.OnSwitchRoverCamEvent -= SwitchButtonColor;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void Update()
    //--------------------------------------------------
    {
        //if buttons aren't assigned before hand in the inspector
        if (roverButtons.Length == 0)
        {
            // int numButtons = this.transform.childCount;

            //Find Button components, add listeners them
            roverButtons = GetComponentsInChildren<Button>();
            foreach(Button button in roverButtons)
            {
                button.onClick.AddListener(FileHighlighter);
            }
        }
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void SwitchButtonColor(int num)
    //--------------------------------------------------
    {
        for (int i =0;i<roverButtons.Length;i++)
        {
            //Set Selected Button Color 
            if (num == i)
                roverButtons[i].image.color = Color.red;
            else //Reset the color of the rest of the buttons
                roverButtons[i].image.color = Color.white;
        }  
    }
    //--------------------------------------------------

    //--------------------------------------------------
    // Check for matching object in buttonlist and change colors 
    private void FileHighlighter()
    //--------------------------------------------------
    {
        //Find the button user clicks on
        GameObject selectedGO = EventSystem.current.currentSelectedGameObject;
        Button selectedButton = selectedGO.GetComponent<Button>();
        for (int i = 0; i < roverButtons.Length; i++)
        {
            //Set Selected Button Color 
            if (selectedButton == roverButtons[i])
                roverButtons[i].image.color = Color.red;
            else //Reset the color of the rest of the buttons
                roverButtons[i].image.color = Color.white;
        }
    }
    //--------------------------------------------------
}
//--------------------------------------------------
