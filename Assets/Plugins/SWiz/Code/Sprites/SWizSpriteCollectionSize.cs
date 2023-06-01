using UnityEngine;
using System.Collections;

/// <summary>
/// Sprite collection size.
/// Supports different methods of specifying size.
/// </summary>
[System.Serializable]
public class SWizSpriteCollectionSize
{
	/// <summary>
	/// When you are using an ortho camera. Use this to create sprites which will be pixel perfect
	/// automatically at the resolution and orthoSize given.
	/// </summary>
	public static SWizSpriteCollectionSize Explicit(float orthoSize, float targetHeight) { 
		return ForResolution(orthoSize, targetHeight, targetHeight);
	}

	/// <summary>
	/// When you are using an ortho camera. Use this to create sprites which will be pixel perfect
	/// automatically at the resolution and orthoSize given.
	/// </summary>
	public static SWizSpriteCollectionSize PixelsPerMeter(float pixelsPerMeter) { 
		SWizSpriteCollectionSize s = new SWizSpriteCollectionSize();
		s.type = Type.PixelsPerMeter;
		s.pixelsPerMeter = pixelsPerMeter;
		return s;
	}

	/// <summary>
	/// When you are using an ortho camera. Use this to create sprites which will be pixel perfect
	/// automatically at the resolution and orthoSize given.
	/// </summary>
	public static SWizSpriteCollectionSize ForResolution(float orthoSize, float width, float height) { 
		SWizSpriteCollectionSize s = new SWizSpriteCollectionSize();
		s.type = Type.Explicit;
		s.orthoSize = orthoSize;
		s.width = width;
		s.height = height;
		return s;
	}

	/// <summary>
	/// Use when you need the sprite to be pixel perfect on a SWizCamera.
	/// </summary>
	public static SWizSpriteCollectionSize ForSWizCamera() { 
		SWizSpriteCollectionSize s = new SWizSpriteCollectionSize();
		s.type = Type.PixelsPerMeter;
		s.pixelsPerMeter = 1;
		return s;
	}

	/// <summary>
	/// Use when you need the sprite to be pixel perfect on a specific SWizCamera.
	/// </summary>
	public static SWizSpriteCollectionSize ForSWizCamera( SWizCamera camera ) { 
		SWizSpriteCollectionSize s = new SWizSpriteCollectionSize();
		SWizCameraSettings cameraSettings = camera.SettingsRoot.CameraSettings;
		if (cameraSettings.projection == SWizCameraSettings.ProjectionType.Orthographic) {
			switch (cameraSettings.orthographicType) {
				case SWizCameraSettings.OrthographicType.PixelsPerMeter:
					s.type = Type.PixelsPerMeter;
					s.pixelsPerMeter = cameraSettings.orthographicPixelsPerMeter;
					break;
				case SWizCameraSettings.OrthographicType.OrthographicSize:
					s.type = Type.Explicit;
					s.height = camera.nativeResolutionHeight;
					s.orthoSize = cameraSettings.orthographicSize;
					break;
			}
		}
		else if (cameraSettings.projection == SWizCameraSettings.ProjectionType.Perspective) {
			s.type = Type.PixelsPerMeter;
			s.pixelsPerMeter = 100; // some random value
		}
		return s;
	}

	/// <summary>
	/// A default setting
	/// </summary>
	public static SWizSpriteCollectionSize Default() {
		return PixelsPerMeter(100);
	}

	// Copy from legacy settings
	public void CopyFromLegacy( bool useSWizCamera, float orthoSize, float targetHeight ) {
		if (useSWizCamera) {
			this.type = Type.PixelsPerMeter;
			this.pixelsPerMeter = 1;
		}
		else {
			this.type = Type.Explicit;
			this.height = targetHeight;
			this.orthoSize = orthoSize;
		}
	}

	public void CopyFrom(SWizSpriteCollectionSize source) {
		this.type = source.type;
		this.width = source.width;
		this.height = source.height;
		this.orthoSize = source.orthoSize;
		this.pixelsPerMeter = source.pixelsPerMeter;
	}
	
	public enum Type {
		Explicit,
		PixelsPerMeter
	};

	// What to do with the values below?
	public Type type = Type.PixelsPerMeter;

	// resolution, used to derive above values
	public float orthoSize = 10;
	public float pixelsPerMeter = 100;
	public float width = 960;
	public float height = 640;

	public float OrthoSize {
		get {
			switch (type) {
				case Type.Explicit: return orthoSize;
				case Type.PixelsPerMeter: return 0.5f;
			}
			return orthoSize;
		}
	}

	public float TargetHeight {
		get {
			switch (type) {
				case Type.Explicit: return height;
				case Type.PixelsPerMeter: return pixelsPerMeter;
			}
			return height;
		}
	}
}
