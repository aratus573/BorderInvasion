using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDrag : MonoBehaviour
{
    private void Update()
    {

        Vector3 tempPos = BuildingSystem.GetMouseWorldPosition();
        Vector3 pos = new Vector3(tempPos.x, 0f, tempPos.z);
        transform.position = Vector3.up*tempPos.y + BuildingSystem.Instance.SnapCoordinateToGrid(pos);

    }

}
