using UnityEngine;
using System.Collections.Generic;


using UnityEngine.Serialization; // implemenar en moons of darsalon MOD

//[ExecuteInEditMode]
public class Sticker : MonoBehaviour
{

    public enum Type
    {
        Normal, Player, PlayerVehicle, Cargo, UnlockablePalette
    }
    public bool debug = false;

    public Type type = Type.Normal;
    public int playerStartPointIndex = 0;
    public bool checkPointNeedsCargo;
    public bool turnOffPlayerAtStart; // todo implementar en MOD lo hice para hazcer cinematicas con rC y rR si no quiero que este player
    public EnableDisableGameObjects enableDisable;
    public bool forceReturnToStoreIfOut = false;
    public bool createRealObjectOnStart = true;
    public bool cameraTargetAtStart = false;
    public float delay = 0;
    public bool delayTwoFrames = false;
    public bool copyMyScaleToItem = true;
    public Vector3 realObjectOffset;
    public Color color;
    [ReadOnly2Attribute] public ItemGroup itemGroup;
    public Renderer rend;
    public SpriteRenderer sprite;
    public SWizSprite tkSprite;
    public LevelObject levelObject;
    public ItemStoreFinder storeFinder;
    [ReadOnly2Attribute] public ItemStore store;
    public Item item;
    public string _item = "";
    
    bool itemBusy = false;

    public float energyMultiplier = 1;
    public Transform stickersT;

    bool ShowItemStoreStuff()
    {
        if (type == Type.Normal || type == Type.PlayerVehicle)
            return true;
        return false;
    }
    public void InitialiseInEditor()
    {
        rend = GetComponentInChildren<Renderer>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        tkSprite = GetComponentInChildren<SWizSprite>();
        itemGroup = GetComponentInParent<ItemGroup>();
        levelObject = GetComponent<LevelObject>();
    }

    bool checkPointInitDone = false;
    // por si soy un StickerModel , ESTO CREA UNA STICKER COPIA DE NOSOTROS QUE SOMOS STICKER MODEL , LA STICKER CREADA NO SERA MODEL Y CREARA UN OBJETO EN START
    public Item obItem;
    //--------------------------------------------------------------------------------------------------------------------
    bool playerGotCargo;
}
