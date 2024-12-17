using UnityEngine;
using System.Collections;

namespace FogOfWar 
{
	[RequireComponent(typeof(Camera))]
	public class FogOfWar : MonoBehaviour
	{
		public Material material;

		public FieldManager field;

		private Camera cam;

		void OnEnable()
		{
			cam = GetComponent<Camera>();
			cam.depthTextureMode = DepthTextureMode.Depth;

			material.SetMatrix("_ViewToWorld", cam.cameraToWorldMatrix);
			material.SetVector("_WorldParams", field.GetWorldParams());
		}

		void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
            material.SetMatrix("_ViewToWorld", cam.cameraToWorldMatrix);
            material.SetVector("_WorldParams", field.GetWorldParams());
            if (material != null)
			{
				Graphics.Blit(source, destination, material);
			}
			else
			{
				Graphics.Blit(source, destination);
			}
		}
	}
}

