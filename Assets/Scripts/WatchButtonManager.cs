using UnityEngine;
using UnityEngine.UI;

//--------------------------------------------------
public class WatchButtonManager : MonoBehaviour
//--------------------------------------------------
{
    //--------------------------------------------------
    [SerializeField] protected Sprite m_normalSprite;
    [SerializeField] protected Sprite m_selectedSprite;

    protected Image m_arrowImage;
    //--------------------------------------------------

    //--------------------------------------------------
    protected virtual void OnEnable()
    //--------------------------------------------------
    {
        m_arrowImage = GetComponent<Image>();
        m_arrowImage.sprite = m_normalSprite;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public void PointerEnter()
    //--------------------------------------------------
    {
        m_arrowImage.sprite = m_selectedSprite;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    public void PointerExit()
    //--------------------------------------------------
    {
        m_arrowImage.sprite = m_normalSprite;
    }
    //--------------------------------------------------
}
//--------------------------------------------------
