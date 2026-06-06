using UnityEngine;
using UnityEngine.UI;

using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using TMPro;

namespace ext {
    #region gameObject
    public static class gameObject {

    }
    #endregion

    #region  transform
    public static class transform {
        public static void noClip(this Transform transform, Transform camera = null, float defSpeed = 1f, bool eev = false) {
            float speed = Input.GetKey(KeyCode.LeftShift) ? defSpeed * 3 : defSpeed;

            float hz = 0;
            float vz = 0;
            float uz = 0;

            if (!eev
            ) {
                if(Input.GetKey(KeyCode.A)) hz -= speed;
                if(Input.GetKey(KeyCode.D)) hz += speed;
                if(Input.GetKey(KeyCode.W)) vz += speed;
                if(Input.GetKey(KeyCode.S)) vz -= speed;
            } else {
                if(eevee.input.Check("Left")) hz -= speed;
                if(eevee.input.Check("Right")) hz += speed;
                if(eevee.input.Check("Up")) vz += speed;
                if(eevee.input.Check("Down")) vz -= speed;
            }

            if(Input.GetKey(KeyCode.LeftControl)) uz -= speed;
            if(Input.GetKey(KeyCode.Space)) uz += speed;
            


            Vector3 forward = camera == null ? transform.forward : camera.forward;
            Vector3 right = camera == null ? transform.right : camera.right;

            Vector3 movement = new Vector3(0, uz, 0) + forward * vz +  right * hz;
            transform.position = Vector3.Lerp(transform.position, transform.position + movement, Time.fixedDeltaTime * 15f);
        }
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

    #region float
    public static class floatStuff {
        public static float roundToNearestCeil(this float source, int round) {
            source /= round;
            source = Mathf.Ceil(source) * round;

            return source;
        }

        public static float roundToNearest(this float source, int round) {
            source /= round;
            source = (int)source * round;

            return source;
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

        public static Vector3 Clamp(this Vector3 vector3, float clampMin, float clampMax) {
            return new Vector3(
                Mathf.Clamp(vector3.x, clampMin, clampMax),
                Mathf.Clamp(vector3.y, clampMin, clampMax),
                Mathf.Clamp(vector3.z, clampMin, clampMax)
            );
	    }

        public static Vector3 Multiply(this Vector3 vector3, Vector3 res) {
            return new Vector3(
                vector3.x * res.x,
                vector3.y * res.y,
                vector3.z * res.z
            );
	    }

        public static float Average(this Vector3 vector3) {
            return (vector3.x + vector3.y + vector3.z) / 3;
        }

    public static bool checkPosition(this Vector3 position, int groundLayer = 3, float groundCheckDistance = 1f, int altGroundLayer = 8) {
        if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, groundCheckDistance)) {
            return hit.collider.gameObject.layer == groundLayer || hit.collider.gameObject.layer == altGroundLayer;
        }

        return false;
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

        /// <summery> removes all null items </summery>
        public static List<T> removeAllNull<T>(this List<T> input) {
            List<T> newList = new List<T>();
            foreach(T item in input) if (item != null) newList.Add(item);

            return newList;
        }

        /// <summery> a function that does a thing </summery>
        public static float FindClosestIndex(this List<float> input, float target) { 
            float closest = 0;
            float? difference = null;

            foreach (float number in input) {
                if (difference == null) {
                    difference = Mathf.Abs(target - number);
                    closest = number;
                } else if (Mathf.Abs(target - number) < difference) {
                    difference = Mathf.Abs(target - number);
                    closest = number;
                }
            }

            return closest;
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

    public static class inputField {
        public static float getFloatValue(this TMP_InputField input, float defaultValue = 0) {
            if (!float.TryParse(input.text, out float parsedVal)) {
                parsedVal = defaultValue;
            }

            return parsedVal;
        }
    }
    #endregion
}