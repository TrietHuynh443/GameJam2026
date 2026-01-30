
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
        public static HumanDirectionType GetReverseDirection(HumanDirectionType dir)
        {
            return (HumanDirectionType)(7 - dir);
        }
    }
}

