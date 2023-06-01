using System;
using UnityEngine;
using System.Collections;


public class CcOffsetZArea : MonoBehaviour
{
    public enum Mode
    {
        RelativeToCcGround,
        RelativeToMyCollider,
        Absolute, 
        ColliderSettings
    };

    public enum WalkerFlyerMode
    {
        Disabled = 0,
        OnlyIfWalker = 100,
        OnlyIfWalkerGrounded = 200,
        OnlyIfWalkerNotGrounded = 300,
        OnlyIfFlyer = 400,
        Always = 500
    };

    [Serializable]
    public class EnterOrExit
    {
        public WalkerFlyerMode walkerFlyer;
        public Mode mode = Mode.RelativeToMyCollider;
        bool IsColliderSettings() { return mode == Mode.ColliderSettings; }
        bool IsNotColliderSettings() { return mode != Mode.ColliderSettings; }
         public Collider2D groundOrLadder;
        public float offset = 0;
        public Collider2D myCol;

        public override string ToString()
        {
            if (walkerFlyer == WalkerFlyerMode.Disabled)
                return "OFF";
            return walkerFlyer + " " + mode + " " + offset;
        }

    }

    public bool enterIsAlsoStay;
    public EnterOrExit enter;
    public EnterOrExit exit;
    public Collider2D myCol;
    
    public void InitialiseInEditor()
    {
        myCol = GetComponent<Collider2D>();
        enter.myCol = myCol;
        exit.myCol = myCol;
        myCol.gameObject.layer = Layers.vision;
    }
}
