using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathLenth : MonoBehaviour
{

    public float speed = 5f;
    public float pathLenth = 0f;
    void Start()
    {

    }

    void FixedUpdate()
    {
        float deltaTime = Time.deltaTime;
        
        // hard-coding, should access "speed" from EnemyMovement
        pathLenth += speed * deltaTime;

    }
}
