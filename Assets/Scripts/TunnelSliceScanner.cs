using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--------------------------------------------------
public class TunnelSliceScanner : MonoBehaviour
//--------------------------------------------------
{
    //--------------------------------------------------
    public GameObject slice;
	public GameObject slicesParent;

    [SerializeField] private GameObject tunnelFloorParent = null;
    [SerializeField] private GameObject floorTilePrefab = null;

	private float moveSpeed = 0.5f;    
    private bool startMove = false;

	public float sliceThickness = 0.01f;
	public float sliceSpacing = 0.1f;
	private float prevSlicePosX;
	public List<Vector3> rayHitPointList = new List<Vector3>();
	private List<Vector3> slicePositionsList = new List<Vector3> ();
	private List<Vector3> sliceDimensionsList = new List<Vector3> ();
    //--------------------------------------------------

    //--------------------------------------------------
    public void StartScan()
    //--------------------------------------------------
    {
        StartCoroutine(DelayStart());
		prevSlicePosX = transform.position.x;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    void Update ()
    //--------------------------------------------------
    {
        if (startMove)
        {
            if (transform.position.x < -2f)
            {
                gameObject.SetActive(false);
                SimulationManager.singleton.TunnelSliceScanComplete();
                return;
            }
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        }

		if (transform.position.x < prevSlicePosX - sliceSpacing) {
			GenerateSlice ();
			prevSlicePosX = transform.position.x;
		}               
	}
    //--------------------------------------------------

    //--------------------------------------------------
    IEnumerator DelayStart()
    //--------------------------------------------------
    {
        yield return new WaitForSeconds(1f);
        startMove = true;
    }
    //--------------------------------------------------

    //--------------------------------------------------
    void GenerateSlice ()
    //--------------------------------------------------
    {
        if (rayHitPointList.Count > 0)
        {

            float maxZ = rayHitPointList[0].z;
            float minZ = rayHitPointList[0].z;
            float maxY = rayHitPointList[0].y;
            float minY = rayHitPointList[0].y;

            for (int i = 0; i < rayHitPointList.Count; i++)
            {
                if (rayHitPointList[i].z > maxZ)
                {
                    maxZ = rayHitPointList[i].z;
                }
                if (rayHitPointList[i].z < minZ)
                {
                    minZ = rayHitPointList[i].z;
                }
                if (rayHitPointList[i].y > maxY)
                {
                    maxY = rayHitPointList[i].y;
                }
                if (rayHitPointList[i].y < minY)
                {
                    minY = rayHitPointList[i].y;
                }
            }

            Vector3 slicePos = new Vector3();
            slicePos.x = rayHitPointList[0].x;
            slicePos.y = (maxY + minY) / 2;
            slicePos.z = (maxZ + minZ) / 2;

            Vector3 sliceDimension = new Vector3();
            sliceDimension.x = maxZ - minZ;
            sliceDimension.y = maxY - minY;
            sliceDimension.z = sliceThickness;

            slicePositionsList.Add(slicePos);
            sliceDimensionsList.Add(sliceDimension);

            GameObject sliceObj = Instantiate(slice, slicePos, Quaternion.identity) as GameObject;
            sliceObj.transform.localScale = sliceDimension;
            sliceObj.transform.SetParent(slicesParent.transform);
            sliceObj.transform.localRotation = Quaternion.identity;
            slicesParent.GetComponent<DataSlicesManager>().AddSlice(sliceObj);

            if (null != floorTilePrefab
                && null != tunnelFloorParent)
            {
                Vector3 floorTilePos = new Vector3(
                  rayHitPointList[0].x,
                  minY,
                  (maxZ + minZ) / 2);
                Vector3 floorTileDimensions = new Vector3(
                    sliceThickness * 2,
                    maxZ - minZ,
                    1f);
                GameObject floorTile = Instantiate(floorTilePrefab);
                floorTile.transform.position = floorTilePos;
                floorTile.transform.localScale = floorTileDimensions;
                floorTile.transform.SetParent(tunnelFloorParent.transform);
            }
        }
		rayHitPointList.Clear ();
	}
    //--------------------------------------------------

    //--------------------------------------------------
    public void AddRayHitPoint (Vector3 point)
    //--------------------------------------------------
    {
        rayHitPointList.Add (point);
	}
    //--------------------------------------------------

    //--------------------------------------------------
    public List<Vector3> GetSlicesPositionsList ()
    //--------------------------------------------------
    {
        return slicePositionsList;
	}
    //--------------------------------------------------

    //--------------------------------------------------
    public List<Vector3> GetSlicesDimensionsList ()
    //--------------------------------------------------
    {
        return sliceDimensionsList;
	}
    //--------------------------------------------------
}
//--------------------------------------------------
