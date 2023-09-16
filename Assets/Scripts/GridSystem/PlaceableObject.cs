using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    // not used
    // public bool isPlaced { get; private set; }
    public Vector3Int size{ get; private set; }
    public Vector3[] Vertices;

    #region Unity methods
    private void Awake()
    {
        GetColliderVertexPositionLocal();
        CalculateSizeInCells();
    }
    
    #endregion

    private void GetColliderVertexPositionLocal()
    {
        BoxCollider bCollider = gameObject.GetComponent<BoxCollider>();
        Vertices = new Vector3[4];
        Vertices[0] = bCollider.center + new Vector3 (-bCollider.size.x, -bCollider.size.y, -bCollider.size.z) * 0.5f;
        Vertices[1] = bCollider.center + new Vector3 (bCollider.size.x, -bCollider.size.y, -bCollider.size.z) * 0.5f;
        Vertices[2] = bCollider.center + new Vector3 (bCollider.size.x, -bCollider.size.y, bCollider.size.z) * 0.5f;
        Vertices[3] = bCollider.center + new Vector3 (-bCollider.size.x, -bCollider.size.y, bCollider.size.z) * 0.5f;
    }

    private void CalculateSizeInCells()
    {

        Vector3Int[] vertices = new Vector3Int[Vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldPos = transform.TransformPoint(Vertices[i]);
            vertices[i] = BuildingSystem.Instance.GridLayout.WorldToCell(worldPos);
        }

        size = new Vector3Int (Mathf.Abs((vertices[0] - vertices[1]).x),
                                Mathf.Abs((vertices[0] - vertices[3]).y),
                                1);
    }

    public Vector3 GetStartPosition()
    {
        return transform.TransformPoint(Vertices[0]);
    }



    public virtual void OnPlace()
    {
        ObjectDrag drag = gameObject.GetComponent<ObjectDrag>();
        GameObject obj = drag.gameObject;
        Destroy(drag);

        // invoke events of placement
        TowerController controller = obj.GetComponent<TowerController>();
        controller.OnPlace();
        controller.IsPlaced = true;

    }

}
