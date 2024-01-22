using System;
using UnityEngine;

namespace SoftBody2D.Core
{
    [Serializable]
    public class SoftObjectJoint
    {
        public CircleCollider2D Collider;
        public Joint2D Joint;
        public GameObject GameObject;
        public Rigidbody2D Rigidbody2D;
        public Transform Transform;
        public float[] Weights;
        public Vector3 StartLocalPosition;
    }
}