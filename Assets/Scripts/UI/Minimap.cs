using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public enum MinimapMode
    {
        Map,
        Radar
    }
    public MinimapMode minimapMode;
    public float RadarRange;
    public float MapRange;
    [SerializeField] private Transform player;
    void Update()
    {
        if(minimapMode == MinimapMode.Radar)
        {
            transform.position = player.position + Vector3.up * 30;
            transform.rotation = Quaternion.Euler(90, player.rotation.eulerAngles.y, 0);
            GetComponent<Camera>().orthographicSize = RadarRange;
        }
        else
        {
            transform.position = Vector3.zero + Vector3.up * 30;
            transform.rotation = Quaternion.Euler(90, 0, 0);
            GetComponent<Camera>().orthographicSize = MapRange;
        }

    }
}
