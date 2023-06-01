using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Light2D;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;
[CreateAssetMenu(fileName = "MaterialData", menuName = "Material DataBase", order = 51)]

public class MaterialDataBase : ScriptableObject
{
    [System.Serializable]
    public class TerrainMatPreset
    {
        public GroundType groundType;
        public BackgroundType backgroundType;
        public Texture2D tile;
        public Material tileMapMaterial;
        public Material cam3DtoSpriteMaterial;
        public Material tileMaterial;

        public override string ToString()
        {
            string r = groundType.ToString();
            switch (groundType)
            {
                case (GroundType.Destructible):
                    r += "/" + tile.name + "/" + tileMaterial.name;
                    break;
                case (GroundType.Indestructible):
                    r += "/" + tile.name + "/" + tileMaterial.name;
                    break;
                case (GroundType.Background):
                    r += "/" + backgroundType;
                    switch (backgroundType)
                    {
                        case (BackgroundType.TileMap):
                            r += "/" + tileMapMaterial.name;
                            break;
                        case (BackgroundType.SingleTile):
                            r += "/" + tile + "/" + tileMaterial.name;
                            break;
                        case (BackgroundType._3DModels):
                            r += "/" + cam3DtoSpriteMaterial.name;
                            break;
                    }
                    break;
            }
            return r;

        }

        public bool IsEqual(Terrain2D t, string lookForThisTileMaterialName)
        {
            if (!t)
            {
                Debug.Log("INTENTO DE COMPARAR TERRENOS PERO ME LLEGA NULL");
                return false;
            }

            if (groundType != t.groundType) return false;
            if (groundType == GroundType.Background)
            {
                if (backgroundType != t.backgroundType) return false;
                if (backgroundType == BackgroundType.TileMap)
                {
                    if (tileMapMaterial != t.tileMapMaterial) return false;
                }
                else if (backgroundType == BackgroundType._3DModels)
                {
                    if (lookForThisTileMaterialName != null && lookForThisTileMaterialName != "")
                    {    
                        if (cam3DtoSpriteMaterial.name != lookForThisTileMaterialName)
                            return false;
                    }
                    else
                    {
                        if (cam3DtoSpriteMaterial != t.cam3DtoSpriteMaterial) return false;
                    }
                }
            }
            else
            {
                if (backgroundType != t.backgroundType) return false;
                if (tile != t.tile) return false;
                if (lookForThisTileMaterialName != null && lookForThisTileMaterialName != "")
                {    if (tileMaterial.name != lookForThisTileMaterialName)
                        return false;
                }
                else
                {
                    if (tileMaterial != t.tileMaterial) return false;
                }
            }
            return true;
        }
        public bool IsEqual(TerrainMatPreset t, string lookForThisTileMaterialName)
        {
            if (t == null)
            {
                Debug.Log("INTENTO DE COMPARAR TERRENOS PERO ME LLEGA NULL");
                return false;
            }

            if (groundType != t.groundType) return false;
            if (groundType == GroundType.Background)
            {
                if (backgroundType != t.backgroundType) return false;
                if (backgroundType == BackgroundType.TileMap)
                {
                    if (tileMapMaterial != t.tileMapMaterial) return false;
                }
                else if (backgroundType == BackgroundType._3DModels)
                {
                    if (lookForThisTileMaterialName != null && lookForThisTileMaterialName != "")
                    {    
                        if (cam3DtoSpriteMaterial.name != lookForThisTileMaterialName)
                            return false;
                    }
                    else
                    {
                        if (cam3DtoSpriteMaterial != t.cam3DtoSpriteMaterial) return false;
                    }
                }
            }
            else
            {
                if (backgroundType != t.backgroundType) return false;
                if (tile != t.tile) return false;
                if (lookForThisTileMaterialName != null && lookForThisTileMaterialName != "")
                {    if (tileMaterial.name != lookForThisTileMaterialName)
                    return false;
                }
                else
                {
                    if (tileMaterial != t.tileMaterial) return false;
                }
            }
            return true;
        }
        
        public void CopyFrom(Terrain2D t)
        {
            if (!t)
            {
                Debug.Log("INTENTO DE COPIAR TERRENOS PERO ME LLEGA NULL");
                return;
            }

            groundType = t.groundType;
            backgroundType = t.backgroundType;
            tile = t.tile;
            tileMapMaterial = t.tileMapMaterial;
            cam3DtoSpriteMaterial = t.cam3DtoSpriteMaterial;
            tileMaterial = t.tileMaterial;
        }
        /*
        public TerrainMatInfo(Terrain2Dinfo t)
        {
            if (t == null)
            {
                Debug.Log("INTENTO DE COPIAR TERRENOS PERO ME LLEGA NULL");
                return;
            }

            groundType = t.groundType;
            backgroundType = t.backgroundType;
            tile = t.tile;
            tileMapMaterial = t.tileMapMaterial;
            cam3DtoSpriteMaterial = t.cam3DtoSpriteMaterial;
            tileMaterial = t.tileMaterial;
        }
        */
    }

    [System.Serializable]
    public class TerrainGenerationPreset
    {
        public string name = "No Name";
        public GroundReference otherTerrainReference;
        public float otherTerrainFactor;
        public int otherTerrainOffsetY ;
        public int fillMid ;
        public int randomRange ;
        public float fillMin ;
        public float fillMax ;
        public float heightCleanFactor ;
        public int smoothRange ;
        public int smoothPasses ;
        public float smoothRangePassMultipler ;
        public float smoothHeightFactor ;
        public float sideBorderSmoothRatio ;
        public float bottomBorderSmoothRatio ;
        public float topBorderSmoothRatio ;
        public float smoothSquareRatio ;
        public int polishRange ;
        public float horizontalTearLumaThreshold ;
        public float horizontalTearLumaThresholdDec ;
        public int horizontalTearLumaDeep ;
        public float verticalTearLumaThreshold ;
        public float verticalTearLumaThresholdDec ;
        public int verticalTearLumaDeep ;
        public float upwardsTearLumaThreshold ;
        public float upwardsTearLumaThresholdDec ;
        public int upwardsTearLumaDeep ;

        public int inGameHorizontalLumaDeep ;
        public int inGameVerticalLumaDeep ;

        public bool IsEqual(Terrain2D t, string lookForThisTileMaterialName)
        {
            if (!t)
            {
                Debug.Log("INTENTO DE COMPARAR TERRENOS PERO ME LLEGA NULL");
                return false;
            }
            if (otherTerrainReference != t.otherTerrainReference) return false;
            if (otherTerrainFactor != t.otherTerrainFactor) return false;
            if (otherTerrainOffsetY != t.otherTerrainOffsetY) return false;
            if (fillMid != t.fillMid) return false;
            if (randomRange != t.randomRange) return false;
            if (heightCleanFactor != t.heightCleanFactor) return false;
            if (smoothRange != t.smoothRange) return false;
            if (smoothPasses != t.smoothPasses) return false;
            if (smoothRangePassMultipler != t.smoothRangePassMultipler) return false;
            if (smoothHeightFactor != t.smoothHeightFactor) return false;
            if (sideBorderSmoothRatio != t.sideBorderSmoothRatio) return false;
            if (bottomBorderSmoothRatio != t.bottomBorderSmoothRatio) return false;
            if (topBorderSmoothRatio != t.topBorderSmoothRatio) return false;
            if (smoothSquareRatio != t.smoothSquareRatio) return false;
            if (polishRange != t.polishRange) return false;
            if (horizontalTearLumaThreshold != t.horizontalTearLumaThreshold) return false;
            if (horizontalTearLumaThresholdDec != t.horizontalTearLumaThresholdDec) return false;
            if (horizontalTearLumaDeep != t.horizontalTearLumaDeep) return false;
            if (verticalTearLumaThreshold != t.verticalTearLumaThreshold) return false;
            if (verticalTearLumaThresholdDec != t.verticalTearLumaThresholdDec) return false;
            if (verticalTearLumaDeep != t.verticalTearLumaDeep) return false;
            if (upwardsTearLumaThreshold != t.upwardsTearLumaThreshold) return false;
            if (upwardsTearLumaThresholdDec != t.upwardsTearLumaThresholdDec) return false;
            if (upwardsTearLumaDeep != t.upwardsTearLumaDeep) return false;

            if (inGameHorizontalLumaDeep != t.inGameHorizontalLumaDeep) return false;
            if (inGameVerticalLumaDeep != t.inGameVerticalLumaDeep) return false;

            return true;
        }

        public void CopyFrom(Terrain2D t)
        {
            if (!t)
            {
                Debug.Log("INTENTO DE COPIAR TERRENOS PERO ME LLEGA NULL");
                return;
            }

            name = t.generationPresetName;
            otherTerrainReference = t.otherTerrainReference;
            otherTerrainFactor = t.otherTerrainFactor;
            otherTerrainOffsetY = t.otherTerrainOffsetY;
            fillMid = t.fillMid;
            randomRange = t.randomRange;
            fillMin = t.fillMin;
            fillMax = t.fillMax;
            heightCleanFactor = t.heightCleanFactor;
            smoothRange = t.smoothRange;
            smoothPasses = t.smoothPasses;
            smoothRangePassMultipler = t.smoothRangePassMultipler;
            smoothHeightFactor = t.smoothHeightFactor;
            sideBorderSmoothRatio = t.sideBorderSmoothRatio;
            bottomBorderSmoothRatio = t.bottomBorderSmoothRatio;
            topBorderSmoothRatio = t.topBorderSmoothRatio;
            smoothSquareRatio = t.smoothSquareRatio;
            polishRange = t.polishRange;
            horizontalTearLumaThreshold = t.horizontalTearLumaThreshold;
            horizontalTearLumaThresholdDec = t.horizontalTearLumaThresholdDec;
            horizontalTearLumaDeep = t.horizontalTearLumaDeep;
            verticalTearLumaThreshold = t.verticalTearLumaThreshold;
            verticalTearLumaThresholdDec = t.verticalTearLumaThresholdDec;
            verticalTearLumaDeep = t.verticalTearLumaDeep;
            upwardsTearLumaThreshold = t.upwardsTearLumaThreshold;
            upwardsTearLumaThresholdDec = t.upwardsTearLumaThresholdDec;
            upwardsTearLumaDeep = t.upwardsTearLumaDeep;

            inGameHorizontalLumaDeep = t.inGameHorizontalLumaDeep;
            inGameVerticalLumaDeep = t.inGameVerticalLumaDeep;
        }
    }
    
    public List<TerrainGenerationPreset> terrainGenerationData;

    [FormerlySerializedAs("terrainMatInfo")] public List<TerrainMatPreset> terrainMaterialPresets;
    public Material terrainLightObstacleMaterial;
    public Material worldWallLightObstacleMaterial;
    public Shader[] foregroundTerrainShaders;
    public Shader[] backgroundTerrainShaders;
    [Header("-----------")]
    public Material defaultSpritesMat;
    public Material grassMat;
    public Material grassExploMat;
    public GrassSettings grass;
    public Material smokeMat;
    
    public Material[] miscMaterials;
    public Sprite singlePixelCenter;

    private static MaterialDataBase _instance;
    public static MaterialDataBase instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameData.instance.materialDataBase;
                if (!_instance)
                {
                    Debug.Log("NO ENCUENTRO MATERIAL DATA");
                }
            }
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    //[AdvancedInspector.Inspect]
    public void Migrate()
    {
        /*
        terrainMatInfo = new List<TerrainMatInfo>();
        foreach (Terrain2Dinfo t in destructible)// itero los antiguos
        {
            var temp = new TerrainMatInfo(t);// convierto a nuevo formatop
            if (!IsInList(temp))
            {
                terrainMatInfo.Add(new TerrainMatInfo(t));
            }
        }
        foreach (Terrain2Dinfo t in indestructible)
        {
            var temp = new TerrainMatInfo(t);// convierto a nuevo formatop
            if (!IsInList(temp))
            {
                terrainMatInfo.Add(new TerrainMatInfo(t));
            }
        }
        foreach (Terrain2Dinfo t in background)
        {
            var temp = new TerrainMatInfo(t);// convierto a nuevo formatop
            if (!IsInList(temp))
            {
                terrainMatInfo.Add(new TerrainMatInfo(t));
            }
        }
        */
    }
    public void AddMaterialIfNotInTheList(Terrain2D terr)
    {
        var list = terrainMaterialPresets;
        bool nullFound;
        do
        {
            nullFound = false;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == null)
                {
                    list.RemoveAt(i);
                    nullFound = true;
                    break;
                }
            }
        } while (nullFound);

        if (!IsThisTerrainMaterialInList(terr))
        {
            TerrainMatPreset newTerrain = new TerrainMatPreset(); // creo nueva instancia ya que si no se borra la referencia al cargar otro nivel
            newTerrain.CopyFrom(terr);
            list.Add(newTerrain);
            Debug.Log("TERRENO AÑADIDO A " + terr.groundType);
        }
        else
        {
            Debug.Log("TERRENO YA EXISTIA");
        }
    }
    public void AddTerrainGenerationPresetToList(Terrain2D terr) {
        var list = terrainGenerationData;

        bool nullFound;

        do
        {
            nullFound = false;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == null)
                {
                    list.RemoveAt(i);
                    nullFound = true;
                    break;
                }
            }
        } while (nullFound);

        if (!IsThisGenerationPresetInList(terr, list))
        {
            TerrainGenerationPreset newTerrain = new TerrainGenerationPreset(); // creo nueva instancia ya que si no se borra la referencia al cargar otro nivel
            newTerrain.CopyFrom(terr);
            list.Add(newTerrain);
            Debug.Log("TERRENO AÑADIDO A " + terr.groundType);
        }
        else
        {
            Debug.Log("TERRENO YA EXISTIA");
        }
    }

    public static int index = 0;
    public static int index2 = 0;

    public TerrainMatPreset GetTerrainMaterialAndInc(Terrain2D terr, int inc)
    {
        //List<Terrain2Dinfo> list = GetTheTightList(type);

        TerrainMatPreset result = null;

        for (int i = 0; i < terrainMaterialPresets.Count; i++)
        {
            index += inc;
            KuchoHelper.WrapInsideArrayLengthIfOutOfrange(ref index, terrainMaterialPresets.Count);
            if (terrainMaterialPresets[index].groundType == terr.groundType && terrainMaterialPresets[index].backgroundType == terr.backgroundType)
            {              
                result = terrainMaterialPresets[index];
                break; // encontrado uno de mi tipo, saliendo
            }
        }

        if (result != null)
        {
            string name = "null";
            if (terr.groundType != GroundType.Background)
            {        if (result.tile != null)
                    name = result.tile.name;
            }
            else
            {
                if (result.backgroundType == BackgroundType._3DModels)
                    name = result.cam3DtoSpriteMaterial.name;
                else
                    name = result.tileMapMaterial.name;
            }
            Debug.Log("TERRENO NUMERO " + index + " " + name);
            return result;
        }
        else
        {
            Debug.LogError("NO HAY TERRENO DEL TIPO " + terr.groundType);
            return null;
        }
    }

    public TerrainGenerationPreset GetTerrainGenerationPresetAndInc(int inc)
    {
        TerrainGenerationPreset result = null;
        
        index2 += inc;
        KuchoHelper.WrapInsideArrayLengthIfOutOfrange(ref index2, terrainGenerationData.Count);
        if (index2 < 0 ||index2 >= terrainGenerationData.Count)
            return null;
        
        result = terrainGenerationData[index2];
                
        if (result != null)
        {
            Debug.Log("TERRENO NUMERO " + index2);
            return result;
        }
        else
        {
            return null;
        }
    }
    public TerrainMatPreset GetTerrainMaterialInfo(GroundType type, int index)
    {
        if (index < 0 || index >= terrainMaterialPresets.Count)
            index = 0;
        if (type != terrainMaterialPresets[index].groundType)
            Debug.LogError( " GROUND TYPE DOESNT MATCH, ASKING FOR " + type + " FOUND " + terrainMaterialPresets[index].groundType);
        return terrainMaterialPresets[index];
    }

    bool IsThisGenerationPresetInList(Terrain2D terr, List<TerrainGenerationPreset>list)
    {
        if (list != null)
        {
            foreach (TerrainGenerationPreset t in list)
            {
                if (t.IsEqual(terr, null))
                    return true;
            }
        }
        return false;
    }

    bool IsThisMaterialInList(TerrainMatPreset tmi)
    {
        if (terrainMaterialPresets != null && tmi != null)
        {
            foreach (TerrainMatPreset t in terrainMaterialPresets)
            {
                if (t.IsEqual(tmi, null))
                    return true;
            }
        }
        return false;
    }
    bool IsThisTerrainMaterialInList(Terrain2D terr)
    {
        if (terrainMaterialPresets != null && terr != null)
        {
            foreach (TerrainMatPreset t in terrainMaterialPresets)
            {
                if (t.IsEqual(terr, null))
                    return true;
            }
        }
        return false;
    }

    public int GetIndexOf(Terrain2D terr, GroundType type)
    {
        var list = terrainMaterialPresets;

        string compareMaterialName = null;
        if (type != GroundType.Background)
        {
            if (terr.tileMaterial.name.EndsWith("(Clone)"))
                compareMaterialName = terr.tileMaterial.name.Substring(0, terr.tileMaterial.name.Length - 7);
        }
        else
        {
            switch (terr.backgroundType)
            {
                case (BackgroundType.TileMap):
                    if (terr.tileMapMaterial.name.EndsWith("(Clone)"))
                        compareMaterialName = terr.tileMapMaterial.name.Substring(0, terr.tileMapMaterial.name.Length - 7);
                    break;
                case (BackgroundType.SingleTile):
                    if (terr.tileMaterial.name.EndsWith("(Clone)"))
                        compareMaterialName = terr.tileMaterial.name.Substring(0, terr.tileMaterial.name.Length - 7);
                    break;
                case (BackgroundType._3DModels):
                    if (terr.cam3DtoSpriteMaterial.name.EndsWith("(Clone)"))
                        compareMaterialName = terr.cam3DtoSpriteMaterial.name.Substring(0, terr.cam3DtoSpriteMaterial.name.Length - 7);
                    break;
            }
        }

        if (list != null)
        {
            for(int i = 0; i < list.Count;i++)
            {
                if (list[i].IsEqual(terr, compareMaterialName))
                    return i;
            }
        }
        return -1;
    }
    
    public int GetIndexInMiscMaterialDatabase(Material mat) // devuelve el indice si lo encuentra
    {
        for (int i = 0; i < miscMaterials.Length; i++)
        {
            Material m = miscMaterials[i];
            if (m != null) // podria haberse nuleado alguno de mi database
            {
                if (mat != null)
                {
                    if (CompareMaterials(m, mat))
                        return i;
                }
            }
        }
        return -1;
    }
    public Material GetMaterialFromMiscListByIndexAndName(int index, string matName)
    {
        if (index > 0 && index < miscMaterials.Length && miscMaterials[index].name == matName)
        {
            MyLogs.Resolutions("ENCONTRADO MATERIAL:" + matName + " POR INDICE:" + index);
            return miscMaterials[index];
        }
        MyLogs.Resolutions("NO ENCONTRADO MATERIAL:" + matName + " POR INDICE:" + index + " BUSCANDO AHORA POR NOMBRE");
        return GetMiscMaterialByName(matName);
    }

    public Material GetMiscMaterialByName(string matName)
    {
        foreach(Material mat in miscMaterials)
            if (mat.name == matName)
            { 
                MyLogs.Resolutions("ENCONTRADO MATERIAL POR NOMBRE:" + matName);
                return mat;
            }
        MyLogs.Resolutions("NO ENCONTRADO MATERIAL POR NOMBRE:" + matName + " ESTO ES UN NULL Y VA A SALIR ROSA");
        return null;
    }

    public bool IsTerrainShader(Shader shader)
    {
        if (IsForegroundShader(shader))
            return true;
        if (IsBackgroundShader(shader))
            return true;
        if (shader.name.StartsWith("_Kucho/Terrain/"))
            return true;
        return false;
    }

    public bool IsOneOfTheThreeTerrainGameObjects(GameObject go)
    {
        if (go == WorldMap.destructible.gameObject || go == WorldMap.indestructible.gameObject || go == WorldMap.background.gameObject)
            return true;
        return false;
    }

    public bool IsForegroundShader(Shader shader)
    {
        if (foregroundTerrainShaders != null)
        {
            foreach (Shader s in foregroundTerrainShaders)
                if (s == shader)
                    return true;
        }

        return false;
    }
    public bool IsBackgroundShader(Shader shader)
    {
        if (backgroundTerrainShaders != null)
        {
            foreach (Shader s in backgroundTerrainShaders)
                if (s == shader)
                    return true;
        }

        return false;
    }
#if UNITY_EDITOR // esto elimina los clones
    
    public bool ShouldBeStoredAndRestored(Material m, GameObject go)
    {
        if (m == null)
            return false;
        if (IsTerrainShader(m.shader)) // los terrenos no los quiero, esos se recuperan restaudando con metodo propio
            return false;
        if (go && IsOneOfTheThreeTerrainGameObjects(go))
            return false;
        if (go && go.GetComponent<TextMesh>()) // un text mesh? es un texto de UI del editor, pero no un font in game
            return false;
        if (m.name == Light2D.CustomSprite.GeneratedMaterialName)
            return false;
        string path = AssetDatabase.GetAssetPath(m);
        if (path == "")
        {
            Debug.LogError("MAT:" + m.name + " IN GAMEOBJECT:" + go +" IS AN INSTANCE; IT CAN'T BE COPIED/RESCUED");
            return false;
        }
        return true;
    }
    public int AddMaterialToMiscDatabaseIfNew(GameObject go,Material mat)
    {
        if (GetIndexInMiscMaterialDatabase(mat) == -1)
        {
            int nullIndex = MiscDatabaseHasNullAt();
            if (nullIndex >= 0)
            {
                miscMaterials[nullIndex] = mat;
                Debug.Log("ADDED MAT:" + mat.name + " ON POS " + nullIndex);
                return nullIndex;
            }
            else
            {
                List<Material> list = miscMaterials.ToList();
                list.Add(mat);
                Debug.Log("ADDED MAT:" + mat.name + " ON POS " + (miscMaterials.Length - 1) +  "(END)");
                miscMaterials = list.ToArray();
                return miscMaterials.Length - 1;
            }
        }
        return -1;
    }

    int MiscDatabaseHasNullAt()
    {
        for (int i = 0; i < miscMaterials.Length; i++)
        {
            if (miscMaterials[i] == null)
                return i;
        }
        return -1;
    }
#endif
    #region COMPARE MATERIALS
    public bool CompareMaterials(Material m1, Material m2)
    {
        if (m1.name != m2.name)
            return false;
        if (m1.HasProperty(ShaderProp._Color))
        {
            if (m2.HasProperty(ShaderProp._Color)) // ambos tienen property color
            {
                if (m1.color != m2.color)
                {
                    return false;
                }
            }
            else // solo el primero tiene color
            {
                return false;
            }
        }
        else// el primero no tiene color
        {
            if (m2.HasProperty(ShaderProp._Color)) // el segundo si tiene property color
            {
                return false;
            }
            else // ambos no tienen color
            {
            }
        }
        if (m1.renderQueue != m2.renderQueue)
            return false;
        if (m1.shader != m2.shader)
            return false;
        if (m1.mainTexture != m2.mainTexture)
            return false;
        if (m1.mainTextureOffset != m2.mainTextureOffset)
            return false;
        if (!SameFloatProperty(m1, m2, ShaderProp._EmissionColorMul))
            return false;
        if (!SameFloatProperty(m1, m2, ShaderProp._ObstacleMul))
            return false;
        if (!SameFloatProperty(m1, m2, ShaderProp._Hue))
            return false;
        if (!SameFloatProperty(m1, m2, ShaderProp._Sat))
            return false;
        if (!SameFloatProperty(m1, m2, ShaderProp._Cont))
            return false;
        if (!SameFloatProperty(m1, m2, ShaderProp._Val))
            return false;
        if (!SameFloatProperty(m1, m2, ShaderProp._Alpha))
            return false;
        if (!SameFloatProperty(m1, m2, ShaderProp._AdditiveColor))
            return false;
        if (!SameFloatProperty(m1, m2, ShaderProp._MultiplicativeColor))
            return false;
        if (!SameFloatProperty(m1, m2, ShaderProp._Add))
            return false;
        if (!SameFloatProperty(m1, m2, ShaderProp._Luminosity))
            return false;
        if (!SameTextureProperty(m1, m2, ShaderProp._AlphaTex))
            return false;
        if (!SameTextureProperty(m1, m2, ShaderProp._AlphaTex1))
            return false;
        if (!SameTextureProperty(m1, m2, ShaderProp._AlphaTex2))
            return false;
        if (!SameTextureProperty(m1, m2, ShaderProp._AlphaTex3))
            return false;
        
        return true;
    }

    public bool SameFloatProperty(Material m1, Material m2, int prop)
    {
        if (m1.HasProperty(prop))
        {
            if (m2.HasProperty(prop)) // ambos tienen property 
            {
                if (m1.GetFloat(prop) != m2.GetFloat(prop))
                {
                    return false;
                }
            }
            else // solo el primero tiene property
            {
                return false;
            }
        }
        else// el primero no tiene esa property
        {
            if (m2.HasProperty(prop)) // el segundo si tiene property 
            {
                return false;
            }
            else // ambos no tienen esa property
            {
                return true;
            }
        }
        return true;
    }
    public bool SameTextureProperty(Material m1, Material m2, int prop)
    {
        if (m1.HasProperty(prop))
        {
            if (m2.HasProperty(prop)) // ambos tienen property 
            {
                if (m1.GetTexture(prop) != m2.GetTexture(prop))
                {
                    return false;
                }
            }
            else // solo el primero tiene property
            {
                return false;
            }
        }
        else// el primero no tiene esa property
        {
            if (m2.HasProperty(prop)) // el segundo si tiene property 
            {
                return false;
            }
            else // ambos no tienen esa property
            {
                return true;
            }
        }
        return true;
    }
    public bool SameColorProperty(Material m1, Material m2, int prop)
    {
        if (m1.HasProperty(prop))
        {
            if (m2.HasProperty(prop)) // ambos tienen property 
            {
                if (m1.GetColor(prop) != m2.GetColor(prop))
                {
                    return false;
                }
            }
            else // solo el primero tiene property
            {
                return false;
            }
        }
        else// el primero no tiene esa property
        {
            if (m2.HasProperty(prop)) // el segundo si tiene property 
            {
                return false;
            }
            else // ambos no tienen esa property
            {
                return true;
            }
        }
        return true;
    }
    #endregion
    public Material GetMaterialFromDatabase(string matName)
    {
        foreach (Material m in miscMaterials)
        {
            if (matName == m.name)
                return m;
        }
        return null;
    }

    public void CleanNullsInMaterialDataBaseWillScrambleExistingPointers()
    {
        var list = new List<Material>();
        foreach (Material m in miscMaterials)
        {
            if (m != null)
                list.Add(m);
        }
        miscMaterials = list.ToArray();
    }

}
