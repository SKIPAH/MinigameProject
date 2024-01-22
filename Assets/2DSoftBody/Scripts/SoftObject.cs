using UnityEngine;
using System;
using System.Collections.Generic;
using SoftBody2D.Core;
using UnityEngine.Serialization;

namespace SoftBody2D
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class SoftObject : MonoBehaviour
	{
		public enum FillType
		{
			Rectangle,
			Ellipse,
			PhysicsShape
		}

		public FillType Filling = FillType.PhysicsShape;
		public int JointsCount = 12;
		public float PathOffset;
		public float CollidersOffset = 1f;

		//Rigidbody2D
		[FormerlySerializedAs("JointsPhysicMaterial")] 
		public PhysicsMaterial2D PhysicsMaterial;
		public bool AutoMass;
		public float Mass = 1f;
		public float LinearDrag;
		public float AngularDrag = 0.05f;
		
		//Joints
		[FormerlySerializedAs("JointRadius")] 
		public float CollidersRadius = 0.5f;
		public LayerMask JointsLayer;
		public bool EnableCollision;
		public bool AutoConfigureConnectedAnchor;
		public bool AutoConfigureDistance;
		public float Distance = 0.05f;
		[FormerlySerializedAs("DampingRation")] 
		public float DampingRatio;
		public float Frequency = 10f;
		public List<SoftObjectJoint> Joints = new List<SoftObjectJoint>();
		public Action OnInitializeCompleted;

		private Transform thisTransform;
		private Rigidbody2D thisRigidbody;
		private MeshFilter meshFilter;
		private SoftSprite softSprite;
		private Mesh sharedMesh;
		[SerializeField]
		private LinesContainer linesContainer;
		private float[] vertsAffected;
		private Vector3[] startVertices;
		private bool canUpdate;
		private bool isCached;

		private const float PIFourThirds = 2.35619449019839f;

		#region Unity
		
		private void Awake()
		{
			if (enabled)
			{
				Initialize();
			}
		}
		
		private void FixedUpdate()
		{
			if (canUpdate)
			{
				var vertices = sharedMesh.vertices;
				if (Filling == FillType.Rectangle)
				{
					for (var i = 0; i < vertices.Length; i++)
					{
						var offset = Joints[i].Transform.localPosition - Joints[i].StartLocalPosition;
						vertices[i] = startVertices[i] + offset;
					}
				}
				else
				{
					var newVerts = new Vector3[vertices.Length];
					foreach (var joint in Joints)
					{
						var offset = joint.Transform.localPosition - joint.StartLocalPosition;
						for (var j = 0; j < joint.Weights.Length; j++)
						{
							newVerts[j] += offset * joint.Weights[j];
						}
					}
					for (var i = 0; i < newVerts.Length; i++)
					{
						if (vertsAffected[i] > 0f)
						{
							newVerts[i] /= vertsAffected[i];
						}
						newVerts[i] += startVertices[i];
					}
					vertices = newVerts;
				}
				sharedMesh.vertices = vertices;
			}
		}
		
		#endregion

		private void Initialize()
		{
			if (!isCached)
			{
				CacheComponents();
			}
			CheckForRemovedJoints();
			if (!HasJoints())
			{
				DestroyJoints();
				Generate(true);
			}
			foreach (var joint in Joints)
			{
				joint.StartLocalPosition = joint.Transform.localPosition;
			}
			CalcJointsWeights();
			CalcVertsWeight();
			IgnoreCollisions();
			if (OnInitializeCompleted != null)
			{
				OnInitializeCompleted();
			}
			canUpdate = true;
		}

		private void CacheComponents()
		{
			thisTransform = transform;
			thisRigidbody = GetComponent<Rigidbody2D>();
			meshFilter = GetComponent<MeshFilter>();
			softSprite = GetComponent<SoftSprite>();
			sharedMesh = meshFilter.sharedMesh;
			startVertices = sharedMesh.vertices;
			isCached = true;
		}

		private void CheckForRemovedJoints()
		{
			Joints.RemoveAll(x => x.GameObject == null);
		}
		
		/// <summary>
		/// Check if this SoftObject component has joints
		/// </summary>
		/// <returns></returns>
		private bool HasJoints()
		{
			if (Joints == null)
				return false;
			var hasJoints = true;
			foreach (var joint in Joints)
			{
				if (joint == null)
				{
					hasJoints = false;
					break;
				}
			}
			return Joints != null && !(Joints.Count > 0 && !hasJoints || Joints.Count == 0);
		}

		private void Generate(bool regenerate)
		{
			var size = sharedMesh.bounds.size;
			var meshQuadSize = GetMeshQuadSize();
			var jointsHorizontalCount = (int) Mathf.Round(size.x / meshQuadSize.x) + 1;
			var jointsVerticalCount = (int) Mathf.Round(size.y / meshQuadSize.y) + 1;
			if (Filling == FillType.Rectangle)
			{
				var scaledSize = size - new Vector3(CollidersRadius * 2f, CollidersRadius * 2f);
				var offset = scaledSize / 2f;
				FillGrid(regenerate, jointsHorizontalCount, jointsVerticalCount, scaledSize, offset);
			}
			else if (Filling == FillType.Ellipse)
			{
				FillCircle(regenerate, jointsHorizontalCount, jointsVerticalCount);
			}
			else
			{
				FillPhysicalShape(regenerate);
			}
		}
		
		private Vector2 GetMeshQuadSize()
		{
			var vertices = meshFilter.sharedMesh.vertices;
			var triangles = meshFilter.sharedMesh.triangles;
			var cell = new Vector2(vertices[triangles[1]].x - vertices[triangles[0]].x, vertices[triangles[0]].y - vertices[triangles[2]].y);
			return cell;
		}

		private void FillGrid(bool regenerate, int jointsHorizontalCount, int jointsVerticalCount, Vector2 scaledSize, Vector2 offset)
		{
			for (int y = 0, ti = 0; y < jointsVerticalCount; y++)
			{
				for (var x = 0; x < jointsHorizontalCount; x++, ti++)
				{
					var jointPosition = new Vector2(
						scaledSize.x * x / (jointsHorizontalCount - 1),
						scaledSize.y * y / (jointsVerticalCount - 1)) - offset;
					if (regenerate)
					{
						CreateJoint(ti, jointPosition);
					}
					else
					{
						UpdateJoint(Joints[ti], jointPosition);
					}
				}
			}
		}

		private void FillCircle(bool regenerate, int jointsHorizontalCount, int jointsVerticalCount)
		{
			var length = Mathf.Max(jointsHorizontalCount, jointsVerticalCount);
			var jointsCount = (int) Mathf.Round(length * PIFourThirds);
			var size = sharedMesh.bounds.size;
			var offset = new Vector2(size.x / 2f - CollidersRadius, size.y / 2f - CollidersRadius);
			for (var i = 0; i < jointsCount; i++)
			{
				var angle = (i + 1) / (float) jointsCount * Mathf.PI * 2f;
				var jointPosition = new Vector3(Mathf.Cos(angle) * offset.x, Mathf.Sin(angle) * offset.y);
				if (regenerate)
				{
					CreateJoint(i, jointPosition);
				}
				else
				{
					UpdateJoint(Joints[i], jointPosition);
				}
			}
		}

		private void FillPhysicalShape(bool regenerate)
		{
			InitLines(true);
			var lengthsSum = linesContainer.GetLength();
			var id = 0;
			var offset = lengthsSum / JointsCount;
			for (var i = 0; i < JointsCount; i++)
			{
				var line = linesContainer.GetLine(offset * i, PathOffset);
				var start = line.Sum - line.Length;
				var currentPos = offset * i + PathOffset;
				if (currentPos > lengthsSum)
				{
					currentPos -= lengthsSum;
				}
				var lerp = (currentPos - start) / (line.Sum - start);
				var pos = Vector2.Lerp(line.Start, line.End, lerp);
				// var perpendicular = Vector2.Perpendicular(line.End - line.Start).normalized;
				var vNormalized = -(line.End - line.Start).normalized;
				var perpendicular = new Vector2(vNormalized.y, -vNormalized.x).normalized;
				var position = Vector2.Scale(perpendicular * (CollidersRadius / softSprite.Scale.x) * CollidersOffset + pos, softSprite.Scale);
				if (regenerate)
				{
					CreateJoint(id, position);
					id++;
				}
				else
				{
					UpdateJoint(Joints[i], position);
				}
			}
		}

		private void CreateJoint(int id, Vector2 jointPosition)
		{
			var gJoint = new GameObject("Joint" + (id + 1));
			gJoint.transform.parent = thisTransform;
			gJoint.transform.localScale = Vector3.one;
			var joint = gJoint.AddComponent<SpringJoint2D>();
			var jointRigidBody = joint.GetComponent<Rigidbody2D>();
			var circleCollider = gJoint.AddComponent<CircleCollider2D>();
			var mJoint = new SoftObjectJoint
			{
				Collider = circleCollider,
				Joint = joint,
				GameObject = gJoint,
				Rigidbody2D = jointRigidBody,
				Transform = gJoint.transform,
				Weights = new float[0]
			};
			UpdateJoint(mJoint, jointPosition);
#if UNITY_EDITOR
			UnityEditor.Undo.RegisterCreatedObjectUndo(gJoint, "Update SoftObject");
#endif
			Joints.Add(mJoint);
		}

		private void UpdateJoint(SoftObjectJoint softObjectJoint, Vector3? localPosition = null, float[] weights = null)
		{
			if (softObjectJoint == null || softObjectJoint.GameObject == null)
				return;
			softObjectJoint.GameObject.layer = JointsLayer;
			softObjectJoint.Rigidbody2D.useAutoMass = AutoMass;
			if (!AutoMass)
			{
				softObjectJoint.Rigidbody2D.mass = Mass;
			}
			softObjectJoint.Rigidbody2D.drag = LinearDrag;
			softObjectJoint.Rigidbody2D.angularDrag = AngularDrag;
			softObjectJoint.Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
			softObjectJoint.Joint.enableCollision = EnableCollision;
			softObjectJoint.Joint.connectedBody = thisRigidbody;
			var springJoint = softObjectJoint.Joint as SpringJoint2D;
			if (springJoint != null)
			{
				springJoint.autoConfigureConnectedAnchor = AutoConfigureConnectedAnchor;
				springJoint.autoConfigureDistance = AutoConfigureDistance;
				springJoint.distance = Distance;
				springJoint.frequency = Frequency;
				springJoint.dampingRatio = DampingRatio;
			}
			softObjectJoint.Collider.radius = CollidersRadius;
			softObjectJoint.Collider.sharedMaterial = PhysicsMaterial;
			if (localPosition != null)
			{
				softObjectJoint.Transform.localPosition = localPosition.Value;
				if (springJoint != null)
				{
					springJoint.connectedAnchor = localPosition.Value;
				}
			}
			if (weights != null)
			{
				softObjectJoint.Weights = weights;
			}
		}

		private void CalcJointsWeights()
		{
			if (Filling == FillType.Rectangle || Filling == FillType.PhysicsShape)
			{
				foreach (var joint in Joints)
				{
					var weights = new float[startVertices.Length];
					for (var k = 0; k < weights.Length; k++)
					{
						var distance = Vector2.Distance(startVertices[k], joint.Transform.localPosition) - CollidersRadius;
						if (distance < 0f)
						{
							distance = 1f;
						}
						else if (distance > CollidersRadius)
						{
							distance = 0f;
						}
						weights[k] = distance;
					}
					joint.Weights = weights;
				}
			}
			else
			{
				var vertsCount = sharedMesh.vertexCount;
				foreach (var joint in Joints)
				{
					var weights = new float[vertsCount];
					for (var j = 0; j < weights.Length; j++)
					{
						var distance = Vector2.Distance(startVertices[j], joint.Transform.localPosition) - CollidersRadius;
						if (distance < 0f)
						{
							distance = 1f;
						}
						else if (distance > CollidersRadius)
						{
							distance = 0f;
						}
						weights[j] = distance;
					}
					joint.Weights = weights;
				}
			}
		}
		
		private void CalcVertsWeight()
		{
			vertsAffected = new float[sharedMesh.vertices.Length];
			foreach (var joint in Joints)
			{
				for (var j = 0; j < joint.Weights.Length; j++)
				{
					if (joint.Weights[j] > 0f)
					{
						vertsAffected[j] += joint.Weights[j];
					}
				}
			}
		}

		private void IgnoreCollisions()
		{
			for (var i = 0; i < Joints.Count - 1; i++)
			{
				for (var j = i + 1; j < Joints.Count; j++)
				{
					if (Joints[i] != null && Joints[j] != null)
					{
						Physics2D.IgnoreCollision(Joints[i].Collider, Joints[j].Collider);
					}
				}
			}
		}

		/// <summary>
		/// Removes/Adds joints GameObject's in a Prefab
		/// </summary>
		private void DestroyInPrefab()
		{
#if UNITY_EDITOR && UNITY_2018_4_OR_NEWER
			var isPartOfPrefabInstance = UnityEditor.PrefabUtility.IsPartOfRegularPrefab(gameObject);
			if (isPartOfPrefabInstance)
			{
				var clone = Instantiate(gameObject);
				var objects = clone.GetComponent<SoftObject>().Joints;
				foreach (var joint in objects)
				{
					if (joint != null)
					{
						DestroyImmediate(joint.GameObject);
					}
				}
				try
				{
					var path = UnityEditor.PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);
					UnityEditor.PrefabUtility.SaveAsPrefabAsset(clone, path);
				}
				catch
				{
					//ignore
				}
				finally
				{
					DestroyImmediate(clone);
				}
				UnityEditor.Undo.ClearUndo(gameObject);
			}
#endif
		}

		private void InitLines(bool forceUpdate)
		{
			if (linesContainer == null)
			{
				linesContainer = new LinesContainer();
				linesContainer.Init(softSprite.Sprite);
			}
			else if (forceUpdate)
			{
				linesContainer.Init(softSprite.Sprite);
			}
		}

		#region Public Methods
		
		/// <summary>
		/// Generates joints
		/// </summary>
		public void GenerateJoints()
		{
			CacheComponents();
			Generate(true);
		}
				
		/// <summary>
		/// Update joints values without recreating them
		/// </summary>
		public void UpdateParams()
		{
			if (HasJoints())
			{
				CacheComponents();
				Generate(false);
			}
		}
		
		/// <summary>
		/// Removes all joints
		/// </summary>
		public void DestroyJoints()
		{
			foreach (var joint in Joints)
			{
				if (joint != null && joint.GameObject != null)
				{
#if UNITY_EDITOR && UNITY_2018_4_OR_NEWER
					var isPartOfPrefab = UnityEditor.PrefabUtility.IsPartOfRegularPrefab(joint.GameObject);
					if (!isPartOfPrefab)
					{
						UnityEditor.Undo.DestroyObjectImmediate(joint.GameObject);
					}
#elif UNITY_EDITOR
                UnityEditor.Undo.DestroyObjectImmediate(joint.GameObject);
#else
                DestroyImmediate(joint.GameObject);
#endif
				}
			}
#if UNITY_EDITOR
			DestroyInPrefab();
#endif
			Joints.Clear();
		}

		/// <summary>
		/// Re-initialize soft object
		/// </summary>
		public void ForceUpdate()
		{
			Initialize();
		}

		/// <summary>
		/// Returns physical shape total length
		/// </summary>
		/// <returns></returns>
		public float GetPhysicalShapeLength()
		{
			InitLines(false);
			return linesContainer.GetLength();
		}
		
		#endregion
	}
}