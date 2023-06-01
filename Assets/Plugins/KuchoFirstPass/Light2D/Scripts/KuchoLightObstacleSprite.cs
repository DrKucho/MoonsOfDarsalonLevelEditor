using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;


namespace Light2D
{
    /// <summary>
    /// Sprite with dual color support. Grabs sprite from GameSpriteRenderer field.
    /// </summary>
    [ExecuteInEditMode]
    public class KuchoLightObstacleSprite : MonoBehaviour
    {
        public Color AdditiveColor;
//		private D2D_DestructibleSprite _spriteD2D; // KUCHO HACK
        [ReadOnly2Attribute] public SpriteRenderer _sprRenderer;
        [FormerlySerializedAs("_spriteD2D")] [ReadOnly2Attribute] public TS_DynamicSprite spriteTs;

        #if UNITY_EDITOR
        public void InitialiseInEditor()
        {
            // antes todos eran Mesh Renderers no se por que , pero creo que eso provocaba el TILE GRID CRASH , ahora son todos sprites, esto podria destruir algunos light obstacles obstaculo y recrearlos como sprites pero mal
            var _meshRenderer = GetComponent<MeshRenderer>();
            var _meshFilter = GetComponent<MeshFilter>();
            DestroyImmediate(_meshRenderer);
            DestroyImmediate(_meshFilter);

            if (transform.parent)
                spriteTs = transform.parent.GetComponent<TS_DynamicSprite>();

            _sprRenderer = GetComponent<SpriteRenderer>();
            if (!_sprRenderer)
            {

                _sprRenderer = gameObject.AddComponent<SpriteRenderer>();
            }

            if (!_sprRenderer.sprite)
            {
                var guids = AssetDatabase.FindAssets("SinglePixelBottomLeft");

                Sprite spr = null;
                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    spr = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                }

                _sprRenderer.sprite = spr;
                _sprRenderer.color = Color.black;
            }

            if (spriteTs != null)
                _sprRenderer.material = KuchoHelper.FindAndLoadMaterial("DualColorAlphatex");
            else
                _sprRenderer.material = KuchoHelper.FindAndLoadMaterial("DualColor");

        }
        #endif

        void Start()
        {
            if (LightingSystem.Instance)
				gameObject.layer = LightingSystem.Instance.LightObstaclesLayer;
        }

    }
}

