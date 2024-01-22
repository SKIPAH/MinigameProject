using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace SoftBody2D.Demo
{
	public class Demo : MonoBehaviour
	{
		public List<GameObject> ObjectsToMove;
		public List<SoftObject> ObjectsToClone;
		public int MaxObjectsCount;
		public float TargetJointMaxForce = 500f;
		public bool EnableSounds;
		public AudioSource CreateSource;
		public AudioClip[] CollideSounds;
		public GameObject Tip;

		private Camera thisCamera;
		private Transform firstTransform;
		private TargetJoint2D targetJoint2D;
		private Transform capturedObject;
		private Vector3 startTapPosition;
		private int currentObjectToInstantiateId;

		private int CurrentObjectToInstantiate
		{
			get
			{
				currentObjectToInstantiateId = (currentObjectToInstantiateId + 1) % ObjectsToClone.Count;
				return currentObjectToInstantiateId;
			}
		}

		void Awake()
		{
			Application.targetFrameRate = 60;
			if (ObjectsToMove.Count > 0)
			{
				firstTransform = ObjectsToMove[0].transform;
			}
			thisCamera = Camera.allCameras[0];
			if (Tip != null)
			{
				Destroy(Tip, 10f);
			}

			if (EnableSounds)
			{
				foreach (var objectToMove in ObjectsToMove)
				{
					var softObject = objectToMove.GetComponent<SoftObject>();
					if (softObject != null)
					{
						AddSoundsForCollisions(softObject);
					}
				}
			}
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
				return;
			}

			if (ObjectsToMove.Count == 0 && ObjectsToClone.Count == 0)
				return;

			if (Input.GetMouseButtonDown(0))
			{
				var position = thisCamera.ScreenToWorldPoint(Input.mousePosition);
				var haveExtraHit = false;
				var hits = new RaycastHit2D[ObjectsToMove.Count]; 
				Physics2D.RaycastNonAlloc(position, Vector2.zero, hits);
				foreach (var hit in hits)
				{
					if (hit.transform != null && (hit.transform.parent != null && ObjectsToMove.Contains(hit.transform.parent.gameObject) || ObjectsToMove.Contains(hit.transform.gameObject)) && hit.collider.isTrigger)
					{
						capturedObject = hit.transform;
						targetJoint2D = capturedObject.gameObject.AddComponent<TargetJoint2D>();
						targetJoint2D.maxForce = TargetJointMaxForce;
						break;
					}
					
					if (hit.transform != null)
					{
						haveExtraHit = true;
					}
				}
				
				if (capturedObject == null && ObjectsToMove.Count < MaxObjectsCount && !haveExtraHit)
				{
					if (Physics2D.CircleCastAll(position, 0.5f, Vector2.zero).Length == 0)
					{
						var newObject = Instantiate(ObjectsToClone[CurrentObjectToInstantiate]) as SoftObject;
						CreateSource.Play();
						if (newObject != null)
						{
							newObject.transform.position = new Vector3(position.x, position.y, firstTransform.position.z);
							newObject.transform.parent = firstTransform.parent;
							ObjectsToMove.Add(newObject.gameObject);
							if (EnableSounds)
								AddSoundsForCollisions(newObject);
						}
					}
				}
			}

			if (Input.GetMouseButton(0))
			{
				if (capturedObject != null)
				{
					var position = thisCamera.ScreenToWorldPoint(Input.mousePosition);
					// Debug.Log(position.magnitude);
					// Debug.Log(targetJoint2D.anchor.magnitude);
					// Debug.Log(targetJoint2D.reactionForce.magnitude);
					// if (targetJoint2D.reactionForce.magnitude > 200f)
					// {
					// 	position *= 200f / targetJoint2D.reactionForce.magnitude;
					// }
					targetJoint2D.target = position;
				}
			}

			if (Input.GetMouseButtonUp(0))
			{
				if (capturedObject != null)
				{
					var body = capturedObject.GetComponent<Rigidbody2D>();
					if (body != null)
					{
						body.velocity = Vector3.zero;
						body.angularVelocity = 0f;
					}
					capturedObject = null;
				}

				if (targetJoint2D != null)
				{
					Destroy(targetJoint2D);
				}
			}
		}

		private void AddSoundsForCollisions(SoftObject softObject)
		{
			softObject.OnInitializeCompleted = () =>
			{
				foreach (var joint in softObject.Joints)
				{
					var jointSoundHelper = joint.GameObject.AddComponent<SoftObjectSoundHelper>();
					jointSoundHelper.demo = this;
					jointSoundHelper.Rigidbody2D = joint.Rigidbody2D;
				}
			};
		}
	}
}