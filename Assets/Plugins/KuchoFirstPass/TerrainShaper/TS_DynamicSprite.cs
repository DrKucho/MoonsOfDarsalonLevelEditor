using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Profiling;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class TS_DynamicSprite : TS_Dynamic
{
	public static List<TS_DynamicSprite> DestructibleSprites = new List<TS_DynamicSprite>();

	public bool useStandardD2dMaterial = true; 

	public bool copyPropertiesFromSorceToClonedMaterial = true; 
	public Material SourceMaterial;

	[SerializeField]
	private Material clonedMaterial;

	[Range(1.0f, 100.0f)]
	public float Sharpness = 1.0f;
	
	[SerializeField]
	public SpriteRenderer spriteRenderer; 

	[SerializeField]
	private float expectedPixelsToUnits;
	
	[SerializeField]
	private Vector2 expectedPivot;

	protected virtual void OnWillRenderObject()
	{
		if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
		
		UpdateSourceMaterial();
		
		DestroyMaterialIfSettingsDiffer();
		
		var sprite = spriteRenderer.sprite;
		if (SourceMaterial != null && sprite != null)
		{
			if (clonedMaterial == null)
			{
				clonedMaterial = TS_Helper.Clone(SourceMaterial, false);
			}
			else
			{
				if (!Application.isPlaying && copyPropertiesFromSorceToClonedMaterial)
					clonedMaterial.CopyPropertiesFromMaterial(SourceMaterial); 
			}

			clonedMaterial.hideFlags = HideFlags.HideInInspector;

			TS_Helper.BeginStealthSet(clonedMaterial);
			clonedMaterial.SetTexture(ShaderProp._MainTex, sprite.texture);
			clonedMaterial.SetTexture(ShaderProp._AlphaTex, AlphaTex);
			clonedMaterial.SetVector(ShaderProp._AlphaScale, CalculateAlphaScale(sprite));
			clonedMaterial.SetVector(ShaderProp._AlphaOffset, CalculateAlphaOffset(sprite));
			clonedMaterial.SetFloat(ShaderProp._Sharpness, Sharpness);

			TS_Helper.EndStealthSet();
		}

		if (spriteRenderer.sharedMaterial != clonedMaterial)
		{
			spriteRenderer.sharedMaterial = clonedMaterial;
		}
	}
   	 public void SetNewMaterial(Material newMat){ 
       	 SourceMaterial = newMat;
    	    clonedMaterial = TS_Helper.Clone(SourceMaterial, false);
  	  }
     private Vector2 CalculateAlphaScale(Sprite sprite)
     {
	     var texture     = sprite.texture;
	     var textureRect = sprite.textureRect;
	     var scaleX      = TS_Helper.Divide(texture.width , Mathf.Floor(textureRect.width ) + AlphaShiftX) * TS_Helper.Divide(OriginalWidth , AlphaWidth );
	     var scaleY      = TS_Helper.Divide(texture.height, Mathf.Floor(textureRect.height) + AlphaShiftY) * TS_Helper.Divide(OriginalHeight, AlphaHeight);
		
	     return new Vector2(scaleX, scaleY);
     }
     private Vector2 CalculateAlphaOffset(Sprite sprite)
     {
	     var scalingX = TS_Helper.Divide(Mathf.Floor(sprite.textureRect.width ), OriginalWidth );
	     var scalingY = TS_Helper.Divide(Mathf.Floor(sprite.textureRect.height), OriginalHeight);
		
	     var texture     = sprite.texture;
	     var textureRect = sprite.textureRect;
	     var offsetX     = TS_Helper.Divide(Mathf.Ceil(textureRect.x + AlphaX * scalingX) - AlphaShiftX / 2, texture.width );
	     var offsetY     = TS_Helper.Divide(Mathf.Ceil(textureRect.y + AlphaY * scalingY) - AlphaShiftY / 2, texture.height);
		
	     return new Vector2(offsetX, offsetY);
     }

     private void UpdateSourceMaterial()
     {
	     if (useStandardD2dMaterial){ 
		     if (SourceMaterial == null)
		     {
			     if (spriteRenderer.sharedMaterial != null)
			     {
				     SourceMaterial = spriteRenderer.sharedMaterial;
			     }
			     else
			     {
				     SourceMaterial = Resources.Load<Material>("Sprites-Default (Destructible 2D)");
			     }
		     }
			
		     if (SourceMaterial != null && SourceMaterial.HasProperty("_AlphaTex") == false)
		     {
			     if (SourceMaterial.name == "Sprites-Default")
			     {
				     SourceMaterial = Resources.Load<Material>("Sprites-Default (Destructible 2D)");
			     }
		     }
	     }
     }
     private void DestroyMaterialIfSettingsDiffer()
     {
	     if (clonedMaterial != null)
	     {
		     if (SourceMaterial == null)
		     {
			     DestroyMaterial(); return;
		     }
			
		     if (clonedMaterial.shader != SourceMaterial.shader)
		     {
			     DestroyMaterial(); return;
		     }
	     }
     }
     	
     private void DestroyMaterial()
     {
	     TS_Helper.Destroy(clonedMaterial);
		
	     clonedMaterial = null;
     }
	public void CompressAlphaDataShort(){
        if (alphaData != null && alphaData.Length > 0)
        {
            List<short> comAlphaData = new List<short>();
            int alphadataLength = alphaData.Length;
            alphaAndSpriteSizeOnCompressionTime = new Vector2Int(AlphaWidth, AlphaHeight);

            int i = 0;
            int cData = 0;
            int repeats = 0;
            while (i < alphadataLength) 
            {
                while (repeats < 32767 && i < alphadataLength && alphaData[i] > 127)
                {
                    cData = 32768; 
                    repeats++; 
                    i++;
                }
                if (repeats > 0)
                {
                    cData = cData | repeats;
                    comAlphaData.Add((short)cData);
                    repeats = 0;
                }
                while (repeats < 32767 && i < alphadataLength && alphaData[i] <= 127)
                {
                    cData = 0; 
                    repeats++; 
                    i++;
                }
                if (repeats > 0)
                {
                    cData = cData | repeats;
                    comAlphaData.Add((short)cData);
                    repeats = 0;
                }
            }
            compressedAlphaData = comAlphaData.ToArray();
            if (Debug.isDebugBuild)
                print(KuchoHelper.GetSceneName() +  " " + this.name + " ALPHA DATA COMPRIMIDA, BYTES=" + comAlphaData.Count * 2);
            compressedAlphaDataIsUpdated = true;
            #if UNITY_EDITOR
            if (!Application.isPlaying)
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            #endif
        }
    }
	public void CleanAlphaData(){
		if (AlphaData != null)
		{
			for (var i = 0; i < AlphaData.Length; i++){
				AlphaData[i] = 0;
			}
			NotifyChanges();
			print (this.name + " ALPHA DATA VACIADO, TotalPixels" + AlphaData.Length);
		}
		else
			print (this.name + " NO EXISTE ALPHA DATA (null)");

	}
	public void UncompressAlphaDataShort(){
		if (!CheckCompressedAlphaData(false))
			return;
		int alphaDataLength = alphaAndSpriteSizeOnCompressionTime.x * alphaAndSpriteSizeOnCompressionTime.y;
		if (alphaData == null || alphaData.Length != alphaDataLength)
			alphaData = new byte[alphaDataLength];
		int alphaDataIndex = 0;
		for (int i = 0; i < compressedAlphaData.Length; i++)
		{
			byte data;
			short cData = compressedAlphaData[i];
			int repeats = (int)(cData & 32767);
			int cDataInt = (int)cData;
			var bit15 = cDataInt & 32768;
			if (bit15 > 0) 
				data = 255;
			else
				data = 0;

			for (int ii = 0; ii < repeats; ii++)
			{
				alphaData[alphaDataIndex] = data;
				alphaDataIndex++;
			}
		}
		compressedAlphaDataIsUpdated = true; 
		AlphaDataToAlphaTex(alphaAndSpriteSizeOnCompressionTime); 
	}
	public void CheckAlphaData()
	{
		Profiler.BeginSample("CHECK ALPHA DATA");
		var solidPixels = 0;
		var clearPixels = 0;
		if (alphaData != null)
		{
			for (var i = 0; i < alphaData.Length; i++){
				if (alphaData[i] >= 127)
					solidPixels ++;
				else
					clearPixels ++;
			}
			print (this.name + " NAD PIX SOLID:" + solidPixels + " CLEAR:" + clearPixels + " TOT:" + alphaData.Length + " LENGTH-BYTES:" + alphaData.Length);
		}
		else
			print (this + " NO EXISTE ALPHA DATA (null)");

		CheckCompressedAlphaData(true);
		Profiler.EndSample();
	}

	public bool CheckCompressedAlphaData(bool countSolidAndTransparentPixels){
		if (compressedAlphaData == null)
		{
			Debug.LogError(this.name + " COMPRSSEDDATA NULO");
			return false;
		}
		if (compressedAlphaData.Length <= 0)
		{
			Debug.LogError(this.name + " COMPRSSEDDATA ES CERO O NEGATIVO");
			return false;
		}
		if (compressedAlphaDataIsUpdated == false)
		{
			Debug.LogError(this.name + " COMPRSSEDDATA NO ACTUALIZADO");
			return false;
		}
       
		if (countSolidAndTransparentPixels)
		{
			int solidPixCount = 0;
			int clearPixCount = 0;
			for (int i = 0; i < compressedAlphaData.Length; i++)
			{
				byte data;
				short cData = compressedAlphaData[i];
				int repeats = (int)(cData & 32767);
				int cDataInt = (int)cData;
				var bit15 = cDataInt & 32768;
				if (bit15 > 0)
					solidPixCount += repeats;
				else
					clearPixCount += repeats;
			}
			Debug.Log(name + " CAD PIX SOLID:" + solidPixCount + " CLEAR:" + clearPixCount + " TOT:" + (solidPixCount + clearPixCount)+ " LENGTH-BYTES:" + compressedAlphaData.Length * 2);
		}
		return true;

	}

}