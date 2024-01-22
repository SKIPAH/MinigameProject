using UnityEngine;
using UnityEditor;

namespace SoftBody2D.Editor
{
	[CustomEditor(typeof(SoftObject)), CanEditMultipleObjects]
	public class SoftObjectInspector : UnityEditor.Editor
	{
		SerializedProperty filling;
		SerializedProperty jointsCount;
		SerializedProperty pathOffset;
		SerializedProperty collidersOffset;
		SerializedProperty physicsMaterial;
		SerializedProperty autoMass;
		SerializedProperty mass;
		SerializedProperty linearDrag;
		SerializedProperty angularDrag;
		SerializedProperty collidersRadius;
		SerializedProperty jointsLayer;
		SerializedProperty enableCollision;
		SerializedProperty autoConfigureConnectedAnchor;
		SerializedProperty autoConfigureDistance;
		SerializedProperty distance;
		SerializedProperty dampingRatio;
		SerializedProperty frequency;
		SerializedProperty joints;
		
		[MenuItem("Component/Physics 2D/Soft Object")]
		static void AddSoftObject()
		{
			Selection.activeGameObject.AddComponent<SoftObject>();
		}

		void OnEnable()
		{
			filling = serializedObject.FindProperty("Filling");
			jointsCount = serializedObject.FindProperty("JointsCount");
			pathOffset = serializedObject.FindProperty("PathOffset");
			collidersOffset = serializedObject.FindProperty("CollidersOffset");
			physicsMaterial = serializedObject.FindProperty("PhysicsMaterial");
			autoMass = serializedObject.FindProperty("AutoMass");
			mass = serializedObject.FindProperty("Mass");
			linearDrag = serializedObject.FindProperty("LinearDrag");
			angularDrag = serializedObject.FindProperty("AngularDrag");
			collidersRadius = serializedObject.FindProperty("CollidersRadius");
			jointsLayer = serializedObject.FindProperty("JointsLayer");
			enableCollision = serializedObject.FindProperty("EnableCollision");
			autoConfigureConnectedAnchor = serializedObject.FindProperty("AutoConfigureConnectedAnchor");
			autoConfigureDistance = serializedObject.FindProperty("AutoConfigureDistance");
			distance = serializedObject.FindProperty("Distance");
			dampingRatio = serializedObject.FindProperty("DampingRatio");
			frequency = serializedObject.FindProperty("Frequency");
			joints = serializedObject.FindProperty("Joints");
		}

		private void LimitPropertyValue(SerializedProperty prop, float minValue, float maxValue)
		{
			if (prop.floatValue < minValue)
			{
				prop.floatValue = minValue;
			}

			if (prop.floatValue > maxValue)
			{
				prop.floatValue = maxValue;
			}
		}
		
		private void Generate(SoftObject softObject)
		{
			if (softObject != null)
			{
				softObject.GenerateJoints();
			}
		}

		private void Remove(SoftObject softObject)
		{
			if (softObject != null)
			{
				softObject.DestroyJoints();
			}
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(filling, new GUIContent("Fill Type"));
			var fillTypeChanged = EditorGUI.EndChangeCheck();
			
			EditorGUI.BeginChangeCheck();
			var jointsCountChanged = false;
			var positionOffsetChanged = false;
			var collidersOffsetChanged = false;
			if ((SoftObject.FillType) filling.enumValueIndex == SoftObject.FillType.PhysicsShape)
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.IntSlider(jointsCount, 1, 100, new GUIContent("Joints Count"));
				jointsCountChanged = EditorGUI.EndChangeCheck();
				
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Slider(pathOffset, 0f, ((SoftObject)target).GetPhysicalShapeLength(), new GUIContent("Path Offset"));
				positionOffsetChanged = EditorGUI.EndChangeCheck();
				
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.Slider(collidersOffset, 0.0001f, 3f, new GUIContent("Colliders Offset"));
				collidersOffsetChanged = EditorGUI.EndChangeCheck();
			}
			
			SoftBodyEditorTools.DrawHorizontalLine();
			
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(physicsMaterial, new GUIContent("Physics Material"));
			var physicsMaterialChanged = EditorGUI.EndChangeCheck();
			
			EditorGUI.BeginChangeCheck();
			autoMass.boolValue = EditorGUILayout.Toggle("Use Auto Mass", autoMass.boolValue);
			var autoMassChanged = EditorGUI.EndChangeCheck();
			var massChanged = false;
			if (!autoMass.boolValue)
			{
				EditorGUI.BeginChangeCheck();
				mass.floatValue = EditorGUILayout.FloatField(new GUIContent("Mass"), mass.floatValue);
				LimitPropertyValue(mass, 0.0001f, 1000000f);
				massChanged = EditorGUI.EndChangeCheck();
			}

			EditorGUI.BeginChangeCheck();
			linearDrag.floatValue = EditorGUILayout.FloatField(new GUIContent("Linear Drag"), linearDrag.floatValue);
			LimitPropertyValue(linearDrag, 0f, 1000000f);
			var linearDragChanged = EditorGUI.EndChangeCheck();

			EditorGUI.BeginChangeCheck();
			angularDrag.floatValue = EditorGUILayout.FloatField(new GUIContent("Angular Drag"), angularDrag.floatValue);
			LimitPropertyValue(angularDrag, 0f, 1000000f);
			var angularDragChanged = EditorGUI.EndChangeCheck();

			SoftBodyEditorTools.DrawHorizontalLine();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.Slider(collidersRadius, 0.0001f, GetMaxRadius(), new GUIContent("Colliders Radius"));
			var colliderRadiusChanged = EditorGUI.EndChangeCheck();
			
			EditorGUI.BeginChangeCheck();
			jointsLayer.intValue = EditorGUILayout.LayerField("Joints Layer", jointsLayer.intValue);
			var jointsLayerChanged = EditorGUI.EndChangeCheck();

			EditorGUI.BeginChangeCheck();
			enableCollision.boolValue = EditorGUILayout.Toggle("Enable Collision", enableCollision.boolValue);
			var enableCollisionChanged = EditorGUI.EndChangeCheck();

			EditorGUI.BeginChangeCheck();
			autoConfigureConnectedAnchor.boolValue = EditorGUILayout.Toggle("Auto Configure Connected Anchor", autoConfigureConnectedAnchor.boolValue);
			var autoConfigureConnectedAnchorChanged = EditorGUI.EndChangeCheck();

			EditorGUI.BeginChangeCheck();
			autoConfigureDistance.boolValue = EditorGUILayout.Toggle("Auto Configure Distance", autoConfigureDistance.boolValue);
			var autoConfigureDistanceChanged = EditorGUI.EndChangeCheck();

			var distanceChanged = false;
			if (!autoConfigureDistance.boolValue)
			{
				EditorGUI.BeginChangeCheck();
				distance.floatValue = EditorGUILayout.FloatField(new GUIContent("Distance"), distance.floatValue);
				LimitPropertyValue(distance, 0.005f, 1000000f);
				distanceChanged = EditorGUI.EndChangeCheck();
			}

			EditorGUI.BeginChangeCheck();
			dampingRatio.floatValue = EditorGUILayout.FloatField(new GUIContent("Damping Ratio"), dampingRatio.floatValue);
			LimitPropertyValue(dampingRatio, 0, 1f);
			var dampingRatioChanged = EditorGUI.EndChangeCheck();

			EditorGUI.BeginChangeCheck();
			frequency.floatValue = EditorGUILayout.FloatField(new GUIContent("Frequency"), frequency.floatValue);
			LimitPropertyValue(frequency, 0, 1000000f);
			var frequencyChanged = EditorGUI.EndChangeCheck();

			var prefabSupported = SoftBodyEditorTools.IsPrefabsSupported();
			var isGameObjectPrefab = SoftBodyEditorTools.IsGameObjectPrefab(((SoftObject)target).gameObject);
			if (!prefabSupported && isGameObjectPrefab)
			{
				EditorGUILayout.HelpBox("Asset does not support joints generation using prefabs in Unity older than 2018.4.0", MessageType.Warning);
			}
			SoftBodyEditorTools.DrawHorizontalLine();
			var regenerate = fillTypeChanged || jointsCountChanged;
			if (EditorGUI.EndChangeCheck() || regenerate)
			{
				foreach (var script in targets)
				{
					var softObject = script as SoftObject;
					if (softObject != null)
					{
						UpdateSoftObject(softObject, regenerate, fillTypeChanged, jointsCountChanged, positionOffsetChanged, collidersOffsetChanged, 
							physicsMaterialChanged, autoMassChanged, massChanged, linearDragChanged, angularDragChanged, colliderRadiusChanged, jointsLayerChanged, 
							enableCollisionChanged, autoConfigureConnectedAnchorChanged, autoConfigureDistanceChanged, distanceChanged, dampingRatioChanged, frequencyChanged);
					}
				}
				if (regenerate)
				{
					Undo.FlushUndoRecordObjects();
				}
				SoftBodyEditorTools.MarkAsDirty(targets);
			}
			EditorGUILayout.PropertyField(joints, new GUIContent("Joints"));
			EditorGUI.BeginDisabledGroup(Application.isPlaying || !prefabSupported && isGameObjectPrefab);
			if (GUILayout.Button("Generate Joints"))
			{
				var hasChanges = false;
				foreach (var script in targets)
				{
					var softObject = script as SoftObject;
					if (softObject != null)
					{
						if (prefabSupported || !SoftBodyEditorTools.IsGameObjectPrefab(softObject.gameObject))
						{
							SoftBodyEditorTools.RecordObject(softObject);
							Remove(softObject);
							Generate(softObject);
							hasChanges = true;
						}
					}
				}
				if (hasChanges)
				{
					Undo.FlushUndoRecordObjects();
					SoftBodyEditorTools.MarkAsDirty(targets);
					serializedObject.Update();
				}
			}
			if (GUILayout.Button("Remove Joints"))
			{
				var hasChanges = false;
				foreach (var script in targets)
				{
					var softObject = script as SoftObject;
					if (softObject != null)
					{
						if (prefabSupported || !SoftBodyEditorTools.IsGameObjectPrefab(softObject.gameObject))
						{
							SoftBodyEditorTools.RecordObject(softObject);
							Remove(softObject);
							hasChanges = true;
						}
					}
				}
				if (hasChanges)
				{
					Undo.FlushUndoRecordObjects();
					SoftBodyEditorTools.MarkAsDirty(targets);
					serializedObject.Update();
				}
			}
			EditorGUI.EndDisabledGroup();
			
			serializedObject.ApplyModifiedProperties();
		}

		private void UpdateSoftObject(SoftObject softObject, bool regenerateCompletely, bool fillingChanged, bool jointsCountChanged, 
			bool pathOffsetChanged, bool collidersOffsetChanged, bool physicsMaterialChanged, bool autoMassChanged, bool massChanged, 
			bool linearDragChanged,  bool angularDragChanged, bool colliderRadiusChanged, bool jointsLayerChanged, bool enableCollisionChanged, 
			bool autoConfigureConnectedAnchorChanged, bool autoConfigureDistanceChanged, bool distanceChanged, bool dampingRatioChanged, bool frequencyChanged)
		{
			SoftBodyEditorTools.RecordObject(softObject, false);
			if (fillingChanged) 
				softObject.Filling = (SoftObject.FillType) filling.enumValueIndex;
			if (jointsCountChanged) 
				softObject.JointsCount = jointsCount.intValue;
			if (pathOffsetChanged)
				softObject.PathOffset = pathOffset.floatValue;
			if (collidersOffsetChanged)
				softObject.CollidersOffset = collidersOffset.floatValue;
			if (physicsMaterialChanged)
				softObject.PhysicsMaterial = physicsMaterial.objectReferenceValue as PhysicsMaterial2D;
			if (autoMassChanged)
				softObject.AutoMass = autoMass.boolValue;
			if (massChanged)
				softObject.Mass = mass.floatValue;
			if (linearDragChanged)
				softObject.LinearDrag = linearDrag.floatValue;
			if (angularDragChanged)
				softObject.AngularDrag = angularDrag.floatValue;
			if (colliderRadiusChanged)
				softObject.CollidersRadius = collidersRadius.floatValue;
			if (jointsLayerChanged)
				softObject.JointsLayer = jointsLayer.intValue;
			if (enableCollisionChanged)
				softObject.EnableCollision = enableCollision.boolValue;
			if (autoConfigureConnectedAnchorChanged)
				softObject.AutoConfigureConnectedAnchor = autoConfigureConnectedAnchor.boolValue;
			if (autoConfigureDistanceChanged)
				softObject.AutoConfigureDistance = autoConfigureDistance.boolValue;
			if (distanceChanged)
				softObject.Distance = distance.floatValue;
			if (dampingRatioChanged)
				softObject.DampingRatio = dampingRatio.floatValue;
			if (frequencyChanged)
				softObject.Frequency = frequency.floatValue;
			if (regenerateCompletely)
			{
				if (SoftBodyEditorTools.IsPrefabsSupported() || !SoftBodyEditorTools.IsGameObjectPrefab(softObject.gameObject))
				{
					SoftBodyEditorTools.RecordHierarchyUndo(softObject);
					Remove(softObject);
					Generate(softObject);
					serializedObject.Update();
				}
			}
			else
			{
				softObject.UpdateParams();
			}
		}

		private float GetMaxRadius()
		{
			var softObject = (SoftObject) target;
			var meshFilter = softObject.GetComponent<MeshFilter>();
			if (meshFilter != null && meshFilter.sharedMesh != null)
			{
				var size = meshFilter.sharedMesh.bounds.size;
				return (size.x > size.y ? size.y : size.x) / 2f;
			}
			return 100f;
		}
	}
}