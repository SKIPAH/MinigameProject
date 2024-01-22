using System;
using UnityEngine;

namespace SoftBody2D.Core
{
    [Serializable]
    public class Line
    {
        public Vector2 Start;
        public Vector2 End;
        public float Length;
        public float Sum;
    }
}