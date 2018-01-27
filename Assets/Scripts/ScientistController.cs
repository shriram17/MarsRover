using UnityEngine;

//--------------------------------------------------
public class ScientistController : MonoBehaviour
//--------------------------------------------------
{
    #region PUBLIC VARIABLES
    //--------------------------------------------------
    [System.Serializable]
    public class Controller
    {
        public GameObject controllerGO;
        public SteamVR_TrackedObject trackedObj;
        public SteamVR_Controller.Device device;
        public bool gripPressed;
    }
    [Header("Tracked controller")]
    public Controller controllerRight, controllerLeft;    
    //--------------------------------------------------
    #endregion PUBLIC VARIABLES


    #region PRIVATE VARIABLES
    //--------------------------------------------------
    #region PRIVATE VARIABLES: Serialize field
    //--------------------------------------------------
    [Header("")]
    [SerializeField] private GameObject m_tunnelMaps;
    [Header("")]
    [SerializeField] private GameObject[] m_teleportationDevices;
    //--------------------------------------------------
    #endregion PRIVATE VARIABLES: Serialize field

    private float previousDistance, previousZDistance;
    private bool scalingStarted, rotatingStarted, 
        movingStartedWithRight, movingStartedWithLeft;
    private Vector3 previousPos, previousDirection;

    private Vector3 m_previousMapPosition;
    private Vector3 m_previousMapScale;
    private Vector3 m_prevContrPosForTranslating;
    private Vector3 m_previousEulerAngles;
    private Vector3 m_scaleOrigin;
    private Vector3 m_scalePositionOffset;
    private Vector3 m_scaleReferencePosition;

    private Vector3
        m_prevLeftContrPosRelativeToHead,
        m_prevRightContrPosRelativeToHead;
    private Vector3 m_vecFromLeftToRightController;
    private bool m_clockwiseRotation, m_counterClockwiseRotation;
    private bool m_controllersIntersectingTunnel = false;
    private bool m_scientistInControlRoom = true;

    private GameObject GrabbedObject;
    private bool Holding;

    private const float ROTATE_ANGLE_THRESHOLD = 10f;
    //--------------------------------------------------
    #endregion PRIVATE VARIABLES


    #region MONOBEHAVIOUR
    //--------------------------------------------------
    private void Start()
    //--------------------------------------------------
    {
        EventsManager.OnTeleportToTunnel += EnableTeleportDevices;
        EventsManager.OnTeleportToCommandCenter += DisableTeleportDevices;

        Invoke("DisableTeleportDevices", 2f);
    }
    //--------------------------------------------------


    //--------------------------------------------------
    private void OnDestroy()
    //--------------------------------------------------
    {
        EventsManager.OnTeleportToTunnel -= EnableTeleportDevices;
        EventsManager.OnTeleportToCommandCenter -= DisableTeleportDevices;
    }
    //--------------------------------------------------


    //--------------------------------------------------
    void Update()
    //--------------------------------------------------
    {
        if (null != controllerRight && (int)controllerRight.trackedObj.index > -1)
            controllerRight.device = SteamVR_Controller.Input((int)controllerRight.trackedObj.index);
        else return;

        if (null != controllerLeft && (int)controllerLeft.trackedObj.index > -1)
            controllerLeft.device = SteamVR_Controller.Input((int)controllerLeft.trackedObj.index);
        else return;

        SimulationManager.singleton.controllersTracked = true;

        if (!SimulationManager.singleton.tunnelScanCompleted)
            return;

        #region Controls for grip buttons simultaneously held
        if (GripPressed(controllerRight) && GripPressed(controllerLeft))
        {
            if (!m_scientistInControlRoom)
                return;

            movingStartedWithRight = false;

            #region Controls for scaling the model
            //scaling the model---------------------------       
            //if both grip button are pressed, enable scaling   
            //at the start of scaling, reset previousDistance
            if (!scalingStarted)
            {
                m_controllersIntersectingTunnel =
                    controllerLeft.controllerGO.
                    GetComponentInChildren<ControllerTrigger>().
                    IntersectingMapModel();
                m_controllersIntersectingTunnel =
                    controllerRight.controllerGO.
                    GetComponentInChildren<ControllerTrigger>().
                    IntersectingMapModel();

                m_vecFromLeftToRightController =
                    controllerRight.trackedObj.transform.position - controllerLeft.trackedObj.transform.position;

                previousDistance =
                    (m_vecFromLeftToRightController)
                    .magnitude;
                m_previousMapPosition = m_tunnelMaps.transform.position;
                m_previousMapScale = m_tunnelMaps.transform.localScale;

                m_scaleOrigin =
                    controllerLeft.trackedObj.transform.position +
                    (controllerRight.trackedObj.transform.position - controllerRight.trackedObj.transform.position) / 2;

                if (m_controllersIntersectingTunnel)
                { m_scalePositionOffset = m_tunnelMaps.transform.position - m_scaleOrigin;
                } else
                {
                    m_scaleReferencePosition = m_tunnelMaps.GetComponent<MapsManager>().
                        FindClosestScaleReference(m_scaleOrigin);
                    m_scalePositionOffset = m_tunnelMaps.transform.position - m_scaleReferencePosition;
                 }

                scalingStarted = true;
            }

            ScaleModel();
            #endregion
            
            #region Controls for rotating the model
            /*//when both grips are pressed, rotate model according to changes in relative distance in Z axis
            // Rotate using change in angle of controller positions ~CF 09/11/2017
            if (!rotatingStarted)
            {
                //float currentZDistance;
                //currentZDistance = controllerRight.trackedObj.transform.position.z - controllerLeft.trackedObj.transform.position.z;
                //previousZDistance = currentZDistance;
                //				Vector3 currentDirection;
                //				currentDirection = controllerRight.trackedObj.transform.position - controllerLeft.trackedObj.transform.position;
                //				previousDirection = currentDirection;

                m_vecFromLeftToRightController =
                    controllerRight.trackedObj.transform.position - controllerLeft.trackedObj.transform.position;

                m_prevLeftContrPosRelativeToHead =
                    controllerLeft.trackedObj.transform.localPosition - m_hmdEye.transform.localPosition;
                m_prevRightContrPosRelativeToHead =
                    controllerRight.trackedObj.transform.localPosition - m_hmdEye.transform.localPosition;

                m_previousEulerAngles = tunnelModel.transform.eulerAngles;

                rotatingStarted = true;
            }
            //RotateModel(ChangeInDistanceRelativeZ());
            RotateModel();*/
            #endregion
        }

        if (!GripPressed(controllerRight) || !GripPressed(controllerLeft))
        {
            scalingStarted = false;
            //rotatingStarted = false;
        }
        #endregion

        #region Controls for moving the model
        //when right grip is pressed down, check if left grip is pressed, only then can you start moving
        //doing this way prevents when player scales/rotates and lets go the left grip before the right grip, which would cause accidental moving
        if (controllerRight.device.GetPressDown(SteamVR_Controller.ButtonMask.Grip) && 
            !GripPressed(controllerLeft) && 
            !movingStartedWithRight)
        {
            if (!m_scientistInControlRoom)
                return;

            movingStartedWithRight = true;
            m_previousMapPosition = m_tunnelMaps.transform.position;
            m_prevContrPosForTranslating = controllerRight.trackedObj.transform.position;
        }

        if (movingStartedWithRight)
        {
            TranslateMap(false);
        }

        if (!GripPressed(controllerRight))
        {
            movingStartedWithRight = false;
        }
        #endregion

        #region Thumbstick controls

        float rightThumbstickX = controllerRight.device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
        float rightThumbstickY = controllerRight.device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).y;

        if (rightThumbstickX < 0f)
        {
            ThumbStickRotate(rightThumbstickX);
        }
        else if (rightThumbstickX > 0f)
        {
            ThumbStickRotate(rightThumbstickX);
        }

        if (controllerRight.device.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad))
        {
            if (!m_scientistInControlRoom)
                return;

            if (SimulationManager.singleton != null)
                SimulationManager.singleton.ResetMapPosition();
        }

        float leftThumbstickX = controllerLeft.device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;

        if (leftThumbstickX < -0.5f)
        {
            // Rewind
            if (SimulationManager.singleton != null)
                SimulationManager.singleton.Rewind();
        }
        else if (leftThumbstickX > 0.5f)
        {
            // Fast forward
            if (SimulationManager.singleton != null)
                SimulationManager.singleton.FastForward();
        }

        #endregion

        #region handUI related
        if (controllerLeft.device.GetPressDown(Valve.VR.EVRButtonId.k_EButton_ApplicationMenu))
        {
            //Debug.Log("HandUI");
            controllerLeft.trackedObj.gameObject.GetComponent<UIActivator>().
                SwitchActiveState();
        }
        #endregion
    }
    //--------------------------------------------------
    #endregion MONOBEHAVIOUR


    #region PRIVATE METHODS
    #region PRIVATE METHODS: Scale, rotate, and move functions
    //--------------------------------------------------
    void ScaleModel()
    //--------------------------------------------------
    {
        if (!m_scientistInControlRoom)
            return;

        const float MIN_SCALE = 0.01f;
        const float MAX_SCALE = .25f;
        const float SCALE_SPEED = 10f;

        Vector3 curVecLeftToRightController =
           controllerRight.trackedObj.transform.position - controllerLeft.trackedObj.transform.position;

        float angleDiff = Vector3.Angle(
            m_vecFromLeftToRightController, curVecLeftToRightController);

        //if (angleDiff > ROTATE_ANGLE_THRESHOLD)
        //    return;

        float distanceBetweenControllers =
            (controllerRight.trackedObj.transform.position -
            controllerLeft.trackedObj.transform.position)
            .magnitude;
        float scaleAmount = distanceBetweenControllers - previousDistance;

        float scaleMult =
            Mathf.Clamp(
                Mathf.Pow(m_tunnelMaps.transform.localScale.x, 3),
                1f,
                Mathf.Pow(MAX_SCALE, 3));

        //Debug.Log(scaleAmount);

        Vector3 targetScale = new Vector3(
            Mathf.Clamp(m_previousMapScale.x + scaleAmount * scaleMult, MIN_SCALE, MAX_SCALE),
            Mathf.Clamp(m_previousMapScale.y + scaleAmount * scaleMult, MIN_SCALE, MAX_SCALE),
            Mathf.Clamp(m_previousMapScale.z + scaleAmount * scaleMult, MIN_SCALE, MAX_SCALE));

        m_tunnelMaps.transform.localScale = Vector3.Lerp(
            m_tunnelMaps.transform.localScale,
            targetScale,
            SCALE_SPEED * Time.unscaledDeltaTime);

        if (m_tunnelMaps.transform.localScale.x <= MIN_SCALE ||
            m_tunnelMaps.transform.localScale.y <= MIN_SCALE ||
            m_tunnelMaps.transform.localScale.z <= MIN_SCALE ||
            m_tunnelMaps.transform.localScale.x >= MAX_SCALE ||
            m_tunnelMaps.transform.localScale.y >= MAX_SCALE ||
            m_tunnelMaps.transform.localScale.z >= MAX_SCALE)
            return;

        Vector3 currentScaleOrigin =
            controllerLeft.trackedObj.transform.position + curVecLeftToRightController / 2;
        Vector3 scaleOriginOffset = currentScaleOrigin - m_scaleOrigin;

        if (scaleOriginOffset.magnitude < 1f)
            scaleOriginOffset = Vector3.zero;

        float scaleRatio =
            m_tunnelMaps.transform.localScale.x / m_previousMapScale.x;

        Vector3 targetPosition = m_tunnelMaps.transform.position;
        if (m_controllersIntersectingTunnel)
        {
            targetPosition = m_scaleOrigin +
              (m_scalePositionOffset + scaleOriginOffset) * scaleRatio;
        }
        else
        {
            targetPosition = m_scaleReferencePosition +
                m_scalePositionOffset * scaleRatio;
        }

        m_tunnelMaps.transform.position =
                Vector3.Lerp(
                    m_tunnelMaps.transform.position,
                    targetPosition,
                    1f);
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void ThumbStickRotate(float pushMagnitude)
    //--------------------------------------------------
    {
        if (!m_scientistInControlRoom)
            return;

        Vector3 mapRotateIncrement = new Vector3(0f, 2f, 0f) * pushMagnitude;
        float mapRotateSpeed = 20f;

        float normalizedRotationDelta =
            Mathf.Clamp(1 / (Mathf.Pow(m_tunnelMaps.transform.localScale.x, 2)), 0.2f, 1f);

        m_tunnelMaps.transform.eulerAngles = Vector3.Lerp(
                m_tunnelMaps.transform.eulerAngles,
                m_tunnelMaps.transform.eulerAngles + mapRotateIncrement * normalizedRotationDelta,
                mapRotateSpeed * Time.deltaTime);
    }
    //--------------------------------------------------

    //--------------------------------------------------
    void TranslateMap(bool left)
    //--------------------------------------------------
    {
        if (!m_scientistInControlRoom)
            return;

        if (scalingStarted || rotatingStarted)
            return;

        Vector3 currentControllerPosition;
        if (!left)
            currentControllerPosition = controllerRight.trackedObj.transform.position;
        else
            currentControllerPosition = controllerLeft.trackedObj.transform.position;

        float moveSpeedFactor = 1f;

        if (m_tunnelMaps.transform.localScale.x >= 0.1f)
            moveSpeedFactor = m_tunnelMaps.transform.localScale.x * 10f;

        if (Vector3.Distance(currentControllerPosition, m_prevContrPosForTranslating) > 0.001f)
        {
            Vector3 offset = currentControllerPosition - m_prevContrPosForTranslating;

            Vector3 targetMapPosition = m_previousMapPosition + offset * moveSpeedFactor;
            m_tunnelMaps.transform.position = targetMapPosition;
        }
    }
    //--------------------------------------------------
    #endregion PRIVATE METHODS: Scale, rotate, and move functions

    //--------------------------------------------------
    private bool GripPressed(Controller controller)
    //--------------------------------------------------
    {
        if (controller.device.GetPress(SteamVR_Controller.ButtonMask.Grip))
        {
            controller.gripPressed = true;
        }
        else
        {
            controller.gripPressed = false;
        }
        return controller.gripPressed;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void EnableTeleportDevices()
    //--------------------------------------------------
    {
        for (int i = 0; i < m_teleportationDevices.Length; i++)
            m_teleportationDevices[i].SetActive(true);
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void DisableTeleportDevices()
    //--------------------------------------------------
    {
        for (int i = 0; i < m_teleportationDevices.Length; i++)
            m_teleportationDevices[i].SetActive(false);
    }
    //--------------------------------------------------
    #endregion PRIVATE METHODS


    #region PUBLIC METHODS
    //--------------------------------------------------
    public bool ScientistInControlRoom
    //--------------------------------------------------
    {
        get
        {
            return m_scientistInControlRoom;
        }

        set
        {
            m_scientistInControlRoom = value;
        }
    }
    //--------------------------------------------------
    #endregion PUBLIC METHODS
}
//--------------------------------------------------
