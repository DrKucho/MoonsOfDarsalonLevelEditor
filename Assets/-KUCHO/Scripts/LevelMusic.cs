using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;


using UnityEngine.Serialization;

public class LevelMusic : MonoBehaviour
{

    public enum Type { Normal, Action, Relax, Summary }

    public bool debug = false;
    public bool playOnEnable = true;
    public bool turnOffInEditor = true;
    public AudioMixerGroup mixerGroup;
    GameObject enemyControl;
    public Type type = LevelMusic.Type.Action;
    [Range(0.5f, 1.5f)] public float musicGain = 1;
    public int startBar = 0; // todo implementar en moons of darsalon MOD
    public int[] cueBarPoints;
    public int noLoopedBarsAtStart = 16;
    public Vector2Int[] noDanceBeats;
    public string jumpForwardKey = "";
    public AudioClip levelMusic;
    public AudioClip barMeasure;
    public int danceIndex = 0;
    public int musicalPhraseBeats = 16;
    public float startDelay = 0.2f;
    public AudioClip vinylNoiseStart;
    public double vinylNoiseStartDelay = 0.2f;
    public float vinylNoiseStartGain = 0.7f;
    public AudioClip vinylNoiseStop;
    public float vinylNoiseStopGain = 0.7f;
    public AudioClip lostOneLifeMusic;
    public AudioClip gameOverMusic;
    public AudioSource[] audioSources;
    [Header("Calculados")]
    public int loopEndPoint;
    public int loopStartPoint;
    public float measureCount;
    public float beatCount;
    [ReadOnly2Attribute] public int barCounter;
    public SWizSprite beatSprite;
    public float beatSpriteHoldSpeed = 0.04f;
    [ReadOnly2Attribute] public float beatDuration;
    [ReadOnly2Attribute] public int beatSamples;
    [ReadOnly2Attribute] public int beatCounter;
    [ReadOnly2Attribute] public int nextBeatPoint;
    [ReadOnly2Attribute] public float lastBeatCountTime = 0;
    [ReadOnly2Attribute] public int lastBeatCountTimeSamples = 0;
    [ReadOnly2Attribute] public float lastBeatCountDiffSeconds = 0;
    bool initialized = false;
    float samplesToTime;
    [ReadOnly2Attribute] public int barCounter1234;
    [ReadOnly2Attribute] public int musicPhraseBeatCounter;
    [ReadOnly2Attribute] public bool newPhrase;
    [ReadOnly2Attribute] public bool dance;
    public delegate void OnBeat(LevelMusic levelMusic);
    public event OnBeat onBeat;


}
