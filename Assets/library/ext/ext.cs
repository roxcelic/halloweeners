using UnityEngine;
using UnityEngine.UI;

using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace ext {
    #region gameObject
    public static class gameObject {

    }
    #endregion

    #region array
    public static class array {
        public static T[] removeAtIndex<T>(this T[] array, int index) {
            if (array.Length == 1) return new T[0];

            T[] newArray = new T[array.Length - 1];

            int newPos = 0;
            for (int i = 0; i < array.Length; i++) {
                if (i != index) {
                    Debug.Log($"adding: {array[i]}");
                    newArray[newPos] = array[i];
                    newPos++;
                }
            }

            return newArray;
        }
    }
    #endregion

    #region string
    public static class stringStuff {
        public static string Multiply(this string source, int multiplier) {
            string newString = "";
            for (int i = 0; i < multiplier; i++) newString += source;

            return newString;
        }
    }

    #endregion

    #region vector3
    public static class vector3Stuff {
        public static Vector3 Round(this Vector3 vector3, int decimalPlaces = 2) {
            float multiplier = 1;
            for (int i = 0; i < decimalPlaces; i++) {
                multiplier *= 10f;
            }

            return new Vector3(
                Mathf.Round(vector3.x * multiplier) / multiplier,
                Mathf.Round(vector3.y * multiplier) / multiplier,
                Mathf.Round(vector3.z * multiplier) / multiplier);
	    }
    }
    #endregion

    #region List
    public static class listStuff {
        /*
            original function from https://stackoverflow.com/questions/42779103/how-to-slice-a-list but that one was for a static type
        */
        /// <summery> a function which replaces unitys random lack of a List[T].Slice() </summery>
        public static List<T> Slice<T>(this List<T> input, int startIndex, int endIndex) { 
            int elementCount = endIndex-startIndex + 1;
            return input.Skip(startIndex).Take(elementCount).ToList();
        }
    }
    #endregion

    #region generic
    public static class generic {
        // get feild
        public static System.Object GetFieldValue(this System.Object obj, String name) {
            foreach (String part in name.Split('.')) {
                if (obj == null) { return "empty object"; }

                Type type = obj.GetType();
                FieldInfo info = type.GetField(part);
                if (info == null) { return "un recognised feild"; }

                obj = info.GetValue(obj);
            }
            return obj;
        }

        public static T GetFieldValue<T>(this System.Object obj, String name) {
            System.Object retval = GetFieldValue(obj, name);
            if (retval == null) { return default(T); }

            // throws InvalidCastException if types are incompatible
            return (T) retval;
        }

        /*
            ArgumentException: Field killCount defined on type attack.attackData is not a field on the target object which is of type System.Int32.
                Parameter name: obj
        */

        // set feild
        public static System.Object SetFieldValue(this System.Object obj, String name, String value) {
            FieldInfo info = null;
            System.Object prvObj = null;

            foreach (String part in name.Split('.')) {
                if (obj == null) { return "empty object"; }
                prvObj = obj;

                Type type = obj.GetType();
                info = type.GetField(part);
                if (info == null) { return "unrecognised feild"; }

                obj = info.GetValue(obj);
            }


            // set val
            switch (info.GetValue(prvObj).GetType().Name) {
                case nameof(String):
                    info.SetValue(prvObj, value);

                    break;
                case nameof(Boolean):
                    info.SetValue(prvObj, bool.Parse(value));

                    break;
                case nameof(Int32):
                    info.SetValue(prvObj, Int32.Parse(value));

                    break;
                case nameof(Single):
                    info.SetValue(prvObj, Convert.ToSingle(value));

                    break;
                case nameof(Double):
                    info.SetValue(prvObj, Convert.ToDouble(value));

                    break;
                default:
                    return $"unable to cast input type: string to type: {info.GetValue(prvObj).GetType().Name}";
            }

            return info.GetValue(prvObj);
        }

        public static T SetFieldValue<T>(this System.Object obj, String name, String value) {
            System.Object retval = SetFieldValue(obj, name, value);
            if (retval == null) { return default(T); }

            // throws InvalidCastException if types are incompatible
            return (T) retval;
        }
    }
    #endregion

    #region UI
    public static class ScrollRectExtensions {
        // https://discussions.unity.com/t/scroll-to-the-bottom-of-a-scrollrect-in-code/572012
        public static void ScrollToTop(this ScrollRect scrollRect) {scrollRect.normalizedPosition = new Vector2(0, 1);}
        public static void ScrollToBottom(this ScrollRect scrollRect){scrollRect.normalizedPosition = new Vector2(0, 0);}
    }
    #endregion
}