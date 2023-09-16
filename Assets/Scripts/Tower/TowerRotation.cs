using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerRotation : MonoBehaviour
{
    private Transform target;
    private Transform firePosition;
    private Vector3 enemyDirection;
    [SerializeField] private Transform rotateAroundPoint;
    private Vector3 lookRotation;
    [SerializeField] private Transform partToYRotate;
    [SerializeField] private Transform partToXRotate;
    private Vector3 yRotation;
    private Vector3 xRotation;
    private float rotateSpeed;
    private float elapsedTime = 0f;
    private float lerpDuration;
    [HideInInspector] public bool startRotate = false;

    void Update()
    {
        if (!startRotate)
        {
            return;
        }

        SetLookDirectionToTarget();

        if (elapsedTime < lerpDuration)
        {
            elapsedTime += Time.deltaTime;
            SetTempRotation();
        }

        ApplyRotation();
    }
    
    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(rotateAroundPoint.position, enemyDirection*50);
    }
    void SetLookDirectionToTarget()
    {
        if (target != null)
        {
            enemyDirection = (target.GetComponent<Collider>().bounds.center - rotateAroundPoint.position);
            
            lookRotation = Quaternion.LookRotation(enemyDirection).eulerAngles;
        }
    }
    
    void SetTempRotation()
    {
        yRotation = (Vector3.Lerp(partToYRotate.localRotation.eulerAngles, lookRotation, elapsedTime / lerpDuration));
        xRotation = (Vector3.Lerp(lookRotation, partToXRotate.localRotation.eulerAngles, elapsedTime / lerpDuration));
    }

    void ApplyRotation()
    {
        partToYRotate.localRotation = Quaternion.Euler(0f, yRotation.y, 0f);
        partToXRotate.localRotation = Quaternion.Euler(Mathf.Clamp(xRotation.x, -40f, 60f), 0f, 0f);
    }

    public void SetTarget(Transform _target)
    {
        target = _target;
        elapsedTime = 0f;
    }
    public void SetFirePosition(Transform _firePos)
    {
        firePosition = _firePos;
    }
    public void SetRotateSpeed(float _rotateSpeed)
    {
        rotateSpeed = _rotateSpeed;
        lerpDuration = 1f / rotateSpeed;
    }
    public void StartRotating()
    {
        startRotate = true;
    }
}
