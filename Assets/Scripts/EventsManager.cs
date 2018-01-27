using UnityEngine;


//--------------------------------------------------
public class EventsManager : MonoBehaviour
//--------------------------------------------------
{
    //--------------------------------------------------
    public delegate void GenerateTunnelSliceEventsHandler (Vector3 location, float var1, float var2);
	public delegate void TunnelEventsHandler ();
    public delegate void RoverCamEventsHandler(int num);
    public delegate void UIEventHandler(bool isOn);
    public delegate void Event_TeleportToTunnel();
    public delegate void Event_TeleportToCommandCenter();

    public delegate void TeleportEventHandler(TeleportCallback teleportCallback);//new
    //--------------------------------------------------

    //--------------------------------------------------
    // public static event GenerateTunnelEventsHandler OnGenerateTunnelEvent;
    public static event GenerateTunnelSliceEventsHandler OnGenerateTunnelSliceEvent;
	public static event TunnelEventsHandler OnShowTemperatureEvent;
    public static event TunnelEventsHandler OnShowOxygenEvent;
    public static event RoverCamEventsHandler OnSwitchRoverCamEvent;
    public static event UIEventHandler OnHandUIEvent;
    public static event Event_TeleportToTunnel OnTeleportToTunnel;
    public static event Event_TeleportToCommandCenter OnTeleportToCommandCenter;

    public static event TeleportEventHandler OnTeleportEvent;//new
    //--------------------------------------------------


    //--------------------------------------------------
    public static void GenerateTunnelSlice (Vector3 location, float var1, float var2)
    //--------------------------------------------------
    {
        if (OnGenerateTunnelSliceEvent != null)
		{
            OnGenerateTunnelSliceEvent(location, var1, var2);
		}
	}
    //--------------------------------------------------

    //--------------------------------------------------
    public static void ShowTemperature ()
    //--------------------------------------------------
    {
        if (OnShowTemperatureEvent != null)
		{
			OnShowTemperatureEvent();
		}
	}
    //--------------------------------------------------

    //--------------------------------------------------
    public static void ShowOxygen()
    //--------------------------------------------------
    {
        if (OnShowOxygenEvent != null)
        {
            OnShowOxygenEvent();
        }
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public static void SwitchRoverCam(int num)
    //--------------------------------------------------
    {
        if (OnSwitchRoverCamEvent != null)
        {
            OnSwitchRoverCamEvent(num);
            //Debug.Log("Switching to rover " + (num+1).ToString());
        }
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public static void UIActiveState(bool isOn)
    //--------------------------------------------------
    {
        if (OnHandUIEvent != null)
        {
            OnHandUIEvent(isOn);
           // Debug.Log("Hand/Watch UI is On");
        }
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public static void TeleportToTunnel()
    //--------------------------------------------------
    {
        if (OnTeleportToTunnel != null)
        {
            OnTeleportToTunnel();
        }
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public static void TeleportToCommandCenter()
    //--------------------------------------------------
    {
        if (OnTeleportToCommandCenter != null)
        {
            OnTeleportToCommandCenter();
        }
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public static void Teleport(TeleportCallback teleportCallback)
    //--------------------------------------------------
    {
        if (OnTeleportEvent != null)
        {
            OnTeleportEvent(teleportCallback);
        }
    }
    //--------------------------------------------------
}
//--------------------------------------------------


