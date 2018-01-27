using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerLightingSwitcher : MonoBehaviour {

    public LightingController lightingController;

	// Use this for initialization
	void Start()
    {
        var trackedController = GetComponent<SteamVR_TrackedController>();
        if (trackedController == null)
        {
            trackedController = gameObject.AddComponent<SteamVR_TrackedController>();
        }

        trackedController.PadClicked += new ClickedEventHandler(PadClicked);
        trackedController.Gripped += new ClickedEventHandler(GripClicked);
    }
	
    private void GripClicked(object sender, ClickedEventArgs e)
    {
        lightingController.ToggleLighting();
    }

    private void PadClicked(object sender, ClickedEventArgs e)
    {
        lightingController.ToggleLighting();
    }
}
