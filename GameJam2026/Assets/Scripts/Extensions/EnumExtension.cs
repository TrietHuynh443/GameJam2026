using System;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

namespace Extension
{
    public static class EnumExtension
    {
        public static string AsSerialized(this Enum value)
        {
            Type type = value.GetType();
            string name = value.ToString();
        
            // Get the field information for this specific enum value
            FieldInfo field = type.GetField(name);
            if (field == null) return name;
            var inspectorAttr = field.GetCustomAttribute<SerializeAsAttribute>();
            if (inspectorAttr == null) return name;
            return inspectorAttr.Name;

        }
    }
}

