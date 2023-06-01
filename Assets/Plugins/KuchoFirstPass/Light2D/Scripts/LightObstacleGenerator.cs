using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Light2D
{
    /// <summary>
    /// That class is generating obstacles for object it attached to.
    /// Obect must have MeshRenderer, SpriteRenderer or CustomSprite script from which texture for obstacle will be grabbed.
    /// For rendering obstacle of SpriteRenderer and CustomSprite LightObstacleSprite with material "Material" (material with dual color shader by default) will be used.
    /// For objects with MeshRenderer "Material" property is ignored. MeshRenderer.sharedMaterial is used instead.
    /// </summary>
    [ExecuteInEditMode]
    public class LightObstacleGenerator : MonoBehaviour
    {
        /// <summary>
        /// Vertex color.
        /// </summary>
        public Color MultiplicativeColor = new Color(0, 0, 0, 1);

        /// <summary>
        /// AdditiveColor that will be used for obstacle when SpriteRenderer or CustomSprite scripts is attached.
        /// Only DualColor shader supports additive color.
        /// </summary>
        public Color AdditiveColor;

        /// <summary>
        /// Material that will be used for obstacle when SpriteRenderer or CustomSprite scripts is attached.
        /// </summary>
        public Material Material;

        public float LightObstacleScale = 1;

		public void Start() // KUCHO HACK la hice publica para llamarla manualmente, no se ejecuta dos veces por que se autodestruye
        {
			#if UNITY_EDITOR
            if (Material == null)
            {
				Material = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Light2D/Materials/DualColor.mat");
            }
			#endif

	        /*if (!Constants.appIsEditor || Constants.appIsEditor) // TODO elimina la segunda, esto es asi para probar y para que hno se me olvide
		        if (Material)
			        Material.shader = ShaderDataBase.instance.GetShaderFromList(Material.shader); 
			*/
	        
			// si no esamos en modo play no hagas nada
            if (!Application.isPlaying)
                return;

			// crea un gameObject vacio para albergar el lightobstacle
			var obstacleObj = new GameObject(gameObject.name + " Light Obstacle");
            obstacleObj.transform.parent = gameObject.transform;
            obstacleObj.transform.localPosition = Constants.zero3;
            obstacleObj.transform.localRotation = Constants.zeroQ;
            obstacleObj.transform.localScale = Vector3.one*LightObstacleScale;

			if (LightingSystem.Instance != null)// si existe una instancia de LightingSystem (colo puede ocurrir cuando estamos en play)
                obstacleObj.layer = LightingSystem.Instance.LightObstaclesLayer; //asigna la layer correcta segun el lighting system

            if (GetComponent<SpriteRenderer>() != null || GetComponent<CustomSprite>() != null) // si tenemos renderer tipo sprite valido
            {
//				print ( this + " AÑADIENDO UN LIGHT OBSTALE SPRITE");
                var obstacleSprite = obstacleObj.AddComponent<LightObstacleSprite>();
                obstacleSprite.Color = MultiplicativeColor;
                obstacleSprite.AdditiveColor = AdditiveColor;
                obstacleSprite.Material = Material;
            }
            else
            {
//				print ( this + " AÑADIENDO UN LIGHT OBSTALE MESH");
				var obstacleMesh = obstacleObj.AddComponent<LightObstacleMesh>();
                obstacleMesh.MultiplicativeColor = MultiplicativeColor;
                obstacleMesh.AdditiveColor = AdditiveColor;
                obstacleMesh.Material = Material;
            }

            Destroy(this);
        }
    }
}
