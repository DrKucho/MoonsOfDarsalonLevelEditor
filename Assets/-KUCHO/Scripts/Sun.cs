using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Light2D;



[ExecuteInEditMode]
public class Sun : MonoBehaviour {

	[Header("---SHAPE")]
	[Range (1, 20)]
	public int points = 5;
	int previousPoints;
	[Range (1, 2000)]
	public float pointSeparation = 500f;
	float previousPointSeparation;
	[Range (8000, 100000)]
	public float pointSize = 8000f;
	float previousPointSize;

	[Header("---LIGHT2D MAT")]
    [Range (-0.03f, 0.03f)] public float _penetrationOffset = 0.0f;
    //[Range (-5f, 5f)] public float distToCenterToObstacleMultFactor = -0.03f;
    //public float _ObstacleMultAdd = 250f;
    [ReadOnly2Attribute] public float distToCamMag;
    public AnimationCurve distCurve;
    [ReadOnly2Attribute] public float curveCurrentValue;
    public float ObstacleMultRatio = 1;
    [ReadOnly2Attribute] public float newObstMult;

    int _Offset;
	int _ObstacleMul;

	[Header("---LIGHT")]
	[Range (0,1)] public float intensity = 0.5f;
	[Range (-1,1)] public float intensityHeightFactor = 0.1f;
	[Range (-1,1)]public float intensityHeightOffset = 0f;
	[Range (0,1)] public float maxIntensity;
	float eachSunIntensity;
	float intensityMultiplier;
	[ReadOnly2Attribute] public float intensityUnClamped;
	[ReadOnly2Attribute] public float finalIntensity;
	float previousIntensity;
	Color previousColor;
	public float colorUpdateRate;
	public float terrainShadowDissapearRateAtNight = 3f;
	[Range (0,1)] public float skyManagerColorRatio;
	[Range (0,1)] public float desaturationFactor = 1f;
	public Gradient color;
	public Color actualColor;
	float colorTime;
	
//	public Item[] sunPoint;
	public LightSprite[] sunPointLightSprite;
//	public ItemStore sunStore;
//	public string _sunStore = "";
	[HideInInspector]
	public ElipseOrbit elipse;
	[HideInInspector] public float reverseAngleToCam;
    Transform myTransform;
    public static Sun instance;
    
	public void Awake ()
	{
		instance = this;
		SetPreviousValues();
		_Offset = Shader.PropertyToID("_Offset");
		_ObstacleMul = Shader.PropertyToID("_ObstacleMul");
        myTransform = transform;
	}

	#if UNITY_EDITOR
	public void Update(){ //  print (this + " UPDATE ");


			if (intensity != previousIntensity) SetIntensity();
			if (pointSize != previousPointSize) SetPointSize();
        if (actualColor != previousColor || myTransform.position != posAtLastSetColor) SetColor();
			SetPreviousValues();

		if (!elipse)
            elipse = GetComponent<ElipseOrbit>();
	}
	#endif




	public void SetPreviousValues(){
		previousIntensity = intensity;
		previousPointSeparation = pointSeparation;
		previousPoints = points;
		previousPointSize = pointSize;
		previousColor = actualColor;
	}

    
	public void InitialiseInEditor(){
//        DestroySun();
        sunPointLightSprite = GetComponentsInChildren<LightSprite>(true);

        if (sunPointLightSprite.Length > 0)
        {
            for (int i = 0; i < sunPointLightSprite.Length; i++)
            {
                if (sunPointLightSprite[i])
                {
                    sunPointLightSprite[i].transform.parent = transform;
                    sunPointLightSprite[i].transform.localPosition = Constants.zero3;
                    sunPointLightSprite[i].gameObject.SetActive(true);
                }
            }
            DistributePoints();
            SetIntensity();
            SetPointSize();
        }
        else
        {
            Debug.LogError(" NO HE ENCONTRADO NINGUN LIGHT SPRITE PARA PUNTOS DEL SOL");
        }



	}

	public void DistributePoints(){
        if (sunPointLightSprite.Length > 1)
		{
			// el primer punto
            sunPointLightSprite[0].transform.localPosition = new Vector3(pointSeparation * Mathf.Cos(0), pointSeparation * Mathf.Sin(0), 0f);
            for(int i = 1; i < sunPointLightSprite.Length; i ++)
			{
                sunPointLightSprite[i].transform.localPosition = new Vector3(pointSeparation * Mathf.Cos(i * 2 * Mathf.PI / points), pointSeparation * Mathf.Sin(i*2 * Mathf.PI / points), 0);
			}
		}
        else sunPointLightSprite[0].transform.localPosition = Constants.zero3;
	}
	public void SetIntensity(){
		CalculateIntensity();
		for(int i = 0; i < sunPointLightSprite.Length; i ++)
		{
			sunPointLightSprite[i].Color.a = eachSunIntensity;
		}
	}

	[HideInInspector] public float extraIntensity;// dada por scripts internos (WorldMap)
	public void CalculateIntensity()
	{
		if (elipse)
			intensityMultiplier = (elipse.heightFactor + intensityHeightOffset) * intensityHeightFactor + 1f;
		else if (Game.sun)
			intensityMultiplier = Game.sun.elipse.heightFactor * intensityHeightFactor + 1f;
		intensityUnClamped = intensity * intensityMultiplier;
		finalIntensity = Mathf.Clamp(intensityUnClamped, -1000, maxIntensity);

//		eachSunIntensity = (intensity / sunPointLightSprite.Length) * intensityMultiplier;
		eachSunIntensity = (finalIntensity / sunPointLightSprite.Length) + extraIntensity;
	}

	public void SetPointSize(){
        Vector3 newScale;
        newScale.x = pointSize;
        newScale.y = pointSize;
        newScale.z = 1;
        for(int i = 0; i < sunPointLightSprite.Length; i ++)
		{
            sunPointLightSprite[i].transform.localScale = newScale;
		}
	}

    Vector3 posAtLastSetColor;
    [HideInInspector] [System.NonSerialized] public bool forceColor;
	public void SetColor(){
        if (!Application.isPlaying)
            return;
		CalculateIntensity();
		if (!forceColor)
		{
			float time = (elipse.sideFactor + 1f) / 2f;
			if (skyManagerColorRatio > 0) // hay que meterle algo del color del cielo al sol
			{
				// por que antes usaba el color mas a la izqueirda del degradado en lugar del color del momento del dia? --> color.Evaluate(0)
				actualColor = Color.Lerp(color.Evaluate(time), SkyManager.instance.realSaturatedSkyColor, skyManagerColorRatio);
			}
			else
			{
//			colorTime = (elipse.preWidth + transform.localPosition.x) / (elipse.preWidth * 2); // formula antigua dejo de calcular el color correcto no se por que , de todas formas la nueva es mejor, menos calculos
				actualColor = color.Evaluate(time);
			}
		}

		actualColor.a = finalIntensity; // no afecta a la luz pero otros scripts pueden usar el alpha del color que equivale a la intensidad, asi solo pillan un dato el color y fuera


			HSLColor hslSunColor = new HSLColor();
			hslSunColor = actualColor; // conversion de RGB a HSL
			hslSunColor.s *= desaturationFactor;
			actualColor = hslSunColor; // no necesito castear en plan (Color) ?

			for (int i = 0; i < sunPointLightSprite.Length; i++)
			{
				sunPointLightSprite[i].Color = actualColor;
				sunPointLightSprite[i].Color.a = eachSunIntensity;
			}
			

		posAtLastSetColor = myTransform.position;
	}

	void OnDestroy()
	{
		Debug.Log("DESTRUYENDO SOL");
	}

}
