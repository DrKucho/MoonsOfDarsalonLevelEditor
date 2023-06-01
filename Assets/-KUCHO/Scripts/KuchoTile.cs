using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Light2D;
using UnityEngine.Serialization;


[ExecuteInEditMode]
public class KuchoTile : MonoBehaviour {

	public KuchoTileGrid tileGrid;
	public enum Type {Normal, Random, Ladder};
	public Type tileType = Type.Random;
	public Vector2 _tilePos = new Vector2(float.MinValue, float.MinValue);
	public Point tilePos;
	public string tileGroupName;
	[ReadOnly2Attribute] public int tileGroup;
	public GameObject currentBlock;
	public GameObject currentLeftSide;
	public GameObject currentRightSide;
	public enum SideType {None, Wall, Door};
	public SideType currentLeftType = SideType.None;
	public SideType currentRightType = SideType.None;
	public SkyLightMark skyLightMark;
//	public List<ColliderSettings> floors;
//	public List<ColliderSettings> ladders;
	public Vector2 skylightCleanRectOffset;
	public Vector2 skyLightCleanExtraMargin = new Vector2(3,3);
	public int blockIndex;
	public GameObject[] blocks;
	public Vector3 leftSideWallOffset;
	public Vector3 rightSideWallOffset;
	public GameObject[] leftWalls;
	public GameObject[] rightWalls;
	public GameObject[] leftDoors;
	public GameObject[] rightDoors;
	[HideInInspector] public AlphaSwitch[] alphaSwitchers;
	[HideInInspector] public KuchoLightObstacleSprite[] lightObstacles;
	[HideInInspector] public LightSprite[] lightSources;
	[HideInInspector] public SpritePlane[] spritePlanes;
	[HideInInspector] public TileComponent[] tileComponents;
	[HideInInspector] public TileComponent[] currentBlockTileComponents;
	[HideInInspector] public TileComponent[] currentLeftSideTileComponents;
	[HideInInspector] public TileComponent[] currentRightSideTileComponents;

	[HideInInspector] public Vector3 previousTransformPosition;

	Rect skyLightRect;

	[HideInInspector] [SerializeField] Renderer rend; // para poder seleccionarlo en scene window

	public int backMaterial;
	public int coverMaterial;
	public int coverSpriteID;
	public int sideMaterial;

	void Start(){
		Debug.Log(this + " DENTRO DE START EN KUCHO TILE");
		if (!rend)
			rend = GetComponent<Renderer>();
		if(Application.isPlaying)
			rend.enabled = false;
		GetGrid();
        if (tileGrid)
        {
            skyLightRect = new Rect();
            skyLightRect.min = (Vector2)transform.position + skylightCleanRectOffset;
            skyLightRect.max = skyLightRect.min + (Vector2)tileGrid.grid + skyLightCleanExtraMargin;
            spritePlanes = new SpritePlane[0];
            previousTransformPosition = transform.position;
            SetAlphaOnSwitchableRenderers(1);
        }

	}
	#if UNITY_EDITOR
	public void Update () {
		if (isActiveAndEnabled)
		{
			if (previousTransformPosition != transform.position)
			{
				SetTilePosAndName();
				previousTransformPosition = transform.position;
			}
			foreach (SpritePlane plane in spritePlanes)
				plane.MyUpdate(transform.localPosition.z);
		}
        if (transform.hasChanged)
        {
            SetTilePosAndName();
            transform.hasChanged = false;
        }
        if (!Application.isPlaying) 
			SetAlphaAsGridToggle();// solo para cuando creo celulas nuevas, por que si intento hacer esto inmediatamente despues de crearlas no funciona no se por que
	}
	#endif
	public void SetTilePosAndName(){
        if (tileGrid)
        {
            var pos = transform.localPosition;
            _tilePos.x = Mathf.RoundToInt(pos.x / tileGrid.grid.x);
            _tilePos.y = Mathf.RoundToInt(pos.y / tileGrid.grid.y);
            tilePos.x = (int)_tilePos.x;
            tilePos.y = (int)_tilePos.y;
            pos.x = _tilePos.x * tileGrid.grid.x;
            pos.y = _tilePos.y * tileGrid.grid.y;
            pos.z = pos.x / tileGrid.grid.x * tileGrid.grid.z;
            transform.localPosition = pos;
            string description = "";
            System.Char space = ' ';
            foreach (Transform trans in transform)
            {
                string[] words = trans.name.Split(space);
                description += " " + words[words.Length - 1]; 
            }
            gameObject.name = "(" + tilePos.x.ToString() + "," + tilePos.y.ToString() + ")" + description;
        }
	}
	void GetGrid(){
		if (!tileGrid) // necesario por que al crear el gameobject no somos hijos de grid, se hace inmediatamente despues
			tileGrid = GetComponentInParent<KuchoTileGrid>();
	}
	//[Inspect(1), Toolbar("MyToolbar", Flexible = false), Style("ToolbarButton", Label = false)]
	public void CycleAdd(){
		CycleBlock(1);
	}
	//[Inspect(1), Toolbar("MyToolbar", Flexible = false), Style("ToolbarButton", Label = false)]
	public void CycleSub(){
		CycleBlock(-1);
	}
	void CycleBlock(int inc){
		KuchoHelper.IncAndWrapInsideArrayLength(ref blockIndex, inc, blocks.Length);
		CreateBlock();
	}

	
	public void RandBlock(){
		blockIndex = Random.Range(0, blocks.Length);
		CreateBlock();
	}
	public void RecreateFromPrefab(){
		CreateBlock(blocks[blockIndex], true);
	}
	void CreateBlock(){
		if (blockIndex >= 0 && blockIndex < blocks.Length && blocks[blockIndex] != null)
			CreateBlock(blocks[blockIndex], false);
	}
	void CreateBlock(GameObject _block, bool recreateFromPrefab){
		if (currentBlock)
			DestroyImmediate(currentBlock);

		if (tileGrid.randomDoorsAndWalls || recreateFromPrefab)
		{
			if (currentLeftSide)
			{
				DestroyImmediate(currentLeftSide);
				if (!tileGrid.randomDoorsAndWalls)
				{
					switch (currentLeftType)
					{
						case (SideType.Wall):
							CreateSideWall(true);
							break;
						case (SideType.Door):
							CreateSideDoor(true);
							break;
					}
				}
			}
			if (currentRightSide)
			{
				DestroyImmediate(currentRightSide);
				if (!tileGrid.randomDoorsAndWalls)
				{
					switch (currentRightType)
					{
						case (SideType.Wall):
							CreateSideWall(false);
							break;
						case (SideType.Door):
							CreateSideDoor(false);
							break;
					}
				}
			}
		}

		if (_block)
			currentBlock = (GameObject)Instantiate(_block, transform.position, Constants.zeroQ);
		currentBlock.transform.parent = transform;
		currentBlock.name = currentBlock.name.Substring(0, currentBlock.name.Length - 7); // borro (Clone)
		GetGrid();
		if(tileGrid.randomDoorsAndWalls && !recreateFromPrefab)
		{
			if (!tileGrid.FindTileOnPos(new Point(tilePos.x - 1, tilePos.y))) // si no hay otra tile a su derecha
				CreateSideWallOrDoorRandomly(true); // crea pared o puerta a su derecha
			if (!tileGrid.FindTileOnPos(new Point(tilePos.x + 1, tilePos.y))) // si no hay otra tile a su izquierda
				CreateSideWallOrDoorRandomly(false); // crea pared o puerta a su izquierda
		}
		GetStuffAfterCreateBlockOrWall();
		tileGrid.SetSwitchers();
	}
	public void FindIndex(){
		for (int i = 0; i < blocks.Length; i++)
			if (currentBlock.name == blocks[i].name)
			{
				blockIndex = i;
				break;
			}
	}
	//[Inspect(1), Toolbar("MyToolbar", Flexible = false), Style("ToolbarButton", Label = false)]
	void NormalColor(){
		KuchoHelper.IncAndWrapInsideArrayLength(ref backMaterial, 1, tileGrid.materials.Length);
		SetBlockMaterials();
	}
	//[Inspect(1), Toolbar("MyToolbar", Flexible = false), Style("ToolbarButton", Label = false)]
	void CoverSprite(){
		KuchoHelper.IncAndWrapInsideArrayLength(ref coverSpriteID, 1, tileGrid.coverSpriteIDs.Length);
		SetCoverSprite();
	}
	//[Inspect(1), Toolbar("MyToolbar", Flexible = false), Style("ToolbarButton", Label = false)]
	void CoverColor(){
		KuchoHelper.IncAndWrapInsideArrayLength(ref coverMaterial, 1, tileGrid.materials.Length);
		SetBlockMaterials();
	}
	//[Inspect(1), Toolbar("MyToolbar", Flexible = false), Style("ToolbarButton", Label = false)]
	void SideColor(){
		KuchoHelper.IncAndWrapInsideArrayLength(ref sideMaterial, 1, tileGrid.sideMaterials.Length);
		SetSideMaterials();
	}

	//[Inspect(1), Toolbar("MyToolbar", Flexible = false), Style("ToolbarButton", Label = false)]
	void LeftSide(){
		LeftOrRightSideButton(true);
	}
	//[Inspect(1), Toolbar("MyToolbar", Flexible = false), Style("ToolbarButton", Label = false)]
	void RightSide(){
		LeftOrRightSideButton(false);
	}
	void LeftOrRightSideButton(bool left){
		var current = currentRightType;
		if (left)
			current = currentLeftType;

		switch (current)
		{
			case (SideType.None):
				CreateSideDoor(left);
				break;
			case (SideType.Door):
				CreateSideWall(left);
				break;
			case (SideType.Wall):
				if (left)
				{
					DestroyImmediate(currentLeftSide);
					currentLeftType = SideType.None;
				}
				else
				{
					DestroyImmediate(currentRightSide);
					currentRightType = SideType.None;
				}
				GetStuffAfterCreateBlockOrWall();
				break;
		}
	}
	void SwapToLadder(){
		GetGrid();
		tileGrid.ChangeType(this, Type.Ladder);
	}
	bool DisplaySwapToLadder(){
		if (tileType != Type.Ladder)
			return true;
			return false;
	}
	void SwapToRandom(){
		GetGrid();
		tileGrid.ChangeType(this, Type.Random);
	}
	bool DisplaySwapToRandom(){
		if (tileType != Type.Random)
			return true;
		return false;
	}
	void CreateSideWall(bool left){
		GameObject[] walls = rightWalls;
		if (left)
		{
			walls = leftWalls;
			currentLeftType = SideType.Wall;
		}
		else
		{
			currentRightType = SideType.Wall;
		}
		CreateSideWallOrDoor(walls, left);
	}
	void CreateSideDoor(bool left){
		GameObject[] doors = rightDoors;
		if (left)
		{
			doors = leftDoors;
			currentLeftType = SideType.Door;
		}
		else
		{
			currentRightType = SideType.Door;
		}
		CreateSideWallOrDoor(doors, left);
	}
	void CreateSideWallOrDoorRandomly(bool left){
		var weHaveDoors = false;
		var weHaveWalls = false;
		if (leftDoors.Length > 0 && rightDoors.Length > 0)
			weHaveDoors = true;
		if (leftWalls.Length > 0 && rightWalls.Length > 0)
			weHaveWalls = true;
		if (weHaveWalls || weHaveDoors)
		{
			float doorChances = -1000;
			float wallChances = 1000;
			if (!weHaveDoors)
				doorChances = 1;
			if (!weHaveWalls)
				wallChances = -1;
			var r = Random.Range(doorChances, wallChances);
			if (r < 0)
				CreateSideDoor(left);
			else
				CreateSideWall(left);
		}
	}
	void CreateSideWallOrDoor(GameObject[] wallsOrDoors, bool left){
		bool iCanCreateWallOrDoor = false;
		GetGrid();
		if (left)
		{
			KuchoTile leftTile = tileGrid.FindTileOnPos(new Point(tilePos.x - 1, tilePos.y));
			if (!leftTile || (leftTile && !leftTile.currentRightSide)) // si no hay otra tile a su izquierda, o si que la hay pero no tiene pared derecha
				iCanCreateWallOrDoor = true;
		}
		else // right
		{
			KuchoTile rightTile = tileGrid.FindTileOnPos(new Point(tilePos.x + 1, tilePos.y));
			if (!rightTile || (rightTile && !rightTile.currentLeftSide)) // si no hay otra tile a su derecha, o si que la hay pero no tiene pared izquierda
				iCanCreateWallOrDoor = true;
		}
		if (iCanCreateWallOrDoor)
		{
			GameObject current;
			if (left)
				current = currentLeftSide;
			else
				current = currentRightSide;

			if (current) // si tenemos pared o puerta
				DestroyImmediate(current);

			var offset = rightSideWallOffset;
			if (left)
				offset = leftSideWallOffset;
			GameObject wallOrDoor = (GameObject)Instantiate(KuchoHelper.GetRandomGameObjectFromArray(wallsOrDoors), transform.position, Constants.zeroQ); 
			wallOrDoor.transform.parent = transform; //hazlo hijo
			wallOrDoor.name = wallOrDoor.name.Substring(0, wallOrDoor.name.Length - 7); // borro (Clone)
			// apaga spriteplanes para que me deje offsetear, piensa que spriteplanes hace su ajuste en onenable!
			var sprPlanes = wallOrDoor.GetComponentsInChildren<SpritePlane>();
			//hago que no se ejecute el spritePlanes pero lo preparo (con el offset) para que se updatee si muevo posicion en mi update
			foreach (SpritePlane sprPlane in sprPlanes)
			{
				sprPlane.offset = offset.z;
				sprPlane.updateOnEnableOnValidate = false;
				sprPlane.enabled = false;
			}
			wallOrDoor.transform.position = transform.position + offset; // ahora si puedo poner offset 
			if (left)
				currentLeftSide = wallOrDoor;
			else
				currentRightSide = wallOrDoor;
		}
		else
		{
			print (" NO PUEDO CREAR PARED/PUERTA, POSIBLEMENTE POR QUE HAY OTRA TILE AL LADO CON PARED/PUERTA");
		}
		GetStuffAfterCreateBlockOrWall();
	}
	
	public void GetStuffAfterCreateBlockOrWall(){
		if (currentBlock)
		{
			skyLightMark = currentBlock.GetComponentInChildren<SkyLightMark>();
			var gSettings = currentBlock.GetComponentsInChildren<ColliderSettings>();
			lightObstacles = GetComponentsInChildren<KuchoLightObstacleSprite>();
			lightSources = GetComponentsInChildren<LightSprite>();

			alphaSwitchers = GetComponentsInChildren<AlphaSwitch>();
			spritePlanes = GetComponentsInChildren<SpritePlane>();
			foreach (SpritePlane plane in spritePlanes)
				plane.updateOnEnableOnValidate = false; // para que no se actualice solo, ya lo hago yo en el orden

			tileComponents = GetComponentsInChildren<TileComponent>();
			currentBlockTileComponents = currentBlock.GetComponentsInChildren<TileComponent>();

			if (currentLeftSide)
				currentLeftSideTileComponents = currentLeftSide.GetComponentsInChildren<TileComponent>();
			else
				currentLeftSideTileComponents = new TileComponent[0];
			
			if (currentRightSide)
				currentRightSideTileComponents = currentRightSide.GetComponentsInChildren<TileComponent>();
			else
				currentRightSideTileComponents = new TileComponent[0];
			
			foreach (TileComponent tComp in tileComponents)
			{
				tComp.tile = this;
			}
            #if UNITY_EDITOR
			// apaga los snape me on editor 2 que sirven mientras creas el tile pero si los dejas encendidos y mueves las tiles a mano da problemas descuadrandose todo
			//var snapMes = GetComponentsInChildren<SnapMeOnEditor2>();
			//foreach (SnapMeOnEditor2 snapMe in snapMes)
			//	snapMe.enabled = false;
            #endif
		}
	}
	public bool Intersect(Vector2 pos){
		bool intersect = KuchoHelper.Intersect(pos, skyLightRect);
		return intersect;
	}
	public void SetAlphaAsGridToggle(){
		SetAlphaOnSwitchableRenderers(System.Convert.ToSingle(tileGrid.toggleCovers));
	}
	public void SetBlockMaterials(){
		Material backMat = tileGrid.materials[backMaterial];
		Material coverMat = tileGrid.materials[coverMaterial];
		if (tileGrid.coverMatLockedToNormalMat)
			coverMat = backMat;
		foreach (TileComponent tc in currentBlockTileComponents)
			tc.SetMaterial(backMat, coverMat);
	}

	public void SetCoverSprite()
	{
		var child = KuchoHelper.FindChildWithNameRecursive("Front", transform);
		if (child)
		{
			var spr = GetComponent<SWizSprite>();
			if (spr)
			{
				spr.SetSprite(coverSpriteID);
			}
		}
	}

	public void SetSideMaterials(){
 		foreach(TileComponent tc in currentLeftSideTileComponents)
			tc.SetMaterial(tileGrid.sideMaterials[sideMaterial], tileGrid.sideMaterials[sideMaterial]);
		foreach(TileComponent tc in currentRightSideTileComponents)
			tc.SetMaterial(tileGrid.sideMaterials[sideMaterial], tileGrid.sideMaterials[sideMaterial]);
	}
	public void SetAlphaOnSwitchableRenderers(float newAlpha){
		float alpha;
		for (int i = 0; i < tileComponents.Length; i++)
		{
			tileComponents[i].SetAlpha(newAlpha);
		}
	}
	public void SnapLightObstacles(Vector2 snap){
		foreach (KuchoLightObstacleSprite lo in lightObstacles)
			lo.transform.position = GetSnapped(lo.transform.position, snap);
	}
	Vector3 GetSnapped(Vector3 pos, Vector2 snap){
		pos.x = Mathf.RoundToInt(pos.x / snap.x) * snap.x;
		pos.y = Mathf.RoundToInt(pos.y / snap.y) * snap.y;
		return pos;
	}

}
