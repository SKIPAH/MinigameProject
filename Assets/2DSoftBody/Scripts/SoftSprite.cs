using UnityEngine;
using UnityEngine.U2D;

namespace SoftBody2D
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
	public class SoftSprite : MonoBehaviour
	{
		public Sprite Sprite;
		public Vector2 Scale = Vector2.one;
		public Material SpriteMaterial;
		public float PixelPerMeter = 100f;
		public string SortingLayer;
		public int SortingOrder;
		[Range(0.05f, float.MaxValue)] 
		public int Density = 3;
		public Color Color = Color.white;

		private MeshRenderer meshRenderer;
		private MeshFilter meshFilter;
		private MaterialPropertyBlock propertyBlock;
		private Vector2 sizeWithoutScale;
		private int verticesHorizontalCount, verticesVerticalCount;
		private const string MainTexShaderField = "_MainTex";

		private void Awake()
		{
			meshFilter = GetComponent<MeshFilter>();
			meshRenderer = GetComponent<MeshRenderer>();
			CreateSpriteMaterial();
			CreateMesh();
		}

		private void CreateSpriteMaterial()
		{
			if (SpriteMaterial == null)
			{
				var shader = Shader.Find("2DSB/VertexColor");
				SpriteMaterial = new Material(shader);
			}
			meshRenderer.sharedMaterial = SpriteMaterial;
		}

		private void CreateMesh()
		{
			UpdateMesh();
			UpdateMaterialSprite();
		}

		private void UpdateMesh()
		{
			if (Sprite == null || Sprite.texture == null)
				return;

			var size = new Vector3(Sprite.texture.width / PixelPerMeter * Scale.x, Sprite.texture.height / PixelPerMeter * Scale.y);
			size = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
			if (size.x == size.y)
			{
				verticesHorizontalCount = verticesVerticalCount = Density;
			}
			else if (size.x < size.y)
			{
				verticesVerticalCount = Density;
				verticesHorizontalCount = Mathf.RoundToInt(size.x / size.y * verticesVerticalCount);
			}
			else
			{
				verticesHorizontalCount = Density;
				verticesVerticalCount = Mathf.RoundToInt(size.y / size.x * verticesHorizontalCount);
			}
			verticesHorizontalCount = verticesHorizontalCount == 0 ? 1 : verticesHorizontalCount;
			verticesVerticalCount = verticesVerticalCount == 0 ? 1 : verticesVerticalCount;
			var verticesCount = (verticesHorizontalCount + 1) * (verticesVerticalCount + 1);
			if (verticesHorizontalCount <= 0 || verticesVerticalCount <= 0 || verticesCount <= 0 || verticesCount >= 65535)
			{
				return;
			}
			var offset = size / 2f;
			var vertices = new Vector3[verticesCount];
			var triangles = new int[verticesHorizontalCount * verticesVerticalCount * 6];
			var uv = new Vector2[verticesCount];
			var colors = new Color[verticesCount];
			for (int y = 0, i = 0; y <= verticesVerticalCount; y++)
			{
				for (var x = 0; x <= verticesHorizontalCount; x++, i++)
				{
					vertices[i] = new Vector3(size.x * x / verticesHorizontalCount,
						size.y - size.y * y / verticesVerticalCount, 0) - offset;
					uv[i] = new Vector3(x / (float) verticesHorizontalCount, 1f - y / (float) verticesVerticalCount, 0);
					colors[i] = Color;
				}
			}
			for (int y = 0, ti = 0, vi = 0; y < verticesVerticalCount; y++, vi++)
			{
				for (var x = 0; x < verticesHorizontalCount; x++, ti += 6, vi++)
				{
					triangles[ti + 0] = vi;
					triangles[ti + 1] = vi + 1;
					triangles[ti + 2] = vi + verticesHorizontalCount + 1;
					triangles[ti + 3] = vi + verticesHorizontalCount + 1;
					triangles[ti + 4] = vi + 1;
					triangles[ti + 5] = vi + verticesHorizontalCount + 2;
				}
			}
			var mesh = new Mesh {vertices = vertices, triangles = triangles, uv = uv, colors = colors};
			meshFilter.sharedMesh = mesh;
		}

		private void UpdateMaterialSprite()
		{
			CreateSpriteMaterial();
			UpdatePropertyBlock();
		}

		private void UpdatePropertyBlock()
		{
			if (Sprite == null)
				return;

			if (propertyBlock == null)
			{
				propertyBlock = new MaterialPropertyBlock();
			}
			meshRenderer.sortingLayerName = SortingLayer;
			meshRenderer.sortingOrder = SortingOrder;
			meshRenderer.GetPropertyBlock(propertyBlock);
			propertyBlock.SetTexture(MainTexShaderField, Sprite.texture);
			meshRenderer.SetPropertyBlock(propertyBlock);
		}

		public void ForceUpdate()
		{
			UpdateMesh();
			UpdateMaterialSprite();
		}

		public void SetColor(Color color)
		{
			Color = color;
			SpriteMaterial.color = Color;
		}
	}
}