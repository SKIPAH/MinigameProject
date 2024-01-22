using UnityEngine;
using UnityEditor;

namespace SoftBody2D.Editor
{
	public class SoftObjectMenu : UnityEditor.Editor
	{
		[MenuItem("GameObject/2D Object/2D Soft Body")]
		static void CreateSoftObject()
		{
			var softGameObject = new GameObject("SoftObject");
			var softSprite = softGameObject.AddComponent<SoftSprite>();
			var softObject = softGameObject.AddComponent<SoftObject>();
			Undo.RegisterCreatedObjectUndo(softGameObject, "Move Component Up");
			MoveComponentUp(softObject, 4);
			MoveComponentUp(softSprite, 3);
		}
		
		[MenuItem("CONTEXT/SpriteRenderer/Make 2D Soft Body")]
        private static void MakeSoftBody()
        {
			foreach (var gameObject in Selection.gameObjects)
            {
                if (gameObject == null)
                    continue;
                var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    var sprite = spriteRenderer.sprite;
                    var color = spriteRenderer.color;
                    var sortingLayer = spriteRenderer.sortingLayerName;
                    var sortingOrder = spriteRenderer.sortingOrder;
                    Undo.DestroyObjectImmediate(spriteRenderer);

                    var path = AssetDatabase.GetAssetPath(sprite);
                    var importer = (TextureImporter) AssetImporter.GetAtPath(path);
                    var textureImporterSettings = new TextureImporterSettings();
                    importer.ReadTextureSettings(textureImporterSettings);
                    var hasPhysicsShape = textureImporterSettings.spriteGenerateFallbackPhysicsShape;
                    if (!hasPhysicsShape)
                    {
                        textureImporterSettings.spriteGenerateFallbackPhysicsShape = true;
                        importer.SetTextureSettings(textureImporterSettings);
                        AssetDatabase.ImportAsset(path);
                        AssetDatabase.Refresh();
                    }

                    var softSprite = Undo.AddComponent(gameObject, typeof(SoftSprite)) as SoftSprite;
                    softSprite.Sprite = sprite;
                    softSprite.Color = color;
                    softSprite.SortingLayer = sortingLayer;
                    softSprite.SortingOrder = sortingOrder;
                    softSprite.ForceUpdate();
                    var softObject = Undo.AddComponent(gameObject, typeof(SoftObject)) as SoftObject;
                    softObject.ForceUpdate();
                    if (gameObject.GetComponent<Rigidbody2D>() == null)
                    {
	                    Undo.AddComponent(gameObject, typeof(Rigidbody2D));
                    }
                    MoveComponentUp(softObject, 4);
                    MoveComponentUp(softSprite, 3);
                }
            }
        }
        
		private static void MoveComponentUp(Component component, int count)
		{
			for (var i = 0; i < count; i++)
			{
				UnityEditorInternal.ComponentUtility.MoveComponentUp(component);
			}
		}
	}
}