using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;

public class BuildModeController : MonoBehaviour
{
    private bool buildMode;

    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera BuildModeCamera;
    [SerializeField] private Transform CameraRoot;
    public float CameraMovementSpeed;
    public float CameraFastMoveSpeed;
    public float CameraSpeedChangeRate;
    public float CameraRootHeight;
    public float MouseRotationSpeed;
    private float currentMoveSpeed;
    public LayerMask TerrainLayer;
    //map limits

    private StarterAssetsInputs Input;
    [Header("UI Manager")]
    [SerializeField] private UIManager UI;


    private void Awake()
    {
        Input = GetComponent<StarterAssetsInputs>();
        buildMode = false;
    }


    void Update()
    {
        // ! things that needs update every frame
        if (buildMode)
        {
            CameraMovement();
            SetCameraHeight();
            CameraRotation();
        }
        else
        {
            //center camera back on to player
            if (!GetComponent<PlayerStats>().PlayerDead && !GameStateManager.Instance.IsPlayingCutscene)
            {
                CameraRoot.position = transform.position + Vector3.up * CameraRootHeight;
                CameraRoot.eulerAngles = Vector3.zero;
            }
        }
    }

    public bool GetBuildMode()
    {
        return buildMode;
    }

    public void SetBuildMode(bool state)
    {
        buildMode = state;
        BuildModeSetup();
    }

    private void BuildModeSetup()
    {
        GameStateManager.Instance.SetCursorState(buildMode, !buildMode, true);
        BuildModeCamera.gameObject.SetActive(buildMode);

        if (buildMode)
        {
            GameStateManager.Instance.SetCursorState(true, true, true);
            UI.SetWeaponPanel(false);
            // cancel by 詹詹, move this to BuildingSystem
            // UI.SetBuildModePanel(true);
            // added by 詹詹
            BuildingSystem.Instance.EnterBuildMode();
        }
        else
        {
            currentMoveSpeed = 0;
            GameStateManager.Instance.SetCursorState(false, true, true);
            UI.SetWeaponPanel(true);
            // cancel by 詹詹, move this to BuildingSystem
            // UI.SetBuildModePanel(false);
            // added by 詹詹
            BuildingSystem.Instance.ExitBuildMode();
        }
    }

    private void CameraMovement()
    {
        float targetSpeed = CameraMovementSpeed;
        if (Input.move == Vector2.zero)
        {
            targetSpeed = 0;
        }
        else if (Input.sprint)
        {
            targetSpeed = CameraFastMoveSpeed;
        }
        currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, targetSpeed, CameraSpeedChangeRate * Time.deltaTime);
        Vector3 direction = new Vector3(Input.move.x, 0f, Input.move.y);
        CameraRoot.Translate(direction * currentMoveSpeed * Time.deltaTime);

    }

    private void SetCameraHeight()
    {
        Ray ray = new Ray(CameraRoot.position + Vector3.up * 100f, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 200f, TerrainLayer))
        {   
            float y = (hit.point.y + CameraRootHeight) - CameraRoot.position.y;
            CameraRoot.position += new Vector3(0f, y, 0f);
        }
    }

    private void CameraRotation()
    {
        //TODO: Keyboard rotation

        //mouse rotation
        if (Input.aim)
        {
            GameStateManager.Instance.SetCursorState(false, true, true);
            CameraRoot.Rotate(Vector3.up, Mathf.Clamp(Input.look.x, -1, 1) * Time.deltaTime * MouseRotationSpeed, Space.World);
            //CameraRoot.Rotate(Vector3.right, Input.look.y * Time.deltaTime * MouseRotationSpeed, Space.World);
        }
        else
        {
            GameStateManager.Instance.SetCursorState(true, true, true);
        }
    }

}
