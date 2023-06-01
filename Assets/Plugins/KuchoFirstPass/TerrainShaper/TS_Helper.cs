using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

public static class TS_Helper
{
	public static int MeshVertexLimit = 65000;
	
	public const string ComponentMenuPrefix = "Destructible 2D/D2D ";

	public static void Swap<T>(ref T a, ref T b)
	{
		var c = b;
		
		b = a;
		a = c;
	}
	
	private static Object stealthSetObject;
	
	private static HideFlags stealthSetFlags;
	
	public static void BeginStealthSet(Object o)
	{
		if (o != null)
		{
			stealthSetObject = o;
			stealthSetFlags  = o.hideFlags;
			
			o.hideFlags = HideFlags.DontSave;
		}
	}
	
	public static void EndStealthSet()
	{
		if (stealthSetObject != null)
		{
			stealthSetObject.hideFlags = stealthSetFlags;
		}
	}
	
	public static T Destroy<T>(T o)
		where T : Object
	{
		if (o != null)
		{
#if UNITY_EDITOR
			if (Application.isPlaying == false)
			{
				Object.DestroyImmediate(o, true); return null;
			}
#endif
			Object.Destroy(o);
		}
		
		return null;
	}



	static int nameIndex = 0; // KUCHO HACK

	public static T Clone<T>(T o, bool keepName = true)
		where T : Object
	{
		if (o != null)
		{
			var c = (T)Object.Instantiate(o);
			
			if (c != null && keepName == true) c.name = o.name;

			nameIndex++; // KUCHO HACK
			c.name += nameIndex.ToString();// KUCHO HACK
			
			return c;
		}
		
		return null;
	}

	public static void SendMessage(Component component, string messageName, object o, SendMessageOptions smo)
	{
		if (component != null)
		{
#if UNITY_EDITOR
			if (Application.isPlaying == false)
			{
				var cs = component.GetComponents<Component>();
				
				foreach (var c in cs)
				{
					if (c != null) // KUCHO HACK podria ser null si el componente se ha perdido (Missing Script)
					{
						var method = c.GetType().GetMethod(messageName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
						
						if (method != null)
						{
							if (o != null)
							{
								method.Invoke(c, new object[] { method });
							}
							else
							{
								method.Invoke(c, null);
							}
						}
					}
				}
				
				return;
			}
#endif
			component.SendMessage(messageName, o, smo);
		}
	}
	
	public static void BroadcastMessage(Transform transform, string messageName, SendMessageOptions smo)
	{
		BroadcastMessage(transform, messageName, null, smo);
	}
	
	public static void BroadcastMessage(Transform transform, string messageName, object o, SendMessageOptions smo)
	{
		if (transform != null)
		{
			SendMessage(transform, messageName, o, smo);
			
			for (var i = transform.childCount - 1; i >= 0; i--)
			{
				BroadcastMessage(transform.GetChild(i), messageName, o, smo);
			}
		}
	}
	
	public static float Divide(float a, float b)
	{
		return Zero(b) == false ? a / b : 0.0f;
	}

	public static float Reciprocal(float v)
	{
		return Zero(v) == false ? 1.0f / v : 0.0f;
	}
	
	
	public static bool Zero(float v)
	{
		return v == 0.0f;
		//return Mathf.Approximately(v, 0.0f);
	}

	public static byte[] ExtractAlphaData(Texture2D texture)
	{
		if (texture != null)
		{
#if UNITY_EDITOR
			TS_Helper.MakeTextureReadable(texture);
#endif
			var width  = texture.width;
			var height = texture.height;
			var total  = width * height;
			var data   = new byte[total];
			
			for (var y = 0; y < height; y++)
			{
				for (var x = 0; x < width; x++)
				{
					data[x + y * width] = (byte)(texture.GetPixel(x, y).a * 255.0f);
				}
			}
			
			return data;
		}
		
		return null;
	}
	
	public static bool ExtractAlphaData(Sprite sprite, ref byte[] data, ref int width, ref int height)
	{
		if (sprite != null && sprite.texture != null)
		{
#if UNITY_EDITOR
			TS_Helper.MakeTextureReadable(sprite.texture);
#endif
			var rect         = sprite.textureRect;
			var sourceWidth  = sprite.texture.width;
			var sourcePixels = sprite.texture.GetPixels32();
			var sourceOffset = sourceWidth * Mathf.CeilToInt(rect.y) + Mathf.CeilToInt(rect.x);
			var targetOffset = 0;
			
			width  = Mathf.FloorToInt(rect.width);
			height = Mathf.FloorToInt(rect.height);
			
			var total = width * height;
			
			if (data == null || data.Length != total)
			{
				data = new byte[width * height];
			}
			
			for (var y = 0; y < height; y++)
			{
				for (var x = 0; x < width; x++)
				{
					data[targetOffset + x] = sourcePixels[sourceOffset + x].a;
				}
				
				sourceOffset += sourceWidth;
				targetOffset += width;
			}
			
			return true;
		}
		
		return false;
	}
	
	#if UNITY_EDITOR
	public static bool BaseRectSet;
	
	public static Rect BaseRect;
	
	private static GUIStyle none;
	
	private static GUIStyle error;
	
	private static GUIStyle noError;
	
	private static string undoName;
	
	public static GUIStyle None
	{
		get
		{
			if (none == null)
			{
				none = new GUIStyle();
			}
			
			return none;
		}
	}
	
	
	
	public static void SetDirty<T>(T t)
		where T : Object
	{
		if (t != null)
		{
			EditorUtility.SetDirty(t);
		}
	}

	public static T GetAssetImporter<T>(Object asset)
		where T : AssetImporter
	{
		return GetAssetImporter<T>((AssetDatabase.GetAssetPath(asset)));
	}
	
	public static T GetAssetImporter<T>(string path)
		where T : AssetImporter
	{
		return (T)AssetImporter.GetAtPath(path);
	}

	public static void ReimportAsset(string path)
	{
		AssetDatabase.ImportAsset(path);
	}
	
	public static void MakeTextureReadable(Texture2D texture)
	{
		if (texture != null)
		{
			var importer = TS_Helper.GetAssetImporter<UnityEditor.TextureImporter>(texture);
			
			if (importer != null && importer.isReadable == false)
			{
				importer.isReadable = true;
				
				TS_Helper.ReimportAsset(importer.assetPath);
			}
		}
	}
	#endif
}