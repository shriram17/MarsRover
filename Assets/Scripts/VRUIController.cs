using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//--------------------------------------------------
[RequireComponent(typeof(SteamVR_LaserPointer))]
public class VRUIController : MonoBehaviour
//--------------------------------------------------
{
    #region PRIVATE VARIABLES
    //--------------------------------------------------
    private SteamVR_LaserPointer laserPointer;
    private SteamVR_TrackedController trackedController;
    private bool isOn_prev;
    public SoundManager soundManager;

    private GameObject currentMonitor = null;
    private bool monitorAttached = false;
    private MeshRenderer highlightMeshRend;

    private GameObject currentSmallMonitor;
    private Color originalColor;
    //--------------------------------------------------
    #endregion PRIVATE VARIABLES


    #region MONOBEHAVIOUR
    //--------------------------------------------------
    private void OnEnable()
    //--------------------------------------------------
    {
        laserPointer = GetComponent<SteamVR_LaserPointer>();
        laserPointer.PointerIn += HandlePointerIn;
        laserPointer.PointerOut += HandlePointerOut;

        trackedController = GetComponent<SteamVR_TrackedController>();
        if (trackedController == null)
        {
            trackedController = GetComponentInParent<SteamVR_TrackedController>();
        }

        //Selecting Object Pointed to
        trackedController.TriggerClicked += HandleTriggerClicked;

        EventsManager.OnHandUIEvent += EventsManager_OnHandUIEvent;
        isOn_prev = false;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void OnDisable()
    //--------------------------------------------------
    {
        laserPointer.PointerIn -= HandlePointerIn;
        laserPointer.PointerOut -= HandlePointerOut;
        trackedController.TriggerClicked -= HandleTriggerClicked;
    }
    //--------------------------------------------------
    #endregion MONOBEHAVIOUR


    #region PRIVATE METHODS
    //--------------------------------------------------
    private void EventsManager_OnHandUIEvent(bool isOn)
    //--------------------------------------------------
    {
        laserPointer.pointer.SetActive(isOn);
        //Play Sound here, if pointer is switched On
        if (isOn == true && isOn_prev == false)
        {
            // Debug.Log("Switched On, Play sound");
            soundManager.OnSelectSound(soundManager.pointerActivated);
        }
        isOn_prev = isOn;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void HandleTriggerClicked(object sender, ClickedEventArgs e)
    //--------------------------------------------------
    {
        #region Attempt at moving camera feeds
        //if (!monitorAttached && currentMonitor != null)
        //{
        //    currentMonitor.transform.SetParent(transform);
        //    monitorAttached = true;
        //    Debug.Log("set parent");
        //    return;
        //}

        //if (monitorAttached)
        //{
        //    currentMonitor.transform.SetParent(null);
        //    monitorAttached = false;
        //    currentMonitor = null;
        //    Debug.Log("unparent, current object = " + currentMonitor.name);
        //    return;
        //}
        #endregion

        if (currentSmallMonitor != null)
        {
            soundManager.OnSelectSound(soundManager.roverButton);
            int cameraFeedNum = int.Parse(currentSmallMonitor.name);
            SimulationManager.singleton.SelectRover = cameraFeedNum + 1;
            EventsManager.SwitchRoverCam(cameraFeedNum);
        }

        if (EventSystem.current.currentSelectedGameObject != null)
        {
            GameObject clickedButton = EventSystem.current.currentSelectedGameObject;
            // Debug.Log(clickedButton.transform.name + " button selected");

            ExecuteEvents.Execute(clickedButton, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);

            // Call Custom Event to Switch Main Rover Camera Display 
            if (EventSystem.current.currentSelectedGameObject.CompareTag("RoverButton"))
                EventsManager.SwitchRoverCam(SimulationManager.roverNum);

            #region Attempt at moving camera feeds

            //if (EventSystem.current.currentSelectedGameObject.CompareTag("Grabbable") && !monitorAttached)
            //{
            //    currentMonitor.transform.SetParent(transform);
            //    monitorAttached = true;
            //    Debug.Log("set parent");
            //}
            //else if (monitorAttached)
            //{
            //    currentMonitor.transform.SetParent(null);
            //    monitorAttached = false;
            //    currentMonitor = null;
            //    Debug.Log("unparent, current object = " + currentMonitor.name);
            //}
            #endregion
        }
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void HandlePointerIn(object sender, PointerEventArgs e)
    //--------------------------------------------------
    {
        if (e.target.CompareTag("Switchable"))
        {
            currentSmallMonitor = e.target.gameObject;
            originalColor = currentSmallMonitor.GetComponent<Renderer>().material.color;
            //Color tempColor = Color.blue;
            currentSmallMonitor.GetComponent<Renderer>().material.color = Color.blue;
        }

        var button = e.target.GetComponent<Button>();
        if (button != null)
        {
            button.Select();            
        }     

        WatchButtonManager arrow = e.target.GetComponent<WatchButtonManager>();
        if (arrow != null)
        {
            arrow.PointerEnter();
        }

        #region Attempt at moving camera feeds
        //if (e.target.CompareTag("Grabbable"))
        //{
        //    currentMonitor = e.target.transform.root.gameObject;
        //    Debug.Log("pointer in Monitor");
        //    Debug.Log("current set object = " + currentMonitor.name);
        //highlightMeshRend = e.target.GetComponent<MeshRenderer>();
        //highlightMeshRend.enabled = true;     

        //right now:
        //pointer in, can select
        //trigger select, trigger again deselect, 
        //but since rightafter you delect, it's pointer in again!
        //monitorSelected = true;

        //{
        //    currentMonitor = null;
        //    Debug.Log("current set object = " + currentMonitor.name);
        //}
        #endregion
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void HandlePointerOut(object sender, PointerEventArgs e)
    //--------------------------------------------------
    {
        #region Attempt at moving camera feeds
        //if (e.target.CompareTag("Grabbable"))
        //{
        //    EventSystem.current.SetSelectedGameObject(null);
        //    currentMonitor = null;
        //    Debug.Log("pointer out");
        //    Debug.Log("pointer out, current = " + currentMonitor.name);
        //}

        //if (e.target.comparetag("grabbable"))
        //{
        //    //highlightmeshrend = e.target.getcomponent<meshrenderer>();
        //    //highlightmeshrend.enabled = false;
        //    currentmonitor = null;
        //    //monitorselected = false;
        //    debug.log("pointer out");
        //    debug.log("pointer out, current = " + currentmonitor.name);
        //}
        #endregion
        if (e.target.CompareTag("Switchable"))
        {
            if (currentSmallMonitor != null)
            {
                currentSmallMonitor.GetComponent<Renderer>().material.color = originalColor;
                currentSmallMonitor = null;
            }            
        }

        var button = e.target.GetComponent<Button>();
        if (button != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        WatchButtonManager arrow = e.target.GetComponent<WatchButtonManager>();
        if (arrow != null)
        {
            arrow.PointerExit();
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
    //--------------------------------------------------
    #endregion PRIVATE METHODS
}
//--------------------------------------------------
