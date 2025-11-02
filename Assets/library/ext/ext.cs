using UnityEngine;

using System;
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
}