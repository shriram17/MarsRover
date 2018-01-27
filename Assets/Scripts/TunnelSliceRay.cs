using UnityEngine;

//--------------------------------------------------
public class TunnelSliceRay : MonoBehaviour
//--------------------------------------------------
{
    //--------------------------------------------------
    private TunnelSliceScanner tunnelSliceScanner;
    public int row;
    private int count;
    //--------------------------------------------------

    //--------------------------------------------------
    void Start ()
    //--------------------------------------------------
    {
        tunnelSliceScanner = GetComponentInParent<TunnelSliceScanner>();
	}
    //--------------------------------------------------

    //--------------------------------------------------
    private void Update()
    //--------------------------------------------------
    {
        CastRay ();

//        if (count >= 2)//reduce raycast frequency
//        {
//            CastRay();
//            count = 0;
//        }
//        count++;
    }
    //--------------------------------------------------


    //--------------------------------------------------
    void CastRay ()
    //--------------------------------------------------
    {
        Ray raycast = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        bool bHit = Physics.Raycast(raycast, out hit);

		if (bHit && hit.transform.gameObject.CompareTag("Paintable"))
        {
			tunnelSliceScanner.AddRayHitPoint(hit.point);
        }
    }
    //--------------------------------------------------
}
//--------------------------------------------------
