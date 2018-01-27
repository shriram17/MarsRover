using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//--------------------------------------------------
public class FadePanel : MonoBehaviour
//--------------------------------------------------
{
    //--------------------------------------------------
    public static FadePanel instance;

    private Image fadePanelUp;
    private float delay;
    //--------------------------------------------------

    //--------------------------------------------------
    void Awake()
    //--------------------------------------------------
    {
        if (null != instance)
        {
            DestroyImmediate(this.gameObject);
        }
        instance = this;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    void Start()
    //--------------------------------------------------
    {
        fadePanelUp = GetComponent<Image>();       
        StartCoroutine(FadeInCR());
    }
    //--------------------------------------------------

    #region Event Listeners
    //--------------------------------------------------
    void OnEnable()
    //--------------------------------------------------
    {
        EventsManager.OnTeleportEvent += FadeOutThenTeleport;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    void OnDisable()
    //--------------------------------------------------
    {
        EventsManager.OnTeleportEvent -= FadeOutThenTeleport;

    }
    //--------------------------------------------------
    #endregion    

    //--------------------------------------------------
    void FadeOutThenTeleport(TeleportCallback teleportCallback)
    //--------------------------------------------------
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutCR(teleportCallback));
    }
    //--------------------------------------------------

    //--------------------------------------------------
    IEnumerator FadeInCR()
    //--------------------------------------------------
    {
        fadePanelUp.enabled = true;
        fadePanelUp.canvasRenderer.SetAlpha(1f);
        fadePanelUp.CrossFadeAlpha(0.1f, 1f, false);
        yield return new WaitForSeconds(1f);
        fadePanelUp.enabled = false;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    IEnumerator FadeOutCR(TeleportCallback teleportCallback)
    //--------------------------------------------------
    {
        fadePanelUp.enabled = true;
        fadePanelUp.canvasRenderer.SetAlpha(0.1f);
        fadePanelUp.CrossFadeAlpha(1f, 1f, false);
        yield return new WaitForSeconds(1f);
        teleportCallback();
        StartCoroutine(FadeInCR());
    }
    //--------------------------------------------------
}
//--------------------------------------------------
