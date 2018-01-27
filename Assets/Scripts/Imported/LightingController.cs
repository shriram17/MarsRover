using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingController : MonoBehaviour {

    public Light flashlight;
    bool realisticLightingOn = false;

	void Start()
    {
        SwitchToRealisticLighting();
	}
	
	void Update()
    {
		if (Input.GetKeyDown(KeyCode.R))
        {
            SwitchToRealisticLighting();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            SwitchToAmbientLighting();
        }
    }

    public void ToggleLighting()
    {
        if (realisticLightingOn)
        {
            SwitchToAmbientLighting();
        }
        else
        {
            SwitchToRealisticLighting();
        }
    }

    private void SwitchToAmbientLighting()
    {
        RenderSettings.ambientLight = new Color(0.5f, 0.5f, 0.5f);
        RenderSettings.reflectionIntensity = 1.0f;
        flashlight.enabled = false;

        realisticLightingOn = false;
    }

    private void SwitchToRealisticLighting()
    {
        RenderSettings.ambientLight = new Color(0.05f, 0.05f, 0.05f);
        RenderSettings.reflectionIntensity = 0.0f;
        flashlight.enabled = true;

        realisticLightingOn = true;
    }
}
