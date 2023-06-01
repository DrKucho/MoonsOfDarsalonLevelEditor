using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions.Must;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SoundData", menuName = "Sound DataBase", order = 51)] 

public class SoundDataBase : ScriptableObject
{
    [System.Serializable]
    public class DoorSoundSet
    {
        public AudioClip goAudio;
        public AudioClip backAudio;
        public AudioClip endPosAudio;
        public AudioClip startPosAudio;

        public void CopyTo(Door door)
        {
            door.goAudio = goAudio;
            door.backAudio = backAudio;
            door.endPosAudio = endPosAudio;
            door.startPosAudio = startPosAudio;
        }
    }

    public AudioClipArray[] audioClipArrays;
    

    public DoorSoundSet[] doorSoundSets;
    public AudioClip[] loopedNoiseClips;
    static SoundDataBase _instance;
    public static SoundDataBase instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameData.instance.soundDataBase;
                if (!_instance)
                {
                    Debug.Log("NO ENCUENTRO SOUND DATA");
                }
            }
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    /// <summary>
    /// Siempre Retorna el indice, tanto si lo añade como si ya existia
    /// </summary>
    /// <param name="aca"></param>
    /// <returns></returns>
    public int AddAudioClipArrayIfNew(AudioClipArray aca)
    {
        for (int i = 0 ; i < audioClipArrays.Length; i++)
        {
            if (audioClipArrays[i].CompareWith(aca))
                return i;
        }
        var n = new AudioClipArray(aca);
        var list = audioClipArrays.ToList();
        list.Add(n);
        audioClipArrays = list.ToArray();
        return audioClipArrays.Length - 1;
    }

    public AudioClipArray GetAudioClipArrayByIndex(int i)
    {
        if (audioClipArrays != null && audioClipArrays.Length > 0)
        {
            if (i >= 0 && i < audioClipArrays.Length)
            {
                return audioClipArrays[i];
            }
            else
            {
                Debug.Log(this + " INDICE FUERA DE RANGO AL RECUPERAR AUDIO CLIP ARRAY DEVUELVO EL PRIMERO ANYWAY");
                return audioClipArrays[0];
            }
        }
        return null;
    }
}