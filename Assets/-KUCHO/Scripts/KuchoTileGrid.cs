using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Light2D;


//[ExecuteInEditMode]
public class KuchoTileGrid : MonoBehaviour {

    [System.Serializable]
    public class TileGroup{
        public string name;
        //  public Material mat;
        //  [HideInInspector]public Material alphaShwitchMaterial;
        //  [HideInInspector]public Material inverseAlphaShwitchMaterial;
        public List<KuchoTile> tiles;
        [HideInInspector]public bool playerInside = false;
        [HideInInspector] public bool playerInAlphaSwitch = false;
        [HideInInspector] public float alphaSwitchGoal = 1;
        [HideInInspector] public float currentAlpha = 1;
        [HideInInspector] public bool fading = false;
    }

    public Sprite restoreSprite;
	public bool initialised = false;
	[Range(0,1)]public float ambientLight;
	public bool randomDoorsAndWalls = false;
	public Vector3 grid;
	public Vector2 playerTilePosOffset;
	public Vector2 skyLightTilePosOffset;
	public Vector2 lightPixelSize;
	public enum SpriteAnchor {Centered, BottomLeft}
	public SpriteAnchor spriteAnchor = SpriteAnchor.BottomLeft;
	public GameObject randomTiles;
	public GameObject normalTiles;
	public GameObject ladderTiles;
	public bool coverMatLockedToNormalMat = true;
	public Material[] materials;
	public Material[] sideMaterials;
	public List<KuchoTile> tiles;
	public int[] coverSpriteIDs;
	KuchoTile tileToBeDestroyed;
	public Vector2 tileMapSize;
	KuchoTile[,] tileMap; // advanced inspector da error si es publica, de todas formas unity no serializa las 2D arrays  asi que la reconstruyo en start!!
	public enum GroupProcessing {AutoDefine , ManuallyDefined_Get}
	public GroupProcessing groupProcessing = GroupProcessing.AutoDefine;
	public List<string> groupNames;
	public List<TileGroup> tileGroups;
	[ReadOnly2Attribute] public Vector2 tilePlayerIsIn;
	[ReadOnly2Attribute] public int groupPlayerIsIn = -1; // -1 = ninguno
	[ReadOnly2Attribute] public int groupPlayerWasIn = -2;
	public List<SkyLightMark> skyLightMarks;
	public float switchSpeed = 0.1f;
    public bool checkRectForCoverLightCam = true;
	public Rect rect = new Rect();
    [ReadOnly2Attribute] public bool inRange;

//	[ReadOnly2Attribute] public Collider2D ground; // el suelo de nuestros colliders que esta pisando el player
//	[ReadOnly2Attribute] public List<Collider2D> ladder; // la escalera de nuestros colliders que esta pisando el player
//	[ReadOnly2Attribute] public List<AlphaSwitch> alphaSwitch; // los alphashwitches sobre los que estamos 
	[ReadOnly2Attribute] public List<int> alphaSwitchGroupIndexes; // los alphashwitches sobre los que estamos 

	[HideInInspector] public bool toggleCovers = false;
	int deleteClickCount = 0;

	Vector3 previousPos;
	public static List<KuchoTileGrid> instances  = new List<KuchoTileGrid>();
	public static KuchoTileGrid instance;

	bool Nunca(){
		return false;
	}

	private float oldAmbientLight = -1;
	void OnValidate(){
		if (isActiveAndEnabled)
		{
			initialised = false;
			//if (WorldMap.instance && !WorldMap.instance.grid)
				//WorldMap.instance.grid = this;

				if (oldAmbientLight != ambientLight)
				{
					AdjustAmbientLights();
				}
		}
	}
	public static void Init()
	{
		instances = new List<KuchoTileGrid>();
	}

	void Awake()
	{
		if (!Constants.appIsEditor && enabled)// solo se cuelga en build
		{
			Debug.Log("DESACTIVANDO TILE GRID EN FRAME " + KuchoTime.frameCount + " NIVEL " + ScenesAndDifficultyManager.instance.currentSceneNameUpper + " PARA EVITAR BUG DEL PANTALLA NEGRA Y CRASH");
			KuchoEvents.onSceneWasLoaded3FramesAfter += EnableMe3FramesLater;
			gameObject.SetActive(false); // 
		}
	}

	void EnableMe3FramesLater()
	{
		Debug.Log("ACTIVANDO TILE GRID EN FRAME " + KuchoTime.frameCount + " NIVEL " + ScenesAndDifficultyManager.instance.currentSceneNameUpper + " PARA EVITAR BUG DEL PANTALLA NEGRA Y CRASH");
		KuchoEvents.onSceneWasLoaded3FramesAfter -= EnableMe3FramesLater;
		gameObject.SetActive(true);
	}
	
	#if UNITY_EDITOR
	void Update(){
		if (!Application.isPlaying)
		{
			//  if (previousPos != transform.position)
			{
                InitialiseInEditor();
				previousPos = transform.position;
			}
		}
	}
	#endif

	//[Inspect(1), Toolbar("MyToolbar", Flexible = false), Style("ToolbarButton", Label = false)]
	public void CreateNormalCell(){
        Point pos = GetFreeTilePos();
        if (pos.x < 0)
            print(" GRID LLENA !!!!!)");
        else
            CreateCell(KuchoTile.Type.Normal, pos);
	}
	//[Inspect(1), Toolbar("MyToolbar", Flexible = false), Style("ToolbarButton", Label = false)]
	public void CreateRandomCell(){
        Point pos = GetFreeTilePos();
        if (pos.x < 0)
            print(" GRID LLENA !!!!!)");
        else
            CreateCell(KuchoTile.Type.Random, pos);
    }
	//[Inspect(1), Toolbar("MyToolbar", Flexible = false), Style("ToolbarButton", Label = false)]
	public void CreateLadderCell(){
        Point pos = GetFreeTilePos();
        if (pos.x < 0)
            print(" GRID LLENA !!!!!)");
        else
            CreateCell(KuchoTile.Type.Ladder, pos);
    }
	//[Inspect(2), Toolbar("MyToolbar", Flexible = false), Style("ToolbarButton", Label = false)]
	public void ToggleSwitchers(){
		InitialiseInEditor();
		toggleCovers = !toggleCovers;
		SetSwitchers(toggleCovers);
	}
	
	public void RandomizeCells(){
		GetTilesAndMore(); // no deberia tener que recrear la lista pero como no voy a hacerlo en runtime y por seguridad lo hago siempre y a tomar por culo
		for (int i = 0; i < tiles.Count; i++)
			tiles[i].RandBlock();
        InitialiseInEditor();
		SetSwitchers();
	}
	
	public void RecreateCellsFromPrefab(){
		GetTilesAndMore(); // no deberia tener que recrear la lista pero como no voy a hacerlo en runtime y por seguridad lo hago siempre y a tomar por culo
		for (int i = 0; i < tiles.Count; i++)
			tiles[i].RecreateFromPrefab();
        InitialiseInEditor();
		SetSwitchers();
	}
	//
	void FindIndexes(){
		for (int i = 0; i < tiles.Count; i++)
			tiles[i].FindIndex();
	}
	public void SetSwitchers(){
		SetSwitchers(toggleCovers);
	}
	public void SetSwitchers(bool onOff){
		foreach (KuchoTile tile in tiles)
			tile.SetAlphaOnSwitchableRenderers(System.Convert.ToSingle(onOff));
	}

	//[Inspect(4), Toolbar("MyToolbar", Flexible = false), Style("ToolbarButton", Label = false)]
	public void DeleteAll(){
		deleteClickCount++;
		if (deleteClickCount >= 5)
		{
			deleteClickCount = 0;
			GetTilesAndMore();
			for (int i = 0; i < tiles.Count; i++)
			{
				DestroyImmediate(tiles[0].gameObject);
				tiles.RemoveAt(0);
			}
		}
	}
	public void ChangeType(KuchoTile tile,  KuchoTile.Type type){ 
		tileToBeDestroyed = tile;
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.delayCall += DestroyTile;
		#else
        Destroy(tile);
		#endif
		GetTilesAndMore();
        Point pos = tile.tilePos;
        tile = CreateCell(type, pos);
		#if UNITY_EDITOR
		UnityEditor.Selection.activeObject = tile;
		#endif
	}
	void DestroyTile(){
		if (tileToBeDestroyed)
			DestroyImmediate(tileToBeDestroyed.gameObject	);
	}
	public KuchoTile CreateCell(KuchoTile.Type type, Point pos){
		GameObject go = GetTileTypePrefab(type);
		if (go)
		{
            Vector3 pos3 = transform.position;
            pos3.x += pos.x * grid.x;
            pos3.y += pos.y * grid.y;
			GameObject cellInstance = (GameObject)Instantiate(go, pos3, Constants.zeroQ);
			cellInstance.name = "Cell";
			cellInstance.transform.parent = transform;
			KuchoTile tile = cellInstance.GetComponent<KuchoTile>();
			tiles.Add(tile);
			tile.tileType = type;
			tile.tileGrid = this;
			tile.SetTilePosAndName();
			tile.RandBlock();
			GetTilesAndMore();
//			SetSwitchers(); // no me hace ni caso , no se por que , asi que he incluido esto en Tile.Update()
			return tile;
		}
		return null;
	}
    Point GetFreeTilePos() {
        if (tiles.Count == 0)
            return new Point(0, 0);
        for (int y = 0; y < grid.y; y++)
        {
            for (int x = 0; x < grid.x; x++)
            {
                bool free = true;
                foreach (KuchoTile tile in tiles)
                {
                    if (tile.tilePos.x == x && tile.tilePos.y == y) // posicion ocupada
                    {
                        free = false;
                        break;
                    }
                }
                if (free)
                    return new Point(x, y);
            }
        }
        return new Point(-1, -1);
    }
	public void InitialiseInEditor(){
		#if UNITY_EDITOR

		GetTilesAndMore();
		Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
		if (tiles.Count > 0)
		{
			foreach (KuchoTile t in tiles)
			{
				t.GetStuffAfterCreateBlockOrWall();
				if (t.transform.position.x < min.x)
					min.x = t.transform.position.x;
				if (t.transform.position.y < min.y)
					min.y = t.transform.position.y;
				t.transform.parent = null;
			}
			// mueve el tilegrid padre ahora que le he sacado los hijos
			transform.position = new Vector3(min.x, min.y, transform.position.z);
			// mueve las tiles para que conserven su posicion 
			tileMapSize = new Vector2(float.MinValue, float.MinValue);
			foreach (KuchoTile t in tiles)
			{
				t.transform.parent = transform; // vuelve a poner el hijo
				t.previousTransformPosition *= 2; // solo para forzar a que se actualice todo en Update
				t.Update();// para que se actualice tilePos
				if (t._tilePos.x > tileMapSize.x)
					tileMapSize.x = t._tilePos.x;
				if (t._tilePos.y > tileMapSize.y)
					tileMapSize.y = t._tilePos.y;

				t.SetBlockMaterials();
				t.SetSideMaterials();
			}
		}
		else
		{
			min = (Vector2)transform.position;
		}
		tileMapSize += Vector2.one;
		CalculateAllTilesRect();
		MakeTileMap();
		DefineGroups();
		initialised = true;
		#endif
		if (!Application.isEditor)
			Debug.LogError(" ALGUIEN HA LLAMADO A INIZIALIZAR UN TILEGRID, ESTO SOLO DEBERIA HACERSE EN EDITOR");
	}

	void AdjustAmbientLights()
	{
		var all = GetComponentsInChildren<LightSprite>();
		foreach (LightSprite ls in all)
		{
			if (ls.name == "AmbientLight")
			{
				var c = ls.OriginalColor;
				c.a = ambientLight;
				ls.OriginalColor = c;
				ls.Color = c;
			}
		}
	}

	void GetTilesAndMore(){
		//todas las tiles
		tiles.Clear();
		var _tiles = GetComponentsInChildren<KuchoTile>();
		foreach (KuchoTile tile in _tiles)
			if (tile != tileToBeDestroyed)
				tiles.Add(tile);
		skyLightMarks.Clear();
		foreach (KuchoTile cell in tiles)
		{
			if (cell.skyLightMark != null)
				skyLightMarks.Add(cell.skyLightMark);
		}
		SnapAllLightObstacles();
	}

	void SnapAllLightObstacles()
	{
		SnapMeOnEditor2 snap2 = GetComponentInParent<SnapMeOnEditor2>();
		if (snap2)
		{
			lightPixelSize = snap2.grid;
			foreach (KuchoTile t in tiles)
				t.SnapLightObstacles(lightPixelSize);
		}
	}

	void CalculateAllTilesRect(){
		if (tileMapSize.x > 0 && tileMapSize.y > 0 && tiles.Count > 0)
		{
			Vector2 max = new Vector2(float.MinValue, float.MinValue);
			Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
			foreach (KuchoTile t in tiles)
			{
				if (t.transform.position.x < min.x)
					min.x = t.transform.position.x;
				if (t.transform.position.x > max.x)
					max.x = t.transform.position.x;

				if (t.transform.position.y < min.y)
					min.y = t.transform.position.y;
				if (t.transform.position.y > max.y)
					max.y = t.transform.position.y;
			}
			switch (spriteAnchor)
			{
				case(SpriteAnchor.Centered):
					min.x -= grid.x / 2;
					min.y -= grid.y / 2;
					max.x += grid.x / 2;
					max.y += grid.y / 2;
					break;
				case(SpriteAnchor.BottomLeft):
					max.x += grid.x;
					max.y += grid.y;
					break;
			}
			rect = new Rect();
			rect.position = min;
			rect.size = max - min;
		}
		else
		{
			print("NO PUEDO CALCULAR RECT POR QUE NO HAY TILES?");
		}
	}
	void MakeTileMap(){
		if (tileMapSize.x > 0 && tileMapSize.y > 0 && tiles.Count > 0)
		{
			tileMap = new KuchoTile[(int)tileMapSize.x, (int)tileMapSize.y];
			foreach (KuchoTile t in tiles)
				tileMap[t.tilePos.x, t.tilePos.y] = t;
		}
		else
		{
			print("NO PUEDO CALCULAR TILEMAP POR QUE NO HAY TILES?");
		}
	}
	void ProcessGroups(){
		switch(groupProcessing)
		{
			case(GroupProcessing.AutoDefine):
				DefineGroups();
				break;
			case(GroupProcessing.ManuallyDefined_Get):
				GetGroups();
				break;
		}
	}
	void DefineGroups(){
		int groupCount = -1;
		bool settingGroup = false;
		KuchoTile t = null;
		for (int y = 0; y < tileMapSize.y; y++)
		{
			for (int x = 0; x < tileMapSize.x; x++)
			{
				t = tileMap[x, y];
				if (t)
				{
					if (!settingGroup)
						groupCount++;
					t.tileGroup = groupCount;
					t.tileGroupName = "Auto Group Number " + groupCount.ToString();
					if (t.currentLeftSide) // tiene pared a la izq
					{
						if (t.currentRightSide) // tambien tiene a la dcha!
						{
							settingGroup = false;
						}
						else // tiene pared a la izq pero no tiene pared a la dcha (normal)
						{
							if (!settingGroup) // no habiamos empezado a definir un grupo
							{
								settingGroup = true;
							}
							else // habiamos empezado a definir un grupo, hay que cerrarlo
							{
								settingGroup = false;
							}
						}
					}
					else // no tiene pared o puerta a la izquierda
					{
						if (t.currentRightSide) // pero tiene a la dcha!
						{
							settingGroup = false;
						}
					}
				}
				else // no hay tile
				{
					settingGroup = false;
				}
			}
		}
		GetGroups();
	}
	public void GetGroups(){
		if (tileGroups == null)
			tileGroups = new List<TileGroup>();
		tileGroups.Clear();
		foreach (KuchoTile tile in tiles)
		{
			TileGroup thisTileGroup = tileGroups.Find(x => x.name == tile.tileGroupName);
			if (thisTileGroup == null) // nuevo grupo encontrado
			{
				thisTileGroup = new TileGroup();
				thisTileGroup.name = tile.tileGroupName;
				thisTileGroup.tiles = new List<KuchoTile>();
				tileGroups.Add(thisTileGroup);
			}
			thisTileGroup.tiles.Add(tile);
			tile.tileGroup = tileGroups.IndexOf(thisTileGroup); // asigno el indice del grupo a la celula para que pueda llamar a onplayerenter
		}
	}
	
	GameObject GetTileTypePrefab(KuchoTile.Type type)
	{
		switch (type)
		{
			case (KuchoTile.Type.Normal):
				return normalTiles;
			case (KuchoTile.Type.Random):
				return randomTiles;
			case (KuchoTile.Type.Ladder):
				return ladderTiles;
		}
		return null;
	}

	public KuchoTile GetTileByWorldPosition(Vector2 worldPosition){
		Vector2 posOffseted = worldPosition - playerTilePosOffset;
		if (KuchoHelper.IntersectIncludingLimits(posOffseted, rect))
			return _GetTileByWorldPositionAlreadyOffseted(posOffseted);
		return null; 
	}
    KuchoTile _GetTileByWorldPositionAlreadyOffseted(Vector2 worldPosOffseted)
    {
        Vector2 relPos = worldPosOffseted - rect.min;
        Point tilePos = new Point((int)(relPos.x / grid.x), (int)(relPos.y / grid.y));
        if (tileMap == null)
        {
            Debug.LogError("KUCHO TILEMAP ES NULL? QUIEN ME HA LLAMADO? Y CUANDO ME HA LLAMADO?");
            return null;
        }
        else
        {
            return tileMap[tilePos.x, tilePos.y];
        }
    }
	public int IsClean2(Vector2 worldPosition){
		if (GetTileByWorldPosition(worldPosition - skyLightTilePosOffset) != null)
			return 255;
		return 0;
	}
	public bool IsClean(Vector2 pos){
		for (int i = 0; i < tiles.Count; i++)
		{
			if (tiles[i].Intersect(pos))
				return false;
		}
		return true;
	}
	public KuchoTile FindTileOnPos(Point tilePos){ // busca a lo bruto, sencillo pero poco eficiente, lo hago asi por ganar libertad en como definir las tiles en inspector y simplicidad
		for (int i = 0; i < tiles.Count; i++)
		{
			KuchoTile tile = tiles[i];
			if (tilePos.x == tile.tilePos.x && tilePos.y == tile.tilePos.y)
				return tile;
		}
		return null;
	}
}
