using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoftBody2D.Core
{
    [Serializable]
    public class LinesContainer
    {
        private Line[] lines;
        private float length;

        public void Init(Sprite sprite)
        {
            var points = new List<Vector2>();
            if (sprite != null)
            {
                sprite.GetPhysicsShape(0, points);
            }

            if (points.Count > 0)
            {
                lines = new Line[points.Count];
                length = 0f;
                for (var i = 0; i < points.Count; i++)
                {
                    var start = points[i];
                    var end = i == points.Count - 1 ? points[0] : points[i + 1];
                    var distance = Vector2.Distance(start, end);
                    lines[i] = new Line
                    {
                        Start = start,
                        End = end,
                        Length = distance,
                        Sum = distance
                    };
                    if (i > 0)
                    {
                        lines[i].Sum += lines[i - 1].Sum;
                    }
                    length += distance;
                }
            }
        }
        
        public Line GetLine(float value, float offset)
        {
            foreach (var line in lines)
            {
                if (value + offset > length)
                {
                    value -= length;
                }
                if (value + offset >= line.Sum - line.Length && value + offset <= line.Sum)
                {
                    return line;
                }
            }
            return null;
        }

        public float GetLength()
        {
            return length;
        }
    }
}