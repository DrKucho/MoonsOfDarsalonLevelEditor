using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Random = UnityEngine.Random;

public enum Liking {Love, Hate, DontCare}
public enum SetZ { Random, ByFiller}


public class GrowAnimator : MonoBehaviour {
	
    public enum DarknessMode
    {
        ScaleBased, zBackgroundBased, zBackgroundAndCovergroundBased
    }
	public bool debug = false;
	public float updateDelay = 1f;
	[Header (" ---- PROFUNDIDAD Y START POS")]
	[HideInInspector] Vector3 startPos; // guarda la posicion donde se encontro tierra ya que transform position puede estar desviada para offsetear el sprite
	[HideInInspector] private Vector3 sprPos; // la posicion del sprite , puede estar offseteada pero debe corresponder con la transform position
	private Vector3 bt_transformPosition;// copia de transform.position para poder ser accedida desde background threads
	private string bt_name; // para poder identificar a la hierba flotante y debuguearla con un break point condicional al nombre
	public Vector3 spriteOffsetMin = Constants.zero3;
	public Vector3 spriteOffsetMax = Constants.zero3;
	public SetZ setZ = SetZ.ByFiller;
	bool ZByFiller(){return setZ == SetZ.ByFiller;}
	bool ZRandom(){return setZ == SetZ.Random;}
	public Vector2 randomZ;
	public Vector2 noZRange;

    public DarknessMode darknessMode = DarknessMode.zBackgroundAndCovergroundBased;
    bool DarknessBackgroundMode(){ return darknessMode == DarknessMode.zBackgroundBased;}
    bool DarknessBackgroundAndCovergroundMode(){ return darknessMode == DarknessMode.zBackgroundAndCovergroundBased;}
    bool DarknessScaleMode(){return darknessMode == DarknessMode.ScaleBased;}
    bool DarknessIsNOT_ScaleMode(){return darknessMode != DarknessMode.ScaleBased;}

    [Range (0,1)] public float scaleToDarknessFactor = 0.5f;
    [Range(0,1)] public RangeFloat zToDarknessBackground ;
    [Range(0,1)] public RangeFloat zToDarknessCoverground ;
	public float zToHeightFactor = 0f;

	[Header (" ---- INCLINACION")]
	[ReadOnly2Attribute] public float groundInclination;
	public float groundInclinationToDarnessFactor;

	bool ShowLightStuff(){return lightBased != Liking.DontCare;}
	[Header ("---- LUZ")]
	public Liking lightBased = Liking.DontCare;
	[ReadOnly2Attribute]public bool needsYellowLight = true;
	[ReadOnly2Attribute][Range (0,1)] public float minLight = 0.01f;
	[ReadOnly2Attribute][Range (0,1)] public float maxLight = 0.6f;
	[ReadOnly2Attribute]public Vector2 readLightOffsetPos;
	[Header("se calculan")]
	WorldLightPixel lightPixel= new WorldLightPixel();
	float lightFactor = 1f; // la que hay en el lugar de nacimiento
	float inverseLightFactor = 0f; // se calcula si odiamos la luz , es el valor maximo - la luz que hay 
	float yellowFactor = 1f; // si es luz amarilla 

	bool LoveLight(){return lightBased == Liking.Love;}
	[ReadOnly2Attribute][Header ("---- CRECIMIENTO POR LUZ")]
						   public float lightToSpeedFactor = 1f;
	[ReadOnly2Attribute]public float maxDelay = 4f; // sistema de seguridad para que las animaciones no se queden en delay infinito por no haber luz
	[ReadOnly2Attribute] public float finalDelay; // se calcula, es el resultado de multiplicar stepDelay con la luz

	[Header (" ---- ESCALA INICIAL")]
	[Tooltip ("pilla solo el velor X para escalar abas X e Y")]
	[Range(0,1)] public float minScale = 0.1f;
	[Range(0,1)] public float maxScale = 1f;
	[Range (0, 4)] public float lightToScaleFactor = 1f; // al nacer se compruba la luz del terreno y se decide la escala
	[Range (0, 1)] public float groundInclinationToScaleFactor = 0f; // al nacer de modiofica la escala segun la inclinacionm de terreno dada
	[Range (0, 1)] public float rayLenghtToScaleFactor = 0f; // al nacer de modiofica la escala segun la longitud de rayo dada
	[Range (0,0.5f)] public float randomScaleRange = 0f;
	float thisTimeScale = 1f;
	public bool flipX_Randomly = true;
	public int shiftOnFlipX = 0;

	[Header (" ---- CRECIMIENTO")]
//	public float coroutineUpdateRate = 0.3f;
	public Vector2 _stepDelay= new Vector2(0.05f, 0.5f);
	public float stepDelayIncreaseFactor = 1f;
    [ReadOnly2Attribute] public float stepDelay= 0.2f;
	public int howManyScaleSteps = 0;
	[HideInInspector] public float scaleInc = 0.1f;
	public float alphaMidPoint = 0.5f;
	
	[Header (" ---- ANIMACION")]
	public bool grow = true;
	public bool randomClip = true;
	public string randomClipNamePrefix;
	public int[] clipIDs;
	public AnimationCurve animCompleteCurve;
	public float sizeMult; // fijado por filler/worldmap
	[ReadOnly2Attribute] public float animCompleteFactor; // donde se calcula el complete factor , pero si la frame event info = "C" se usara el valor del frame venet float
	int endFrame;
	
    [HideInInspector] public SWizSpriteAnimator anim;
	SWizSpriteAnimationClip clip;
	Point pos;
	int[] groundAlphaIndex = new int[4]; // indices de los pixeles de suelo en linea recta hacia abajo , se guardan al plantarse la planta, se usan para comprobar que sigue existiendo suelo
    [HideInInspector] public Decorative decorative;


    [HideInInspector] public Item item;
	bool initialized = false;

    public void InitialiseInEditor(){
        if (!anim)
            anim = GetComponent<SWizSpriteAnimator>();
        if (!item)
            item = GetComponent<Item>();
        if (!decorative)
            decorative = GetComponent<Decorative>();
        instantGrowCount = 0;
        var list = anim.GetClipIDsByNamePrefix(randomClipNamePrefix);
        if (list == null)
        {
	        Debug.LogError(this + " NO HE ENCONTRADO CLIPS CON ESTE PREFIJO " + randomClipNamePrefix);
        }
        else
        {
	        clipIDs = list.ToArray();
        }
    }

    private Vector3 originalSpritescale;

    private void Awake()
    {
	    originalSpritescale = anim.Sprite.scale;
	    anim.enabled = false; //lo apago para que no salte su update y consuma llamadas a su Update() inutilmente, ya que yo voy a reproducir los frames uno a uno
		if (item && item.isOriginal && Game.levelFrame <= 1)
            gameObject.SetActive(false); // asi apago el grass original que si no, al aparecer activado intenta crecer fuera del mapa
	}


	void OnEnable()
	{
		bt_transformPosition = transform.position;
		bt_name = name;
	}
	
	public void OnSpritePlaneChanged(float plane){
        switch (darknessMode)
        {
            case (DarknessMode.ScaleBased):
                SetStartScale(1 - plane);
                relativeSpritePlanesZHasBeenSet = false; // importante antes de SetDarkness
                SetDarkness(true);
                break;
        }
    }

	public void StartGrowing(bool instantGrow){
		StartGrowing(0f, 1f, 0f, instantGrow);
	}
	public void StartGrowing(float rayLength, float offsetZ, bool instantGrow){
		StartGrowing(groundInclination, rayLength, offsetZ, instantGrow);
	}
    [HideInInspector] public bool relativeSpritePlanesZHasBeenSet;
	float relativeSpritePlanesZ;
    public static int instantGrowCount = 0;
    
	public void StartGrowing(float _groundInclination, float rayLength,float _relativeSpritePlanesZ, bool instantGrow){
		currentScale = 0;
	    frameIndex = 0;
	    groundInclination = _groundInclination;
		// ANIMACION
		if (randomClip)
		{
			int i = Random.Range(0, clipIDs.Length - 1);
			clip = anim.GetClipById(clipIDs[i]);
		}
		else
			clip = anim.DefaultClip;

		if (clip.wrapMode == SWizSpriteAnimationClip.WrapMode.RandomFrame)
		{
			int fi = Random.Range(0, clip.frames.Length);
			anim.Sprite.SetSprite(clip.frames[fi].spriteId);
		}
		else
		{
			Debug.LogError(this + " TRYING TO START GROWING OF SOMETHING THAT IS NOT A RANDOM FRAME ANIMATION, MOD LEVEL EDITOR DOESN'T SUPPORT THIS");
			return;
		}

		//anim.PlayFromFrame(clip, 0);
		//anim.Pause();
		oneFrameOrEquivalent = AnimationUsesOneFrameOnly();		
		// PROFUNDIDAD Y START POS
		var z = 0f;
		var closeToMin = 0f;
		var closeToMax = 0f;

		startPos = transform.position;
	    sprPos = startPos;
		float offsetY = 0;

		switch (setZ)
		{
			case (SetZ.Random):
				relativeSpritePlanesZHasBeenSet = false;
				z = Random.Range(randomZ.x, randomZ.y);
				closeToMin = z - noZRange.x;
				closeToMax = z - noZRange.y;
				if (noZRange.x < z && z < noZRange.y)
				{
					if (Mathf.Abs(closeToMin) > Mathf.Abs(closeToMax))
						z = noZRange.x;
					else
						z = noZRange.y;
				}
				sprPos = new Vector3(transform.position.x, transform.position.y, z);
				offsetY = z * zToHeightFactor;
				break;
			case (SetZ.ByFiller):
				relativeSpritePlanesZHasBeenSet = true;
				relativeSpritePlanesZ = _relativeSpritePlanesZ;
				offsetY = relativeSpritePlanesZ * zToHeightFactor;
				break;
		}

		if (relativeSpritePlanesZHasBeenSet)
			offsetY = relativeSpritePlanesZ * zToHeightFactor;
		else
			offsetY = z * zToHeightFactor;

		sprPos.y += offsetY;
		Vector3 spriteOffset;
		spriteOffset.x = Random.Range(spriteOffsetMin.x, spriteOffsetMax.x);
		spriteOffset.y = Random.Range(spriteOffsetMin.y, spriteOffsetMax.y);
		spriteOffset.z = Random.Range(spriteOffsetMin.z, spriteOffsetMax.z);
		sprPos += spriteOffset;
		transform.position = sprPos; // ???
		bt_transformPosition = sprPos;

		if (clip.frames[0].eventInfo == "C")
			animCompleteFactor = clip.frames[0].eventFloat;
		else
			animCompleteFactor = animCompleteCurve.Evaluate(Random.Range(0f, 1f));

		animCompleteFactor *= sizeMult;
		if (animCompleteFactor > 1)
			animCompleteFactor = 1;
		SetEndFrame();
		
        SetStartScale(rayLength);

        SetDarkness(false);
        
		Flip();
		
        // CRECIMIENTO 
        if (grow)
        {
            if (instantGrow)
            {
                InstantGrow();
            }
            else
            {
                stepDelay = Random.Range(_stepDelay.x, _stepDelay.y);

            }
        }
        // PREPARAR MYUPDATE PARA QUE PUEDA COMPROBAR SI SIGUE HABIENDO TIERRA DEBAJO
		pos.x = (int)startPos.x;
		pos.y = (int)startPos.y;
		int posYminus1 = pos.y - 1;
        for (int i = 0; i < groundAlphaIndex.Length; i++)
        {
            int newY = posYminus1 - i;
            if (newY < 0)
                newY = 0;
            groundAlphaIndex[i] = PixelTools.AlphaDataGetIndex(pos.x, newY);
        }

	}

        
	
	public void InitAndSetDarkness(){
		SetDarkness(true);
		anim.Sprite.enabled = false;
		anim.Sprite.enabled = true;
	}
    public void SetDarkness(){
        SetDarkness(true);
    }
    public void SetDarkness(bool whiteReset){
        if (whiteReset)
            anim.Sprite.color = Constants.solidWhite;
		float darkness = 0;
        if (darknessMode == DarknessMode.ScaleBased)
        {
            darkness = (1 - anim.Sprite.scale.x) * scaleToDarknessFactor;
        }
        else
        {
            if (relativeSpritePlanesZHasBeenSet)
            {
                if (relativeSpritePlanesZ >= 0)
                    darkness = Mathf.Lerp(zToDarknessBackground.min, zToDarknessBackground.max, relativeSpritePlanesZ);
                else
                    darkness = Mathf.Lerp(zToDarknessCoverground.min, zToDarknessCoverground.max, Mathf.Abs(relativeSpritePlanesZ));	
            }
            else
            {

                Vector3 sprPos = transform.position;
                float invLerp = 0;
                if (!WorldMap.spritePlanes)
                {
                    var map = FindObjectOfType<WorldMap>();
                    if (map)
                        map.PublicOnValidate();
                }
                if (sprPos.z >= WorldMap.spritePlanes.farBackground.min)
                {
                    invLerp = Mathf.InverseLerp(WorldMap.spritePlanes.farBackground.min, WorldMap.spritePlanes.farBackground.max, sprPos.z);
                    darkness = Mathf.Lerp(zToDarknessBackground.min, zToDarknessBackground.max, invLerp);
                }
                else if (sprPos.z < WorldMap.spritePlanes.coverground.max)
                {
                    invLerp = Mathf.InverseLerp(WorldMap.spritePlanes.coverground.min, WorldMap.spritePlanes.coverground.max, sprPos.z);
                    darkness = Mathf.Lerp(zToDarknessCoverground.min, zToDarknessCoverground.max, invLerp);
                }
            }
		darkness -= groundInclination * groundInclinationToDarnessFactor;
		//darkness = 1f - sprPos.z * zToDarknessBackground - groundInclination * groundInclinationToDarnessFactor; //old
        }
		var lightness = 1 - darkness;
        Color newColor = anim.Sprite.color * new Color(lightness, lightness, lightness, 1);
        newColor.a = anim.Sprite.color.a;
        anim.Sprite.color = newColor;
	}
    public void SetColor(Color c){
        anim.Sprite.color = c;
    }
    public void SetStartScale(float scale){
        // ESCALA / TAMAÑO
        // escalo segun parametro dado
        thisTimeScale = 1 - ( rayLenghtToScaleFactor * (1 - scale));

        // escalo segun luz
        if (lightBased != Liking.DontCare)
        {
            GetLightFactor();
            inverseLightFactor = maxLight - lightFactor; // la pongo aqui para que se calcule siempre y poder ver los resultados mintras debugeo
            if (lightBased == Liking.Love) thisTimeScale = lightFactor * lightToScaleFactor;
            if (lightBased == Liking.Hate)
            {
                inverseLightFactor = maxLight - lightFactor;
                thisTimeScale = inverseLightFactor * lightToScaleFactor;
            }
        }
        // escalo segun inclinacion de suelo
        thisTimeScale -= groundInclinationToScaleFactor * groundInclination;

        // escalo segun random
        var randomScaleAdd = 0f; // para sumarlo

        randomScaleAdd = Random.Range(-randomScaleRange, randomScaleRange);

        thisTimeScale += randomScaleAdd;

        // si se pasa del limite la trunco al maximo/minimo
        thisTimeScale = Mathf.Clamp(thisTimeScale, minScale, maxScale);

        anim.Sprite.scale = new Vector3(thisTimeScale, thisTimeScale, anim.Sprite.scale.z);
    }
	public void GetLightFactor(){

	}

	float currentScale = 0;
	int frameIndex;


	private bool oneFrameOrEquivalent;

	public float GetCompletionValue()
	{
		if (oneFrameOrEquivalent)
		{
			return currentScale;
		}
		else
		{
			return frameIndex/anim.CurrentClip.frames.Length;
		}
	}

	public void InstantGrow(){
        PlayFrameAndPause(endFrame);

        if (oneFrameOrEquivalent)
	        anim.Sprite.scale = new Vector3(animCompleteFactor, animCompleteFactor, animCompleteFactor);
        else
	        anim.Sprite.scale = Constants.one3;
        instantGrowCount++;
    }

	bool AnimationUsesOneFrameOnly()
	{
		if (anim.CurrentClip.wrapMode == SWizSpriteAnimationClip.WrapMode.RandomFrame)
			return true;
		else if (anim.CurrentClip.wrapMode == SWizSpriteAnimationClip.WrapMode.Once)
		{
			if (anim.CurrentClip.frames.Length > 1)
				return  true;
			else
			return false;
		}
		else
		{
			if (Constants.isDebugBuild)
				Debug.LogError(this + " ANIMACION CON WRAP MODE NO VALIDO:" + anim.CurrentClip.wrapMode);
			return true;// por si acaso
		}
	}

	void SetEndFrame()
    {
	    if (clip.wrapMode != SWizSpriteAnimationClip.WrapMode.Once) // randomframe y otros 
		    endFrame = 0;
	    else
	    {
		    float delta = (clip.frames.Length - 1) * animCompleteFactor;
		    endFrame = Mathf.RoundToInt(delta);
		    //KuchoHelper.ClampInsideArrayLength(ref endFrame, clip.frames.Length); ???
	    }
    }
    void PlayFrameAndPause(int frameToPlay){

	    switch (anim.CurrentClip.wrapMode)
	    {
		    case (SWizSpriteAnimationClip.WrapMode.RandomFrame):
			    //anim.Play();
			    break;
		    case (SWizSpriteAnimationClip.WrapMode.Once):
			    //anim.PlayFromFrame(frameToPlay);
			    //if (clip.frames[anim.CurrentFrame].eventInfo == "R" && clip.frames[anim.CurrentFrame].eventFloat > 0)
			    //{
				//    stepDelay = clip.frames[anim.CurrentFrame].eventFloat;
			    //}
			    //anim.Pause();
			    break;
		    default:
			    Debug.LogError(this + " INTENTO DE USAR ANIM CLIP CON WRAP MORE " + anim.CurrentClip.wrapMode + " PERO SOLO PUEDO RANDOM FRAME Y ONCE");
			    break;
	    }

    }
	public void Flip(){
		if (flipX_Randomly)
		{
			float c = Random.Range(100f,-100f);
			c = Mathf.Sign(c);
			transform.localScale = new Vector3(c, transform.localScale.y, transform.localScale.z);
			if (c == -1f && shiftOnFlipX !=0) TransformHelper.SetPosX(transform, transform.position.x + shiftOnFlipX);
		}
	}
}
