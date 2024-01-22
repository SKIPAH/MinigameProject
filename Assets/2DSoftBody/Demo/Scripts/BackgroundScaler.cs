using UnityEngine;

namespace SoftBody2D.Demo
{
	public class BackgroundScaler : MonoBehaviour
	{
		private SpriteRenderer background;
		private Vector2 sizeInWorld;
		private const float Offset = 0.01f;

		void Awake()
		{
			background = GetComponent<SpriteRenderer>();

			var mainCamera = Camera.allCameras[0];
			var v1 = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));
			var v2 = mainCamera.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
			var v3 = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0));
			sizeInWorld = new Vector2(v1.x - v3.x, v2.y - v3.y);

			if (background.bounds.size.y < sizeInWorld.y)
			{
				background.transform.localScale *= sizeInWorld.y / background.bounds.size.y + Offset;
			}
			else if (background.bounds.size.x < sizeInWorld.x)
			{
				background.transform.localScale *= sizeInWorld.x / background.bounds.size.x + Offset;
			}
		}
	}
}