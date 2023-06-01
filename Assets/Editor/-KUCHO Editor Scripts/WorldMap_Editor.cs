using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(WorldMap))]
public class WorldMap_Editor : Editor
	{

	public override void OnInspectorGUI()
	{
		WorldMap sc = (WorldMap) target;
		EditorGUI.BeginChangeCheck();
		
		//DrawDefaultInspector();

		//sc.useDesignedMap = EditorGUILayout.Toggle("UseDesignedMap", sc.useDesignedMap);

		// CONTROLES PARA MAPA DISEÑADO
		if(sc.useDesignedMap)
		{
			// esto es el primer deditor con photoshop y tal super precario
			
		}
		// CONTROLES PARA MAPA ALEATORIO
		else
		{
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("====================================================================================================================================================", EditorStyles.boldLabel);
			EditorGUILayout.LabelField("SUN ", EditorStyles.boldLabel);

			float left =3.8f;
			float right = -0.8f;
			var d = Mathf.InverseLerp(left, right, sc.sunStartPosition);
			
			if(d < 0.1f)
				EditorGUILayout.LabelField("POSITION: NIGHT " );
			else if(d < 0.2f)
				EditorGUILayout.LabelField("POSITION: SUN SET " );
			else if(d < 0.40f)
				EditorGUILayout.LabelField("POSITION: EVENING " );
			else if(d < 0.6f)
				EditorGUILayout.LabelField("POSITION: MIDDAY " );
			else if(d < 0.8f)
				EditorGUILayout.LabelField("POSITION: MORNING ");
			else if(d < 0.9f)
				EditorGUILayout.LabelField("POSITION: DAWN ");
			else
				EditorGUILayout.LabelField("POSITION: NIGHT ");
			
			sc.sunStartPosition = GUILayout.HorizontalSlider(sc.sunStartPosition, left, right);
			
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			sc.skyToSunColorRatio = EditorGUILayout.Slider("Sky to Sun Color Ratio", sc.skyToSunColorRatio, 0,1);
			sc.sunIntensity = EditorGUILayout.Slider("Intensity", sc.sunIntensity, 0,1);
			sc.forceSunColor = EditorGUILayout.Toggle("Force Color", sc.forceSunColor);
			if (sc.forceSunColor)
			{
				sc.sunColor = EditorGUILayout.ColorField("Color", sc.sunColor);
			}
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("====================================================================================================================================================", EditorStyles.boldLabel);
			EditorGUILayout.LabelField("SKY LIGHT ", EditorStyles.boldLabel);
			sc.sunToSkylightsColorRatio = EditorGUILayout.Slider("Sun to Sky Light Color Ratio", sc.sunToSkylightsColorRatio, 0,1);
			sc.skyLightsIntensity = EditorGUILayout.Slider("Intensity", sc.skyLightsIntensity, 0,1);
			sc.skyLightsSaturation = EditorGUILayout.Slider("Saturation", sc.skyLightsSaturation, 0,1);

			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("====================================================================================================================================================", EditorStyles.boldLabel);

			var wraps = FarBackgroundData.instance.skyTexturesWrap;
			int maxValue = wraps.Length-1;

			if (sc.skyIndex >= wraps.Length)
				sc.skyIndex = maxValue;
			var sm = wraps[sc.skyIndex];
			string s = "Sky Texture: " + wraps[sc.skyIndex].name;
			if (wraps[sc.skyIndex].scrollable)
				s += " (Scrollable)";
			else
				s += " (Static)";
			EditorGUILayout.LabelField(s.ToUpper(), EditorStyles.boldLabel);

			sc.skyIndex = (int) GUILayout.HorizontalSlider(sc.skyIndex, 0, maxValue);
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			
			sc.sunColorToSky = EditorGUILayout.Slider("Sun to Sky Texture Color Ratio", sc.sunColorToSky, 0,1);
			sc.skyColor = EditorGUILayout.ColorField("Day Color", sc.skyColor);
			sc.nightColor = EditorGUILayout.ColorField("Night Color", sc.nightColor);
			sc.cosmosHueShift = EditorGUILayout.Slider("Hue Shift", sc.cosmosHueShift, -180, 180);
			EditorGUILayout.LabelField("---------DayTime--------", EditorStyles.boldLabel);
			sc.cosmosSaturation = EditorGUILayout.Slider("Saturation", sc.cosmosSaturation, 0,2);
			sc.cosmosBright = EditorGUILayout.Slider("Bright", sc.cosmosBright, 0,2);
			sc.cosmosContrast = EditorGUILayout.Slider("Contrast", sc.cosmosContrast, 0,2);
			sc.useDayAndNightCSV = EditorGUILayout.Toggle("Night Has Own CSV Values", sc.useDayAndNightCSV);
			if (sc.useDayAndNightCSV)
			{
				EditorGUILayout.LabelField("---------NightTime--------", EditorStyles.boldLabel);
				sc.cosmosAtNightSaturation = EditorGUILayout.Slider("Saturation", sc.cosmosAtNightSaturation, 0, 2);
				sc.cosmosAtnightBright = EditorGUILayout.Slider("Bright", sc.cosmosAtnightBright, 0, 2);
				sc.cosmosAtNightContrast = EditorGUILayout.Slider("Contrast", sc.cosmosAtNightContrast, 0, 2);
			}

			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("====================================================================================================================================================", EditorStyles.boldLabel);
			EditorGUILayout.LabelField("BACKGROUND ROCKS", EditorStyles.boldLabel);
			sc.wallGain = EditorGUILayout.Slider("Contrast", sc.wallGain, 0.5f, +1.5f);
			sc.wallShift = EditorGUILayout.Slider("Bright", sc.wallShift,  -0.3f, +0.3f);
			if (WorldMap.background && WorldMap.background.spriteRenderer && WorldMap.background.spriteRenderer.sharedMaterial)
			{
				WorldMap.background.spriteRenderer.sharedMaterial.SetFloat("_PreGain", sc.wallGain);
				WorldMap.background.spriteRenderer.sharedMaterial.SetFloat("_PreShift", sc.wallShift);
			}
			
			
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("====================================================================================================================================================", EditorStyles.boldLabel);
			EditorGUILayout.LabelField("PARALLAX: " + sc.parallaxName.ToUpper(), EditorStyles.boldLabel);

			maxValue = FarBackgroundData.instance.parallaxNames.Length -1;
			sc.parallaxIndex = (int) GUILayout.HorizontalSlider(sc.parallaxIndex, 0, maxValue);
			sc.fixedColorParallax = false; // En Level Maker siempre asi por simplicidad ( funciona bien con white rocks y classic mountains?)
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			
			if (sc.parallaxIndex < FarBackgroundData.instance.parallaxNames.Length && sc.parallaxIndex >= 0) 
			{
				string n = FarBackgroundData.instance.parallaxNames[sc.parallaxIndex];
				if (n.StartsWith("Parallax") || n.StartsWith("PARALLAX"))
					n = n.Substring(8, n.Length - 8);
				sc.parallaxName = n;
			}
			else
				sc.parallaxName = "OUT OF RANGE";
			
			

			sc.parallaxOffset.x = EditorGUILayout.Slider("Horizontal Shift", sc.parallaxOffset.x, -1000,1000);
			sc.parallaxOffset.y = EditorGUILayout.Slider("Vertical Shift", sc.parallaxOffset.y, -500,500);
			
			EditorGUILayout.LabelField("====================================================================================================================================================", EditorStyles.boldLabel);
			EditorGUILayout.LabelField("PHYSICS", EditorStyles.boldLabel);
			sc.temp = EditorGUILayout.Slider("Air Temperature", sc.temp, -20,60);
			sc.useTopLimit = EditorGUILayout.Toggle("Top Limit is a Wall", sc.useTopLimit);
			sc.topWorldLimitExtraHeight = EditorGUILayout.Slider("Top Limit Extra Height", sc.topWorldLimitExtraHeight, 0, 1000);
			sc.SetTopLimit();

			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("====================================================================================================================================================", EditorStyles.boldLabel);

			EditorGUILayout.LabelField("SIZE", EditorStyles.boldLabel);

			sc.wMultiplier = EditorGUILayout.IntSlider("Width", sc.wMultiplier, 0,32);
			sc.hMultiplier = EditorGUILayout.IntSlider("Height", sc.hMultiplier, 0,32);

			EditorGUILayout.BeginHorizontal();
			//if(GUILayout.Button("SCALE DOWN")) SizeInc(-1);
			//if(GUILayout.Button("SCALE UP")) SizeInc(1);
			if(GUILayout.Button("COMMIT SCALE")) sc.ScaleAllMaps();
			if(GUILayout.Button("COMMIT RESIZE")) sc.ResizeAllMaps();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();

			if(GUILayout.Button("CLEAR DES ON IND"))
			{
				sc.FillTerrain1BasedOnTerrain2(false, WorldMap.destructible, WorldMap.indestructible, 0);
				sc.ApplyMapChangesOnAlphaData(WorldMap.destructible);
			}
            if (GUILayout.Button("FILL DES WITH IND"))
            {
                sc.FillTerrain1BasedOnTerrain2(false, WorldMap.destructible, WorldMap.indestructible, 255);
                sc.ApplyMapChangesOnAlphaData(WorldMap.destructible);
            }
            EditorGUILayout.EndHorizontal();

            { // esto sustituye a Void Onvalidate que ya no es llamada por si sola
	            if (!Application.isPlaying && WorldMap.background && WorldMap.background.spriteRenderer.sharedMaterial)
	            {//NO ENTIENDO POR QUE HACIA ESTO
		            //sc.wallGain = WorldMap.background.spriteRenderer.sharedMaterial.GetFloat("_PreGain");
		            //sc.wallShift = WorldMap.background.spriteRenderer.sharedMaterial.GetFloat("_PreShift");
	            }

	            if (!EditorApplication.isPlayingOrWillChangePlaymode && !Application.isPlaying) // editor esta realmente parado
	            {
		            sc.PublicOnValidate();
		            sc.InitialiseLimits();
		            sc.AdjustWorldToNewSize();
	            }
            }

            //if(GUILayout.Button("REBUILD ALL COLLIDERS"))
			//{
			//	sc.RebuildAllCollidersOfAllMaps();
			//}
			if (EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(sc.gameObject);
			}
		}
	}
	void SizeInc(int inc){
		Point sizeMultipliers = GetNewSizeMultipliers(inc);
		WorldMap sc = (WorldMap) target;
		if ( sizeMultipliers.x > 0 && sizeMultipliers.x <= 32 && sizeMultipliers.y > 0 && sizeMultipliers.y <= 32)
		{
			sc.wMultiplier = sizeMultipliers.x;
			sc.hMultiplier = sizeMultipliers.y;
			sc.CalculateMapSizeBasedOnTilesAndMultipliers();
		}
		else Debug.Log(" ERROR, TAMAÑO MUY GRENDE O MUY PEQUEÑO");
		sc.PublicOnValidate();
	}
	Point GetNewSizeMultipliers(int inc){
		WorldMap sc = (WorldMap) target;

		string smaller = "";
		float a;
		float newWidthMult;
		float newHeightMult;

		if (sc.wMultiplier < sc.hMultiplier){
			smaller = "width";
		}
		if (smaller == "width"){
			newWidthMult = sc.wMultiplier + inc;
			a = newWidthMult / sc.wMultiplier; // factor por el que hay que multiplicar al otro
			newHeightMult = Mathf.Round(sc.hMultiplier * a); 
		}
		else{
			newHeightMult = sc.hMultiplier + inc;
			a = newHeightMult / sc.hMultiplier; // factor por el que hay que multiplicar al otro
			newWidthMult = Mathf.Round(sc.wMultiplier * a); 
		}
		return new Point((int)newWidthMult, (int)newHeightMult);
	}
}
