using UnityEngine;
using System.Collections;
using System.Collections.Generic;



[System.Serializable]
public class LevelMusicList{
    public List<LevelMusic> list;
    [System.NonSerialized] int index  = -1;

    public LevelMusic GetRandom(){
        int previousIndex = index;
        int tries = 0;

        if (list.Count > 1)
        {
            while (index == previousIndex && tries < 100) // manera chapu de seguir adelante si solo hay un levelMusic, por que aleatoriamente siempre saldra el mismo, pero es poco probable tener un juego con un music loader que solo tiene un tema
            {
                index = Random.Range(0, list.Count); 
                tries++;
            }
            return list[index];
        }
        else
            return list[0];
    }
    public LevelMusic GetNext(){
        KuchoHelper.IncAndWrapInsideArrayLength(ref index, 1, list.Count);
        return list[index];
    }
    public LevelMusic Find(string _string){
        foreach(LevelMusic lm in list)
            if (lm.name.Contains(_string))
                return lm;
        return null;
    }
    public void Add(LevelMusic lm){
        list.Add(lm);
    }
    public void Clear(){
        list.Clear();
    }
}
public class LevelMusicLoader : MonoBehaviour {

	public float playDelay = 2f;
    public LevelMusic[] allMusics;
    public LevelMusicList normalMusic;
    public LevelMusicList actionMusic;
    public LevelMusicList relaxMusic;
    public LevelMusicList summaryMusic;
    LevelMusicList currentLevelMusicList;
	public LevelMusic currentLevelMusic;
	public string levelName = "";


    public void InitialiseInEditor(){
        normalMusic.Clear();
        actionMusic.Clear();
        relaxMusic.Clear();
        summaryMusic.Clear();
        foreach (LevelMusic lm in allMusics)
        {
            switch (lm.type)
            {
                case (LevelMusic.Type.Normal):
                    normalMusic.Add(lm);
                    break;
                case (LevelMusic.Type.Action):
                    actionMusic.Add(lm);
                    break;
                case (LevelMusic.Type.Relax):
                    relaxMusic.Add(lm);
                    break;
                case (LevelMusic.Type.Summary):
                    summaryMusic.Add(lm);
                    break;
            }
        }

    }




}
