using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerHeadup : MonoBehaviour
{
    [SerializeField] private Transform head;
    private float headUpAngles = 40f;
    private float headDownAngles = -20f;
    private float tiltingSpeed;
    [HideInInspector] public bool startToTilt = false;
    private bool tiltUp = false;
    private float deltaAngle = 0;

    void Update()
    {
        float angleToChange = tiltingSpeed * Time.deltaTime;
        if (startToTilt)
        {

            if (tiltUp)
            {
                if (deltaAngle < headUpAngles)
                {
                    deltaAngle += angleToChange;
                    head.RotateAround(head.position, Vector3.left, angleToChange);
                }
                
            }
            else
            {
                if (deltaAngle > headDownAngles)
                {
                    deltaAngle -= angleToChange;
                    head.RotateAround(head.position, Vector3.right, angleToChange);
                }
            }
        }
        
    }

    public void StartTilting()
    {
        startToTilt = true;
    }

    public void TiltUp()
    {
        tiltUp = true;
    }
    public void TiltDown()
    {
        tiltUp = false;
    }

    public void SetTiltingSpeed(float _speed)
    {
        tiltingSpeed = _speed;
    }
    


}
