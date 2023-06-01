using System;
using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(Terrain2D))]
public class Terrain2D_Editor : Editor
{
    private bool expertMode;

    public override void OnInspectorGUI()
    {
        Terrain2D sc = (Terrain2D) target;
//		WorldMap map = sc.GetComponentInParent<WorldMap>();
        WorldMap map = WorldMap.instance;
        
        EditorGUI.BeginChangeCheck();
        
        if (!map)
            map = sc.GetComponentInParent<WorldMap>();

        string typeOfTerrain = sc.groundType.ToString().ToUpper();

//		EditorGUILayout.LabelField(typeOfTerrain, EditorStyles.boldLabel);
//		sc.tile = (Texture2D) EditorGUI.ObjectField(new Rect(40,40,100,200), typeOfTerrain, sc.tile, typeof (Texture2D), false);

/*
        if (sc.backgroundType != BackgroundType.SingleTile)
            if (GUILayout.Button("Set Single Tile Mode", EditorStyles.miniButtonMid))
                sc.SetSingleTileMode();
        if (sc.backgroundType != BackgroundType.TileMap)
            if (GUILayout.Button("Set Tile Map Mode", EditorStyles.miniButtonMid))
                sc.SetTileMapMode();
        if (sc.backgroundType != BackgroundType._3DModels)
            if (GUILayout.Button("Set 3D to Sprite Mode", EditorStyles.miniButtonMid))
                sc.Set3dObjectsToSpriteMode();
*/
        expertMode = (bool) EditorGUILayout.Toggle("Expert Mode", expertMode);

        TerrainDataBaseButtons(sc);

        if (sc.backgroundType == BackgroundType.TileMap)
        {
            sc.tileMap = (Tilemap) EditorGUILayout.ObjectField("Tile Map", sc.tileMap, typeof(Tilemap), true);
            if (sc.tileMap)
            {
                if (!sc.tileMapMaterial && sc.d2dSprite.SourceMaterial)
                    sc.tileMapMaterial = sc.d2dSprite.SourceMaterial;
                sc.tileMapMaterial = (Material) EditorGUILayout.ObjectField("Tile Map Material", sc.tileMapMaterial, typeof(Material), true);

                if (sc.tileMap.isActiveAndEnabled)
                {
                    if (GUILayout.Button("Fill Sprite With Tile Map & Switch Off TileMap"))
                    {
                        sc.FillSpriteTextureWithTileMap();
                        sc.tileMap.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (GUILayout.Button("Switch On TileMap"))
                    {
                        sc.tileMap.gameObject.SetActive(true);
                    }
                }
            }
        }

        if (sc.backgroundType == BackgroundType.SingleTile)
        {

            if (expertMode)
            {
                sc.tile = (Texture2D) EditorGUILayout.ObjectField(typeOfTerrain, sc.tile, typeof(Texture2D), false);

                if (!sc.tileMaterial && sc.d2dSprite.SourceMaterial) // para pillar los materiales que ya tenia asignados en el d2d
                    sc.tileMaterial = sc.d2dSprite.SourceMaterial;
                sc.tileMaterial = (Material) EditorGUILayout.ObjectField("Tile Material", sc.tileMaterial, typeof(Material), true);
            }

            if (sc.tile != null && sc.tile != sc.oldTile)
            {
                sc.FillSpriteTextureWithTile();
                sc.oldTile = sc.tile;
            }

            if (map.useDesignedMap)
            {
                sc.srcTerrain = (Texture2D) EditorGUILayout.ObjectField("Source Terrain Texture", sc.srcTerrain, typeof(Texture2D), false);
            }
            else // RANDOM GENERATION
            {


//			GUILayout.Button("minibuttonleft","minibuttonleft");
//			GUILayout.Button("minibuttonright","minibuttonright");



                GUILayout.BeginHorizontal();
                if (sc.d2dPoly)
                {

                }
                else
                {
                    GUILayout.Button("NO POLYGON COLLIDER ON THIS TERRAIN");
                }

                GUILayout.EndHorizontal();

                if (expertMode)
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Resize", EditorStyles.miniButtonMid))
                        map.ScaleMap(sc);
                    if (GUILayout.Button("Copy Alpha To Temp", EditorStyles.miniButtonMid))
                        map.CopyAlphaData(sc.smallAlphaData, map.tempAlphaData);
                    if (GUILayout.Button("ReMake SmallAlphaData", EditorStyles.miniButtonMid))
                        sc.GenerateSmallAlphaDataWithAlphaData(WorldMap.width, WorldMap.height, WorldMap.smallWidth, WorldMap.smallHeight);
                    if (GUILayout.Button("Fill Tex w Tile", EditorStyles.miniButtonMid))
                        sc.FillSpriteTexture(WorldMap.width, WorldMap.height, true);
                    if (GUILayout.Button("Adjust World To My Size", EditorStyles.miniButtonMid))
                        map.AdjustWorldToNewSize();
                    GUILayout.EndHorizontal();
                }


                EditorGUILayout.Space();
                EditorGUILayout.LabelField("1- ADJUST PSEUDO RANDOM GENERATION VALUES ----------------------------------------------------------", EditorStyles.whiteBoldLabel); // GENERATE SMALL DATA AND UPDATE BIG DATA

                sc.heightCleanFactor = (float) EditorGUILayout.Slider("Height Clean Factor", sc.heightCleanFactor, 0f, 0.1f);
                if (expertMode)
                {
                    /*
                    GUILayout.BeginHorizontal();
                    if (false && GUILayout.Button("GNRT"))
                    {
                        map.GenerateMap(sc, false); // genera sin aplicar pases de suavizado !!, solo el boton de general all maps lo usa
                        map.ApplyMapChangesOnSmallAlphaData(sc);
                    }
                    if (GUILayout.Button("GNRTE & SMTH PSES"))
                    {
                        map.GenerateMap(sc, true); // true significa aplicar smoothpases
                        map.ApplyMapChangesOnSmallAlphaData(sc);
                    }
                    if (GUILayout.Button("CLEAN SMALL REGIONS"))
    
                    {
                        map.DeleteAllSmallRegionsInSmallAlphaData(sc, true);
                        map.DeleteAllSmallRegionsInSmallAlphaData(sc, false);
                        map.ApplyMapChangesOnSmallAlphaData(sc);
                    }
                    RestoreButton(sc);
                    GUILayout.EndHorizontal();
                    */

                    sc.otherTerrainReference = (GroundReference) EditorGUILayout.EnumPopup("Other Terrain Reference", sc.otherTerrainReference, GUIStyle.none);
                    if (sc.otherTerrainReference != GroundReference.None)
                    {
                        sc.otherTerrainFactor = (float) EditorGUILayout.Slider("Other Terrain Factor", sc.otherTerrainFactor, 0, 0.5f);
                        sc.otherTerrainOffsetY = (int) EditorGUILayout.IntSlider("Other Terrain Offset Y", sc.otherTerrainOffsetY, -40, 40);
                    }

                    sc.fillMid = (int) EditorGUILayout.IntSlider("Fill Mid Value", sc.fillMid, 120, 138);
                    sc.randomRange = (int) EditorGUILayout.IntSlider("Fill Random Range", sc.randomRange, 0, 150);

                    sc.fillMin = sc.fillMid - sc.randomRange;
                    sc.fillMax = sc.fillMid + sc.randomRange;

                    sc.binarizeThreshold = (int) EditorGUILayout.IntSlider("BinarizeThreshold", sc.binarizeThreshold, 0, 255);
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("2- GENERATE & SMOOTH TERRAIN-----------------------------------------------------------------------", EditorStyles.whiteBoldLabel); // SHAPE SMALL DATA AND UPDATE BIG DATA

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("GNRTE & SMOOTH PASSES "))
                {
                    map.GenerateMap(sc, true); // true significa aplicar smoothpases
                    map.ApplyMapChangesOnSmallAlphaData(sc);
                }

                if (GUILayout.Button("SMOOTH PASS"))
                {
                    sc.BackupAlphaData();
                    map.StartPerformanceTest();
                    map.SmoothMap(sc, 1, false); // false significa no polish
                    map.ApplyMapChangesOnSmallAlphaData(sc);
                    map.PrintElapsedTime(" SMOOTH MAP HA TARDADO");
                }

                if (GUILayout.Button("SOFT SMOOTH PASS"))
                {
                    sc.BackupAlphaData();
                    map.StartPerformanceTest();
                    map.SmoothMap(sc, 0.5f, false); // false significa no polish
                    map.ApplyMapChangesOnSmallAlphaData(sc);
                    map.PrintElapsedTime(" SMOOTH MAP HA TARDADO");
                }

                UndoButton(sc);
                GUILayout.EndHorizontal();

                sc.smoothRange = (int) EditorGUILayout.IntSlider(new GUIContent("Smooth Range/Thickness (CPU EXPENSIVE!!!)", "CPU EXPENSIVE! radious of pixels around to consider when smoothing"), sc.smoothRange, 1, 20);
                if (expertMode)
                {
                    sc.smoothPasses = (int) EditorGUILayout.IntSlider(new GUIContent("Passes", "Amount of times the smoothing is repeated"), sc.smoothPasses, 0, 10);
                    sc.smoothRangePassMultipler = (float) EditorGUILayout.Slider(new GUIContent("Range Pass Multipler (!)", "Each repetition range will be multiplied by this"), sc.smoothRangePassMultipler, 0f, 1f);
                    sc.smoothHeightFactor = (float) EditorGUILayout.Slider(new GUIContent("Height Factor", "Height related radious change"), sc.smoothHeightFactor, -0.5f, 0.5f);
                    EditorGUILayout.LabelField("Borders:", EditorStyles.whiteBoldLabel);
                    sc.sideBorderSmoothRatio = (float) EditorGUILayout.Slider("Side Ratio", sc.sideBorderSmoothRatio, 0f, 1f);
                    sc.bottomBorderSmoothRatio = (float) EditorGUILayout.Slider("Bottom Ratio", sc.bottomBorderSmoothRatio, 0f, 1f);
                    sc.topBorderSmoothRatio = (float) EditorGUILayout.Slider("Top Ratio", sc.topBorderSmoothRatio, 0f, 1f);
                }

//			EditorGUILayout.LabelField("Threshold", sc.smoothThreshold.ToString());

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("3- LOOKING GOOD? REMOVE SMALL ISLANDS AND HOLES -----------------------------------------", EditorStyles.whiteBoldLabel);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("CLEAN SMALL REGIONS"))
                {
                    sc.BackupAlphaData();
                    map.DeleteAllSmallRegionsInSmallAlphaData(sc, true);
                    map.DeleteAllSmallRegionsInSmallAlphaData(sc, false);
                    map.ApplyMapChangesOnSmallAlphaData(sc);
                }

                UndoButton(sc);
                GUILayout.EndHorizontal();

                sc.wallThresholdSize = (int) EditorGUILayout.IntSlider("Island Size Threshold", sc.wallThresholdSize, 0, 500);
                sc.roomThresholdSize = (int) EditorGUILayout.IntSlider("Hole Size Threshold", sc.roomThresholdSize, 0, 500);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("4- POLISH -------------------------------------------------------------------------------", EditorStyles.whiteBoldLabel);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("HARD POLISH"))
                {
                    sc.BackupAlphaData();
                    map.StartPerformanceTest();
                    map.SmoothMap(sc, 2, true); // tyrue significa polish
                    map.ApplyMapChangesOnAlphaData(sc);
                    map.PrintElapsedTime(" POLISH HA TARDADO");
                }

                if (GUILayout.Button("SOFT POLISH"))
                {
                    sc.BackupAlphaData();
                    map.StartPerformanceTest();
                    map.SmoothMap(sc, 1, true); // tyrue significa polish
                    map.ApplyMapChangesOnAlphaData(sc);
                    map.PrintElapsedTime(" POLISH HA TARDADO");
                }

                if (GUILayout.Button("FIX CORNERS"))
                {
                    sc.BackupAlphaData();
                    map.StartPerformanceTest();
                    map.UglyCornerCheckPublic(sc);
                    map.ApplyMapChangesOnAlphaData(sc);
                    map.PrintElapsedTime(" UGLY CORNER HA TARDADO");
                }

                UndoButton(sc);
                GUILayout.EndHorizontal();
                if (expertMode)
                {
                    sc.smoothSquareRatio = (float) EditorGUILayout.Slider("Polish And Smooth Square Ratio (usually 0.5)", sc.smoothSquareRatio, 0, 1);
                    sc.polishRange = (int) EditorGUILayout.IntSlider("Range", sc.polishRange, 0, 10);
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("5- ERODE DARK AREAS-------------------------------------------------------------------------------", EditorStyles.whiteBoldLabel);

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("HORIZONTAL TEARS", EditorStyles.whiteBoldLabel);
                if (GUILayout.Button("HORIZONTAL TEARS"))
                {
                    sc.BackupAlphaData();
                    map.StartPerformanceTest();
                    map.DetailHorizontalTearsMap(sc);
                    map.ApplyMapChangesOnAlphaData(sc);

                    int smooth = sc.polishRange;
                    sc.polishRange = 1;
                    map.SmoothMap(sc, 1, true);
                    sc.smoothRange = smooth;
                    map.ApplyMapChangesOnAlphaData(sc);

                    map.PrintElapsedTime(" DETAIL HORIZONTAL FRACTURE HA TARDADO");
                }

                UndoButton(sc);
                GUILayout.EndHorizontal();
                sc.horizontalTearLumaThreshold = (float) EditorGUILayout.Slider("Threshold", sc.horizontalTearLumaThreshold, 0, 1);
                if (expertMode)
                {
                    sc.horizontalTearLumaThresholdDec = (float) EditorGUILayout.Slider("Threshold Dec", sc.horizontalTearLumaThresholdDec, 0, 0.2f);
                    sc.horizontalTearLumaDeep = (int) EditorGUILayout.IntSlider("Deep", sc.horizontalTearLumaDeep, 0, 5);
                }

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("VERTICAL TEARS", EditorStyles.whiteBoldLabel);
                if (GUILayout.Button("VERTICAL TEARS"))
                {
                    sc.BackupAlphaData();
                    map.StartPerformanceTest();
                    map.DetailVerticalTearMap(sc);
                    map.ApplyMapChangesOnAlphaData(sc);

                    int smooth = sc.polishRange;
                    sc.polishRange = 1;
                    map.SmoothMap(sc, 1, true);
                    sc.smoothRange = smooth;
                    map.ApplyMapChangesOnAlphaData(sc);

                    map.PrintElapsedTime(" DETAIL HORIZONTAL FRACTURE HA TARDADO");
                }

                if (GUILayout.Button("DOWNWARDS TEARS"))
                {
                    sc.BackupAlphaData();
                    map.StartPerformanceTest();
                    map.DetailDownwardsTearMap(sc);
                    map.ApplyMapChangesOnAlphaData(sc);

                    int smooth = sc.polishRange;
                    sc.polishRange = 1;
                    map.SmoothMap(sc, 1, true);
                    sc.smoothRange = smooth;
                    map.ApplyMapChangesOnAlphaData(sc);

                    map.PrintElapsedTime(" DETAIL HORIZONTAL FRACTURE HA TARDADO");
                }

                UndoButton(sc);
                GUILayout.EndHorizontal();

                sc.verticalTearLumaThreshold = (float) EditorGUILayout.Slider("Threshold", sc.verticalTearLumaThreshold, 0, 1);
                if (expertMode)
                {
                    sc.verticalTearLumaThresholdDec = (float) EditorGUILayout.Slider("Threshold Dec", sc.verticalTearLumaThresholdDec, 0, 0.2f);
                    sc.verticalTearLumaDeep = (int) EditorGUILayout.IntSlider("Deep", sc.verticalTearLumaDeep, 0, 5);
                }

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("UPWARDS TEARS", EditorStyles.whiteBoldLabel);
                if (GUILayout.Button("UPWARDS TEARS"))
                {
                    sc.BackupAlphaData();
                    map.StartPerformanceTest();
                    map.DetailUpwardsTearMap(sc);
                    map.ApplyMapChangesOnAlphaData(sc);

                    int smooth = sc.polishRange;
                    sc.polishRange = 1;
                    map.SmoothMap(sc, 1, true);
                    sc.smoothRange = smooth;
                    map.ApplyMapChangesOnAlphaData(sc);

                    map.PrintElapsedTime(" DETAIL HORIZONTAL FRACTURE HA TARDADO");
                }

                UndoButton(sc);

                GUILayout.EndHorizontal();
                sc.upwardsTearLumaThreshold = (float) EditorGUILayout.Slider("Threshold", sc.upwardsTearLumaThreshold, 0, 1);
                if (expertMode)
                {
                    sc.upwardsTearLumaThresholdDec = (float) EditorGUILayout.Slider("Threshold Dec", sc.upwardsTearLumaThresholdDec, 0, 0.2f);
                    sc.upwardsTearLumaDeep = (int) EditorGUILayout.IntSlider("Deep", sc.upwardsTearLumaDeep, 0, 30);
                }

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("                         --------IN GAME---------", EditorStyles.whiteBoldLabel);
                if (sc.gameObject.CompareTag("Indestructible"))
                {
                    sc.groundMakerNeedCatalyst = (bool) EditorGUILayout.Toggle("GroundMaker Need Catalyst", sc.groundMakerNeedCatalyst);
                }

                if (expertMode)
                {
                    sc.inGameHorizontalLumaDeep = (int) EditorGUILayout.IntSlider("In Game Horizontal Luma Deep", sc.inGameHorizontalLumaDeep, 0, 5);
                    sc.inGameVerticalLumaDeep = (int) EditorGUILayout.IntSlider("In Game Vertical Luma Deep", sc.inGameVerticalLumaDeep, 0, 5);
                }

                //			EditorGUILayout.LabelField("Threshold", sc.polishThreshold.ToString());

                /* no usar , son lentos y operan sobre big alpha data
                GUILayout.BeginHorizontal();
                
                EditorGUILayout.LabelField("-SEPARATED AREAS- NO FUNCIONA -------------", EditorStyles.whiteBoldLabel);
                if (GUILayout.Button("GET SMALL ISLES"))
                {
                    map.GetIsles(sc);
                    map.ApplyMapChangesOnAlphaData(sc);
                }
                if (GUILayout.Button("REMOVE SMALL ISLES"))
                {
                    map.RemoveSmallIsles(sc);
                    map.ApplyMapChangesOnAlphaData(sc);
                }
                if (GUILayout.Button("REMOVE SMALL HOLES"))
                {
                    map.RemoveSmallCaves(sc);
                    map.ApplyMapChangesOnAlphaData(sc);
                }
                GUILayout.EndHorizontal();
                */
                if (expertMode)
                {
                    EditorGUILayout.LabelField("-----------------", EditorStyles.whiteBoldLabel);
                    GUILayout.BeginHorizontal();
                    sc.copyRandomSettingsTo = (GroundType) EditorGUILayout.EnumPopup("Copy Settings To", sc.copyRandomSettingsTo, GUIStyle.none);
                    if (GUILayout.Button("COPY", EditorStyles.miniButtonRight))
                        map.CopyRandomSettingsTo(sc, sc.copyRandomSettingsTo);
                    GUILayout.EndHorizontal();
                }
            }
        }

        if (sc.backgroundType == BackgroundType._3DModels)
        {
            sc.cam3DtoSpriteMaterial = (Material) EditorGUILayout.ObjectField("Cam 3D to Sprite Material", sc.cam3DtoSpriteMaterial, typeof(Material), true);

            if (sc.cam3DtoSprite)
            {
                if (!sc.spriteRenderer.enabled)
                {
                    if (GUILayout.Button("RENDER 3D MODELS & FILL SPRITE TEX (F3)", GUILayout.Height(60)))
                    {
                        sc.FillSpriteTextureWith3DObjects(); // true significa FORCE
                    }
                    //que hacen estos dos aqui?
                    //sc.polishRange = (int)EditorGUILayout.IntSlider("Polish Range", sc.polishRange, 0, 10);
                    //sc.smoothSquareRatio = (float)EditorGUILayout.Slider("Polish Square Ratio", sc.smoothSquareRatio, 0, 1);

                    // creoque estos son de cuando probabal difernetes metodos de compresion antes de decidirme por uno solo
                    // sc.compressionFormat = (TextureFormat)EditorGUILayout.EnumPopup("Texture Format", sc.compressionFormat);
                    // sc.compressionQuality = (TextureCompressionQuality)EditorGUILayout.EnumPopup("Compresion Quality", sc.compressionQuality);
                }
                else
                {
                    if (GUILayout.Button("DISABLE SPRITE & ENABLE 3D MODELS (F3)", GUILayout.Height(60)))
                    {
                        sc.DisableSpriteAndEnable3DModels();
                    }

//                    sc.saturation = (float)EditorGUILayout.Slider("Polish Range", sc.saturation, 0, 10);
                }


                int count = 0;
                if (sc.cam3dtospriteCompressedTexData != null)
                    count = sc.cam3dtospriteCompressedTexData.Count;
                EditorGUILayout.IntField("Compressed Data Size", count);
                //sc.background3DModelsName = EditorGUILayout.TextField("Background 3D Models Name", sc.background3DModelsName);
                EditorGUILayout.BeginHorizontal();
                if (sc.cam3dtospriteCompressedTexData != null && sc.cam3dtospriteCompressedTexData.Count > 0)
                {
                    if (GUILayout.Button("Destroy Compressed Data"))
                    {
                        sc.cam3dtospriteCompressedTexData = null;
                    }

                    if (GUILayout.Button("Uncompress data to real texture"))
                    {
                        sc.FillSpriteTextureWithCompressedData();
                    }
                }
                else
                {
                    if (sc.spriteRenderer.enabled && sc.spriteRenderer.sprite && sc.texture)
                    {
                        if (GUILayout.Button("Compress texture"))
                        {
                            sc.CompressGrayscaleKuchoFormatTexture();
                        }
                    }
                }

                EditorGUILayout.EndHorizontal();
            }
        }
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(sc.gameObject);
        }
    }

    void UndoButton(Terrain2D sc)
    {
        if (GUILayout.Button("UNDO"))
        {
            if (sc.undoAlphaData == null || sc.undoAlphaData.Length != sc.d2dSprite.alphaData.Length)
            {
                Debug.Log(" NO PUEDO RESTAURAR, UNDO ALPHA DATA NULL O TAMAÑO ERRONEO");
            }
            else
            {
                WorldMap map = WorldMap.instance;
                map.CopyAlphaData(sc.undoAlphaData, sc.d2dSprite.alphaData);
                map.ApplyMapChangesOnAlphaData(sc);
            }
        }
    }

    void TerrainDataBaseButtons(Terrain2D sc)
    {
        EditorGUILayout.LabelField("- MATERIAL ---------", EditorStyles.whiteBoldLabel);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("PREV MAT"))
        {
            //Game.MakeSureGameInstanceIsNotNull();
            var newTerrainData = MaterialDataBase.instance.GetTerrainMaterialAndInc(sc, -1);
            sc.CopyFrom(newTerrainData, MaterialDataBase.index);
        }

        if (GUILayout.Button("NEXT MAT"))
        {
            //Game.MakeSureGameInstanceIsNotNull();
            var newTerrainData = MaterialDataBase.instance.GetTerrainMaterialAndInc(sc, +1);
            sc.CopyFrom(newTerrainData, MaterialDataBase.index);
        }

        if (GameData.instance.isMother && GUILayout.Button("Add to Data Base", EditorStyles.miniButtonRight))
        {
            sc.AddMaterialToTerrainDataBase();
        }

        GUILayout.EndHorizontal();
        if (sc.groundType != GroundType.Background)
        {
            EditorGUILayout.LabelField("- GENERATION ---------", EditorStyles.whiteBoldLabel);
            GUILayout.BeginHorizontal();
            if (GameData.instance.isMother && GUILayout.Button("PREV PRESET"))
            {
                var newTerrainData = MaterialDataBase.instance.GetTerrainGenerationPresetAndInc(-1);
                sc.CopyFrom(newTerrainData);
            }

            if (GameData.instance.isMother && GUILayout.Button("NEXT PRESET"))
            {
                var newTerrainData = MaterialDataBase.instance.GetTerrainGenerationPresetAndInc(+1);
                sc.CopyFrom(newTerrainData);
            }

            if (GameData.instance.isMother && GUILayout.Button("ADD PRESSET", EditorStyles.miniButtonRight))
            {
                sc.AddMaterialToTerrainDataBase();
            }

            GUILayout.EndHorizontal();
            if (GameData.instance.isMother)
                EditorGUILayout.TextField("PresetName", sc.generationPresetName);
        }
    }
}
