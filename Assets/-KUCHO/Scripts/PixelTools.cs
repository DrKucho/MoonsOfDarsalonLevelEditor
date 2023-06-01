using UnityEngine;
using System.Collections;
using UnityEngine.Profiling;

public struct ColorAndTerrainType
{
    public Color color;
    public bool destructible;
}

public class PixelTools{

//    public bool debug = false;
//    public GameObject markPrefab;
//    public LineRenderer lineR;
    
//    public void Start(){ //  print(this + "START ");

////        if (debug) lineR = GetComponent<LineRenderer>();
//    }

    public static bool IsInsideTexture(Vector2 pos, Texture2D tex){
        if (pos.x > 0 && pos.x < tex.width && pos.y > 0 && pos.y < tex.height) return true;
        else return false;
    }
    public static bool IsInsideTexture(Point pos, Texture2D tex){
        if (pos.x > 0 && pos.x < tex.width && pos.y > 0 && pos.y < tex.height) return true;
        else return false;
    }
    public static bool IsInsideTexture(int x, int y, Texture2D tex){
        if (x > 0 && x < tex.width && y > 0 && y < tex.height) return true;
        else return false;
    }
    public static bool IsInsideGroundTexture(int x, int y){
        if (x > 0 && x < WorldMap.width && y > 0 && y < WorldMap.height) return true;
        else return false;
    }
    public static bool IsInsideGroundTexture(Vector2 pos){
        int x= Mathf.FloorToInt(pos.x);
        int y= Mathf.FloorToInt(pos.y);
        return IsInsideGroundTexture(x,y);
    }
    public static PixelRaycastHit AlphaColorRaycast(Vector2 start, Vector2 direction, int distance, bool lookForSolidPixel, bool destructible, bool indestructible, bool useShaderHSV_Cont){
        PixelRaycastHit hit = new PixelRaycastHit();
        int x = Mathf.FloorToInt(start.x);
        int y = Mathf.FloorToInt(start.y);
        int dirX = Mathf.FloorToInt(direction.x);
        int dirY = Mathf.FloorToInt(direction.y);
        for (hit.distance = 0; hit.distance < distance; hit.distance ++)
        {
            //      if (x >= 0 && x < Game.destructibleGroundD2D.AlphaWidth && y >=0 && y < Game.destructibleGroundD2D.AlphaHeight){ // dentro de la textura?
            if (IsInsideGroundTexture(x,y))
            {
                ColorAndTerrainType c = AlphaDataGetColorPixel(x,y, destructible, indestructible, useShaderHSV_Cont);
                //print ("ALPHA DE " + x + "," + y + "=" + hit.alpha);
                if ((lookForSolidPixel && c.color.a > 0.5f) || (!lookForSolidPixel && c.color.a <= 0.5f)) // encontramos lo que buscabamos
                {
                    hit.pixel = c.color;
                    hit.point.x = x;
                    hit.point.y = y;
                    hit.found = true;
                    hit.insideOfTexture = true;
                    hit.destructible = c.destructible;
                    return hit; // al volver, hit.distance se queda con la distancia buena
                }
            }
            else // se salio de la textura
            {
                hit.pixel = Constants.transparentBlack;
                hit.point.x = x;
                hit.point.y = y;
                hit.found = false;
                hit.insideOfTexture = false;
                hit.alpha = 0;
                hit.destructible = false;
                return hit;
            }
            x += dirX;
            y += dirY;
        }
        // se acaba el raycast y no ha encontrado nada
        hit.pixel = Constants.transparentBlack;
        hit.point.x = x;
        hit.point.y = y;
        hit.found = false;
        hit.insideOfTexture = true;
        hit.alpha = 0;
        hit.destructible = false;
        return hit;
    }
    public static PixelRaycastHit AlphaRaycast(Vector2 start, Vector2 direction, int distance, bool lookForSolidPixel, bool destructible, bool indestructible){
        PixelRaycastHit hit = new PixelRaycastHit();
        int x = Mathf.FloorToInt(start.x);
        int y = Mathf.FloorToInt(start.y);
        int dirX = Mathf.FloorToInt(direction.x);
        int dirY = Mathf.FloorToInt(direction.y);
        for (hit.distance = 0; hit.distance < distance; hit.distance ++)
        {
    //        if (x >= 0 && x < Game.destructibleGroundD2D.AlphaWidth && y >=0 && y < Game.destructibleGroundD2D.AlphaHeight){ // dentro de la textura?
            if (IsInsideGroundTexture(x,y))
            {
                int alpha = AlphaDataGetPixel(x,y, destructible, indestructible);
                byte alphaByte = (byte)alpha;
                if (alpha > 255) alphaByte = 255;
                hit.alpha = alphaByte;
                //print ("ALPHA DE " + x + "," + y + "=" + hit.alpha);
                if ((lookForSolidPixel && hit.alpha > 127) || (!lookForSolidPixel && hit.alpha <= 127)) // encontramos lo que buscabamos
                {
                    hit.point.x = x;
                    hit.point.y = y;
                    hit.found = true;
                    hit.insideOfTexture = true;
                    return hit; // al volver, hit.distance se queda con la distancia buena
                }
            }
            else // se salio de la textura
            {
                hit.point.x = x;
                hit.point.y = y;
                hit.found = false;
                hit.insideOfTexture = false;
                hit.alpha = 0;
                return hit;
            }
            x += dirX;
            y += dirY;
        }
        // se acaba el raycast y no ha encontrado nada
        hit.point.x = x;
        hit.point.y = y;
        hit.found = false;
        hit.insideOfTexture = true;
        hit.alpha = 0;
        return hit;
    }
    public static PixelRaycastHit Raycast(Vector2 start, Vector2 direction, int distance, Texture2D texture, bool solidPixel){ // soliPixel significa que estamos buscando pixles de color opaco , si esta a false buscariamos pixeles transparentes
        PixelRaycastHit hit = new PixelRaycastHit();
        Vector2 previousHitPoint = Constants.zero2;
//        if (debug) lineR.SetPosition(1, new Vector3(start.x, start.y, -10));
        for ( hit.distance = 0; hit.distance < distance; hit.distance ++){
            hit.point.x = Mathf.RoundToInt(start.x);
            hit.point.y = Mathf.RoundToInt(start.y);
            if (hit.point != previousHitPoint){ // por que hago esto? las coordenadas pueden ser la misma? seguramente si la direccion es en diagonal , al redondear a pixeles puede que si que un punto a comprobar sea igual al anterior
                hit.pixel = GetPixelFromWorld(hit.point, texture);
                if (hit.insideOfTexture){
                    if ( solidPixel && hit.pixel.a > 0.15f ) { hit.found = true; return hit; }
                    if (!solidPixel && hit.pixel.a < 0.15f ) { hit.found = true; return hit; }
                }
                else {
                    hit.found = false;
                    return hit;
                }
            }
            //if (debug) lineR.SetPosition(2, new Vector3(hit.point.x, hit.point.y, -10));
            previousHitPoint = hit.point;
            start += direction;
        }
        hit.found = false;
        return hit;
    }
    public static Color GetPixelFromTex(Point pos, Texture2D tex) { // no funciona si tenemos varias texturas tipo world !!
        var hit = new PixelRaycastHit();
        if (KuchoHelper.Intersect(new Vector2(pos.x, pos.y), new Rect(0,0,tex.width, tex.height))){
            Color pixel = tex.GetPixel(pos.x, pos.y);
//            if (debug) Instantiate(markPrefab, new Vector2(pos.x, pos.y) , Quaternion.Euler(Constants.zero3));
            hit.insideOfTexture = true;
            return pixel;
        }
        hit.insideOfTexture = false;
        Debug.Log (" TRATANDO DE SACAR UN PIXEL DE FUERA DEL SPRITE " + pos);
        return Constants.transparentBlack; ;
    }
    public static Color GetPixelFromWorld_old(Vector2 pos, Texture2D texture) { // no funciona si tenemos varias texturas tipo world !!
        var hit = new PixelRaycastHit();
        Rect worldRect = WorldMap.destructible.rect; // optimization
        Bounds groundSpriteBounds = WorldMap.destructible.spriteRenderer.bounds; // optimization
        if (KuchoHelper.Intersect(pos, worldRect)){
            Vector2 pixelPos = new Vector2((pos.x - groundSpriteBounds.min.x) / WorldMap.destructible.spriteGO.transform.localScale.x , pos.y - groundSpriteBounds.min.y ); 
            Color pixel = texture.GetPixel(Mathf.RoundToInt(pixelPos.x), Mathf.RoundToInt(pixelPos.y));
//            if (debug) Instantiate(markPrefab, pos , Quaternion.Euler(Constants.zero3));
            hit.insideOfTexture = true;
            return pixel;
        }
        hit.insideOfTexture = false;
        Debug.Log (" TRATANDO DE SACAR UN PIXEL DE FUERA DEL SPRITE " + pos);
        Debug.Log (" xMin=" + worldRect.xMin +" yMin=" + worldRect.yMin +" xMax=" + worldRect.xMax +" yMax=" + worldRect.yMax );
        Debug.Log ("PRINT GROUND RECT=" + worldRect);
        return Constants.transparentBlack; ;
    }
    public static Color GetPixelFromWorld(Vector2 pos, Texture2D texture)
    { // no funciona si tenemos varias texturas tipo world !!
        Rect worldRect = WorldMap.destructible.rect; // optimization
        if (KuchoHelper.Intersect(pos, worldRect))
        {
            Color pixel = texture.GetPixel(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
            return pixel;
        }
        return Constants.transparentBlack; ;
    }
    public static bool ThereIsPixelTex(Point pos, Texture2D texture){
        Color pixel = GetPixelFromTex(pos, texture);
        if (pixel.a < 0.15f) return false; else return true;
    }
    public static bool ThereIsPixelTex(Vector2 pos, Texture2D texture){
        Color pixel = GetPixelFromTex(new Point((int)pos.x, (int)pos.y), texture);
        if (pixel.a < 0.15f) return false; else return true;
    }
    public static bool ThereIsPixelWorld(Vector2 pos, Texture2D texture){
        Color pixel = GetPixelFromWorld(pos, texture);
        if (pixel.a < 0.15f) return false; else return true;
    }
    // NUEVA , CON OPCION A AMBOS TERRENOS INDEPENDIENTEMENTE
    /// <summary>
    /// retorna con el primer pixel que encuentre
    /// </summary>
    /// <returns>The data get color pixel.</returns>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="destructible">If set to <c>true</c> destructible.</param>
    /// <param name="indestructible">If set to <c>true</c> indestructible.</param>
    public static ColorAndTerrainType AlphaDataGetColorPixel(int x, int y, bool destructible, bool indestructible, bool useShaderHSV_Cont){
        int index = AlphaDataGetIndex(x,y);
        int   alpha = 0;
        ColorAndTerrainType result = new ColorAndTerrainType();
        result.color = Constants.transparentBlack;
        Terrain2D terr = WorldMap.destructible;
        if (destructible) // hay que leer destructible?
        {
            terr = WorldMap.destructible;
            if (terr) // hay terreno destructible?
            {
                alpha = terr.d2dSprite.AlphaData[index]; // primero miro el alpha en alpha data 
                if (alpha > 0)
                {
                    if (useShaderHSV_Cont)
                        result.color = terr.GetPixelWithShader(x, y);
                    else
                        result.color = terr.texture.GetPixel(x, y);
                    result.color.a = (float)(alpha / 255f);
                    result.destructible = true;
                    return result;
                }
            }
        }
        if (indestructible)
        {
            terr = WorldMap.indestructible;
            if (terr) // hay terreno destructible?
            {
                alpha = terr.d2dSprite.AlphaData[index]; // primero miro el alpha en alpha data 
                if (alpha > 0)
                {
                    if (useShaderHSV_Cont)
                        result.color = terr.GetPixelWithShader(x, y);
                    else
                        result.color = terr.texture.GetPixel(x, y);
                    result.color.a = (float)(alpha / 255f);
                    result.destructible = false;
                    return result;
                }
            }
        }
        return result;
    }
    public static int AlphaDataGetPixel(int x, int y, bool destructible, bool indestructible){
        int index = AlphaDataGetIndex(x,y);
        int   destAlpha = 0;
        int indestAlpha = 0;
        if (destructible)     destAlpha = WorldMap.destructible.d2dSprite.AlphaData[index];
        if (indestructible) indestAlpha = WorldMap.indestructible.d2dSprite.AlphaData[index];
        return destAlpha + indestAlpha;
    }
    public static int AlphaDataGetPixel(int index, bool destructible, bool indestructible){
        int   destAlpha = 0;
        int indestAlpha = 0;
        if (destructible)     destAlpha = WorldMap.destructible.d2dSprite.AlphaData[index];
        if (indestructible) indestAlpha = WorldMap.indestructible.d2dSprite.AlphaData[index];
        return destAlpha + indestAlpha;
    }
    public static int AlphaDataGetPixel(int x, int y, bool destructible, bool indestructible, bool background){
        int index = AlphaDataGetIndex(x,y);
        int   destAlpha = 0;
        int indestAlpha = 0;
        int backgrAlpha = 0;
        if (destructible)
            destAlpha = WorldMap.destructible.d2dSprite.AlphaData[index];
        if (indestructible)
            indestAlpha = WorldMap.indestructible.d2dSprite.AlphaData[index];
        if (background)
            backgrAlpha = WorldMap.background.d2dSprite.AlphaData[index];
        return destAlpha + indestAlpha + backgrAlpha;
    }
    public static bool AlphaDataCheckClearRegionInAllTerrains(int _x, int _y, int width, int height, int threshold){ // deberia volver en cuanto encontrara un pixel relleno asi ahorro
        int x = _x;
        int y = _y;
        int endX = x + width;
        int endY = y + width;
        int index = AlphaDataGetIndex(x,y);

        byte[] destructibleAlphaData = WorldMap.destructible.d2dSprite.AlphaData; // optimizacion
        byte[] indestructibleAlphaData = WorldMap.indestructible.d2dSprite.AlphaData; // optimizacion
        byte[] backgroundAlphaData;

        bool backgroundIsAlpha8Texture;
        Texture2D tileMapTexture;
        if (WorldMap.background.tileMap || WorldMap.background.cam3DtoSprite)
        {
            tileMapTexture = WorldMap.background.spriteRenderer.sprite.texture; // optimizacion
            backgroundIsAlpha8Texture = true;
            backgroundAlphaData = null;
        }
        else
        {
            backgroundIsAlpha8Texture = false;
            tileMapTexture = null;
            backgroundAlphaData = WorldMap.background.d2dSprite.AlphaData;
        }
        while (y < endY)
        {
            while (x < endX)
            {
                if (index < WorldMap.destructible.d2dSprite.AlphaData.Length)
                {
                    if (destructibleAlphaData[index] > threshold)
                        return false;
                    if (indestructibleAlphaData[index] > threshold)
                        return false;
                    if (backgroundIsAlpha8Texture)
                    {
                        if (tileMapTexture.GetPixel(x, y).a > 0) // aqui no hay grados de alpha , asi que no comparo con threshold, si es 0 es transparente, si es mayor es solido
                            return false;
                    }
                    else // textura con alphadata (ya no las uso , la pared de backgroudn quedaba muy fea)
                    {
                        if (backgroundAlphaData[index] > threshold)
                            return false;
                    }
                
                }
                index++;
                x++;
            }
            y ++;
            x = _x;
            index -= width;
            index += WorldMap.width;
        }
        return true;
    }
    // ESTAS SIEMPRE BUSCAN EN DESTRUCTIBLE GROUND CON OPCION A BUSCAR EN INSDESTRUCTIBLE
    static int sum = 0;
    public static int AlphaDataGetPixel(int i, bool indestructibleAsWell){
        if (i < 0 || i > WorldMap.destructible.d2dSprite.alphaData.Length)
        {
            Debug.Log(" INDICE ERRONEO=" + i + " ALPHADATA LENGTH=" + WorldMap.destructible.d2dSprite.AlphaData.Length);
        }
        else
        {
            Profiler.BeginSample("PIXEL TOOLS ALPHADATAGETPIXEL");
            sum = WorldMap.destructible.d2dSprite.alphaData[i];
            if (indestructibleAsWell)
            {
                sum += WorldMap.indestructible.d2dSprite.alphaData[i];
            }
            return sum;
            Profiler.EndSample();
        }
        return -1;
    }
    public static int AlphaDataGetPixel(int x, int y, bool indestructibleAsWell){
        int index = AlphaDataGetIndex(x,y);
        if (indestructibleAsWell && WorldMap.indestructible.d2dSprite)
            return WorldMap.destructible.d2dSprite.AlphaData[index] + WorldMap.indestructible.d2dSprite.AlphaData[index];
        else
            return WorldMap.destructible.d2dSprite.AlphaData[index];
    }
    public static int AlphaDataGetPixel(Vector2 pos, bool indestructibleAsWell){
        int index = AlphaDataGetIndex(pos);
        if (indestructibleAsWell && WorldMap.indestructible.d2dSprite) return WorldMap.destructible.d2dSprite.AlphaData[index] + WorldMap.indestructible.d2dSprite.AlphaData[index];
        else return WorldMap.destructible.d2dSprite.AlphaData[index];
    }
    // ESTAS BUSCAN ALPHA DATA EN BACKGROUND UNICAMENTE
    public static int AlphaDataGetBackGroundPixel(Vector2 pos){
        int posX = (int)pos.x;
        int posY = (int)pos.y;
        int index = (int)(posX / WorldMap.background.spriteGO.transform.localScale.x + posY * WorldMap.width);
        return WorldMap.background.d2dSprite.AlphaData[index];
    } 
    public static int AlphaDataGetIndex(int x, int y){
        return (int)(x + y * WorldMap.width);
    }

    
    public static int AlphaDataGetIndex(Vector2 pos){
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        return (int)(x + y * WorldMap.width); 
    }
    public static Vector2 AlphaDataGetPosition(int i){
        float width = WorldMap.width;
        float ifloat= i; // tiene que set todo floats o si no los calculos se van a la mierda, si metes un int en una division el resultado pierde los decimales !!!???
        float yf= ifloat / width;
        float y= Mathf.Floor(yf);
        float x= (yf - y) * width;
        return new Vector2(x, y);
    }
    public static void PrintSpriteToTexture(Sprite sprite, Vector2 pos, int xFlip, Texture2D texture, bool apply){ // esto se podria mejorar usando points en lugar de vector2
        Vector2 writePos = new Vector2( pos.x + sprite.bounds.min.x * xFlip, pos.y + sprite.bounds.min.y);
        Vector2 readPos = new Vector2(sprite.rect.x, sprite.rect.y);
        Texture2D readTexture = sprite.texture as Texture2D;
        for (int x = 0; x < sprite.bounds.size.x; x++){
            for (int y = 0; y < sprite.bounds.size.y; y ++){
                Color pixel = readTexture.GetPixel((int)readPos.x + x, (int)readPos.y + y);
                if (pixel.a > 0.5f) texture.SetPixel((int)writePos.x + x * xFlip, (int)writePos.y + y, pixel);
            }
        }
        if (apply) texture.Apply();
    }
    //function GetRectFromSpriteSheet(spriteSheet:Texture2D, grid:Vector2, spriteNumber:int){
    //    int howManySpritesOnX= spriteSheet.width / grid.x;
    //    int howManySpritesOnY= spriteSheet.heith / grid.y;
    //    var x = grid.x * spriteNumber - howManySpritesOnX * grid.x;
    //    var y = Mathf.Floor(spriteNumber/howManySpritesOnY) * grid.y;
    //    return new Rect(x,y,grid.x, grid.y);
    //}
    public static void PrintSwizSpriteToTexture(SWizSpriteDefinition sprDef, Vector2 pos, Texture2D texture, bool apply){// esto se podria mejorar usando points en lugar de vector2
        Vector2 writePos = new Vector2( pos.x + sprDef.positions[0].x , pos.y + sprDef.positions[0].y);
        Vector2 readPos = new Vector2(sprDef.regionX, sprDef.regionY);
        Texture2D readTexture = sprDef.material.mainTexture as Texture2D;
        for (int x = 0; x < sprDef.regionW; x++){
            for (int y = 0; y < sprDef.regionH; y ++){
                Color pixel = readTexture.GetPixel((int)readPos.x + x, (int)readPos.y + y);
                if (pixel.a > 0.5f) texture.SetPixel((int)writePos.x + x, (int)writePos.y + y, pixel);
            }
        }
        if (apply) texture.Apply(false);
    }
}
