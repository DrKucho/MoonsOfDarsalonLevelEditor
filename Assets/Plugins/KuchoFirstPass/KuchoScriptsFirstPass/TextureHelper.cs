using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;

/// A unility class with functions to scale Texture2D Data.
///
/// Scale is performed on the GPU using RTT, so it's blazing fast.
/// Setting up and Getting back the texture data is the bottleneck.
/// But Scaling itself costs only 1 draw call and 1 RTT State setup!
/// WARNING: This script override the RTT Setup! (It sets a RTT!)        
///
/// Note: This scaler does NOT support aspect ratio based scaling. You will have to do it yourself!
/// It supports Alpha, but you will have to divide by alpha in your shaders,
/// because of premultiplied alpha effect. Or you should use blend modes.
[System.Serializable]
public class KuchoGrayScaleTextureCompressionData
{
    public ushort zeros;
    public byte[] nonZeros;

    public KuchoGrayScaleTextureCompressionData(ushort zeros)
    {
        this.zeros = zeros;
        this.nonZeros = null;
    }
    public KuchoGrayScaleTextureCompressionData(byte[] nonZeros)
    {
        this.zeros = 0;
        this.nonZeros = nonZeros;
    }
    public void FillTex(ref int index, byte[] texRawData)
    {
        for (int z = 0; z < zeros; z++)
        {
            texRawData[index] = 0;
            index++;
        }
        if (nonZeros != null)
        {
            foreach (byte b in nonZeros)
            {
                texRawData[index] = b;
                index++;
            }
        }
    }
}
public class TextureHelper
{

    public static Color32[] pixels32 = new Color32[4096]; // para usar en operaciones temporales y no crear basura (la usa d2d alphadatatotext()

    static public Texture2D ReuseOrCreateNewSpriteText(SpriteRenderer spriteRenderer, int width, int height, TextureFormat format){
        if (spriteRenderer.sprite && spriteRenderer.sprite.texture.width == width && spriteRenderer.sprite.texture.height == height && spriteRenderer.sprite.texture.format == format) // si es del mismo tamaño que la que tenemos
            return spriteRenderer.sprite.texture;// la reuso
        else// distinto tamaño
            return new Texture2D(width, height, format, false); // creo una nueva
    }
    static public Texture2D ReuseOrCreateNewSpriteText(Texture2D texture, int width, int height, TextureFormat format){
        if (texture && texture.width == width && texture.height == height && texture.format == format) // si es del mismo tamaño que la que tenemos
            return texture;// la reuso
        else// distinto tamaño
            return new Texture2D(width, height, format, false); // creo una nueva
    }
    public static void RenderTextureToTexture2D(RenderTexture rendTex, Texture2D tex2D){
        int width = rendTex.width;
        if (rendTex.width >= tex2D.width)
            width = tex2D.width;
        int height = rendTex.height;
        if (rendTex.height >= tex2D.height)
            height = tex2D.height;
        var backup = RenderTexture.active;
        RenderTexture.active = rendTex;
        tex2D.ReadPixels(new Rect(0,0, width, height), 0, 0); // leo de le render text activa y vuelco a tex2D 
        RenderTexture.active = backup;
    }
	/// <summary>
	///     Returns a scaled copy of given texture.
	/// </summary>
	/// <param name="tex">Source texure to scale</param>
	/// <param name="width">Destination texture width</param>
	/// <param name="height">Destination texture height</param>
	/// <param name="mode">Filtering mode</param>
	public static Texture2D GetScaled(Texture2D src, int width, int height, FilterMode mode = FilterMode.Trilinear)
	{
		Rect texR = new Rect(0,0,width,height);
		GpuScale(src,width,height,mode);

		//Get rendered data back to a new texture
		Texture2D result = new Texture2D(width, height, TextureFormat.ARGB32, true);
		result.Reinitialize(width, height);
		result.ReadPixels(texR,0,0,true);
		return result;                 
	}

	/// <summary>
	/// Scales the texture data of the given texture.
	/// </summary>
	/// <param name="tex">Texure to scale</param>
	/// <param name="width">New width</param>
	/// <param name="height">New height</param>
	/// <param name="mode">Filtering mode</param>
	public static void Scale(Texture2D tex, int width, int height, FilterMode mode = FilterMode.Trilinear)
	{
		Rect texR = new Rect(0,0,width,height);
		GpuScale(tex,width,height,mode);

		// Update new texture
		tex.Reinitialize(width, height);
		tex.ReadPixels(texR,0,0,true);
		tex.Apply(true);        //Remove this if you hate us applying textures for you :)
	}

	// Internal unility that renders the source texture into the RTT - the scaling method itself.
	static void GpuScale(Texture2D src, int width, int height, FilterMode fmode)
	{
		//We need the source texture in VRAM because we render with it
		src.filterMode = fmode;
		src.Apply(true);       

		//Using RTT for best quality and performance. Thanks, Unity 5
		RenderTexture rtt = new RenderTexture(width, height, 32);

		//Set the RTT in order to render to it
		Graphics.SetRenderTarget(rtt);

		//Setup 2D matrix in range 0..1, so nobody needs to care about sized
		GL.LoadPixelMatrix(0,1,1,0);

		//Then clear & draw the texture to fill the entire RTT.
		GL.Clear(true,true,new Color(0,0,0,0));
		Graphics.DrawTexture(new Rect(0,0,1,1),src);
	}
	public static Color32 GetAverageColor(Texture2D tex)
	{
		Color32[] texColors = tex.GetPixels32();	
		int total = texColors.Length;
		float r = 0;
		float g = 0;
		float b = 0;
		for(int i = 0; i < total; i++)
		{
			r += texColors[i].r;	
			g += texColors[i].g;
			b += texColors[i].b;
		}
		return new Color32((byte)(r / total) , (byte)(g / total) , (byte)(b / total) , 0);
	}
	public static Texture2D Rotate90(Texture2D tex)
	{
		tex.wrapMode = TextureWrapMode.Clamp;
		tex.filterMode = FilterMode.Point;

		Texture2D newTex = new Texture2D(tex.height, tex.width,tex.format, false);
		newTex.filterMode = FilterMode.Point;
		newTex.wrapMode = TextureWrapMode.Clamp;

		for (int x = 0; x < tex.width; x++)
		{
			for (int y = 0; y < tex.height; y++)
			{
				Color c = tex.GetPixel(x, y);
				newTex.SetPixel(y, newTex.height - x - 1, c);
			}
		}
		newTex.Apply();
		return newTex;
	}
	public static Texture2D MirrorLeftToRight(Texture2D tex)
	{
		tex.wrapMode = TextureWrapMode.Clamp;
		tex.filterMode = FilterMode.Point;

		int width = tex.width;
		int widthMinusOne = width - 1;
		int height = tex.height;

		Texture2D newTex = new Texture2D(width, height, tex.format, false);
		newTex.filterMode = FilterMode.Point;
		newTex.wrapMode = TextureWrapMode.Clamp;

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				Color c = tex.GetPixel(x, y);
				newTex.SetPixel(widthMinusOne - x, y, c);
			}
		}
		newTex.Apply();
		return newTex;
	}
	public static Texture2D MirrorUpToDown(Texture2D tex)
	{
		tex.wrapMode = TextureWrapMode.Clamp;
		tex.filterMode = FilterMode.Point;

		int width = tex.width;
		int height = tex.height;
		int heightMinusOne = height - 1;

		Texture2D newTex = new Texture2D(width, height, tex.format, false);
		newTex.filterMode = FilterMode.Point;
		newTex.wrapMode = TextureWrapMode.Clamp;

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				Color c = tex.GetPixel(x, y);
				newTex.SetPixel(x, heightMinusOne - y, c);
			}
		}
		newTex.Apply();
		return newTex;
	}
    public static bool IsReadable(Texture2D tex){
        try
        {
            tex.GetPixel(0, 0);
            return true;
        }
        catch(UnityException e)
        {
            if (e.Message.StartsWith("Texture '" + tex.name + "' is not readable"))
            {
                return false;
            }
            else
            {
                Debug.Log("INTENTABA SABER SI " + tex.name + " ES LEGIBLE PERO ME HA VENIDO OTRO ERROR: " + e.Message);
                return false;
            }
        }

    }
    public static void Polish(Texture2D tex, int pixelsAround, float smoothSquareRatio){        
        Color32[] pix = tex.GetPixels32();
        Color32[] tPix = Polish(pix, tex.width, tex.height, pixelsAround, smoothSquareRatio);
        tex.SetPixels32(tPix);
    }
    //TODO seguro que no se puede pulir sobre la misma array y asi evitar crear otra array igual de grande?
    public static Color32[] Polish(Color32[] pix, int width, int height, int pixelsAround, float smoothSquareRatio){ 
        Color32[] tPix; //tempPix o una nueva temporal igual de grande que pix que solo uso para pulir
        int polishThreshold;

        tPix = new Color32[height * width];

        int squareSide = (pixelsAround * 2) + 1;
        int maxValue = (int)(squareSide * squareSide * 255);
        polishThreshold = (int) (maxValue * smoothSquareRatio);

        int solidCount = 0;
        int transpCount = 0;

        int sum = 0;      

        int endH = height - pixelsAround; // no empiezo en 0 , asi no me salgo del mapa y es mas rapido, pero no pulira los contornos
        int endW = width - pixelsAround; // no empiezo en 0 , asi no me salgo del mapa y es mas rapido, pero no pulira los contornos
        for (int y = pixelsAround; y < endH; y++) // no empiezo en 0 , asi no me salgo del mapa y es mas rapido, pero no pulira los contornos
        {
            for (int x = pixelsAround; x < endW; x++) // no empiezo en 0 , asi no me salgo del mapa y es mas rapido, pero no pulira los contornos
            {
                var neighbourSolidPixels = GetSurroundingSum(pix, width, height, x, y, pixelsAround);
                var index = x + y * width; //GetIndex(x, y, w);
                tPix[index] = pix[index]; // el color hay que copiarlo si o si , luego cambio el alpha

                if (neighbourSolidPixels >= polishThreshold)
                {
                    tPix[index].a = 255;
                    solidCount++;
                }
                else
                {
                    tPix[index].a = 0;
                    transpCount++;
                }
            }
        }
        return tPix;
    }
    public static void PolishAlpha8Data(byte[] pix, int width, int height, int pixelsAround, float smoothSquareRatio){ 
        byte[] tPix; //tempPix o una nueva temporal igual de grande que pix que solo uso para pulir
        int polishThreshold;

        tPix = new byte[height * width];

        int squareSide = (pixelsAround * 2) + 1;
        int maxValue = (int)(squareSide * squareSide * 255);
        polishThreshold = (int) (maxValue * smoothSquareRatio);

        int solidCount = 0;
        int transpCount = 0;

        int sum = 0; 

        int endH = height - pixelsAround; // no empiezo en 0 , asi no me salgo del mapa y es mas rapido, pero no pulira los contornos
        int endW = width - pixelsAround; // no empiezo en 0 , asi no me salgo del mapa y es mas rapido, pero no pulira los contornos
        for (int y = pixelsAround; y < endH; y++) // no empiezo en 0 , asi no me salgo del mapa y es mas rapido, pero no pulira los contornos
        {
            for (int x = pixelsAround; x < endW; x++) // no empiezo en 0 , asi no me salgo del mapa y es mas rapido, pero no pulira los contornos
            {
                var neighbourSolidPixels = GetSurroundingSum(pix, width, height, x, y, pixelsAround);
                var index = x + y * width; //GetIndex(x, y, w);
                tPix[index] = pix[index]; // el color hay que copiarlo si o si , luego cambio el alpha

                if (neighbourSolidPixels >= polishThreshold)
                {
                    // aqui antes ponia a 255 , pero esto no lo puedo hacer en este tipo de textura trucada, realmente la maxima solidez es todo lo que sea mayuor que 0 asi que no hago nada
                    solidCount++;
                }
                else
                {
                    tPix[index] = 0;
                    transpCount++;
                }
            }
        }
        for (int y = pixelsAround; y < endH; y++) // no empiezo en 0 , asi no me salgo del mapa y es mas rapido, pero no pulira los contornos
        {
            for (int x = pixelsAround; x < endW; x++) // no empiezo en 0 , asi no me salgo del mapa y es mas rapido, pero no pulira los contornos
            {
                var i = x + y * width;
                pix[i] = tPix[i];
            }
        }
    }
    static int GetIndex(int x, int y, int _width){
        return x + y * _width;
    }
    static int GetSurroundingSum(Color32[] pix, int pixWidth, int pixelHeight, int x, int y, int pixelsAround) { // saca la suma del cuadrado en mapa 1
        float sum = 0;
        for (int iy = -pixelsAround; iy <= pixelsAround; iy++) // lo hago float para que s emultiplique
        {
            for (int ix = -pixelsAround; ix <= pixelsAround; ix++)
            {
                int readX = x + ix;
                int readY = y + iy; //TODO esto deberia ir arriba en el buclee iy
                int index = readX + readY * pixWidth; //  GetIndex(readX,readY,pixWidth);
                sum += pix[index].a;
            }
        }
        return (int)sum;
    }
    static int GetSurroundingSum(byte[] pix, int pixWidth, int pixelHeight, int x, int y, int pixelsAround) { // saca la suma del cuadrado en mapa 1
        int sum = 0;
        for (int iy = -pixelsAround; iy <= pixelsAround; iy++) // lo hago float para que s emultiplique
        {
            int readY = y + iy; //TODO esto deberia ir arriba en el buclee iy
            for (int ix = -pixelsAround; ix <= pixelsAround; ix++)
            {
                int readX = x + ix;
                int index = readX + readY * pixWidth; //  GetIndex(readX,readY,pixWidth);
                byte b = pix[index];
                if (b > 0)
                    sum += 255; // por simplicidad sigo usando valor 255 como el maximo asi el codigo que tenia hecho para texturas normales sigue valiendo
            }
        }
        return sum;
    }
    public static List<KuchoGrayScaleTextureCompressionData> CompressKuchoGrayScaleTexture(Texture2D tex){
        if (tex)
        {
	        if (tex.format == TextureFormat.Alpha8)
	        {
		        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
		        sw.Start();
		        
		        List<KuchoGrayScaleTextureCompressionData> compressedTexData = new List<KuchoGrayScaleTextureCompressionData>();
		        byte[] data = tex.GetRawTextureData(); // pillo los datos de la textura, supuestamente en unity 2018 ya no produce memory alloc
		        int size = tex.width * tex.height;
		        int i = 0;
		        ushort repeats = 0;
		        while (i < size)
		        {
			        while (i < size && data[i] == 0 && repeats < ushort.MaxValue)
			        {
				        repeats++;
				        i++;
			        }

			        if (repeats > 0)
			        {
				        compressedTexData.Add(new KuchoGrayScaleTextureCompressionData(repeats)); // zeros contabilizados
				        repeats = 0;
			        }

			        int start = i;
			        while (i < size && data[i] != 0 && repeats < ushort.MaxValue)
			        {
				        repeats++;
				        i++;
			        }

			        if (repeats > 0)
			        {
				        byte[] nonZeros = new byte[repeats];
				        for (int j = 0; j < repeats; j++)
				        {
					        nonZeros[j] = data[j + start];
				        }

				        compressedTexData.Add(new KuchoGrayScaleTextureCompressionData(nonZeros));
				        repeats = 0;
			        }
		        }
		        sw.Stop();
		        Debug.Log(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + "COMPRESSION OF BACKGROUND TO KUCHO GREYSCALE FORMAT ms=" + sw.ElapsedMilliseconds);
		        return compressedTexData;
	        }
	        else
	        {
		        Debug.LogError("FORMATO INCORRECTO AL COMPRIMIR TEXTURA, TEX.FORMAT= " + tex.format);
	        }
        }
        else
        {
	        Debug.LogError("ALGO HA IDO MAL AL COMPRIMIR TEXTURA , TEX = " + tex);
        }
        return null;
    }
}