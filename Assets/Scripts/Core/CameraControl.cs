using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("Configure")]
    [SerializeField] float minZoom = 1f;
    [SerializeField] float maxZoom = 3f;
    [SerializeField] float panSpeed = 3000;

    [Header("Reference")]
    [SerializeField] Transform toPan = null;
    [SerializeField] Transform model = null;
    [SerializeField] Transform toZoom = null;

    float originalZoomValue = 1f;

    void Update()
    {
        if(Time.timeScale < 1f) { return; }
        float scrollValue = Input.mouseScrollDelta.y;

        //Move closer/farther from pivot
        if (scrollValue != 0)
        {
            float newValue = Mathf.Clamp(toZoom.localPosition.z + scrollValue, -maxZoom, -minZoom);
            originalZoomValue = newValue;
            toZoom.localPosition = new Vector3(0f, 0f, newValue);
        }

        PanCamera();
    }

    private void LateUpdate()
    {
        if(RaycastToCameraObstacle(out float correctionDistance))
        {
            toZoom.localPosition = new Vector3(0f, 0f, -correctionDistance);
        }

    }

    //can be called from another class
    public void PanCamera()
    {
        //Pan the camera pivot
        if (Input.GetMouseButton(1))
        {
            float x = -Input.GetAxis("Mouse Y") * panSpeed * Time.deltaTime;
            float y = Input.GetAxis("Mouse X") * panSpeed * Time.deltaTime;

            // Rotate the camera with respect to mouse movement
            toPan.Rotate(new Vector3(x, y, 0f));

            x = toPan.rotation.eulerAngles.x;
            y = toPan.rotation.eulerAngles.y;
            toPan.rotation = Quaternion.Euler(x, y, 0);


            if (model != null)
            {
                Vector3 f = toPan.transform.forward;
                f.y = 0f;
                model.transform.forward = f;
            }
        }

        //if (Input.GetMouseButton(0))
        //{
        //    float x = -Input.GetAxis("Mouse Y") * panSpeed * Time.deltaTime;
        //    float y = Input.GetAxis("Mouse X") * panSpeed * Time.deltaTime;

        //    // Rotate the camera with respect to mouse movement
        //    toPan.Rotate(new Vector3(x, y, 0f));

        //    x = toPan.rotation.eulerAngles.x;
        //    y = toPan.rotation.eulerAngles.y;
        //    toPan.rotation = Quaternion.Euler(x, y, 0);
        //}
    }

    public Transform GetPivot()
    {
        return toPan;
    }

    public Transform SetModel(Transform m)
    {
        return model = m;
    }


    public bool RaycastToCameraObstacle(out float correctionDistance)
    {
        RaycastHit[] hits = Physics.RaycastAll(toPan.transform.position,
            toZoom.transform.position - toPan.transform.position,
            Mathf.Abs(toZoom.transform.position.z));
        hits = hits.SortHitsByDistance();

        if (hits.Length > 0)
        {
            if (hits[0].collider.gameObject != toZoom.gameObject)
            {
                //Debug.Log(hits[0].collider.name + " " + (hits[0].point - toPan.transform.position).magnitude);
                correctionDistance = Mathf.Clamp((hits[0].point - toPan.transform.position).magnitude - .1f, minZoom, maxZoom);
                return true;
            }
        }

        correctionDistance = originalZoomValue;
        return false;
    }
}
