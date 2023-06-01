using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]

public class SWizCamera : MonoBehaviour 
{
	static int CURRENT_VERSION = 1;
	public int version = 0;

	[SerializeField] private SWizCameraSettings cameraSettings = new SWizCameraSettings();

	public SWizCameraSettings CameraSettings {
		get {
			return cameraSettings;
		}
	}

	public SWizCameraResolutionOverride[] resolutionOverride = new SWizCameraResolutionOverride[1] {
		SWizCameraResolutionOverride.DefaultOverride
	};

	public SWizCameraResolutionOverride CurrentResolutionOverride {
		get {
			return null;
		}
	}

	public SWizCamera InheritConfig {
		get { return inheritSettings; }
		set {
			if (inheritSettings != value) 
			{
				inheritSettings = value;
				_settingsRoot = null;
			}
		}
	}

	[SerializeField]
	private SWizCamera inheritSettings = null;
	
	public int nativeResolutionWidth = 960;

	public int nativeResolutionHeight = 640;
	
	[SerializeField]
	private Camera _unityCamera;
    public Camera UnityCamera { // KUCHO HACK la hice publica
        get
        {
	        return _unityCamera;
		}
	}


	static SWizCamera inst;

	public  static SWizCamera Instance {
		get {
			return inst;
		}
	}
	private static List<SWizCamera> allCameras = new List<SWizCamera>();
	
    public Rect NativeScreenExtents { get { return _nativeScreenExtents; } }

	public bool viewportClippingEnabled = false;

	public Vector4 viewportRegion = new Vector4(0, 0, 100, 100);

	public Vector2 TargetResolution { get { return _targetResolution; } }
	Vector2 _targetResolution = Vector2.zero;

	public Vector2 NativeResolution { get { return new Vector2(nativeResolutionWidth, nativeResolutionHeight); } }
	
	public float ZoomFactor {
		get { return zoomFactor; }
		set { zoomFactor = Mathf.Max(0.01f, value); }
	}

	[System.Obsolete]
	public float zoomScale {
		get { return 1.0f / Mathf.Max(0.001f, zoomFactor); }
		set { ZoomFactor = 1.0f / Mathf.Max(0.001f, value); }
	}

	[SerializeField] float zoomFactor = 1.0f;


    public bool forceResolutionInEditor = false;

    public Vector2 forceResolution = new Vector2(960, 640);

#if UNITY_EDITOR
    public bool useGameWindowResolutionMasterSw = false; 

    public bool useGameWindowResolutionInEditor = false; 

    public Vector2 gameWindowResolution = new Vector2(960, 640); 
#endif
	
    public Rect _screenExtents; 
    public Rect _nativeScreenExtents;
    public Rect unitRect = new Rect(0, 0, 1, 1); 
    
	SWizCamera _settingsRoot;
	public SWizCamera SettingsRoot {
		get { 
			if (_settingsRoot == null) {
				_settingsRoot = (inheritSettings == null || inheritSettings == this) ? this : inheritSettings.SettingsRoot;	
			}
			return _settingsRoot;
		}
	}
#if UNITY_EDITOR
	static bool Editor__getGameViewSizeError = false;
	public static bool Editor__gameViewReflectionError = false;
#endif



}
