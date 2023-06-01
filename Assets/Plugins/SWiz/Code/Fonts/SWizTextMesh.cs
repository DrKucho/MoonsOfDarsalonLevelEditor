using UnityEngine;
using System.Collections;

[System.Serializable]
public class SWizTextMeshData
{
	public int version = 0;

	public SWizFontData font;
	public string text = ""; 
	public Color color = Color.white; 
	public Color color2 = Color.white; 
	public bool useGradient = false; 
	public int textureGradient = 0;
	public TextAnchor anchor = TextAnchor.LowerLeft; 
	public int renderLayer = 0;
	public Vector3 scale = Vector3.one; 
	public bool kerning = false; 
	public int maxChars = 16; 
	public bool inlineStyling = false;

	public bool formatting = false; 
	public int wordWrapWidth = 0; 

	public float spacing = 0.0f;
	public float lineSpacing = 0.0f;
}

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
/// <summary>
/// Text mesh
/// </summary>
public class SWizTextMesh : MonoBehaviour
{
	SWizFontData _fontInst;
	string _formattedText = "";

	// This stuff now kept in SWizTextMeshData. Remove in future version.
	[SerializeField] SWizFontData _font = null;
	[SerializeField] string _text = ""; 
	[SerializeField] Color _color = Color.white; 
	[SerializeField] Color _color2 = Color.white; 
	[SerializeField] bool _useGradient = false; 
	[SerializeField] int _textureGradient = 0;
	[SerializeField] TextAnchor _anchor = TextAnchor.LowerLeft; 
	[SerializeField] Vector3 _scale = new Vector3(1.0f, 1.0f, 1.0f); 
	[SerializeField] bool _kerning = false; 
	[SerializeField] int _maxChars = 16; 
	[SerializeField] bool _inlineStyling = false;
	
	[SerializeField] bool _formatting = false; 
	[SerializeField] int _wordWrapWidth = 0; 

	[SerializeField] float spacing = 0.0f;
	[SerializeField] float lineSpacing = 0.0f;

	[SerializeField] SWizTextMeshData data = new SWizTextMeshData();

	public string FormattedText {
		get {return _formattedText;}
	}
	
	Vector3[] vertices;
	Vector2[] uvs;
	Vector2[] uv2;
	Color32[] colors;
	Color32[] untintedColors;
	


	void FormatText() {
		FormatText(ref _formattedText, data.text);
	}

	void FormatText(ref string _targetString, string _source)
	{

	}

	[System.FlagsAttribute]
	enum UpdateFlags
	{
		UpdateNone		= 0,
		UpdateText		= 1,	
		UpdateColors	= 2,	
		UpdateBuffers	= 4,	
	};
	UpdateFlags updateFlags = UpdateFlags.UpdateBuffers;

	Mesh mesh;
	MeshFilter meshFilter;
	
	public SWizFontData font 
	{ 
		get {  return data.font; } 
		set 
		{ 
			data.font = value; 
			_fontInst = data.font.inst;

		} 
	}

	public bool formatting
	{
		get { return data.formatting; }
		set
		{
			if (data.formatting != value)
			{
				data.formatting = value;
			}
		}
	}

	public int wordWrapWidth
	{
		get {  return data.wordWrapWidth; }
		set {  if (data.wordWrapWidth != value) { data.wordWrapWidth = value;  } }
	}

	public string text 
	{ 
		get { return data.text; } 
		set 
		{
			data.text = value;
		}
	}

	public Color color { get {  return data.color; } set {  data.color = value;  } }
	public Color color2 { get {  return data.color2; } set {  data.color2 = value;  } }
	public bool useGradient { get {  return data.useGradient; } set {  data.useGradient = value; } }
	public TextAnchor anchor { get {  return data.anchor; } set {  data.anchor = value; } }
	public Vector3 scale { get {  return data.scale; } set {  data.scale = value;  } }
	public bool kerning { get {  return data.kerning; } set {  data.kerning = value;  } }
	public int maxChars { get {  return data.maxChars; } set {  data.maxChars = value; } }
	public int textureGradient { get {  return data.textureGradient; } set {  data.textureGradient = value % font.gradientCount;  } }
	public bool inlineStyling { get {  return data.inlineStyling; } set {  data.inlineStyling = value;  } }
	public float Spacing { get {  return data.spacing; } set {  if (data.spacing != value) { data.spacing = value; } } }
	public float LineSpacing { get {  return data.lineSpacing; } set {  if (data.lineSpacing != value) { data.lineSpacing = value;  } } }
	

	void InitInstance()
	{
		if (data != null && data.font != null) {
			_fontInst = data.font.inst;
			_fontInst.InitDictionary();
		}
	}

	Renderer _cachedRenderer = null;
	Renderer CachedRenderer {
		get {
			if (_cachedRenderer == null) {
				_cachedRenderer = GetComponent<Renderer>();
			}
			return _cachedRenderer;
		}
	}



	
	bool useInlineStyling { get { return inlineStyling && _fontInst.textureGradients; } }
	


	

}

