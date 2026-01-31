
using System.Collections.Generic;
using UnityEngine;

namespace Human
{
    public interface IHuman
    {
        void Back();
    }
    public enum HumanDirectionType
    {
        TopLeft = 0,
        Top = 1,
        TopRight = 2,
        Right = 3,
        Left = 4,
        BottomLeft = 5,
        Bottom = 6,
        BottomRight = 7,
    }

    public static class HumanDirectionExtension
    {
        public static Dictionary<HumanDirectionType, Vector2> DirectionMap = new()
        {
            { HumanDirectionType.TopLeft, new Vector2(-1, 1)},
            { HumanDirectionType.Top, new Vector2(0, 1)},
            { HumanDirectionType.TopRight, new Vector2(1, 1)},
            { HumanDirectionType.Right, new Vector2(1, 0)},
            { HumanDirectionType.BottomRight, new Vector2(1, -1)},
            { HumanDirectionType.Bottom, new Vector2(0, -1)},
            { HumanDirectionType.BottomLeft, new Vector2(-1, -1)},
            { HumanDirectionType.Left, new Vector2(-1, 0)},
        };
        public static Dictionary<HumanDirectionType, string> DirectionNameMap = new()
        {
            { HumanDirectionType.TopLeft, "Top Left"},
            { HumanDirectionType.Top, "Top"},
            { HumanDirectionType.TopRight, "Top Right"},
            { HumanDirectionType.Right, "Right"},
            { HumanDirectionType.BottomRight, "Bottom Right"},
            { HumanDirectionType.Bottom, "Bottom"},
            { HumanDirectionType.BottomLeft, "Bottom Left"},
            { HumanDirectionType.Left, "Left"},
        };
        public static HumanDirectionType GetReverseDirection(HumanDirectionType dir)
        {
            return (HumanDirectionType)(7 - dir);
        }
        
        public static HumanDirectionType GetDirection(Vector2 dir)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            angle = (angle + 360f + 22.5f) % 360f;

            if (angle < 45f) return HumanDirectionType.Right;
            if (angle < 90f) return HumanDirectionType.TopRight;
            if (angle < 135f) return HumanDirectionType.Top;
            if (angle < 180f) return HumanDirectionType.TopLeft;
            if (angle < 225f) return HumanDirectionType.Left;
            if (angle < 270f) return HumanDirectionType.BottomLeft;
            if (angle < 315f) return HumanDirectionType.Bottom;
            return HumanDirectionType.BottomRight;
        }
    }
}

