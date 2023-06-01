using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

namespace Light2D
{
    /// <summary>
    /// Main script for lights. Should be attached to camera.
    /// Handles lighting operation like camera setup, shader setup, merging cameras output together, blurring and some others.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof (Camera))]
    public class LightingSystem : MonoBehaviour
    {
		public bool debug = false;
        public bool debug2 = false;
        public string _guiText;
        public Vector2 guiPos = new Vector2(500, 100);
        public RenderTexture lightSourcesTex; // KUCHO HACK el proceso de la textura de luces se hace en una variable local , yo la he hecho global para poder acceder a ella fura de la funcion processoverlay
        public int updateEach = 1; // KUCHO HACK
		public int updateCountDown = 0; // KUCHO HACK
        public bool frameJustRendered = false; // KUCHO HACK comprobandola se puede saber si ha ocurrido una update de lighting system y hacer cosas solo cuando se ha actualizado
        public int lastRenderFrame; // KUCHO HACK 

        /// <summary>
        /// Size of lighting pixel in Unity meters. Controls resoultion of lighting textures. 
        /// Smaller value - better quality, but lower performance.
        /// </summary>
        public float LightPixelSize = 0.05f;

        /// <summary>
        /// Needed for off screen lights to work correctly. Set that value to radius of largest light. 
        /// Used only when camera is in orthographic mode. Big values could cause a performance drop.
        /// </summary>
        public float LightCameraSizeAdd = 3;

        /// <summary>
        /// Needed for off screen lights to work correctly.
        /// Used only when camera is in perspective mode.
        /// </summary>
        public float LightCameraFovAdd = 30;

		/// <summary>
		/// hack al shader lightBase, ajusta la cantidad de sombra pryectada por todos los obstaculos
		/// </summary>
		public float GlobalObstacleMultiplier = 1; // KUCHO HACK

        /// <summary>
        /// Enable/disable ambient lights. Disable it to improve performance if you not using ambient light.
        /// </summary>
        public bool EnableAmbientLight = true;

        /// <summary>
        /// LightSourcesBlurMaterial is applied to light sources texture if enabled. Disable to improve performance.
        /// </summary>
        public bool BlurLightSources = true;

        /// <summary>
        /// AmbientLightBlurMaterial is applied to ambient light texture if enabled. Disable to improve performance.
        /// </summary>
        public bool BlurAmbientLight = true;

        /// <summary>
        /// If true RGBHalf RenderTexture type will be used for light processing.
        /// That could improve smoothness of lights. Will be turned off if device is not supports it.
        /// </summary>
        public bool HDR = true;

        /// <summary>
        /// If true light obstacles will be rendered in 2x resolution and then downsampled to 1x.
        /// </summary>
        public bool LightObstaclesAntialiasing = true;

        /// <summary>
        /// Set it to distance from camera to plane with light obstacles. Used only when camera in perspective mode.
        /// </summary>
        public float LightObstaclesDistance = 10;

        public Material AmbientLightComputeMaterial;
        public Material LightOverlayMaterial;
        public Material LightSourcesBlurMaterial;
        public Material AmbientLightBlurMaterial;
        public Camera LightCamera;
        public int LightSourcesLayer;
        public int AmbientLightLayer;
        public int LightObstaclesLayer;


		public RenderTexture _ambientEmissionTexture; // KUCHO HACK (public)
		public RenderTexture _ambientTexture; // KUCHO HACK (public)
		public RenderTexture _prevAmbientTexture; // KUCHO HACK (public)
		public RenderTexture _bluredLightTexture; // KUCHO HACK (public)
		public RenderTexture _obstaclesDownsampledTexture; // KUCHO HACK (public)
		public RenderTexture _lightSourcesTexture; // KUCHO HACK (public)
		public RenderTexture _obstaclesTexture; // KUCHO HACK (public)

        private Camera _camera;
		public ObstacleCameraPostPorcessor _obstaclesPostProcessor; // KUCHO HACK (public)
        public Point2 _lightTextureSize; // KUCHO HACK (public)
        public Vector2 _lightTextureSizeV2; // KUCHO HACK
        public Vector2 _halfLightTextureSizeV2; // KUCHO HACK
		public Vector3 _oldPos; // KUCHO HACK (public)
		public Vector3 _currPos; // KUCHO HACK (public)
        private RenderTextureFormat _texFormat;
		public int _aditionalAmbientLightCycles = 0; // KUCHO HACK (public)
		public RenderTexture _screenBlitTempTex; // KUCHO HACK (public)
        private static LightingSystem _instance;
#if LIGHT2D_2DTK
        private Camera _SwizCamera; // ERA SwizCamera , lo cambio por camara confiando en que ocupe lo mismo serializado , es una referencia a un componente asi que deberia
#endif

        private float LightPixelsPerUnityMeter
        {
            get { return 1/LightPixelSize; }
        }

        public static LightingSystem Instance
        {
            get { return _instance != null ? _instance : (_instance = FindObjectOfType<LightingSystem>()); }
        }

        public void OnSetResolution(ArcadeResolution arcadeRes){ // KUCHO HACK la llama especificamente SM.SetResolution por que lightingSystem no puede ver SM para apuntarse a su OnSetResolution
            //TODO usar arcadeRes para inicializar lightingSystem teniendo en cuenta la resolucion de pantalla y zoom en lugar de su forma habitual en Start()
            Initialize(null); // KUCHO HACK inicializacion para redefinir mapa de luz segun tamaño de la nueva resolucion y... el zoom no lo tiene en cuenta? una misma resolucion con dos niveles de zoom deberia tener tamaño de light texture diferentes por que una recoge mas mundo que otra
        }

        private void OnEnable()
        {
            _instance = this;
            _camera = GetComponent<Camera>();
        }

        public void Start(){ // la hice publica para que pudiera llamarla desde _Editor
            Initialize(null);
        }
        /// <summary>
        /// si mandamos arcadeRes se usa para definir tamaño de texturas, si no, tira de la camara
        /// </summary>
        /// <param name="arcadeRes">Arcade res.</param>
        public void Initialize(ArcadeResolution arcadeRes) // KUCHO HACK antes era Start
        {
			if (debug)
                Debug.Log( this + " DENTRO DE START"); // KUCHO HACK
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Shader.SetGlobalTexture("_ObstacleTex", Texture2D.whiteTexture);
                return;
            }
#endif

			Shader.SetGlobalFloat("_GlobalObstacleMultiplier" , GlobalObstacleMultiplier); // KUCHO HACK

            if (LightCamera == null)
            {
                Debug.LogError(
                    "Lighting Camera in LightingSystem is null. Please, select Lighting Camera camera for lighting to work.");
                enabled = false;
                return;
            }
            if (LightOverlayMaterial == null)
            {
                Debug.LogError(
                    "LightOverlayMaterial in LightingSystem is null. Please, select LightOverlayMaterial camera for lighting to work.");
                enabled = false;
                return;
            }

            _camera = GetComponent<Camera>();

            if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
                HDR = false;
            _texFormat = HDR ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32;

            var lightPixelsPerUnityMeter = LightPixelsPerUnityMeter;


            if (arcadeRes == null)
            {
                if (_camera.orthographic)
                {
                    var rawCamHeight = (_camera.orthographicSize + LightCameraSizeAdd) * 2f;
                    var rawCamWidth = (_camera.orthographicSize * _camera.aspect + LightCameraSizeAdd) * 2f;

                    _lightTextureSize = new Point2(
                        Mathf.RoundToInt(rawCamWidth * lightPixelsPerUnityMeter),
                        Mathf.RoundToInt(rawCamHeight * lightPixelsPerUnityMeter));
                }
                else
                {
                    var lightCamHalfFov = (_camera.fieldOfView + LightCameraFovAdd) * Mathf.Deg2Rad / 2f;
                    var lightCamSize = Mathf.Tan(lightCamHalfFov) * LightObstaclesDistance * 2;
                    LightCamera.orthographicSize = lightCamSize / 2f;

                    var gameCamHalfFov = _camera.fieldOfView * Mathf.Deg2Rad / 2f;
                    var gameCamSize = Mathf.Tan(gameCamHalfFov) * LightObstaclesDistance * 2;
                    _camera.orthographicSize = gameCamSize / 2f;

                    var texHeight = Mathf.RoundToInt(lightCamSize / LightPixelSize);
                    var texWidth = texHeight * _camera.aspect;
                    _lightTextureSize = Point2.Round(new Vector2(texWidth, texHeight));
                }
                _lightTextureSizeV2.x = (float)_lightTextureSize.x;
                _lightTextureSizeV2.y = (float)_lightTextureSize.y;
                _halfLightTextureSizeV2.x = _lightTextureSizeV2.x * 0.5f;
                _halfLightTextureSizeV2.y = _lightTextureSizeV2.y * 0.5f;
            }
            else // INICIALIZA USANDO ARCADE RES PARA DEFINIR TAMAÑO DE TEXTURA DE LUZ
            {
                
            }

            if (_lightTextureSize.x%2 != 0)
                _lightTextureSize.x++;
            if (_lightTextureSize.y%2 != 0)
                _lightTextureSize.y++;

            var obstacleTextureSize = _lightTextureSize*(LightObstaclesAntialiasing ? 2 : 1);

            _screenBlitTempTex = new RenderTexture(Screen.width, Screen.height, 0, _texFormat);

            LightCamera.orthographicSize = _lightTextureSize.y/(2f*lightPixelsPerUnityMeter);
            LightCamera.fieldOfView = _camera.fieldOfView + LightCameraFovAdd;
            LightCamera.orthographic = _camera.orthographic;

            _lightSourcesTexture = new RenderTexture(_lightTextureSize.x, _lightTextureSize.y,
                0, _texFormat);
            _obstaclesTexture = new RenderTexture(obstacleTextureSize.x, obstacleTextureSize.y,
                0, _texFormat);
            _ambientTexture = new RenderTexture(_lightTextureSize.x, _lightTextureSize.y,
                0, _texFormat);

            if (LightObstaclesAntialiasing)
            {
                _obstaclesDownsampledTexture = new RenderTexture(_lightTextureSize.x, _lightTextureSize.y,
                    0, _texFormat);
            }

            LightCamera.aspect = _lightTextureSize.x/(float) _lightTextureSize.y;

            _obstaclesPostProcessor = new ObstacleCameraPostPorcessor();

            if (debug)
			{
				Debug.Log( this + " MAPA DE LUZ CREADO, TAMAÑO=" + _lightTextureSize.x + "x" + _lightTextureSize.y); // KUCHO HACK
				Debug.Log( this + " CAM ORTHO SIZE=" + _camera.orthographicSize + "CAM SIZE ADD" + LightCameraSizeAdd + " CAM ASPECT" + _camera.aspect + " LIGHT PIX SIZE" + LightPixelSize); // KUCHO HACK
			}
		}
        public Delegates.Simple onRenderIsDone;
        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
#if UNITY_EDITOR
			if (!Application.isPlaying) // KUCHO HACK antes tambien tenia esto---> || Util.IsSceneViewFocused)
			{
				Shader.SetGlobalTexture("_ObstacleTex", Texture2D.whiteTexture);
				if (dest != null)
					dest.DiscardContents();
				Graphics.Blit(src, dest);
				return;
			}
#endif
			if (updateCountDown <= 0) // KUCHO HACK
			{ // KUCHO HACK
				if (debug2)
					print(this + " COUNT DOWN TERMINADO: PROCESANDO LIGHT SYSTEM EN FRAME " + Time.frameCount);
                if (gameObject.CompareTag("MainCamera"))
                    Profiler.BeginSample("MAIN LIGHT SYSTEM RENDER");
                else if (gameObject.CompareTag("Front"))
                    Profiler.BeginSample("COVER LIGHT SYSTEM RENDER");
                else if (gameObject.CompareTag("Light"))
                    Profiler.BeginSample("WORLD LIGHT SYSTEM RENDER");
                
				updateCountDown = updateEach; // KUCHO HACK
				UpdateCameraPosition();
				RenderObstacles();
				SetupShaders();
				RenderLightSources();
				RenderAmbientLight();
				RenderLightOverlay(src, dest);
				frameJustRendered = true; // KUCHO HACK
                lastRenderFrame = KuchoTime.frameCount;
                Profiler.EndSample();
//                print("LIGHT 2D RENDER DONE FRAME=" + KuchoTime.frameCount);
                if (onRenderIsDone != null)
                    onRenderIsDone();
			}
			else
			{
				
			}
			updateCountDown --; // KUCHO HACK
//			if (debug) print (this + "ON RENDER IMAGE TERMINADO EN FRAME " + Time.frameCount);
		}




        private void LateUpdate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && LightCamera != null)
            {
                _camera = GetComponent<Camera>();
                if (_camera != null)
                {
                    LightCamera.orthographic = _camera.orthographic;
                    if (_camera.orthographic)
                    {
                        LightCamera.orthographicSize = _camera.orthographicSize + LightCameraSizeAdd;
                    }
                    else
                    {
                        LightCamera.fieldOfView = _camera.fieldOfView + LightCameraFovAdd;
                    }
                }
            }
            if (!Application.isPlaying || Util.IsSceneViewFocused)
            {
                Shader.SetGlobalTexture("_ObstacleTex", Texture2D.whiteTexture);
                return;
            }
#endif
        }

        private void RenderObstacles()
        {
            LightCamera.enabled = false;

            LightCamera.targetTexture = _obstaclesTexture;
            LightCamera.cullingMask = 1 << LightObstaclesLayer;
            LightCamera.backgroundColor = new Color(1, 1, 1, 0);

            _obstaclesPostProcessor.DrawMesh(LightCamera, LightObstaclesAntialiasing ? 2 : 1);

            LightCamera.Render();
            LightCamera.targetTexture = null;
            LightCamera.cullingMask = 0;
            LightCamera.backgroundColor = new Color(0, 0, 0, 0);

            if (LightObstaclesAntialiasing && _obstaclesDownsampledTexture != null)
            {
                _obstaclesDownsampledTexture.DiscardContents();
                Graphics.Blit(_obstaclesTexture, _obstaclesDownsampledTexture);
            }
        }

        private void SetupShaders()
        {
            var lightPixelsPerUnityMeter = LightPixelsPerUnityMeter;

            if (HDR) Shader.EnableKeyword("HDR");
            else Shader.DisableKeyword("HDR");

            if (_camera.orthographic) Shader.DisableKeyword("PERSPECTIVE_CAMERA");
            else Shader.EnableKeyword("PERSPECTIVE_CAMERA");

            Shader.SetGlobalTexture("_ObstacleTex",
                _obstaclesDownsampledTexture != null ? _obstaclesDownsampledTexture : _obstaclesTexture);
            Shader.SetGlobalFloat("_PixelsPerBlock", lightPixelsPerUnityMeter);
        }

        private void RenderLightSources()
        {
            LightCamera.targetTexture = _lightSourcesTexture;
            LightCamera.cullingMask = 1 << LightSourcesLayer;
            LightCamera.backgroundColor = new Color(0, 0, 0, 0);
            LightCamera.Render();
            LightCamera.targetTexture = null;
            LightCamera.cullingMask = 0;

            if (BlurLightSources && LightSourcesBlurMaterial != null)
            {
                UnityEngine.Profiling.Profiler.BeginSample("LightingSystem.OnRenderImage Bluring Light Sources");

                if (_bluredLightTexture == null)
                {
                    _bluredLightTexture = new RenderTexture(_lightTextureSize.x, _lightTextureSize.y, 0,
                        _texFormat);
                }

                _bluredLightTexture.DiscardContents();
                LightSourcesBlurMaterial.mainTexture = _lightSourcesTexture;
                Graphics.Blit(null, _bluredLightTexture, LightSourcesBlurMaterial);

                UnityEngine.Profiling.Profiler.EndSample();
            }
        }

        private void RenderAmbientLight()
        {
            if (EnableAmbientLight && AmbientLightComputeMaterial != null)
            {
                UnityEngine.Profiling.Profiler.BeginSample("LightingSystem.OnRenderImage Ambient Light");

                if (_ambientTexture == null)
                {
                    _ambientTexture =
                        new RenderTexture(_lightTextureSize.x, _lightTextureSize.y, 0, _texFormat);
                }
                if (_prevAmbientTexture == null)
                {
                    _prevAmbientTexture =
                        new RenderTexture(_lightTextureSize.x, _lightTextureSize.y, 0, _texFormat);
                }
                if (_ambientEmissionTexture == null)
                {
                    _ambientEmissionTexture =
                        new RenderTexture(_lightTextureSize.x, _lightTextureSize.y, 0, _texFormat);
                }

                if (EnableAmbientLight)
                {
                    LightCamera.targetTexture = _ambientEmissionTexture;
                    LightCamera.cullingMask = 1 << AmbientLightLayer;
                    LightCamera.backgroundColor = new Color(0, 0, 0, 0);
                    LightCamera.Render();
                    LightCamera.targetTexture = null;
                    LightCamera.cullingMask = 0;
                }

                for (int i = 0; i < _aditionalAmbientLightCycles + 1; i++)
                {
                    var tmp = _prevAmbientTexture;
                    _prevAmbientTexture = _ambientTexture;
                    _ambientTexture = tmp;

                    var texSize = new Vector2(_ambientTexture.width, _ambientTexture.height);
                    var posShift = ((Vector2) (_currPos - _oldPos)/LightPixelSize).Div(texSize);
                    _oldPos = _currPos;

                    AmbientLightComputeMaterial.SetTexture("_LightSourcesTex", _ambientEmissionTexture);
                    AmbientLightComputeMaterial.SetTexture("_MainTex", _prevAmbientTexture);
                    AmbientLightComputeMaterial.SetVector("_Shift", posShift);

                    _ambientTexture.DiscardContents();
                    Graphics.Blit(null, _ambientTexture, AmbientLightComputeMaterial);

                    if (BlurAmbientLight && AmbientLightBlurMaterial != null)
                    {
                        UnityEngine.Profiling.Profiler.BeginSample("LightingSystem.OnRenderImage Bluring Ambient Light");

                        _prevAmbientTexture.DiscardContents();
                        AmbientLightBlurMaterial.mainTexture = _ambientTexture;
                        Graphics.Blit(null, _prevAmbientTexture, AmbientLightBlurMaterial);

                        var tmpblur = _prevAmbientTexture;
                        _prevAmbientTexture = _ambientTexture;
                        _ambientTexture = tmpblur;

                        UnityEngine.Profiling.Profiler.EndSample();
                    }
                }

                _aditionalAmbientLightCycles = 0;
                UnityEngine.Profiling.Profiler.EndSample();
            }
        }

        private void RenderLightOverlay(RenderTexture src, RenderTexture dest)
        {
            UnityEngine.Profiling.Profiler.BeginSample("LightingSystem.OnRenderImage Light Overlay");

            Vector2 lightTexelSize = new Vector2(1f/_lightTextureSize.x, 1f/_lightTextureSize.y);
            float lightPixelsPerUnityMeter = LightPixelsPerUnityMeter;
            Vector2 worldOffest = LightCamera.transform.position - _camera.transform.position;
            Vector2 offest = Vector2.Scale(lightTexelSize, -worldOffest*lightPixelsPerUnityMeter);

            lightSourcesTex = BlurLightSources && LightSourcesBlurMaterial != null // KUCHO HACK habia un var al comienzo de la linea haciendo esta variable local, yo la he hecho global
                ? _bluredLightTexture
                : _lightSourcesTexture;

            float xDiff = _camera.aspect/LightCamera.aspect;

            if (!_camera.orthographic)
            {
                var gameCamHalfFov = _camera.fieldOfView * Mathf.Deg2Rad / 2f;
                var gameCamSize = Mathf.Tan(gameCamHalfFov) * LightObstaclesDistance * 2;
                _camera.orthographicSize = gameCamSize / 2f;
            }

            float scaleY = _camera.orthographicSize/LightCamera.orthographicSize;
            var scale = new Vector2(scaleY*xDiff, scaleY);

            LightOverlayMaterial.SetTexture("_AmbientLightTex", EnableAmbientLight ? _ambientTexture : null);
            LightOverlayMaterial.SetTexture("_LightSourcesTex", lightSourcesTex);
			LightOverlayMaterial.SetTexture("_GameTex", src);
            LightOverlayMaterial.SetVector("_Offest", offest);
            LightOverlayMaterial.SetVector("_Scale", scale);

            if (_screenBlitTempTex == null || _screenBlitTempTex.width != src.width ||
                _screenBlitTempTex.height != src.height)
            {
                if (_screenBlitTempTex != null)
                    _screenBlitTempTex.Release();
                _screenBlitTempTex = new RenderTexture(src.width, src.height, 0, _texFormat);
            }

            _screenBlitTempTex.DiscardContents();
            Graphics.Blit(null, _screenBlitTempTex, LightOverlayMaterial);
            Graphics.Blit(_screenBlitTempTex, dest);

            UnityEngine.Profiling.Profiler.EndSample();

        }
        // KUCHO HACK S , pero al final no los estoy usando poruqe leer la textura grande costaria otro pico, ya que no se puede hacer GetPixel de una RenderTRexture, tendria que ponerla activ y lugo un ReadPixels osea traerla de la tarjeta grafica 
        //aunque no me valen al final, los dejo, pñueden ser utiles
        public float leftBorder; // KUCHO HACK
        public float rightBorder; // KUCHO HACK
        public float topBorder; // KUCHO HACK
        public float bottomBorder;// KUCHO HACK
        public Rect camRect;// KUCHO HACK


        private void UpdateCameraPosition()
        {
            var lightPixelsPerUnityMeter = LightPixelsPerUnityMeter;
            var mainPos = _camera.transform.position;
            var pos = new Vector3(
                Mathf.Round(mainPos.x*lightPixelsPerUnityMeter)/lightPixelsPerUnityMeter,
                Mathf.Round(mainPos.y*lightPixelsPerUnityMeter)/lightPixelsPerUnityMeter,
                mainPos.z);
            LightCamera.transform.position = pos;
            _currPos = pos;

            camRect = new Rect(pos, _lightTextureSizeV2 * LightPixelSize);// KUCHO HACK
            camRect.center = pos;// KUCHO HACK
            leftBorder = camRect.min.x;// KUCHO HACK
            rightBorder = camRect.max.x;// KUCHO HACK
            topBorder = camRect.max.y;// KUCHO HACK
            bottomBorder = camRect.min.y;// KUCHO HACK
        }

        public void LoopAmbientLight(int cycles)
        {
            _aditionalAmbientLightCycles += cycles;
        }
        // KUCHO HACK
//        public void GetLightPixel(Vector2 worldPos){
//            Vector2 distToCenter;
//            distToCenter.x = worldPos.x - _currPos.x;
//            distToCenter.y = worldPos.y - _currPos.y;
//            Vector2 lightTexPos;
//            lightTexPos.x = -1;
//            lightTexPos.y = -1;
//            if (KuchoHelper.Intersect2(worldPos, camRect))
//            {
//                lightTexPos.x = worldPos.x - leftBorder;
//                lightTexPos.y = worldPos.y - bottomBorder;
//                var pixel = _bluredLightTexture.GetPixel ..?¿ NO PUEDO HACER UN GETPIXEL DE UNA RENDER TEXTURE ;-(
//            }
//
//        }

	// KUCHO HACK
        #if UNITY_EDITOR
        void OnGUI_disabled(){
            if (debug)
            {
                float x = guiPos.x;
                float y = guiPos.y;
                GUI.Label(new Rect(x, y, 600, 20), System.String.Format(_guiText + "Size: {0}", _lightTextureSize));
                y += 20;
            }
         }
        #endif
	}
}