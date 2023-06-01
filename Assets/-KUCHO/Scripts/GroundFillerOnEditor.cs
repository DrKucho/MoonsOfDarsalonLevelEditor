using UnityEngine;
using System.Collections;
 // para rangefloat
using UnityEditor;

[System.Serializable]
/// <summary>
/// OjoCuidao! mientras que x e y son valores en pixeles, Z es un valor lineal que depende de SpritePlanes
/// </summary>
public class DecoPostOffset{
	public Vector3 min = new Vector3(0,0,-1);
	public Vector3 max = new Vector3(0,0,1);
	public Vector3 inc = new Vector3(0,0,0.01f);
}
[ExecuteInEditMode][SelectionBase]
public class GroundFillerOnEditor : MonoBehaviour {

	public enum FillMode {Create, Delete, CreateAndModify, Modify, None}
	public FillMode fillMode;
	public int createEach;
	public bool debug = false;
	public bool debugFillOnly = false;
	public bool debugNote = false;
	[Header ("----GROUND DETECTION")]
	public int repetitions= 400;
	public Vector2 rayStartOffset = new Vector2(3,8);
	public float rayOffsetXRandomRange;
	public Vector2 rayDirection = new Vector2(0,-1);
	public int rayLength = 300;
	[Space (5)]
	public bool fillDestructible = true;
	public bool fillIndestructible = true;
	public bool pixelRaycast = false;
	public PixelRaycastHit pixelHit;
	[Header ("For Normal Raycast Only")]
	public MaskType _groundMask = MaskType.GroundObstacleAndLevel;
	public float maxInclination = 0.6f;
	int groundMask;
	public enum PlaceThingsMethod {useLightNeedsOfThings, cycleThingStoreSequentially, randomThingStore}
	[Header (" - - - - ")]
	public PlaceThingsMethod placeThingsMethod = PlaceThingsMethod.useLightNeedsOfThings;
	[Header ("----LIGHT")]
	public Vector2 readLightOffsetPos;
	public WorldLightPixel lightPixel;
	[Header ("----THINGS")]
	public DecoThingsCreatedInEditor parent_List;
	public enum PosOffset {Random, Cycle}
	public PosOffset posOffset = PosOffset.Random;

	public DecoPostOffset offset;
	Vector3 cyclingOffset;
	DecoCell thingFound;
	GameObject thingFoundGO;
	float groundInclination = 0;
	float distance = 0; // la distancia desde el comienzo del rayo al suelo
	[Header ("----MISC")]
	public bool shift1PixelOnFlipSprite = false;
	public Material[] materials;
	public  Renderer arrowRend;
	[HideInInspector]
	public float right = 1;

	Vector3 pos;
	bool thereWasPlant = false; // flag, se activa si encontramos planta para mas adelante tomar una decision

	public enum Found {Valid, Invalid, NotFound}
	[System.Serializable]
	public class GroundFillerDecoThing{
		public ItemStore itemStore;
		public string findThisItemStore = "";
	}
	public void InitialiseInEditor(){
		if (!parent_List)
			parent_List = transform.GetComponentInParent<DecoThingsCreatedInEditor>();
		var rends = GetComponentsInChildren<Renderer>();
		foreach (Renderer r in rends)
		{
			if (r.name == "Arrow")
				arrowRend = r;
		}
	}
	public void OnEnable(){ //  print(name + " ONENABLE ");
		groundMask = Masks.GetLayerMaskFromEnum(_groundMask);
		if (Application.isPlaying)
            gameObject.SetActive(false);
	}
	#if UNITY_EDITOR
	void Update(){
        if (parent_List)
    		parent_List.transform.position = Constants.zero3;
        if (Selection.gameObjects.Length > 0 && Selection.gameObjects[0] == gameObject)
        {
	        Tools.pivotMode = PivotMode.Pivot;
        }
	}
	#endif
	public void OnValidate(){
		pos = transform.position; 
		rayDirection = rayDirection.normalized;

		//no se por que hice esto
		//offset.min.z = Mathf.Clamp(offset.min.z, -1, 0);
		//offset.max.z = Mathf.Clamp01(offset.max.z);
		offset.inc.z = Mathf.Clamp01(offset.inc.z);

	}
	public void SetCreateMode(){    
		fillMode = FillMode.Create;
	}
	public void SetDeleteMode(){
		fillMode = FillMode.Delete;
	}
	public void SetCreateAndModifyMode(){
		fillMode = FillMode.CreateAndModify;
	}
	public void SetModifyMode(){
		fillMode = FillMode.Modify;
	}
	public void SetNoneMode(){
		fillMode = FillMode.None;
	}
	public void ClearAllThings(){
		parent_List.ClearList();
	}
	public void DoIt(){
		pos = transform.position;
//		pos.x = Mathf.Round(pos.x); // asi evito problemas de decomanager en GetIndex(Vector2 position){
//		pos.y = Mathf.Round(pos.y);
		if (!parent_List)
		{
			print (" NO TENGO PADRE PARA LAS PLANTAS/THINGS");
			return;
		}
		//		rayDirection = rayDirection.normalized;
//		groundMask = Game.GetLayerMaskFromEnum(_groundMask);
//		if (!decoManager) decoManager = Game.FindDecomanager(_decoManager);

		var groundFound = LookForGround();

		if (groundFound == Found.Valid || groundFound == Found.Invalid)
		{
			if (debug) print(name + "SUELO ENCONTRADO EN " + pos);

			if (groundFound == Found.Valid)
			{
				thingFoundGO = parent_List.GetGameObject(pos);
				if (thingFoundGO)
				{
					ThereWasThing();
				}
				else
				{
					ThereWasntThing();
				}
			}
			// INCREMENTANDO
			Vector2 incMultiplier = new Vector2(1f,1f);

			if (thereWasPlant) incMultiplier.x = 2f; // incremento extra para que vaya mas rapido si esta en zona de plantas, probablemente dejara un hueco cuando termine la zona de plantas pero es un mal menor
			else incMultiplier.x = 1f;

			if (groundFound == Found.Invalid) incMultiplier.y = 0f; // asi si encuentro un obstaculo no hay por que incrementar y
			else incMultiplier.y = 1f; 

//				var rayOffsetRandomX = Mathf.RoundToInt(Random.Range(Mathf.Abs(rayOffsetXRandomRange), -Mathf.Abs(rayOffsetXRandomRange)));
//				var increment = new Vector2((rayStartOffset.x + rayOffsetRandomX) * incMultiplier.x * right, rayStartOffset.y * incMultiplier.y);
//				pos += (Vector3)increment;
//				if (debug) print(name + "INCREMENTADA POSITION EN " + increment + " NUEVA POSICION EN " + pos);


		}
		else //(Found.NotFound)
		{
			if (debug) print(name + "NO HAY SUELO??? ");
		}
	}
	void ThereWasntThing(){
		if (debug) print(name + "NO HABIA PLANTA EN " + pos);
		if (fillMode == FillMode.Create || fillMode == FillMode.CreateAndModify)
		{
			CreatePlant();
		}
	}
	void ThereWasThing(){
		if (debug) print(name + "HABIA ALGO " + pos);
		if(fillMode == FillMode.Create) return;

		if (thingFoundGO)
		{
			if (fillMode == FillMode.Delete)
			{
                parent_List.items.Remove(thingFoundGO.GetComponent<Item>());
				DestroyImmediate(thingFoundGO);
			}
			else if (fillMode == FillMode.Modify)
			{
				GrowAnimator grow = thingFoundGO.GetComponent<GrowAnimator>();
				if (grow)
                    grow.StartGrowing(distance/rayLength, GetPosOffset().z, true);
			}
		}
		else
		{
			print(name + " thing Found Go Vacio? como es posible");
		}
	}
	public Found LookForGround(){
		Found groundFound = Found.NotFound;
		var rayStart = new Vector2(pos.x, arrowRend.bounds.min.y);	
		
		if (pixelRaycast)
		{
			groundInclination = 0;
	//		pixelHit = PixelTools.Raycast(rayStart , rayDirection, rayLength, Game.destructibleGroundD2D.AlphaTex, true); // true significa que buscamos pixeles solidos
			pixelHit = PixelTools.AlphaRaycast(rayStart, rayDirection, rayLength, true, fillDestructible, fillIndestructible); // true signifoca que buscamos pixel solido
			if (pixelHit.insideOfTexture){
				if (pixelHit.found){//hemos encontrado un pixel solido
					if(debug) print(name+"PIXEL SOLIDO ENCONTRADO EN " + pixelHit.point);
					distance = pixelHit.distance;
					if(distance > 0){ // si no estamos metidos dentro del suelo
						groundFound = Found.Valid;
						pos = new Vector3(pixelHit.point.x, pixelHit.point.y - rayDirection.y, pos.z); // me muevo al punto donde hemos encontrado el pixel -1 en la coordenada Y , osea en la superficie
					}
					else { // estabamos metidos dentro del suelo asi que buscamos ahora la superficie de la roca
						if(debug) print(name+"PIXEL DENTRO DE ROCA, BUSCO ARRIBA");
	//					pixelHit = PixelTools.Raycast(rayStart , -rayDirection, rayLength, Game.destructibleGroundD2D.AlphaTex, false); // false significa que buscamos pixeles transparentes
						pixelHit = PixelTools.AlphaRaycast(rayStart, -rayDirection, rayLength, false, fillDestructible, fillIndestructible); // false significa que buscamos pixel transparente
						
						if (pixelHit.found){ // hemos encontrado un pixel transparnte
							if(debug) print(name+"PIXEL TRANSPARENTE ENCONTRADO EN " + pixelHit.point);
							// ahora busco por si hubiera suelo indestructible justo en el borde del destructible, si las plantas crecen solo en destructible puede ocurrir que haya indestrucitble encima y nodeben crecer ahi 
							// pero no funciona, siguen creciendo 
							var indest = PixelTools.ThereIsPixelWorld(new Vector2(pixelHit.point.x, pixelHit.point.y + 3), WorldMap.indestructible.d2dSprite.AlphaTex);
							if (indest == false){
								if(debug) print(name+"NO HABIA SUELO INDESTRUCTIBLE EN EL BORDE" + pixelHit.point);
								groundFound = Found.Valid;
								pos = new Vector3(pixelHit.point.x, pixelHit.point.y, pos.z);						
							}
							else{
								if(debug) print(name+"HABIA SUELO INDESTRUCTIBLE EN EL BORDE, NO CREO PLANTA" + pixelHit.point);
							}
						}
					}
				}
			}
			else {
				if (debug) print(name + " FUERA DE LA TEXTURA!");
			}
			if(debug){
				if (groundFound != Found.NotFound) print(name + " SUELO EN " + pixelHit.point);
				else print(name + " NO HAY SUELO EN " + pixelHit.point);
			}
		}
		else // NORMAL RAY CAST
		{
			var hit = Physics2D.Raycast(rayStart, rayDirection, rayLength, Masks.decoSpecial);
            if (hit)
            {
                if (!hit.transform.gameObject.name.StartsWith("CellChid"))
                {
                    if (debug) print(name + " SUELO NO VALIDO=" + hit.transform.gameObject.name);
                }
                //			var ground = hit.transform.gameObject.collider2D; // obten el collider del suelo // por que hago esto? puedo pillarlo de hit.collider, seguramente era muy torpe cuando lo hice al principio
                var go = hit.transform.gameObject;
                if (groundMask == 0)
                {
                    Debug.LogError(this + " GROUND MASK ES 0? OBTENIENDO MASK DE NUEVO");
                    groundMask = Masks.GetLayerMaskFromEnum(_groundMask);
                }
                if (KuchoHelper.IsLayerInLayerMask(hit.transform.gameObject.layer, groundMask) && (fillDestructible && go.CompareTag("Destructible") || fillIndestructible && go.CompareTag("Indestructible")))
                { // SUELO ENCONTRADO
                    distance = hit.fraction * rayLength;
                    groundInclination = Mathf.Abs(hit.normal.x);
                    if (groundInclination == 0)
                    {
                        print(" OJO NORMAL CERO?");
                    }
                    if (groundInclination < maxInclination) // NORMAL BUENA
                    {
                        if (distance > 0) // EL RAYO HA SALIDO DEL CENTRO DE LA PARED?
                        { // NO
                            groundFound = Found.Valid;
                            pos = new Vector3(hit.point.x, hit.point.y, pos.z);
                        }
                        else // SI, EL RAYO HA SALIDO DEL CENTRO DE LA PARED
                        {
                            if (debug) print(name + " ESTAMOS DENTRO DE ROCA =" + hit.point + " BUSCANDO ARRIBA");
                            // busca la superficie hacia arriba, solo se hacerlo usando pixelRayCast
                            pixelHit = PixelTools.AlphaRaycast(rayStart, -rayDirection, rayLength, false, fillDestructible, fillIndestructible); // false significa que buscamos pixel transparente
                            if (pixelHit.found)
                            { // hemos encontrado un pixel transparnte
                                var surfacePlus5 = pixelHit.point + new Vector2(0, 5);
                                if (debug) print(name + "PIXEL TRANSPARENTE (SUPERFICIE) ENCONTRADO EN " + pixelHit.point + " NUEVO RAY START + 5 ARRIBA=" + surfacePlus5);
                                hit = Physics2D.Raycast(surfacePlus5, rayDirection, rayLength, Masks.decoSpecial); // hay que supar 5 pixels hacia arriba para estar seguros de que disparamos por encima de collider, piensa que no siempre el collider va ajustado al pixel, en zonas rugosas el sollider esta simplificado y no es perfectamente fiel
                                if (KuchoHelper.IsLayerInLayerMask(hit.transform.gameObject.layer, groundMask) && (fillDestructible && go.CompareTag("Destructible") || fillIndestructible && go.CompareTag("Indestructible")))
                                {// SUELO ENCONTRADO
                                    distance = hit.fraction * rayLength;
                                    if (groundInclination < maxInclination) // NORMAL BUENA
                                    {
                                        if (distance > 0) // EL RAYO HA SALIDO DEL CENTRO DE LA PARED?
                                        { // NO
                                            groundFound = Found.Valid;
                                            pos = new Vector3(hit.point.x, hit.point.y, pos.z);
                                        }
                                        else // SI, EL RAYO HA SALIDO DEL CENTRO DE LA PARED osea que NOS HEMOS METIDO EN OTRA PARED SEPARADA POR MUY POCO DEL PRIMER COLLIDER
                                        {
                                            if (debug) print(name + " NOS HEMOS METIDO EN OTRA PARED!!!! =" + hit.point + " INVALIDANDO");
                                            groundFound = Found.Invalid;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else // NOEMAL MALA
                    {
                        if (debug) print(name + " SUELO NO VALIDO, NORMAL BAJA = " + hit.normal.x);
                        groundFound = Found.Invalid;
                    }
                }
                else // ha encontrado algo pero no correspode con la mascara
                {
                    if (debug) print(name + "SUELO INVALIDO" + hit.collider);
                    groundFound = Found.Invalid;
                }
            }
			if(debug){
				if (groundFound == Found.NotFound) print(name + " NO HAY SUELO EN " + hit.point);
			}
		}
		return groundFound;
	}
	public void CreatePlant(){
		if (debug) print(name + "CREANDO PLANTA EN " + pos);
		GameObject originalThing = null;
		int birngPlantFromThisStoreIndex = -1; // si encuentro un item que pueda traer, pondre el indice de su almacen aqui y al final de esta funcion lo traigo
		switch(placeThingsMethod)
		{	
			case (PlaceThingsMethod.useLightNeedsOfThings):
				if (debug) print(name + " INTENTANDO CREAR PLANTA");
				//Game.worldLightSystemExtras.GetLightPixelFromTexture2D(lightPixel, (Vector2)pos + readLightOffsetPos);
				// traigo una planta de cada almacen de la lista al mismo punto?!! si , se supone que esara configurado para que unos items se instanciaran si hay luz y otros si no hay luz, es un modo de con un solo filler poder rellenar facilmente...
				// ahora mismo no podria funcionar , ya que solo defina un intide pero ni aun trayendo el item al que punta el indice podria funcionar bien
				// ya que solo traeria el ultimo ..... WTF 
				for (int i = 0; i < parent_List.original.Length; i++) 
				{
					var nextItem = parent_List.original[i];
					if (nextItem)
					{
						GrowAnimator grow = nextItem.gameObject.GetComponent<GrowAnimator>();
						if (grow)
						{
							if (grow.lightBased != Liking.DontCare)
							{
								if (grow.maxLight >= lightPixel.visibility && lightPixel.visibility >= grow.minLight)
								{
									if (debug) print(name + " HAY LUZ SUFICIENTE PARA " + nextItem + " TRAYENDO ITEM DE ALMACEN");
									birngPlantFromThisStoreIndex = i;
								}
								else // no habia la luz requerida
								{
									if (debug) print(name + " NO HABIA LUZ PARA NINGUN ITEM, VISIBILITY = " + lightPixel.visibility);
								}
							}
							else // no necesita luz
							{
								if (debug) print(name + " EL ITEM A TRAER NO NECESITA LUZ");
								birngPlantFromThisStoreIndex = i;
							}
						}
						else // no tiene script grow animator
						{
							if (debug) print(name + " EL ITEM A TRAER NO TIENE GROW ANIMATOR");
							birngPlantFromThisStoreIndex = i;
						}
					}
					else // no quedan mas items en el alamcen
					{
						if (debug) print(name + " NO QUEDAN ITEMS EN EL ALMACEN");
					}
					
					if (birngPlantFromThisStoreIndex == i) i = 999999999; // asi rompo el bucle
				}
				break;
			case (PlaceThingsMethod.cycleThingStoreSequentially):// si no hay que usar la luz para ver cual traigo se hace en plan secuencial
                originalThing = parent_List.CycleOriginal().gameObject;
				break;
			case (PlaceThingsMethod.randomThingStore):
                originalThing = parent_List.RandomOriginal().gameObject;
				break;
		}
		if(originalThing != null) InstantiatePlant(originalThing, GetPosOffset());
	}
	public void InstantiatePlant(GameObject thing, Vector3 _posOffset){
        Vector3 worldPos = pos + _posOffset;
        worldPos.x = Mathf.Round(worldPos.x); // asi evito problemas de decomanager en GetIndex(Vector2 position){
        worldPos.y = Mathf.Round(worldPos.y);
        GameObject newThing = Instantiate(thing, worldPos, Constants.zeroQ) as GameObject;
		newThing.transform.parent = parent_List.transform;
		GrowAnimator grow = newThing.GetComponent<GrowAnimator>();
        Renderer rend = newThing.GetComponent<Renderer>();
        Item decoThing = newThing.GetComponent<Item>();
        SpritePlane spritePlane = newThing.GetComponent<SpritePlane>();
		if (spritePlane)
			spritePlane.RandomizeMyPlane();
        decoThing.isOriginal = false;
        decoThing.bt_pos = decoThing.transform.position;
		rend.enabled = true;

        if(grow)
			grow.StartGrowing(groundInclination, distance/rayLength, relativeSpritePlanesOffsetZ, true);
		newThing.SetActive(true);
//        parent_List.things.Add(newThing);
        parent_List.items.Add(decoThing);

		if (debugFillOnly) print(name + " CREADA PLANTA " + newThing.name + " EN " + newThing.transform.position);
		if (debugNote)
		{
			Notes note = newThing.AddComponent<Notes>();
			note._ = " normal = " + groundInclination.ToString();
		}
	}
//	public Vector3 GetPosOffset_no(){
//		switch (posOffset)
//		{
//			case (PosOffset.Random):
//				finalPosOffset = new Vector3(Random.Range(posOffsetMin.x, posOffsetMax.x), Random.Range(posOffsetMin.y, posOffsetMax.y), Random.Range(posOffsetMin.z, posOffsetMax.z));
//				finalPosOffset.z = ForbiddenRange(finalPosOffset.z, posOffsetNoZ);
//				break;
//			case (PosOffset.Cycle):
//				finalPosOffset += posOffsetInc;
//				if (finalPosOffset.x > posOffsetMax.x) finalPosOffset.x = posOffsetMin.x + finalPosOffset.x - posOffsetMax.x; 
//				if (finalPosOffset.y > posOffsetMax.y) finalPosOffset.y = posOffsetMin.y + finalPosOffset.y - posOffsetMax.y;
//				if (finalPosOffset.z > posOffsetMax.z) finalPosOffset.z = posOffsetMin.z + finalPosOffset.z - posOffsetMax.z;
//				if (finalPosOffset.x < posOffsetMin.x) finalPosOffset.x = posOffsetMax.x - finalPosOffset.x - posOffsetMin.x;
//				if (finalPosOffset.y < posOffsetMin.y) finalPosOffset.y = posOffsetMax.y - finalPosOffset.y - posOffsetMin.y;
//				if (finalPosOffset.z < posOffsetMin.z) finalPosOffset.z = posOffsetMax.z - finalPosOffset.z - posOffsetMin.z;
//				finalPosOffset.z = ForbiddenRange(finalPosOffset.z, posOffsetNoZ);
//				break;
//		}
//		return finalPosOffset;
//	}
	float relativeSpritePlanesOffsetZ;

	public Vector3 GetPosOffset(){
		Vector3 finalOffset = Constants.zero3;
		switch (posOffset)
		{
			case (PosOffset.Random):
				finalOffset = new Vector3(Random.Range(offset.min.x, offset.max.x), Random.Range(offset.min.y, offset.max.y), Random.Range(offset.min.z, offset.max.z));
				relativeSpritePlanesOffsetZ = finalOffset.z;
				finalOffset.z = GetValidDecoZ(finalOffset.z);
				break;
			case(PosOffset.Cycle):
				print(name + " PRE CYC OFFSET=" + cyclingOffset);
				KuchoHelper.Wrap(ref cyclingOffset.x, offset.inc.x, offset.min.x, offset.max.x);
				KuchoHelper.Wrap(ref cyclingOffset.y, offset.inc.y, offset.min.y, offset.max.y);
				KuchoHelper.Wrap(ref cyclingOffset.z, offset.inc.z, offset.min.z, offset.max.z);
				print(name + " POS CYC OFFSET=" + cyclingOffset);
				finalOffset = cyclingOffset; // antes de modificar Z a un valor real
				relativeSpritePlanesOffsetZ = finalOffset.z;
				finalOffset.z = GetValidDecoZ(finalOffset.z);
				print(name + " FINAL REAL Z  =" + finalOffset.z);
				break;
		}
		return finalOffset;				
	}
	float GetValidDecoZ(float z){
		var factor = 0f;
		RangeFloat r;
		if (z > 0)
		{
			factor = Mathf.InverseLerp(0, 1, z);
			r = new RangeFloat(WorldMap.spritePlanes.farBackground.min, WorldMap.spritePlanes.farBackground.max);
		}
		else
		{
			factor = 1 - Mathf.InverseLerp(0, -1, z);
			r = new RangeFloat(WorldMap.spritePlanes.coverground2.min, WorldMap.spritePlanes.coverground2.max);
		}
		return  Mathf.Lerp(r.min, r.max, factor);
	}
	public float ForbiddenRange(float z, MinMax noZ){
		var closeToMin = z - noZ.min;
		var closeToMax = z - noZ.max;
		if (noZ.min < z && z < noZ.max)
		{
			if (Mathf.Abs(closeToMin) > Mathf.Abs(closeToMax)) z = noZ.min;
			else z = noZ.max;
		}
		return z;
	}
	[HideInInspector] public int materialIndex = -1;
	public void PreviousMaterial()
	{
		ShiftMaterial(-1);
	}
	public void NextMaterial()
	{
		ShiftMaterial(1);
	}
	void ShiftMaterial(int inc)
	{
		if (materials != null && materials.Length > 0)
		{
			KuchoHelper.IncAndWrapInsideArrayLength(ref materialIndex, inc, materials.Length);
			var m = materials[materialIndex];

			foreach (Item it in parent_List.items)
			{
				SetMat(it, m);
			}

			foreach (Item it in parent_List.originals)
			{
				SetMat(it, m);
			}

			string s;
			if (m)
				s = m.ToString();
			else
				s = "Default";

			if (s.EndsWith("(UnityEngine.Material)"))
				s = s.Substring(0, s.Length - "(UnityEngine.Material)".Length);
			
			Debug.Log(name + " MATERIAL:" + s + " SET");
		}
		else
		{
			Debug.Log(name + " NO MATERIALS TO SET");
		}
	}

	void SetMat(Item it, Material m)
	{
		if (m)
			it.firstRenderer.sharedMaterial = m;
		else
			it.GetComponent<SWizSprite>().ForceUpdateMaterial();
	}
}
