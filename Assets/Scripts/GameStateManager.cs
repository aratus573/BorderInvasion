using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using StarterAssets;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    private enum GameState
    {
        action, planning, complete
    }
    [SerializeField] private GameState gameState = GameState.planning;
    [SerializeField] private float waveEndDelayTime = 3;
    private bool isPaused = false;
    private UIManager UI;
    [SerializeField] private PlayerStats player;
    private BuildModeController buildModeController;
    private InputActionMap PlayerInput;
    private StarterAssetsInputs starterAssetsInputs;

    [Header("Cutscenes")]
    public bool IsPlayingCutscene = false;
    [SerializeField] private PlayableDirector cutscenePlayer;
    [SerializeField] private PlayableAsset reviveCutscene;
    [SerializeField] private PlayableAsset gameOverCutscene;
    [SerializeField] private PlayableAsset gameWinCutscene;

    private void Awake()
    {
        Instance = this;
        PlayerInput = player.GetComponent<PlayerInput>().currentActionMap;
        buildModeController = player.GetComponent<BuildModeController>();
        starterAssetsInputs = player.GetComponent<StarterAssetsInputs>();
        SetCursorState(false);
        Time.timeScale = 1;
    }
    private void Start()
    {
        UI = UIManager.instance;
        StartCoroutine(WaveComplete(true));
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (UI.isUpgradePanelOpen)
            {
                UI.isUpgradePanelOpen = false;
                UI.SetUpgradePanel(false);
            }
            GamePause();
        }
        if (gameState == GameState.planning && Input.GetKeyDown(KeyCode.U))
        {
            UI.isUpgradePanelOpen = !UI.isUpgradePanelOpen;
            UI.SetUpgradePanel(UI.isUpgradePanelOpen);
        }
        //cheats
        if (Input.GetKeyDown(KeyCode.I))
        {
            GameOver();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            StartCoroutine(GameWin(true));
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameRestart();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            player.GodMode = !player.GodMode;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            UpgradeManager.Instance.AddResource(100);
        }
    }

    public void NextWave()
    {
        buildModeController.SetBuildMode(false);
        UI.isUpgradePanelOpen = false;
        UI.SetUpgradePanel(false);
        EnemyManager.Instance.WaveSetup();
        gameState = GameState.action;
    }

    public IEnumerator WaveComplete(bool instant = false)
    {
        if (!instant)
        {
            yield return new WaitForSeconds(waveEndDelayTime);
        }
        buildModeController.SetBuildMode(true);
        gameState = GameState.planning;
    }

    public void GameOver()
    {
        GameStop();
        PlayGameOverCutscene();
        MainAudioController.Instance.PlayLoseClip();
    }
    public IEnumerator GameWin(bool instant = false)
    {
        if (!instant)
        {
            yield return new WaitForSeconds(waveEndDelayTime);
        }
        GameStop();
        PlayGameWinCutscene();
        MainAudioController.Instance.PlayWinClip();
    }
    public void GamePause()
    {
        if (gameState == GameState.complete) { return; }
        isPaused = !isPaused;
        UI.SetPausePanel(isPaused);
        SetCursorState(isPaused);
        if (isPaused)
        {
            PlayerInput.Disable();
            Time.timeScale = 0;
        }
        else
        {
            PlayerInput.Enable();
            Time.timeScale = 1;
        }
    }
    public void GamePause(bool pause)
    {
        if (gameState == GameState.complete) { return; }
        isPaused = pause;
        UI.SetPausePanel(isPaused);
        SetCursorState(isPaused);
        if (isPaused)
        {
            PlayerInput.Disable();
            Time.timeScale = 0;
        }
        else
        {
            PlayerInput.Enable();
            Time.timeScale = 1;
        }
    }

    public void GameRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void GameStop()
    {
        if (gameState == GameState.complete)
        {
            return;
        }
        gameState = GameState.complete;
        PlayerInput.Disable();
        player.PlayerStop();
        EnemyManager.Instance.StopAllEnemy();
        SetCursorState(false);
    }

    //TODO: turn off unnecessary UI in cutscene
    private void CutsceneStart()
    {
        IsPlayingCutscene = true;
        PlayerInput.Disable();
        cutscenePlayer.Play();
    }
    public void CutsceneEnd()
    {
        IsPlayingCutscene = false;
        PlayerInput.Enable();
    }
    public void PlayReviveCutscene()
    {
        IsPlayingCutscene = true;
        cutscenePlayer.playableAsset = reviveCutscene;
        CutsceneStart();
    }
    public void PlayGameWinCutscene()
    {
        IsPlayingCutscene = true;
        cutscenePlayer.playableAsset = gameWinCutscene;
        CutsceneStart();
    }
    public void PlayGameOverCutscene()
    {
        EnemyManager.Instance.EnemyCheer();
        IsPlayingCutscene = true;
        cutscenePlayer.playableAsset = gameOverCutscene;
        CutsceneStart();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(false);
    }


    public void SetCursorState(bool enableCursor)
    {
        //force cursor out when paused or game end
        if (isPaused || gameState == GameState.complete)
        {
            Cursor.lockState = CursorLockMode.None;
            return;
        }

        //toggle cursor otherwise
        //new State == True -> cursor exist
        //new State == False -> cursor locked
        Cursor.lockState = enableCursor ? CursorLockMode.None : CursorLockMode.Locked;

        //disable mouse movement input when cursor is on
        //enable mouse input when cursor off
        starterAssetsInputs.cursorInputForLook = !enableCursor;
        starterAssetsInputs.mouseButtonInput = !enableCursor;
    }

    //or directly decide mouse input by variable
    public void SetCursorState(bool enableCursor, bool allowMouseMovementForLook, bool allowMouseButtonInput)
    {
        if (isPaused || gameState == GameState.complete)
        {
            Cursor.lockState = CursorLockMode.None;
            return;
        }
        Cursor.lockState = enableCursor ? CursorLockMode.None : CursorLockMode.Locked;
        starterAssetsInputs.cursorInputForLook = allowMouseMovementForLook;
        starterAssetsInputs.mouseButtonInput = allowMouseButtonInput;
    }

}
