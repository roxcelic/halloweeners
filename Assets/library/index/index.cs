using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TMPro;

// based off the index prescript, project moon
namespace index {
    /// <summery> a text type animation </summery>
    public static class prescript {
        /// <summery> basic index type animation </summery>
        public static bool recieve(
            this TMP_Text text, 
            string input,
            bool endOnDifferentString = true,
            bool generateExtraLastChar = true,
            char startChar = '[',
            char endChar = ']'
        ) {
            if (text.text.Length < 2) {
                text.text = $"{startChar}{input[0]}{endChar}";
                return true;
            }

            // a simple check
            if (text.text.Substring(1, text.text.Length - 2) == input) return false;

            // check the starting character
            string matchText = text.text.Substring(1, generateExtraLastChar ? text.text.Length - 3 : text.text.Length - 2);
            bool isDifferentString = text.text[0] != startChar || text.text[text.text.Length - 1] != endChar;

            if (!isDifferentString) isDifferentString = !input.StartsWith(matchText);

            if (isDifferentString && endOnDifferentString) return false;

            int nextIndex = isDifferentString ? 0 : matchText.Length;

            // random character
            System.Random random = new System.Random();
            string radnomChar = new string(Enumerable.Repeat(sys.var.keywords.allCharacters, 1).Select(s => s[random.Next(s.Length)]).ToArray());

            bool final = $"{matchText}{input[nextIndex]}" == input;

            if (nextIndex == input.Length) return false;

            text.text = $"{startChar}{(isDifferentString ? "" : matchText)}{input[nextIndex]}{(final ? "" : (generateExtraLastChar ? radnomChar : ""))}{endChar}";

            return true;
        }

        /// <summery> an easy coroutine to get it done quicker </summery>
        public static IEnumerator proxyDevice(TMP_Text text, string input, float delayBetweenChars = 0.025f) {
            while (text.recieve(input)) yield return new WaitForSeconds(delayBetweenChars);
        }

        /// <summery> a smoother animated version of proxyDevice </summery>
        public static IEnumerator lerpProxyDevice(TMP_Text text, string input, float delayBetweenChars = 0.025f) {
            while (text.recieve(input)) {
                delayBetweenChars = Mathf.Lerp(delayBetweenChars, 0f, Time.deltaTime * 15f);

                yield return new WaitForSeconds(delayBetweenChars);
            }
        }

        /// <summery> a coroutine for the story section of the game </summery>
        public static IEnumerator storyProxyDevice(
            TMP_Text pseudoDisplay,
            TMP_Text mainDisplay,
            string input,
            System.Action act = null,
            float delayBetweenChars = 0.1f,
            AudioClip onChar = null,
            AudioClip onFinish = null,
            Vector3 soundPoint = new Vector3(),
            int playSoundOnMultipleOf = 5
        ) {
            int letterCount = 0;

            float timePassed = 0f;
            while (pseudoDisplay.recieve(input)) {
                mainDisplay.text = pseudoDisplay.text; // copy the text over

                delayBetweenChars = Mathf.Lerp(delayBetweenChars, 0f, Time.deltaTime * 15f);


                letterCount++;

                // play a sound
                if (letterCount == playSoundOnMultipleOf) {
                    if (onChar != null) AudioSource.PlayClipAtPoint(onChar, soundPoint);
                    letterCount = 0;
                }

                if (!(eevee.input.Collect("interact", "PM2") && timePassed > 0.1f)) {
                    yield return new WaitForSeconds(delayBetweenChars);
                    timePassed += delayBetweenChars;
                } else {
                    pseudoDisplay.text = $"{'['}{input}{']'}";
                    mainDisplay.text = pseudoDisplay.text;
                }
            }

            Debug.Log($"finished with {pseudoDisplay.text}");
            if (act != null) act();
            if (onFinish != null) AudioSource.PlayClipAtPoint(onFinish, soundPoint);
        }
    }
}