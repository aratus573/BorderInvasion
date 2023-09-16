using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceabilityIndicatorTransform : MonoBehaviour
{
    // public GameObject parentGO;
    public Vector3 oriLocalPos;
    public Vector3 oriWorldPos;
    public Vector3 parentWorldPos;
    public Vector3 oriParentWorldPos;

    void Awake() 
    {
        oriLocalPos = transform.localPosition;
        oriWorldPos = transform.position;
        oriParentWorldPos = transform.parent.position;
    }

    void Start()
    {

    }


    void Update()
    {
        parentWorldPos = transform.parent.position;
        SnapToGround();
        
    }

    private void SnapToGround()
    {
        // float newYPosition = (oriLocalPos.y - (parentWorldPos.y - oriParentWorldPos.y));
        // float yValue = Mathf.Clamp(newYPosition, oriLocalPos.y, newYPosition);
        transform.position = new Vector3(transform.position.x, oriWorldPos.y, transform.position.z);
        // transform.localPosition = new Vector3(transform.localPosition.x, oriWorldPos.y,transform.localPosition.z);
    }
}
