using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using ext;

using save;

public class basic : MonoBehaviour {}

namespace sys {

    public static class var {
        public static class levels {
            public static string tutorial = "custom/levels/level1/level1";
        }
        
        public static class screen {
            public static int height = 256;
            public static int width = 512;
        }
        
        public static class config {
            public static bool flatDash = true;
            public static bool devBuild = true;
            public static string playerTag = "Player";
            public static string otherPlayerTag = "PlayerB";
        }

        public static class layers {
            public static int ground = LayerMask.NameToLayer("Ground");
            public static int enemys = LayerMask.NameToLayer("enemys");
            public static int ignoreRP = LayerMask.NameToLayer("ignoreRP");
            public static int ingoreRPGround = LayerMask.NameToLayer("ignoreRPGround");

            public static LayerMask groundLayerLock = (1 << ground) | (1 << ingoreRPGround);
        }

        public static class keywords {
            public static string devPass = "qoh1206";
            public static string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            public static string specialCharacters = "!\"£$%^&*()_+=[{}];:'@~#,<>./?|`¬";
            public static string allCharacters => $"{characters}{specialCharacters}";
            public static string defaultCharName = "";
            public static string e = "2.71 8281 8284 5904 5235 3602 8747 1352 6624 9775 7247 0936 9995 9574 9669 6762";

            public static List<String> languages = new List<String> {
                "English",
                "cat",
                "dev"
            };
        }

        public static class components {
            public static LoadingScreen loadingScreen() {return LoadingScreen.mainScreen;}
            public static playerController player() {return playerController.mainPlayer;}
            public static saveData save() {return getData.viewSave();}
            public static fullConfig config() {return getData.config();}
            public static pauseMenuController pauseMenu() {return pauseMenuController.instance;}
            public static roomLoader roomloader() {return roomLoader.instance;}
        }

    }

    public static class programNames {
        public static sys.Text dev = new sys.Text();
        public static sys.Text system = new sys.Text();
        public static sys.Text user = new sys.Text();
        public static sys.Text controller_input = new sys.Text();
        public static sys.Text keyboard_input = new sys.Text();
        public static sys.Text evil = new sys.Text();

        static programNames() {
            dev.text = Resources.Load("text/sys/programs/dev") as textobject;
            system.text = Resources.Load("text/sys/programs/system") as textobject;
            user.text = Resources.Load("text/sys/programs/user") as textobject;
            controller_input.text = Resources.Load("text/sys/programs/controller_input") as textobject;
            keyboard_input.text = Resources.Load("text/sys/programs/keyboard_input") as textobject;
            evil.text = Resources.Load("text/sys/programs/evil") as textobject;
        }
    }

    public class utils {
        public static class waiting {
            public static void waitForSeconds(System.Action act, float time) {
                GameObject self = new GameObject();
                basic MB_self = self.AddComponent<basic>();
                self.name = "waitForSeconds";
                MB_self.StartCoroutine(CO_waitForSeconds(act, time, self));
            }
            
            public static void waitForSecondsRealtime(System.Action act, float time) {
                GameObject self = new GameObject();
                basic MB_self = self.AddComponent<basic>();
                self.name = "waitForSecondsRealtime";
                MB_self.StartCoroutine(CO_waitForSecondsRealtime(act, time, self));
            }

            public static void waitUntil(System.Action act, Func<bool> predicate) {
                GameObject self = new GameObject();
                basic MB_self = self.AddComponent<basic>();
                self.name = "waitForSecondsRealtime";
                MB_self.StartCoroutine(CO_waitUntil(act, predicate, self));
            }

            // waits for seconds
            private static IEnumerator CO_waitForSeconds(System.Action act, float time, GameObject self) {
                yield return new WaitForSeconds(time);
                act();
                GameObject.Destroy(self);
            }

            private static IEnumerator CO_waitForSecondsRealtime(System.Action act, float time, GameObject self) {
                yield return new WaitForSecondsRealtime(time);
                act();
                GameObject.Destroy(self);
            }

            public static IEnumerator CO_waitUntil(System.Action act, Func<bool> predicate, GameObject self) {
                yield return new WaitUntil(predicate);
                act();
                GameObject.Destroy(self);
            }
        }
        
        public static void log(string input) {
            Debug.Log(input);
            if(pauseMenuController.instance != null) pauseMenuController.instance.log(input);
        }

        public static void displayOnPlayer(sys.Text input) {
            textDisplay.instance.textToDisplay.Add(input);
        }

        public static void playScreenEffect(string name) {
            playerController.mainPlayer.ScreenEffect.Play(name);
        }

        public static IEnumerator wait(System.Action run, float delay = 1f) {
            yield return new WaitForSeconds(delay);
            run();
        }

        public static IEnumerator relayActive(System.Action start, System.Action end, Transform target) {
            start();
            yield return new WaitUntil(() => target == null);
            end();
        }
    }
    
    [System.Serializable]
    public class Text {
        public string overrideName = "";
        public textobject text = null;

        public virtual string localise() {
            if (this.overrideName != "") return this.overrideName;
            if (this.text == null) return "//////////////////////";
            
            switch (save.getData.config().language) {
                case "cat": return $"meo{"w".Multiply(this.text.English.Length - 3)}";
                case "dev": return $"dev:{this.text.English}";

                case "English":default: return sys.text.displayKeyButton(this.text.English);
            }
        }

        public virtual string displayVar(Dictionary<string, string> data) {
            string[] words = this.localise().Split(" ");
            List<string> selectedWords = new List<string>();

            foreach(string word in words) {
                if ((word.Length - 1) > 5 && word.Substring(0, 5) == "!var:") {
                    string key = word.Substring(5, word.Length - 5);
                    if (data.ContainsKey(key)) selectedWords.Add(data[key]);
                    else selectedWords.Add(word);
                } else {
                    selectedWords.Add(word);
                }
            }

            return sys.text.displayKeyButton(string.Join(" ", selectedWords));
        }

        public Text(string name = "", textobject text = null) {
            this.overrideName = name;
            if (text != null) this.text = text;
        }
    }

    /// <summery> an "inline" text object </summery>
    [System.Serializable]
    public class inlineText : Text {
        [Header("inline")]
        [TextArea] public string English;

        public override string localise() {
            if (this.overrideName != "") return this.overrideName;
            string final = "";

            switch (save.getData.config().language) {
                case "cat": final = $"meo{"w".Multiply(this.English.Length - 3)}"; break;
                case "dev": final = $"dev:{this.English}"; break; 

                case "English":default: final = sys.text.displayKeyButton(this.English); break;
            }

            if (final == "") return "//////////////////////";
            else return final;
        }

        public inlineText(string name = "", textobject text = null){
            this.overrideName = name;
            if (text != null) {
                this.English = text.English;
            }
        }
    }

    public class system {

        // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        // public static void applyRes() {
        //     string[] res = config.read.String("screenRes", "1920x1080").Split("x");

        //     Screen.SetResolution(int.Parse(res[0]), int.Parse(res[1]), true);
        // }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void applyFrameCap() {
            Application.targetFrameRate = 1231233;
        }
    }

    public class text {
        public static string displayKeyButton(string input) {
            // find text
            string[] words = input.Split(" ");
            List<string> selectedWords = new List<string>();

            foreach(string word in words) {
                if ((word.Length - 1) > 5 && word.Substring(0, 5) == "!key:") {
                    string key = word.Substring(5, word.Length - 5);
                    Dictionary<string, eevee.config> FullConfig = eevee.inject.retrieve().FullConfig;

                    if (!FullConfig.ContainsKey(key)) selectedWords.Add(word);
                    else {
                        eevee.config selected_input = FullConfig[key];
                        string result = "";

                        switch(eevee.conf.autoDetect()) {
                            case eevee.inputCL.keyboard: 
                                foreach (int keyCode in selected_input.KEYBOARD_code) result = ((KeyCode)keyCode).ToString();

                                break;
                            case eevee.inputCL.controller: 
                                foreach (string buttonCode in selected_input.CONTROLLER_name) result = buttonCode;

                                break;
                        }

                        if (result == "") result = $"ERROR could not find {key} in inputs";
                        selectedWords.Add(result);
                    }
                } else {
                    selectedWords.Add(word);
                }
            }

            return string.Join(" ", selectedWords);
        }
    }

    public class nockback {
        public static Vector3 calculateNockback(Vector3 explosionPoint, Vector3 playerPos, float explosionForce = 5f) {
            Vector2 originalForce = new Vector2();
            originalForce.x = playerPos.x - explosionPoint.x;
            originalForce.y = playerPos.y - explosionPoint.y;   

            Vector2 forceDirection = new Vector2();
            forceDirection = originalForce * originalForce; 

            Vector2 forcePercent = new Vector2();
            float totalForce = forceDirection.x + forceDirection.y;

            forcePercent.x = Math.Abs(forceDirection.x / totalForce);
                if (originalForce.x < 0) forcePercent.x = forcePercent.x * -1;
            forcePercent.y = Math.Abs(forceDirection.y / totalForce);
                if (originalForce.y < 0) forcePercent.y *= -1;

            return (new Vector3(-forcePercent.x, 1, -forcePercent.y)) * explosionForce;
        }
    }
}