using System;
using UnityEngine;
using UnityEditor;

namespace SoftBody2D.Editor
{
	[CustomEditor(typeof(SoftSprite)), CanEditMultipleObjects]
	public class SoftSpriteInspector : UnityEditor.Editor
	{
		SerializedProperty texture;
		SerializedProperty scale;
		SerializedProperty material;
		SerializedProperty sortingOrder;
		SerializedProperty pixelPerMeter;
		SerializedProperty density;
		SerializedProperty color;

		private SoftSprite sprite;
		private string[] sortingLayerNames;
		private string currentSortingLayer = "Default";

		[MenuItem("Component/Rendering/Soft Sprite")]
		static void AddSoftSprite()
		{
			Selection.activeGameObject.AddComponent<SoftSprite>();
		}

		void OnEnable()
		{
			texture = serializedObject.FindProperty("Sprite");
			scale = serializedObject.FindProperty("Scale");
			material = serializedObject.FindProperty("SpriteMaterial");
			sortingOrder = serializedObject.FindProperty("SortingOrder");
			pixelPerMeter = serializedObject.FindProperty("PixelPerMeter");
			density = serializedObject.FindProperty("Density");
			color = serializedObject.FindProperty("Color");
			
			sortingLayerNames = new string[SortingLayer.layers.Length];
			for (var i = 0; i < SortingLayer.layers.Length; i++)
			{
				var layer = SortingLayer.layers[i];
				sortingLayerNames[i] = layer.name;
			}
			sprite = target as SoftSprite;
			if (sprite != null)
			{
				currentSortingLayer = sprite.GetComponent<MeshRenderer>().sortingLayerName;
			}
		}

		private int GetLayerIndex(string layerToFind)
		{
			if (sortingLayerNames.Length == 0 || sortingLayerNames == null)
				return -1;
			for (var i = 0; i < sortingLayerNames.Length; i++)
			{
				if (sortingLayerNames[i].Equals(layerToFind))
					return i;
			}
			return -1;
		}

		private void UpdateSprite(SoftSprite softSprite, bool spriteChanged, bool scaleChanged, bool materialChanged, 
			bool sortingLayerChanged, bool sortingOrderChanged, bool pixelPerMeterChanged, bool densityChanged, bool colorChanged)
		{
			SoftBodyEditorTools.RecordObject(softSprite, false);
			if (softSprite != null)
			{
				try
				{
					if (spriteChanged)
						softSprite.Sprite = texture.objectReferenceValue as Sprite;
					if (scaleChanged)
						softSprite.Scale = scale.vector2Value;
					if (materialChanged)
						softSprite.SpriteMaterial = material.objectReferenceValue as Material;
					if (sortingLayerChanged)
						softSprite.SortingLayer = currentSortingLayer;
					if (sortingOrderChanged)
						softSprite.SortingOrder = sortingOrder.intValue;
					if (pixelPerMeterChanged)
						softSprite.PixelPerMeter = pixelPerMeter.floatValue;
					if (densityChanged)
						softSprite.Density = density.intValue;
					if (colorChanged)
						softSprite.Color = color.colorValue;
					softSprite.ForceUpdate();
				}
				catch
				{
					//ignored
				}
			}
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			if (texture.objectReferenceValue != null)
			{
				SoftBodyEditorTools.DrawHorizontalLine();
				var rect = GUILayoutUtility.GetRect(160, 120, GUILayout.ExpandWidth(true));
				var unitySprite = texture.objectReferenceValue as Sprite;
				var missMatchType = ReferenceEquals(unitySprite, null);
				if (missMatchType)
				{
					texture.objectReferenceValue = null;
					serializedObject.ApplyModifiedProperties();
				}
				else
				{
					GUI.DrawTexture(rect, unitySprite.texture, ScaleMode.ScaleToFit);
					if (unitySprite.packed)
					{
						EditorGUILayout.HelpBox("Please, remove packing tag from Sprite. Packed atlases are not supported!", MessageType.Warning);
					}
				}
				SoftBodyEditorTools.DrawHorizontalLine();
			}

			EditorGUI.BeginChangeCheck();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(texture, new GUIContent("Sprite"));
			var spriteChanged = EditorGUI.EndChangeCheck();
			
			EditorGUI.BeginChangeCheck();
			scale.vector2Value = EditorGUILayout.Vector2Field("Scale", scale.vector2Value);
			var scaleChanged = EditorGUI.EndChangeCheck();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(material, new GUIContent("Material"));
			var materialChanged = EditorGUI.EndChangeCheck();

			var sortingLayerIndex = GetLayerIndex(currentSortingLayer);
			EditorGUI.BeginChangeCheck();
			sortingLayerIndex = EditorGUILayout.Popup("Sorting Layer", sortingLayerIndex, sortingLayerNames, GUILayout.ExpandWidth(true));
			var sortingLayerChanged = EditorGUI.EndChangeCheck();
			currentSortingLayer = sortingLayerNames[sortingLayerIndex];

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(sortingOrder, new GUIContent("Sorting Order"));
			var sortingOrderChanged = EditorGUI.EndChangeCheck();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.Slider(pixelPerMeter, 1f, 100f, new GUIContent("Pixel Per Meter"));
			var pixelPerMeterChanged = EditorGUI.EndChangeCheck();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.IntSlider(density, 1, 20, new GUIContent("Density"));
			var densityChanged = EditorGUI.EndChangeCheck();

			EditorGUI.BeginChangeCheck();
			color.colorValue = EditorGUILayout.ColorField("Color", color.colorValue);
			var colorChanged = EditorGUI.EndChangeCheck();
			
			var changed = EditorGUI.EndChangeCheck();

			serializedObject.ApplyModifiedProperties();

			if (GUILayout.Button("Update") || changed)
			{
				var hasChanges = false;
				foreach (var script in targets)
				{
					var softSprite = script as SoftSprite;
					if (softSprite != null)
					{
						SoftBodyEditorTools.RecordObject(softSprite);
						UpdateSprite(softSprite, spriteChanged, scaleChanged, materialChanged, sortingLayerChanged, 
							sortingOrderChanged, pixelPerMeterChanged, densityChanged, colorChanged);
						hasChanges = true; 
					}
				}
				if (hasChanges)
				{
					Undo.FlushUndoRecordObjects();
					SoftBodyEditorTools.MarkAsDirty(targets);
					serializedObject.Update();
				}
			}
		}
	}
}