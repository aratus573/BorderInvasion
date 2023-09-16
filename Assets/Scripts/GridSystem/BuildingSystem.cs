using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;
using Text = TMPro.TextMeshProUGUI;


public class BuildingSystem : MonoBehaviour
{
    # region Show in inspector 
        [Header("Components setup")]
        public GridLayout GridLayout;
        [SerializeField] private UpgradeManager upgradeManager;
        [SerializeField] private Text checkPanelText;
        [SerializeField] private Tilemap mainTilemap;
        [SerializeField] private TileBase whiteTile;
        [SerializeField] private TowerManager towerManager;
        [SerializeField] private UIManager UIManager;

        [Space]
        [Header("Materials")]
        [SerializeField] private Material PlaceableGreen_Range;
        [SerializeField] private Material UnplaceableRed_Range;
        [SerializeField] private Material NormalRangeIndicator;
    # endregion
    
    # region Public fields
    [HideInInspector] public static BuildingSystem Instance;
    [HideInInspector] public GameObject SelectedSellingTower;
    # endregion

    # region Private fields
        private bool isInBuildMode = false;
        private bool isInSellSelectMode = false;
        private bool isEfxPlaying = false;
        private bool isTimerRunning = false;

        private float timer = 0;
        private const float selectedRange = 20f;

        private Grid grid;
        private Collider[] TowerNearByMouse;
        private PlaceableObject objToPlace;
        private TowerController sellingTowerController;
        private TowerController placingTowerController;
        private BuildModeController buildModeController;
        private MeshRenderer rangeIndicatorRenderer;

        private GameObject navMeshObj;
        private GameObject rangeIndicatorGO;
        private List<GameObject> towersPrefabList;
    # endregion

    # region Unity methods
    
    private void Awake() 
    {
        Instance = this;
        grid = GridLayout.gameObject.GetComponent<Grid>();    
        buildModeController = GameObject.FindGameObjectWithTag("Player").GetComponent<BuildModeController>();
        ChangeTilesToTransparent(mainTilemap);
    }
    private void Start()
    {
        towersPrefabList = new List<GameObject>(towerManager.AvailableTypesOfTowerPrefab);
    }

    private void Update() 
    {
        SwitchTimerByState();

        if (isInSellSelectMode)
        {
            // select a tower
            if (Input.GetMouseButtonDown(0) 
                && timer > 0.2f 
                && UpdateSelectedTower())
            {
                SelectTowerForSelling();
            }
        }

        if (SelectedSellingTower && isInBuildMode)
        {
            IndicateSelectedTower();
        }

        if (!isInBuildMode || !objToPlace)
        {
            return;
        }

        if (!placingTowerController.IsPlaced)
        {
            IndicatePlacability(canThisBePlaced(objToPlace));
        }

        
        // left-click to place a tower
        if (Input.GetMouseButtonDown(0) && timer > 0.2f)
        {
            TryPlacingTower();

        }
        else if (Input.GetMouseButtonDown(1))
        {
            CancelPlacingTower();
        }
    }
    #endregion

    #region Utils
    public static Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            return raycastHit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public Vector3 SnapCoordinateToGrid(Vector3 position)
    {
        Vector3Int cellPos = GridLayout.WorldToCell(position);
        position = grid.GetCellCenterWorld(cellPos);
        return position;
    }

    private static TileBase[] GetTileBlocks(BoundsInt area, Tilemap tilemap)
    {
        TileBase[] array = new TileBase[area.size.x * area.size.y * area.size.z];
        int counter = 0;

        foreach (var v in area.allPositionsWithin)
        {
            Vector3Int pos = new Vector3Int (v.x, v.y, 0);
            array[counter] = tilemap.GetTile(pos);
            counter ++;
        }

        return array;
    }


    public void SetSellMode(bool target)
    {
        isInSellSelectMode = target;
    }

    public GameObject UpdateSelectedTower()
    {
        Vector3 mousePos = GetMouseWorldPosition();
        GameObject nearestTower = null;

        float minDistance = Mathf.Infinity;

        TowerNearByMouse = Physics.OverlapSphere(mousePos, selectedRange, 1 << towerManager.LayerIgnoreRaycast);
        foreach (Collider tower in TowerNearByMouse)
        {
            float sqrDistanceToMouse = (tower.transform.position - mousePos).sqrMagnitude;
            if (!tower || tower.gameObject.tag != "Tower")
            {
                continue;
            }
            if (sqrDistanceToMouse < minDistance)
            {
                minDistance = sqrDistanceToMouse;
                nearestTower = tower.gameObject;
            }
        }
        return nearestTower;
    }
    # endregion

    # region Building placement
    
    private void InitializeWithObject(GameObject prefab)
    {
        if (objToPlace)
        {
            DeleteTowerAndRefund();
        }

        Vector3 pos = SnapCoordinateToGrid(GetMouseWorldPosition());

        GameObject obj = Instantiate(prefab, pos, Quaternion.identity);

        placingTowerController = obj.GetComponent<TowerController>();
        towerManager.controllers.Add(placingTowerController);

        rangeIndicatorGO = placingTowerController.RangeIndicatorRenderer.gameObject;
        rangeIndicatorRenderer = rangeIndicatorGO.GetComponent<MeshRenderer>();

        navMeshObj = placingTowerController.TowerBase;
        
        obj.GetComponent<BoxCollider>().enabled = false;
        objToPlace = obj.GetComponent<PlaceableObject>();
        obj.AddComponent<ObjectDrag>();


        float detectRange = placingTowerController.DetectRange;
        
        if (placingTowerController.TemplateStats.name != "Wall")
        {
            rangeIndicatorGO.transform.localScale = Vector3.one * detectRange * 2;
        }
    }

    private void DeleteTowerAndRefund()
    {
        upgradeManager.AddResource(placingTowerController.CurrentStats.resourceCost);
        Destroy(objToPlace.gameObject);
    }

    private bool canThisBePlaced(PlaceableObject placeableObject)
    {
        BoundsInt area = new BoundsInt();
        area.position = GridLayout.WorldToCell(objToPlace.GetStartPosition());
        area.size = placeableObject.size;
        area.size = new Vector3Int(area.size.x + 1, area.size.y + 1, area.size.z);

        TileBase[] baseArray = GetTileBlocks(area, mainTilemap);

        foreach (var b in baseArray )
        {
            if (b == whiteTile)
            {
                return false;
            }
        }

        return true;
    }

    private void TakeArea(TileBase tileBase, Vector3Int start, Vector3Int size)
    {
        mainTilemap.BoxFill(start, whiteTile, start.x, start.y, 
                            start.x + size.x, start.y + size.y);
    }
    
    private void CleanArea(Vector3Int start, Vector3Int size)
    {
        for (int i = start.x; i <= start.x+size.x; i++)
        {
            for (int j = start.y; j <= start.y+size.y; j++)
            {
                mainTilemap.SetTile(new Vector3Int(i,j,start.z), null);
            }
        }
    }

    private void IndicatePlacability(bool _canBePlaced)
    {
        if (_canBePlaced)
        {
            rangeIndicatorRenderer.material = PlaceableGreen_Range;
        }
        else
        {
            rangeIndicatorRenderer.material = UnplaceableRed_Range;
        }
    }

    private void PlaceCurrentTower()
    {
        objToPlace.OnPlace();
        towerManager.AddTowerCountByItsName(placingTowerController.CurrentStats.name);
        TowerCostCalculator.Instance.SetCostByNumOfTower(placingTowerController.CurrentStats.name);

        // 讓path finding偵測為障礙物
        EnableNavMeshObstacle();
        EnableBoxCollider();

        TakeArea(whiteTile, CalculateAreaStartPoint(), objToPlace.size);
        
        rangeIndicatorRenderer.material = NormalRangeIndicator;

        objToPlace = null;

    }

    private void EnableNavMeshObstacle()
    {
        navMeshObj.GetComponent<NavMeshObstacle>().enabled=true;
        navMeshObj.GetComponent<NavMeshObstacle>().carving=true;
    }

    private void EnableBoxCollider()
    {
        navMeshObj.GetComponent<BoxCollider>().enabled = true;
        objToPlace.GetComponent<BoxCollider>().enabled = true;

    }
    
    private Vector3Int CalculateAreaStartPoint()
    {
        Vector3Int start = GridLayout.WorldToCell(objToPlace.GetStartPosition());
        start = new Vector3Int(start.x, start.y, 0);
        return start;

    }
    
    private void CancelPlacingTower()
    {
        towerManager.controllers.Remove(placingTowerController);
        DeleteTowerAndRefund();
        UIManager.SetBuildModePanel(true);
        ChangeTimerState(false);
    }

    private void TryPlacingTower()
    {
        ChangeTimerState(false);
        if (canThisBePlaced(objToPlace))
        {
            PlaceCurrentTower();
        }
        else
        {
            DeleteTowerAndRefund();
        }
        UIManager.SetBuildModePanel(true);
    }
    # endregion

    # region Build mode
    
    public void EnterBuildMode()
    {
        if (isInBuildMode)
        {
            return;
        }
        UIManager.SetBuildModePanel(true);
        isInSellSelectMode = false;
        isInBuildMode = true;
        ChangeTilesBackToWhite(mainTilemap);
        towerManager.ShowAllIndicatorsIfTrue(true);
        towerManager.EnableMainPartColliders();
    }
    
    public void ExitBuildMode()
    {
        if (!isInBuildMode)
        {
            return;
        }
        ChangeTimerState(false);
        UIManager.SetBuildModePanel(false);
        UIManager.SetSellPanelAndButton(false);
        isInSellSelectMode = false;
        isInBuildMode = false;
        if (SelectedSellingTower)
        {
            CancelSellingTower();
        }

        if (objToPlace)
        {
            towerManager.controllers.Remove(placingTowerController);
            DeleteTowerAndRefund();
            objToPlace = null;
        }
        ChangeTilesToTransparent(mainTilemap);
        towerManager.ShowAllIndicatorsIfTrue(false);
        towerManager.DisableMainPartColliders();
    }

    // manual set index in inspector button's component
    // called by buttons in play-mode
    public void SelectTowerByType(int index)
    {
        GameObject selectedTower = towersPrefabList[index];
        TowerController controller = selectedTower.GetComponent<TowerController>();
        controller.SetTowerAttributes(TowerAttributeManager.Instance.currTowerAttributeList[index]);

        if (controller.CurrentStats.resourceCost > upgradeManager.playerResource)
        {
            Debug.Log("you don't have enough resource");
            ChangeTimerState(false);
            UIManager.SetBuildModePanel(true);
            return;
        }
        else
        {
            ChangeTimerState(true);
            upgradeManager.SubtractResource(controller.CurrentStats.resourceCost);
            InitializeWithObject(selectedTower);
            UIManager.SetBuildModePanel(false);
        }
    }
    
    private void ChangeTilesToTransparent(Tilemap tilemap)
    {
        // this will change the WHOLE tilemap's color
        tilemap.color = Color.clear;
    }
    
    private void ChangeTilesBackToWhite(Tilemap tilemap)
    {
        // this will change the WHOLE tilemap's color
        tilemap.color = Color.white;
    }

    private void IndicateSelectedTower()
    {
        if (!isEfxPlaying)
        {
            GameObject efxObj = SelectedSellingTower.transform.Find(towerManager.selectedEFXName).gameObject;
            ParticleSystem efx = efxObj.GetComponent<ParticleSystem>();
            efxObj.SetActive(true);
            
            if (!efx.isPlaying)
            {
                efx.Play();
            }
            isEfxPlaying = true;
        }
    }

    public void CancelSellingTower()
    {
        ParticleSystem efx = SelectedSellingTower.transform.GetComponentInChildren<ParticleSystem>();
        efx.Stop();
        SelectedSellingTower = null;
        towerManager.selectedTower = null;
        isEfxPlaying = false;
    }

    public void ChangeTimerState(bool _state)
    {
        isTimerRunning = _state;
    }

    private void SwitchTimerByState()
    {
        if (isTimerRunning)
        {
            timer += Time.deltaTime;
        }
        else
        {
            if (timer != 0f)
            {
                timer = 0f;
            }
        }
    }
    # endregion

    # region Sell mode

    // called by buttons in play-mode
    public void SellTheSelectedTower()
    {

        PlaceableObject _objToPlace = SelectedSellingTower.GetComponent<PlaceableObject>();
        towerManager.controllers.Remove(sellingTowerController);
        towerManager.SubtractTowerCount(sellingTowerController.CurrentStats.name);
        TowerCostCalculator.Instance.SetCostByNumOfTower(sellingTowerController.CurrentStats.name);

        upgradeManager.AddResource(sellingTowerController.ResourceRefund);
        sellingTowerController.OnSold();

        // calculate area to be cleaned
        BoundsInt area = new BoundsInt();
        Vector3Int tempPosition = GridLayout.WorldToCell(_objToPlace.GetStartPosition());
        area.position = new Vector3Int(tempPosition.x, tempPosition.y, 0);
        area.size = _objToPlace.size;
        area.size = new Vector3Int(area.size.x + 1, area.size.y + 1, area.size.z);

        CleanArea(area.position, area.size);
        Destroy(SelectedSellingTower);
        SelectedSellingTower = null;
        towerManager.selectedTower = null;
        ChangeTimerState(false);
    }

    private void SelectTowerForSelling()
    {
        ChangeTimerState(false);
        // towerManager.ClearUpTowersList();
        if (towerManager.selectedTower)
        {
            // 如果已經有選擇要賣的塔就取消舊有的選擇，selectedTower會變成null
            CancelSellingTower();
        }
        towerManager.selectedTower = UpdateSelectedTower();
        SelectedSellingTower = towerManager.selectedTower;
        sellingTowerController = SelectedSellingTower.GetComponent<TowerController>();
        isInSellSelectMode = false;
        UIManager.SetCheckPanel(true);
        ShowRefundInSellCheckPanel();
    }

    private void ShowRefundInSellCheckPanel()
    {
        int refund = sellingTowerController.CurrentStats.resourceRefund;
        checkPanelText.text = $"Recycle this tower? \nYou will get {refund} resources back.";
    }
    

    # endregion

}
