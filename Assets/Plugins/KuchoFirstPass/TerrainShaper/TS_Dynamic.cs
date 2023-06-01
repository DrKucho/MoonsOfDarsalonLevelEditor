using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;
public struct TS_Rect
{
	public int XMin;
	public int XMax;
	public int YMin;
	public int YMax;
}
public abstract class TS_Dynamic : MonoBehaviour
{
	public static List<TS_Dynamic> Destructibles = new List<TS_Dynamic>();
	
	public static List<TS_Dynamic> DestructiblesCopy = new List<TS_Dynamic>();
	
		public TS_SpriteCollider spriteCollider; // KUCHO HACK
	
	public Texture2D DensityTex;
	
	public bool Indestructible;
	
	public bool Binary;

	public bool notifyChangesAllowed = true; //  KUCHO HACK para desactivar la regeneracion de colliders temporalmente, en fases de pruebas o en generacion de mapas aleatorios hast que das con la forma deseada esto es util
	
    [ReadOnly2Attribute] public bool dirty; // KUCHO HACK read only y lo movi arriba para mejor visibilidad
    [ReadOnly2Attribute] public bool compressedAlphaDataIsUpdated;// KUCHO HACK

	public int SplitDepth;
	
	public int MinSplitPixels = 50;
	
	[SerializeField]
	private int originalSolidPixelCount;
	
	[SerializeField]
	private int originalWidth;
	
	[SerializeField]
	private int originalHeight;
	
	[SerializeField]
	private int alphaX;
	
	[SerializeField]
	private int alphaY;
	
	[SerializeField]
	private int alphaWidth;
	
	[SerializeField]
	private int alphaHeight;
	
	[SerializeField] [HideInInspector] public byte[] alphaData; // KUCHO HACK la hice publica por que para acceder a ella venia a traves de una proeprty pero es absurdo por que la property no hace nada mas que fijar valor y retornar valor, osea que para que la property?
    [SerializeField] public short[] compressedAlphaData;// KUCHO HACK
    [SerializeField] [HideInInspector] public Vector2Int alphaAndSpriteSizeOnCompressionTime;// KUCHO HACK
//	[System.NonSerialized]
	public Texture2D alphaTex;
	
	[SerializeField]
	private float alphaScaleX; // Used for density tex sampling
	
	[SerializeField]
	private float alphaScaleY; // Used for density tex sampling
	
	[SerializeField]
	private float alphaShiftX; // Used to fix odd to even downscaling
	
	[SerializeField]
	private float alphaShiftY; // Used to fix odd to even downscaling
	
	[SerializeField]
	private int cachedSolidPixelCount = -1;

	private TS_Rect notifyRect = new TS_Rect(); //KUCHO HACK
	
	[HideInInspector] public bool pendingChanges = false; // KUCHO HACK
	[HideInInspector] public bool textureApplyPending = false; // KUCHO HACK


	
	public Texture2D AlphaTex
	{
		get
		{
			DeserializeAlphaTex();
			
			return alphaTex;
		}
	}
	
	public int SolidPixelCount
	{
		get
		{
			if (cachedSolidPixelCount == -1)
			{
				cachedSolidPixelCount = 0;
				
				if (alphaData != null)
				{
					for (var i = alphaData.Length - 1; i >= 0; i--)
					{
						if (alphaData[i] >= 128)
						{
							cachedSolidPixelCount += 1;
						}
					}
				}
			}
			
			return cachedSolidPixelCount;
		}
	}
	
	public int OriginalSolidPixelCount
	{
		get
		{
			return originalSolidPixelCount;
		}
	}
	
	public int OriginalWidth
	{
		get
		{
			return originalWidth;
		}
	}
	
	public int OriginalHeight
	{
		get
		{
			return originalHeight;
		}
	}
	
	public int AlphaX
	{
		get
		{
			return alphaX;
		}
	}
	
	public int AlphaY
	{
		get
		{
			return alphaY;
		}
	}
	
	public int AlphaWidth
	{
		get
		{
			return alphaWidth;
		}
	}
	
	public int AlphaHeight
	{
		get
		{
			return alphaHeight;
		}
	}
	
	public float AlphaShiftX
	{
		get
		{
			return alphaShiftX;
		}
	}
	
	public float AlphaShiftY
	{
		get
		{
			return alphaShiftY;
		}
	}
	
	public byte[] AlphaData
	{
		// NOTE: Don't call this unless you know what you're doing!
		set
		{
			alphaData = value;
		}
		
		get
		{
			return alphaData;
		}
	}
	
	public float SolidPixelRatio
	{
		get
		{
			return TS_Helper.Divide(SolidPixelCount, originalSolidPixelCount);
		}
	}
	
	
	public void MarkAsDirty()
	{
#if UNITY_EDITOR
		if (dirty == false)
		{
			TS_Helper.SetDirty(this);
		}
#endif
		dirty                 = true;
		cachedSolidPixelCount = -1;
        compressedAlphaDataIsUpdated = false; // KUCHO HACK
	}
	
	[ContextMenu("Recalculate Original Solid Pixel Count")]
	public void RecalculateOriginalSolidPixelCount()
	{
		originalSolidPixelCount = SolidPixelCount;
	}
	
	[ContextMenu("Blur Alpha Tex")]
	public void BlurAlphaTex()
	{
		AlphaTexture.Load(alphaData, alphaWidth, alphaHeight);
		
		AlphaTexture.Blur();
		
		UpdateAlphaWith(AlphaTexture.Data, AlphaTexture.Width, AlphaTexture.Height);
	}
					

	public void ScaleAlphaTex(float xMult, float yMult) // KUCHO HACK toda esta funcion es nueva, esta basada en HalveAlphaTex()
	{
		AlphaTexture.Load(alphaData, alphaWidth, alphaHeight);

		var oldWidth  = alphaWidth;
		var oldHeight = alphaHeight;

		originalWidth  = Mathf.RoundToInt(originalWidth * xMult);
		originalHeight = Mathf.RoundToInt(originalHeight * yMult);
		alphaWidth     = Mathf.RoundToInt(alphaWidth * xMult);
		alphaHeight    = Mathf.RoundToInt(alphaHeight * yMult);
		alphaX         = Mathf.RoundToInt(alphaX * xMult);
		alphaY         = Mathf.RoundToInt(alphaY * yMult);
		alphaShiftX    = Mathf.RoundToInt(alphaShiftX / xMult);
		alphaShiftY    = Mathf.RoundToInt(alphaShiftY / yMult);

		var shiftX = oldWidth  - alphaWidth  / xMult;
		var shiftY = oldHeight - alphaHeight / yMult;

		AlphaTexture.Resize(alphaWidth, alphaHeight);

		UpdateAlphaWith(AlphaTexture.Data, AlphaTexture.Width, AlphaTexture.Height);

		alphaShiftX += shiftX;
		alphaShiftY += shiftY;
	}
	
	
	public void ReplaceAlphaWith(Texture2D newTexture)
	{
		if (newTexture != null)
		{
			ReplaceAlphaWith(TS_Helper.ExtractAlphaData(newTexture), newTexture.width, newTexture.height);
		}
		else
		{
			DestroyAlphaTex();
		}
	}
	
	public void ReplaceAlphaWith(byte[] newAlphaData, int newAlphaWidth, int newAlphaHeight)
	{
		if (newAlphaData != null && newAlphaWidth > 0 && newAlphaHeight > 0 && newAlphaData.Length >= newAlphaWidth * newAlphaHeight)
		{
			ReplaceAlphaData(newAlphaData, newAlphaWidth, newAlphaHeight);
			
			ResetAlphaData();
			
			originalWidth  = newAlphaWidth;
			originalHeight = newAlphaHeight;
			
			MarkAsDirty();
			NotifyChanges();
		}
		else
		{
			DestroyAlphaTex();
		}
	}
	public void ReplaceOrUpdateAlphaWith(byte[] newAlphaData, int newAlphaWidth, int newAlphaHeight) // KUCHO HACK esta funcion es totalmente nueva
	{
		if (alphaWidth == newAlphaWidth && alphaHeight == newAlphaHeight)
				UpdateAlphaWith(newAlphaData, newAlphaWidth, newAlphaHeight);
		else
				ReplaceAlphaWith(newAlphaData, newAlphaWidth, newAlphaHeight);
	}

	
	// This will preserve any subset settings
	public void UpdateAlphaWith(byte[] newAlphaData, int newAlphaWidth, int newAlphaHeight)
	{
		if (alphaWidth == newAlphaWidth && alphaHeight == newAlphaHeight)
		{
			ReplaceAlphaData(newAlphaData, newAlphaWidth, newAlphaHeight);
			
			MarkAsDirty();
			NotifyChanges();
		}
	}
	
	protected virtual void ResetAlphaData()
	{
		SplitDepth  = 0;
		alphaX      = 0;
		alphaY      = 0;
		alphaShiftX = 0;
		alphaShiftY = 0;
	}
	
	// Doesn't reallocate memory if the new alpha data is the same size

	public void DeserializeAlphaTex()// nombre raro pero creo que lo que hace es convertir alphadata en textura y aplicarla
	{
		if (alphaData != null && (alphaData.Length != alphaWidth * alphaHeight))// KUCHO HACK a veces me daba un error en editor mas abajo por alphaData.Length = 0  
			alphaData = null;
        
		if (dirty == true || alphaTex == null)
		{
			dirty = false;
			
			if (alphaData != null)
			{
				System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
				sw.Start();

				alphaTex = new Texture2D(alphaWidth, alphaHeight, TextureFormat.Alpha8, false);
				alphaTex.hideFlags = HideFlags.DontSave;
				alphaTex.wrapMode  = TextureWrapMode.Clamp;
				
				for (var y = 0; y < alphaHeight; y++)
				{
					for (var x = 0; x < alphaWidth; x++)
					{
						var color = default(Color);
						var alpha = alphaData[x + y * alphaWidth];
						
						color.a = AlphaTexture.ConvertAlpha(alpha);
						
						alphaTex.SetPixel(x, y, color);
					}
				}
				
				alphaTex.Apply();

				sw.Stop();
				if (Debug.isDebugBuild)
					print( "DeserializeAlphaTex ms= " + sw.ElapsedMilliseconds);
                
				alphaTex.name = gameObject.name + " Alpha";// KUCHO HACK
			}
			else
			{
				DestroyAlphaTex();
			}
		}
		
		if (alphaTex != null)
		{
			if (Binary == true)
			{
				if (alphaTex.filterMode != FilterMode.Point)
				{
					alphaTex.filterMode = FilterMode.Point;
				}
			}
			else
			{
				if (alphaTex.filterMode != FilterMode.Bilinear)
				{
					alphaTex.filterMode = FilterMode.Bilinear;
				}
			}
		}
	}


	public void AlphaDataToAlphaTex(Vector2Int newAlphaSize)
	{
		// KUCHO HACK esta funcion es totalmente nueva, en principio es equivalente a DeserializeAlphaTex pero intento que sea mas rapida

		alphaWidth = newAlphaSize.x;
		alphaHeight = newAlphaSize.y;

		if (alphaData != null)
		{
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			sw.Start();

			alphaTex = new Texture2D(alphaWidth, alphaHeight, TextureFormat.Alpha8, false);
			alphaTex.hideFlags = HideFlags.DontSave;
			alphaTex.wrapMode = TextureWrapMode.Clamp;


			for (var y = 0; y < alphaHeight; y++)
			{
				int lineYPos = y * alphaWidth;
				for (int x = 0; x < alphaWidth; x++)
				{
					TextureHelper.pixels32[x].a = alphaData[lineYPos + x];
				}

				alphaTex.SetPixels32(0, y, alphaWidth, 1, TextureHelper.pixels32);
			}

			alphaTex.Apply();

			sw.Stop();
		}
		else
		{
			DestroyAlphaTex();
		}

		if (alphaTex != null)
		{
			if (Binary == true)
			{
				if (alphaTex.filterMode != FilterMode.Point)
				{
					alphaTex.filterMode = FilterMode.Point;
				}
			}
			else
			{
				if (alphaTex.filterMode != FilterMode.Bilinear)
				{
					alphaTex.filterMode = FilterMode.Bilinear;
				}
			}

			alphaTex.name = gameObject.name + " Alpha"; // KUCHO HACK
		}
	}

	private void DestroyAlphaTex()
		{
			if (alphaTex != null)
			{
				TS_Helper.Destroy(alphaTex);
			
				alphaTex = null;
			}
		
			alphaData   = null;
			alphaWidth  = 0;
			alphaHeight = 0;
		}

	private void ReplaceAlphaData(byte[] newAlphaData, int newAlphaWidth, int newAlphaHeight)
	{
		if (alphaData != newAlphaData || alphaWidth != newAlphaWidth || alphaHeight != newAlphaHeight)
		{
			// KUCHO HACK en el caso de cambiar datos de alphaData y llamar a UpdateAlphaData, esta llamara luego aqui y es estupido hacer el proceso de copia cuando en realidad el fuente y el destino son la misma cosa
			var newAlphaTotal = newAlphaWidth * newAlphaHeight;

			if (alphaData == null || alphaData.Length != newAlphaTotal)
			{
				alphaData = new byte[newAlphaTotal];
			}

			int solidPixelsCount = 0; //KUCHO HACK borrame
			for (var i = 0; i < newAlphaTotal; i++)
			{
				alphaData[i] = newAlphaData[i];
				if (newAlphaData[i] > 0) //KUCHO HACK borrame
					solidPixelsCount++; //KUCHO HACK borrame
			}

			print("REPLACEALPHADATA: SOLID PIXELS= " + solidPixelsCount); //KUCHO HACK borrame

			alphaWidth = newAlphaWidth;
			alphaHeight = newAlphaHeight;
			alphaScaleX = TS_Helper.Reciprocal(originalWidth);
			alphaScaleY = TS_Helper.Reciprocal(originalHeight);
		}
	}

	public void NotifyChanges()
	{
		if (notifyChangesAllowed){ // KUCHO HACK para evitar que se regenere en cada paso de general el mapa aleatorio
			TS_Helper.BroadcastMessage(transform, "OnAlphaTexReplaced", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void NotifyChanges(int xMin, int xMax, int yMin, int yMax) 
	{
		if (xMin < notifyRect.XMin || pendingChanges == false) notifyRect.XMin = xMin;
		if (xMax > notifyRect.XMax || pendingChanges == false) notifyRect.XMax = xMax;
		if (yMin < notifyRect.YMin || pendingChanges == false) notifyRect.YMin = yMin;
		if (yMax > notifyRect.YMax || pendingChanges == false) notifyRect.YMax = yMax;
		pendingChanges = true;
		textureApplyPending = true;
	}
	public bool NotifyChangesAndRebuildColliders() // KUCHO HACK esta funcion es totalmente nueva
	{
		return false;
	}
}
