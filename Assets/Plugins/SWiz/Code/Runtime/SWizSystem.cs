using UnityEngine;
using System.Collections;

[System.Serializable]
public class SWizResourceTocEntry
{
	public string resourceGUID = "";
	public string assetName = "";
	public string assetGUID = "";
}

[System.Serializable]
public class SWizAssetPlatform
{
	public SWizAssetPlatform(string name, float scale) { this.name = name; this.scale = scale; }
	public string name = "";
	public float scale = 1.0f;
}


public class SWizSystem : ScriptableObject
{
	// prefix to apply to all guids to avoid matching errors
	public const string guidPrefix = "Swiz/Swiz_";
	public const string assetName = "SWiz/SWizSystem";
	public const string assetFileName = "SWizSystem.asset";

	// platforms
	[System.NonSerialized]
	public SWizAssetPlatform[] assetPlatforms = new SWizAssetPlatform[] {
		new SWizAssetPlatform("1x", 1.0f),
		new SWizAssetPlatform("2x", 2.0f),
		new SWizAssetPlatform("4x", 4.0f),
	};

	private SWizSystem() { }

	static SWizSystem _inst = null;
	public static SWizSystem inst
	{
		get 
		{
			if (_inst == null)
			{
				// Attempt to load the global instance and create one if it doesn't exist
				_inst = Resources.Load(assetName, typeof(SWizSystem)) as SWizSystem;
				if (_inst == null)
				{
					_inst = ScriptableObject.CreateInstance<SWizSystem>();
				}
				// We don't want to destroy this throughout the lifetime of the game
				DontDestroyOnLoad(_inst);
			}
			return _inst;
		}
	}

	// Variant which will not create the instance if it doesn't exist
	public static SWizSystem inst_NoCreate
	{
		get
		{
			if (_inst == null)
				_inst = Resources.Load(assetName, typeof(SWizSystem)) as SWizSystem;
			return _inst;
		}
	}

#region platforms

#if UNITY_EDITOR
	static bool currentPlatformInitialized = false;
#endif
	static string currentPlatform = ""; // Not serialized, this should be set up on wake
	public static string CurrentPlatform
	{
		get 
		{ 
#if UNITY_EDITOR
			if (!currentPlatformInitialized)
			{
				// Hack, don't have access to editor classes from here
				currentPlatform = UnityEditor.EditorPrefs.GetString("SWiz_platform", "");
				currentPlatformInitialized = true;
			}
#endif
			return currentPlatform; 
		} 
		set 
		{ 
			if (value != currentPlatform)
			{
#if UNITY_EDITOR
				currentPlatformInitialized = true;
#endif
				currentPlatform = value; 
			}
		}
	}


	public static bool OverrideBuildMaterial {
		get {
#if UNITY_EDITOR
			return System.IO.File.Exists("SWizOverrideBuildMaterial");
#else
			return false;
#endif
		}
	}

	public static SWizAssetPlatform GetAssetPlatform(string platform)
	{
		SWizSystem inst = SWizSystem.inst_NoCreate;
		if (inst == null) return null;

		for (int i = 0; i < inst.assetPlatforms.Length; ++i)
		{
			if (inst.assetPlatforms[i].name == platform)
				return inst.assetPlatforms[i];
		}
		return null;
	}

#endregion

#region Resources

	[SerializeField]
	SWizResourceTocEntry[] allResourceEntries = new SWizResourceTocEntry[0];

	#if UNITY_EDITOR
	public SWizResourceTocEntry[] Editor__Toc { get { return allResourceEntries; } set { allResourceEntries = value; } }
	#endif

	// Loads a resource by GUID
	// Return null if it doesn't exist
	T LoadResourceByGUIDImpl<T>(string guid) where T : UnityEngine.Object
	{
		SWizResource resource = Resources.Load(guidPrefix + guid, typeof(SWizResource)) as SWizResource;
		if (resource != null)
			return resource.objectReference as T;
		else
			return null;
	}

	// Loads a resource by name
	// Returns null if the name can't be found, or load fails for any other reason
	T LoadResourceByNameImpl<T>(string name) where T : UnityEngine.Object
	{
		// TODO: create and use a dictionary
		for (int i = 0; i < allResourceEntries.Length; ++i)
		{
			if (allResourceEntries[i] != null && allResourceEntries[i].assetName == name)
				return LoadResourceByGUIDImpl<T>(allResourceEntries[i].assetGUID);
		}
		return null;
	}

	public static T LoadResourceByGUID<T>(string guid) where T : UnityEngine.Object { return inst.LoadResourceByGUIDImpl<T>(guid); }
	public static T LoadResourceByName<T>(string guid) where T : UnityEngine.Object { return inst.LoadResourceByNameImpl<T>(guid); }

#endregion
}

