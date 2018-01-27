using UnityEngine;

//--------------------------------------------------
//[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
//--------------------------------------------------
{
    //--------------------------------------------------
    public AudioClip roverButton;
    public AudioClip pointerActivated;
    [SerializeField] private AudioClip teleportButton;
    [SerializeField] private AudioClip pointerDeactivated;
    [SerializeField] private AudioClip loadButton;
    [SerializeField] private AudioClip watchButton;
    [SerializeField] private AudioClip roverMoving;
    [SerializeField] private AudioClip roverTurning;
    //[SerializeField] private AudioClip backgroundScore;
    [SerializeField] private AudioSource backgroundSource;
    private AudioSource audioSource;
    private bool isInControlRoom = true;

    // public AudioClip audioClip;
    //--------------------------------------------------

    //--------------------------------------------------
    // Use this for initialization
    void Start ()
    //--------------------------------------------------
    {
        audioSource = GetComponent<AudioSource>();
        EventsManager.OnTeleportToTunnel += InTunnelCheck;
        EventsManager.OnTeleportToCommandCenter += InCommandCenterCheck;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    private void OnDestroy()
    //--------------------------------------------------
    {
        EventsManager.OnTeleportToTunnel -= InTunnelCheck;
        EventsManager.OnTeleportToTunnel -= InCommandCenterCheck;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public void OnSelectSound(AudioClip currentClip)
    //--------------------------------------------------
    {
        //AudioClip audioClip = audioSource.GetComponent<AudioClip>();
        audioSource.clip = currentClip;
       // audioClip = currentClip;
       
       //Lower Volume for Laser Pointer
       if(currentClip == pointerActivated)
        {
            //Debug.Log("Pointer On Sound");
            audioSource.volume = 0.25f;
            audioSource.pitch = 1.2f;
            audioSource.Play();
        }
       else
        {
            audioSource.volume = 1.0f;
            audioSource.pitch = 1.0f;
            audioSource.Play();
        }
        //Debug.Log("Sound " + audioSource.clip.ToString() +" Played");
    }
    //--------------------------------------------------

    //--------------------------------------------------
    void InTunnelCheck()
    //--------------------------------------------------
    {
        isInControlRoom = false;
        backgroundSource.Stop();
    }
    //--------------------------------------------------

    //--------------------------------------------------
    void InCommandCenterCheck()
    //--------------------------------------------------
    {
        isInControlRoom = true;
        //backgroundSource.clip = backgroundScore;
        backgroundSource.Play();
    }
    //--------------------------------------------------
}
//--------------------------------------------------
