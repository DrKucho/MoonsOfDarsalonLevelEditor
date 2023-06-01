using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Serialization;


public class DayNightCycleColorProcessor : MonoBehaviour
{

    public enum ColorToMix { DontGetAny = 0, GameSun = 100, GameSky = 200, GameCosmos = 300, GameAtmos = 400, VisibleSkyColor = 350, WorldMapSkyColor = 500, WorldMapNightColor = 600,
        WorldMapParallaxColorAdd = 700, WorldMapParallaxColorMult  = 800} //TODO implementar en moons of darsalon MOD
    public enum ColorMixMode { Add, Mult }

    [System.Serializable]
    public class ColorReader
    {
        public enum CSVenum { Contrast, Saturation, Value };
        public enum HSVenum { Hue, Saturation, Value };
        public override string ToString()
        {
            if (!enabled)
                return "Disabled";
            else
                return sourceColor.ToString();
        }
        [System.Serializable]
        public class SatCompressor{
            public enum Input {Luma, Saturation, Alpha}
            public Input input = Input.Saturation;
            [ReadOnly2Attribute] public float inputValue;
            [Range(0, 1)] public float threshold = 0.5f;
            [ReadOnly2Attribute] public float excess;
            [Range(0, 1)] public float ratio = 0.5f;
            [ReadOnly2Attribute] public float compressed;
            [ReadOnly2Attribute] public float output;
            [Range(0, 1)] public float gain = 1;
            public Color result;
            public HSLColor resultHSL;

            public void Process(Color color)
            {
                resultHSL = HSLColor.FromRGBA(color);
                switch (input)
                {
                    case (Input.Luma):
                        inputValue = ColorHelper.Luma(color);
                        break;
                    case (Input.Saturation):
                        inputValue = resultHSL.s;
                        break;
                    case (Input.Alpha):
                        inputValue = resultHSL.a;
                        break;
                }
                excess = inputValue - threshold;
                float underExcess = inputValue - excess;

                if (excess > 0)
                {
                    compressed = excess * ratio;
                    output = underExcess + compressed;
                }
                else
                {
                    compressed = 0;
                    output = resultHSL.s;
                }

                resultHSL.s = output;
                resultHSL.s *= gain;

                result = resultHSL;
            }
        }
        [System.Serializable]
        public class CaracolChange{
            public bool enabled = true;
            public enum Input { Luma, CurveResult, Saturation, LumaMinus0_5};
            public CSVenum modify = CSVenum.Contrast;
            public Input input = Input.Luma;
            [Range(-3, 3)] public float change;
            public bool useCompressor = false;
            bool UseCompressor() { return useCompressor; }
            [ReadOnly2Attribute] public float preCompressorResult;
            [Range(0, 1)] public float compThreshold = 1f;
            [ReadOnly2Attribute] public float excess;
            [Range(0, 1)] public float compRatio = 1f;
            [ReadOnly2Attribute] public float compressed;
            [ReadOnly2Attribute] public float result;

            public override string ToString()
            {
                return modify.ToString();
            }

            public void Do(ColorReader cr){
                if (enabled)
                {
                    float i = 0;
                    switch (input)
                    {
                        case (Input.Luma):
                            i = cr.luma;
                            break;
                        case (Input.LumaMinus0_5):
                            i = cr.luma - 0.5f;
                            break;
                        case (Input.CurveResult):
                            i = cr.curveResult;
                            break;
                        case (Input.Saturation):
                            i = cr.hslColor.s;
                            break;
                    }
                    if (useCompressor)
                    {
                        preCompressorResult = i * change;
                        excess = preCompressorResult - compThreshold;

                        float modAmount;
                        if (change > 0)
                        {
                            if (excess > 0)
                            {
                                compressed = excess * compRatio;
                                modAmount = compThreshold + compressed;
                            }
                            else
                            {
                                compressed = 0;
                                modAmount = 0;
                            }
                        }
                        else
                        {
                            if (excess < 0)
                            {
                                compressed = excess * compRatio;
                                modAmount = compThreshold + compressed;
                            }
                            else
                            {
                                compressed = 0;
                                modAmount = 0;
                            }
                        }
                        result = preCompressorResult + modAmount;
                    }
                    else
                    {
                        result = i * change;
                    }
                }
                else
                {
                    result = 0;
                }
            }
        }
        public bool enabled = true;
        public ColorToMix sourceColor;
        public Color _sourceColor;
        public SatCompressor satComp;
        //public Color desaturatedSourceColor;
        public ColorMixMode mixMode = ColorMixMode.Add;
        public AnimationCurve curve;
        [ReadOnly2Attribute] public float curveResult;
        public float curveMult = 1;
        public float curveAdd;
        [ReadOnly2Attribute] public float finalCurveResult;
        public Color finalColor;
        [ReadOnly2Attribute] public float luma;
        public float addColorFactor;
        public float multColorFactor;
        [Range(-0.005f, 0.005f)] public float zAddFactor;
        [Range(-0.005f, 0.005f)] public float zMulFactor;

        [HideInInspector] public HSLColor hslColor;

        [Header("ORIGINAL COLOR MOD")]
        [HideInInspector] public bool useNewCaracols = true; // todo borra los antiguos caracols
        [HideInInspector] public CaracolChange contrastChange;
        [HideInInspector] public CaracolChange saturationChange;
        [HideInInspector] public CaracolChange valueChange;
        public CaracolChange[] conSatValMod;

        public void Process(float input)
        {    
            if (Game.sun)
            {
                int i = (int)sourceColor;
                if (i < 100)
                {
                    switch (i) // para antes de haber metido los numeros 100, 200, 300, y asi poder insertar
                    {
                        case (0): //(ColorToMix.DontGetAny):
                            break;
                        case (1): //(ColorToMix.GameSun):
                            _sourceColor = Game.sun.actualColor;
                            break;
                        case (2): //(ColorToMix.GameSky):
                            _sourceColor = Game.skyManager.skyColor;
                            break;
                        case (3): //(ColorToMix.GameCosmos):
                            _sourceColor = Game.skyManager.cosmosColor;
                            break;
                        case (4): //(ColorToMix.GameAtmos):
                            _sourceColor = Game.skyManager.atmosColor;
                            break;
                        case (5): //(ColorToMix.WorldMapSkyColor):
                            _sourceColor = WorldMap.instance.skyColor;
                            break;
                        case (6): //(ColorToMix.WorldMapNightColor):
                            _sourceColor = WorldMap.instance.nightColor;
                            break;
                        case (7): //(ColorToMix.WorldMapParallaxColorAdd):
                            _sourceColor = WorldMap.instance.parallaxColorAdd;
                            break;
                        case (8): //(ColorToMix.WorldMapParallaxColorMult):
                            _sourceColor = WorldMap.instance.parallaxColorMult;
                            break;
                        case (9): //(ColorToMix.GameCosmosPlusAtmos):
                            _sourceColor = Game.skyManager.atmosColor + Game.skyManager.cosmosColor;
                            break;
                    }
                }
                else
                {
                    switch (sourceColor)
                    {
                        case (ColorToMix.DontGetAny):
                            break;
                        case (ColorToMix.GameSun):
                            _sourceColor = Game.sun.actualColor;
                            break;
                        case (ColorToMix.GameSky):
                            _sourceColor = Game.skyManager.skyColor; // esto es cosmosColor + atmosColor, se mezcla en sky manager
                            break;
                        case (ColorToMix.GameCosmos):
                            _sourceColor = Game.skyManager.cosmosColor;
                            break;
                        case (ColorToMix.GameAtmos):
                            _sourceColor = Game.skyManager.atmosColor;
                            break;
                        case (ColorToMix.WorldMapSkyColor):
                             _sourceColor = WorldMap.instance.skyColor;
                             break;
                        case (ColorToMix.WorldMapNightColor):
                             _sourceColor = WorldMap.instance.nightColor;
                             break;
                        case (ColorToMix.WorldMapParallaxColorAdd):
                            _sourceColor = WorldMap.instance.parallaxColorAdd;
                            break;
                        case (ColorToMix.WorldMapParallaxColorMult):
                            _sourceColor = WorldMap.instance.parallaxColorMult;
                            break;
                        case (ColorToMix.VisibleSkyColor):
                            _sourceColor = Game.skyManager.atmosColor + Game.skyManager.cosmosColor;
                            break;
                    }
                }
            }

            satComp.Process(_sourceColor);
            hslColor = satComp.resultHSL;
            //desaturatedSourceColor = satComp.result; //todo borrame

            //hslColor = _sourceColor; // conversion de RGB a HSL
            //hslColor.s *= saturation;

            curveResult = curve.Evaluate(input);
            finalCurveResult = curveResult * curveMult + curveAdd;
            if (finalCurveResult < 0)
                finalCurveResult = 0;
            
            //hslColor.a = finalCurveResult;

            //desaturatedSourceColor = hslColor; // cast, por que lo hago? por que quiero modificar pickedupcolor? todo desaturated color esta mostrando los resultados de la curva, deberia llamarse curveResultColor? //todo borrame
            finalColor = satComp.result * finalCurveResult;
            finalColor.a = finalCurveResult;
            hslColor = finalColor;
            //hslColor.l *= finalResult;
            //finalColor = hslColor;


            luma = ColorHelper.Luma(finalColor);
            foreach(CaracolChange c in conSatValMod)
            {
                c.Do(this);
            }
            //contrastChange.Do(this);
            //saturationChange.Do(this);
            //valueChange.Do(this);
        }
        [HideInInspector] public Color zColor;
        public Color GetAddColorForZ(float posZ)
        {
            float zChange = posZ * zAddFactor;
            Color result = finalColor + finalColor * zChange;
            return result;
        }
        public Color GetMulColorForZ(float posZ)
        {
            float zChange = posZ * zMulFactor;
            Color result = finalColor + finalColor * zChange;
            return result;
            
            HSLColor hslColorZ = hslColor;
            hslColorZ.l += zChange;
            zColor = hslColorZ.ToRGBA();
            return zColor;
        }    }
    [System.Serializable]
    public class NightDarkener
    {
        public AnimationCurve curve;
        public float curveMult = 1;
        public float curveAdd;
        [Range(-0.005f, 0.005f)] public float darkToZ;
        [ReadOnly2Attribute] public float darkValue;


        public void Process(float input)
        {
            darkValue = curve.Evaluate(input) * curveMult + curveAdd;
        }
        [HideInInspector] public float zDarkValue;
        public float GetColorForZ(float posZ)
        {
            float zChange = posZ * darkToZ;
            zDarkValue = darkValue + darkValue * zChange;
            return zDarkValue;
        }
    }
    [System.Serializable]
    public class MaterialAndTransform{
        public Material mat;
        public Transform trans;
        public Color addColor;
        public Color multColor;

        public MaterialAndTransform(Material mat, Transform trans)
        {
            this.mat = mat;
            this.trans = trans;
        }
    }

    public float updateRate = 0.0333333333f; // 30fps
    [ReadOnly2Attribute] public float height;

    [Header("---")]
    public bool heightAbsoluteMode = false;
    public bool heightNegativeIsZero = false;

    private Color sunColor; // solo para verlo en inspector ...? sigo usando esto?



    [Header("SAT CONT VAL ----")]
    [Tooltip("Set To First Plane Z")]
    public float firstPlaneZ; // fijar a mano el primer plano parallax
    [Range(-4, 4)] public float saturationAdd;
    [Range(0, 0.005f)] public float saturationZ_Factor;
    [Range(-4, 4)] public float contrastAdd;
    [Range(0, 0.005f)] public float contrastZ_Factor;
    [Range(-4, 4)] public float valueAdd;
    [Range(-0.005f, 0.005f)] public float valueZ_Factor;

    public bool fixedColormode;

    bool FixedColorMode() { return fixedColormode; }
    bool ColorReadersMode() { return !fixedColormode; }

     public ColorReader[] colorReaders;
    public Color fixedColorAdd;
    [Range(-0.005f, 0.005f)] public float zAddFactor;
    public Color fixedColorMult;
    [Range(-0.005f, 0.005f)] public float zMulFactor;
    
     public bool useNight;
     public NightDarkener night;
    [Header("Doesn't show Z depth Lightness/Darkening")]
     public bool overrideFinalColor = false;
     bool OverrideFinalColor() { return overrideFinalColor; }
    bool DontOverrideFinalColor() { return !overrideFinalColor; }
    bool ShowFinalColor() { return !fixedColormode & !overrideFinalColor; }
    [ReadOnly2Attribute] public Color finalColor; // el que colos que se va a aplicar a los sprites
    public Color colorOverride;



    Renderer[] renderers;
    public List<MaterialAndTransform> materialsAndTransforms;

    void Awake()
    {
        Game.onFindLevelSpecificStuff += OnFindLevelSpecificStuff;
    }
    void OnDestroy()
    {
        Game.onFindLevelSpecificStuff -= OnFindLevelSpecificStuff;
    }
#if UNITY_EDITOR
    void OnValidate()
    { 
        if (isActiveAndEnabled && !Application.isPlaying && !UnityEditor.BuildPipeline.isBuildingPlayer) // estamos parados , y no no haciendo una buiild
        {
            MyUpdate();
        }
    }
#endif
    void OnFindLevelSpecificStuff()
    {
        if (Game.sun != null)
        {
            if (this == null)
            {
                print("SI LLEGA AQUI ES POR QUE EL DELEGATE/EVENT HA LLAMADO AQUI PERO LA INSTANCIA DE ESTE SCRIPT YA NO EXISTE, POR ESO AÑADI UN ELIMINAR DEL DELEGATE/EVENT EN ONDESTROY");
            }
        }
        else
        {
            this.enabled = false;
        }
    }

    
    public void InitialiseInEditor()
    {


        renderers = GetComponentsInChildren<Renderer>();
        materialsAndTransforms = new List<MaterialAndTransform>();
        foreach (Renderer r in renderers)
        {
            bool found = false;
            foreach (MaterialAndTransform mt in materialsAndTransforms)
            {
                if (mt.mat == r.sharedMaterial)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                materialsAndTransforms.Add(new MaterialAndTransform(r.sharedMaterial, r.transform));
            }
        }
        //Upgrade a nuevo enum com valores de 100 en 100
        foreach (ColorReader cr in colorReaders)
        {
            int i = (int) cr.sourceColor;
            if (i < 100)
            {
                switch (i) // para antes de haber metido los numeros 100, 200, 300, y asi poder insertar
                {
                    case (0): //(ColorToMix.DontGetAny):
                        cr.sourceColor = ColorToMix.DontGetAny;
                        break;
                    case (1): //(ColorToMix.GameSun):
                        cr.sourceColor = ColorToMix.GameSun;
                        break;
                    case (2): //(ColorToMix.GameSky):
                        cr.sourceColor = ColorToMix.GameSky;
                        break;
                    case (3): //(ColorToMix.GameCosmos):
                        cr.sourceColor = ColorToMix.GameCosmos;
                        break;
                    case (4): //(ColorToMix.GameAtmos):
                        cr.sourceColor = ColorToMix.GameAtmos;
                        break;
                    case (5): //(ColorToMix.WorldMapSkyColor):
                        cr.sourceColor = ColorToMix.WorldMapSkyColor;
                        break;
                    case (6): //(ColorToMix.WorldMapNightColor):
                        cr.sourceColor = ColorToMix.WorldMapNightColor;
                        break;
                    case (7): //(ColorToMix.WorldMapParallaxColorAdd):
                        cr.sourceColor = ColorToMix.WorldMapParallaxColorAdd;
                        break;
                    case (8): //(ColorToMix.WorldMapParallaxColorMult):
                        cr.sourceColor = ColorToMix.WorldMapParallaxColorMult;
                        break;
                }
            }
        }
    }

    Sun sun;
    public void MyUpdate()
    {
        sun = Game.sun;
        if (!sun)
        {
            sun = FindObjectOfType<Sun>();
            print("WARNING BUSCANDO SOL");
        }
        if (sun)
        {
            height = sun.elipse.heightFactor;

            finalColor = Constants.transparentBlack;
            foreach (ColorReader cr in colorReaders)
            {
                if (cr.enabled)
                {
                    cr.Process(height);
                    if (cr.mixMode == ColorMixMode.Add)
                    {
                        //finalColor.r += cr.pickedUpColor.r * cr.pickedUpColor.a;
                        //finalColor.g += cr.pickedUpColor.g * cr.pickedUpColor.a;
                        //finalColor.b += cr.pickedUpColor.b * cr.pickedUpColor.a;

                        finalColor.r += cr.finalColor.r;
                        finalColor.g += cr.finalColor.g;
                        finalColor.b += cr.finalColor.b;
                    }
                    else
                    {
                        //finalColor.r *= cr.pickedUpColor.r * cr.pickedUpColor.a;
                        //finalColor.g *= cr.pickedUpColor.g * cr.pickedUpColor.a;
                        //finalColor.b *= cr.pickedUpColor.b * cr.pickedUpColor.a;

                        finalColor.r *= cr.finalColor.r;
                        finalColor.g *= cr.finalColor.g;
                        finalColor.b *= cr.finalColor.b;
                    }
                }
            }
            night.Process(height);
            if (overrideFinalColor)
                finalColor = colorOverride;

            finalColor.a = 1;

            for (int n = 0; n < materialsAndTransforms.Count; n++)
            {
                //materialsAndTransforms[n].mat.color = finalColor;
                ApplyChangesToMaterials(materialsAndTransforms[n]);// materialsAndTransforms[n].trans.position.z, materialsAndTransforms[n].mat);
            }
        }
    }
    public float[] darkNightArray = new float[3];
    int di = 0;
    void ApplyChangesToMaterials(MaterialAndTransform mt)
    {
        float amount = 1;
        float posZ = mt.trans.position.z;
        float f = firstPlaneZ - posZ;
        Color finalAdditiveColorForZ = Constants.transparentBlack;
        Color finalMultiplicativeColorForZ = Constants.transparentBlack;
        finalAdditiveColorForZ.a = 1;
        float contrastChange = 0;
        float saturationChange = 0;
        float valueChange = 0;
        if (fixedColormode)
        {
            finalAdditiveColorForZ = GetAddColorForZ(fixedColorAdd, f); // additive color for z
            finalMultiplicativeColorForZ = GetMulColorForZ(fixedColorMult, f); // additive color for z
        }
        else
        {
            foreach (ColorReader cr in colorReaders)
            {

                if (cr.enabled)
                {
                    if (overrideFinalColor)
                    {
                        cr.satComp.result = colorOverride;
                        cr.hslColor = colorOverride;
                    }

                    Color czAdd = cr.GetAddColorForZ(f); // additive color for z
                    Color czMul = cr.GetMulColorForZ(f); // additive color for z

                    float addFactor = cr.finalColor.a * cr.addColorFactor;
                    float multFactor = cr.finalColor.a * cr.multColorFactor;
                    if (cr.mixMode == ColorMixMode.Add)
                    {
                        finalAdditiveColorForZ.r += czAdd.r * addFactor;
                        finalAdditiveColorForZ.g += czAdd.g * addFactor;
                        finalAdditiveColorForZ.b += czAdd.b * addFactor;

                        finalMultiplicativeColorForZ.r += czMul.r * multFactor;
                        finalMultiplicativeColorForZ.g += czMul.g * multFactor;
                        finalMultiplicativeColorForZ.b += czMul.b * multFactor;
                    }
                    else
                    {
                        finalAdditiveColorForZ.r *= czAdd.r * addFactor;
                        finalAdditiveColorForZ.g *= czAdd.g * addFactor;
                        finalAdditiveColorForZ.b *= czAdd.b * addFactor;

                        finalMultiplicativeColorForZ.r *= czMul.r * multFactor;
                        finalMultiplicativeColorForZ.g *= czMul.g * multFactor;
                        finalMultiplicativeColorForZ.b *= czMul.b * multFactor;
                    }

                    //if (cr.useNewCaracols)
                    //{
                    foreach (ColorReader.CaracolChange c in cr.conSatValMod)
                    {
                        switch (c.modify)
                        {
                            case (ColorReader.CSVenum.Contrast):
                                contrastChange += c.result;
                                break;
                            case (ColorReader.CSVenum.Saturation):
                                saturationChange += c.result;
                                break;
                            case (ColorReader.CSVenum.Value):
                                valueChange += c.result;
                                break;
                        }
                    }

                    //}
                    //else
                    //{
                    //    contrastChange += cr.contrastChange.result;
                    //    saturationChange += cr.saturationChange.result;
                    //    valueChange += cr.valueChange.result;
                    //}
                }
            }


            if (useNight)
            {
                float n = night.GetColorForZ(f);
                finalAdditiveColorForZ.r -= n;
                finalAdditiveColorForZ.g -= n;
                finalAdditiveColorForZ.b -= n;

                finalMultiplicativeColorForZ.r -= n;
                finalMultiplicativeColorForZ.g -= n;
                finalMultiplicativeColorForZ.b -= n;

                darkNightArray[di] = n;
                di++;
                if (di >= darkNightArray.Length)
                    di = 0;
            }
        }
        finalMultiplicativeColorForZ.r = Mathf.Clamp01(finalMultiplicativeColorForZ.r);
        finalMultiplicativeColorForZ.g = Mathf.Clamp01(finalMultiplicativeColorForZ.g);
        finalMultiplicativeColorForZ.b = Mathf.Clamp01(finalMultiplicativeColorForZ.b);
        finalMultiplicativeColorForZ.a = Mathf.Clamp01(finalMultiplicativeColorForZ.a);

        if (!fixedColormode) // por alguna extraña razon, en el modo normal de los colorreaders invierto el luma, pero no tiene sentido hacerlo en modo fixed
        {
            HSLColor mulColorForZ_hsl = HSLColor.FromRGBA(finalMultiplicativeColorForZ);
            mulColorForZ_hsl.l = 1 - mulColorForZ_hsl.l;
            finalMultiplicativeColorForZ = mulColorForZ_hsl;
        }

        mt.addColor = finalAdditiveColorForZ ; // para verlo en inspector
        mt.multColor = finalMultiplicativeColorForZ;
        mt.mat.SetColor(ShaderProp._AdditiveColor, finalAdditiveColorForZ);
        mt.mat.SetColor(ShaderProp._MultiplicativeColor, finalMultiplicativeColorForZ);

        mt.mat.SetFloat(ShaderProp._Sat, f * saturationZ_Factor + saturationAdd + saturationChange);
        mt.mat.SetFloat(ShaderProp._Cont, f * contrastZ_Factor + contrastAdd + contrastChange);
        mt.mat.SetFloat(ShaderProp._Val, f * valueZ_Factor + valueAdd + valueChange);
        
        if (mt.mat.HasProperty(ShaderProp._Hue))
        {
            mt.mat.SetFloat(ShaderProp._Hue, WorldMap.instance.parallaxHueShift);
        }
    }
    public Color GetAddColorForZ(Color color, float posZ)
    {
        float zChange = posZ * zAddFactor;
        Color result = color + color * zChange;
        return result;
    }
    public Color GetMulColorForZ(Color color, float posZ)
    {
        float zChange = posZ * zMulFactor;
        Color result = color + color * zChange;
        return result;
    }
}
