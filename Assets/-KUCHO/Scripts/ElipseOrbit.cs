using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ElipseOrbit : MonoBehaviour
{

    public bool staticSun = true;
    public float updateRate = 0.2f;
    public float updateInterval = 0f;
    public int intervalMult = 1000;
    private float previousTime = 0f;
    public bool adjustSizeByWorldSize;
    [Range(0, 4000)] public float widthAdd;
    [Range(0, 4000)] public float heightAdd;

    [Range(0.00000001f, 0.005f)] public float worldWidthFactor;
    [Range(0.00000001f, 0.005f)] public float worldHeightFactor;
    [Range(0, 10)] public float widthMult = 1f;
    [Range(0, 10)] public float heigthMult = 1f;
    [HideInInspector] public float preHeight;
    [HideInInspector] public float preWidth;
    public Vector2 distToCenter;
    public Vector2 normDistToCenter;
    public float angleToCenter;
    public bool UpdateAngleToCam = false;
    public float reverseAngleToCam;
    [HideInInspector] public float reverseAngleToCenter;

    public Vector2 offset;

    public float alpha = 1f; // la posicion en la elipse?
    public bool rotating = true;
    [Range(0, 10)] public float speed = 0.1f;
    [Range(0, 40)] public float highSpeed = 20f;
    [HideInInspector] public float actualSpeed = 0f;
    private float speedMultiplier; // donde se calculan los factores que aumental na velocidad como noche y altura.
    private float speedMultiplier2 = 0.0001f; // solo para que speed pueda taner valores altos y manejables en inspector
    [Range(0, 10)] public float speedHeightFactor;
    [Range(1, 100)] public float shorterNightsFactor = 1f;
    [Range(-1, 0.25f)] public float nightIsBellowThis = 0f;
    public Transform orbitalBody;
    Vector2 pos;
    public float[] snapPoints;
    private int snapIndex = 0;

    [Header("---info")]
    public bool nightTime;
    public float heightFactor; // toma valor entre 0 y 1 dependiendo de la altura 1 = maxma altura de la elipse
    public float sideFactor;


    void Start()
    {
        CalculateSize();
    }

    void Update()
    {
        CalculateSize();
    }
    void OnValidate()
    {
        if (gameObject.activeInHierarchy)
        {
            if (WorldMap.instance && this.enabled)
                MyUpdate();
        }
    }

    public void MyUpdate()
    {
        if (heightFactor < nightIsBellowThis)
        {
            speedMultiplier = (heightFactor * -shorterNightsFactor * speedHeightFactor) + 1f;
            nightTime = true;
        }
        else
        {
            speedMultiplier = heightFactor * speedHeightFactor + 1;
            nightTime = false;
        }

        if (rotating) alpha += actualSpeed * speedMultiplier * speedMultiplier2;

        preHeight = heigthMult * WorldMap.height + heightAdd;
        preWidth = widthMult * WorldMap.width + widthAdd;

        float height = preHeight * Mathf.Sin(alpha) + offset.y;
        float width = preWidth * Mathf.Cos(alpha) + offset.x;

        pos = new Vector2(width, height);
        heightFactor = (pos.y / preHeight);
        sideFactor = (pos.x / preWidth);

        Vector3 finalPos = WorldMap.worldCenter + pos;
        finalPos.z = orbitalBody.position.z;
        orbitalBody.transform.position = finalPos;

        if (Application.isPlaying)
        {
            updateInterval = (Time.realtimeSinceStartup - previousTime) * intervalMult;
            previousTime = Time.realtimeSinceStartup;
        }
        distToCenter = WorldMap.worldCenter - (Vector2)orbitalBody.transform.position;
        normDistToCenter = distToCenter.normalized;
        angleToCenter = Vector2.Angle(new Vector2(-1, 0), distToCenter);
        reverseAngleToCenter = angleToCenter - 180;
    }
    public void NextSnapPoint()
    {
        alpha = snapPoints[snapIndex];
        snapIndex++;
        if (snapIndex >= snapPoints.Length) snapIndex = 0;
        MyUpdate();
    }
    void CalculateSize()
    {
        if (adjustSizeByWorldSize)
        {
            widthMult = WorldMap.width * worldWidthFactor;
            heigthMult = WorldMap.height * worldHeightFactor;
        }
    }
}
