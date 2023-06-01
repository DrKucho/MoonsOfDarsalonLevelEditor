using UnityEngine;
using System.Collections;


public enum zOffsetBasedOn {MyGround, SpritePlanesPlusItemNumber, PlayerGround, Player, None}
[System.Serializable]
public class Render{
	public Snap pixelSnapX = Snap.RealPixel;
	public Snap pixelSnapY = Snap.ArcadePixel;
	public zOffsetBasedOn zOffsetWalkerBasedOn = zOffsetBasedOn.MyGround;
	public float zOffsetWalker = -0.01f;
	public zOffsetBasedOn zOffsetFlyerBasedOn = zOffsetBasedOn.Player;
    public SpritePlane.Type flyerSpritePlaneType = SpritePlane.Type.Foreground1;
    [Range(0,1)] public float flyerPlane = 0.5f;
    public float zOffsetFlyer = -13f;
	public bool doublePixelMargin = true;

    bool FlyerIsSpritePlane(){return zOffsetFlyerBasedOn == zOffsetBasedOn.SpritePlanesPlusItemNumber; }
    bool FlyerIsNotSpritePlane(){return zOffsetFlyerBasedOn != zOffsetBasedOn.SpritePlanesPlusItemNumber; }

    public float GetFlyerSpritePlanesOffset(){
        return SpritePlanes.GetZ(flyerSpritePlaneType, flyerPlane);
    }
	public void CopyTo(Render to){
		to.pixelSnapX = pixelSnapX;
		to.pixelSnapY = pixelSnapY;
		to.zOffsetWalkerBasedOn = zOffsetWalkerBasedOn;
		to.zOffsetWalker = zOffsetWalker;
		to.zOffsetFlyerBasedOn = zOffsetFlyerBasedOn;
        to.flyerSpritePlaneType = flyerSpritePlaneType;
        to.flyerPlane = flyerPlane;
		to.zOffsetFlyer = zOffsetFlyer;
		to.doublePixelMargin = doublePixelMargin;
	}
}
