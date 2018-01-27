using UnityEngine;

//--------------------------------------------------
public class UIActivator : MonoBehaviour
//--------------------------------------------------
{
    #region PRIVATE VARIABLES
    //--------------------------------------------------
    [SerializeField] private Transform m_eyeTransform;
    [SerializeField] private GameObject m_handUI = null;
    [SerializeField] private GameObject m_watchUI = null;

    private SteamVR_TrackedController trackedController;
    private bool m_handUIActive = true;
    //--------------------------------------------------
    #endregion PRIVATE VARIABLES


    #region MONOBEHAVIOUR
    //--------------------------------------------------
    private void Start ()
    //--------------------------------------------------
    {
        if (m_watchUI != null)
            m_watchUI.SetActive(false);

        if (m_handUI != null)
            m_handUI.SetActive(m_handUIActive);

        trackedController = GetComponent<SteamVR_TrackedController>();
        if (trackedController == null)
        {
            trackedController = GetComponentInParent<SteamVR_TrackedController>();
        }
        //trackedController.MenuButtonClicked += TrackedController_MenuButtonClicked;
        //trackedController.MenuButtonUnclicked += TrackedController_MenuButtonUnclicked;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void Update()
    //--------------------------------------------------
    {
        if (m_watchUI != null)
        {
            if (WatchViewPosition())
            {
                m_watchUI.SetActive(true);
                m_handUI.SetActive(false);
                //HandUIActivated();
            }
            else
            {
                m_watchUI.SetActive(false);
                m_handUI.SetActive(m_handUIActive);
            }
        }

        if (m_watchUI.activeSelf || m_handUI.activeSelf)
            EventsManager.UIActiveState(true);
        else
            EventsManager.UIActiveState(false);
    }
    //--------------------------------------------------
    #endregion MONOBEHAVIOUR


    #region PUBLIC METHODS
    //--------------------------------------------------
    public void RefreshUI()
    //--------------------------------------------------
    {
        Invoke("SwitchActiveState", .5f);
        Invoke("SwitchActiveState", 1f);
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public void SwitchActiveState()
    //--------------------------------------------------
    {
        m_handUIActive = !m_handUIActive;
    }
    //--------------------------------------------------
    #endregion PUBLIC METHODS


    #region PRIVATE METHODS
    //--------------------------------------------------
    private bool WatchViewPosition()
    //--------------------------------------------------
    {
        Vector3 rotationRelativeToHead = 
            this.transform.eulerAngles - m_eyeTransform.eulerAngles;
        //Debug.Log(rotationRelativeToHead);

        float xRot = rotationRelativeToHead.x;
        float yRot = rotationRelativeToHead.y;
        float zRot = rotationRelativeToHead.z;

        if (180f <= xRot)
        {
            xRot -= 360f;
        }
        else if (-180f >= xRot)
        {
            xRot += 360f;
        }

        if (180f <= yRot)
        {
            yRot -= 360f; ;
        }
        else if (-180f >= yRot)
        {
            yRot += 360f;
        }

        if (180f <= zRot)
        {
            zRot -= 360f;
        }
        else if (-180f >= zRot)
        {
            zRot += 360f;
        }

        //Debug.Log("xRot: " + xRot + ", yRot: " + yRot + ", zRot: " + zRot);

        if (-50f <= xRot && 70f >= xRot &&
            20f <= yRot && 130f >= yRot &&
            -150f <= zRot && 20f >= zRot)
        {
            return true;
        }

        return false;
    }
    //--------------------------------------------------
    #endregion PRIVATE METHODS
}
//--------------------------------------------------
