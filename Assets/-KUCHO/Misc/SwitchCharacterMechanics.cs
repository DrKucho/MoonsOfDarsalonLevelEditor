using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CharacterMechanic {Run, Crowch, Jump, Climb, LightSensititivity, LaserGun, GroundMaker, LightGun, VoiceCommandReach, hitSide, VoiceCommandFollowMe, VoiceCommandLeftRightAndWait}
[System.Serializable]
public class MechanicModifier{
    public CharacterMechanic type;
    public ThreeWaySwitch action;
    public UpDownLeftRight voiceCommandNeedAim;
    public PhraseArray reasonsToDisable;
    public float offset;
    bool NeedsOffset(){ return type == CharacterMechanic.LightSensititivity || type == CharacterMechanic.VoiceCommandReach || type == CharacterMechanic.hitSide; }
    bool DoesNotNeedsOffset() { return !NeedsOffset(); }
    bool IsVoiceCommand() { return type == CharacterMechanic.VoiceCommandFollowMe | type == CharacterMechanic.VoiceCommandLeftRightAndWait; }
    bool IsDisabling(){ return action == ThreeWaySwitch.SwitchOff; }

}
public class SwitchCharacterMechanics : MonoBehaviour {

    public MechanicModifier[] modifier;


    public Collider2D myCol;
    List<CC> CCs = new List<CC>();

    void OnValidate(){
        if (!myCol)
            myCol = GetComponent<Collider2D>();
    }

}
