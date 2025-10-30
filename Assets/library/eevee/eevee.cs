using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

// limited contacts "..." and "eevee.system"

namespace eevee {
    public enum inputCL {
        both,
        keyboard,
        controller
    }
    // variables
    public class var {
        public static float ver = 1.10f;
        public static string name = $"eevee-{ver}";
        public static string ConfPath = Path.Combine(Application.persistentDataPath, "EeveeConfig.json");
        public static eevee.inputCL inputType;
        public static eevee.inputCL lastUsed;
    }
    
    // controll config
    [System.Serializable]
    public class config {
        [Header("general")]
        public string displayName;
        public bool Pressed;
        public bool forcePressed = false;

        [Header("keyboard")]
        public int[] KEYBOARD_code;
        
        [Header("controller")]
        public string[] CONTROLLER_name;
    }

    [System.Serializable]
    public class KeyValue {
        public string key;
        public eevee.config value;
    }

    [System.Serializable]
    public class ConfigWrapper {
        public float ver;
        public List<KeyValue> configData;
    }

    // eevee config editor
    //  eevee.conf.switchToLastUsed() -- locks the input to the last used input device
    //  eevee.conf.limitInput() -- locks the input to a certain type (keyboard, controller, both)
    //  eevee.conf.autoDetect() -- will limit the input device to the last used
    //  eevee.conf.readLimiter() -- returns the input type eevee is currently limited to
    public class conf {
        public static void switchToLastUsed() {
            eevee.var.inputType = eevee.var.lastUsed;
        }
        public static void limitInput(eevee.inputCL input) {
            eevee.var.inputType = input;
        }
        public static eevee.inputCL autoDetect() {
            return eevee.var.lastUsed;
        }
        public static eevee.inputCL readLimiter() {
            return eevee.var.inputType;
        }
    }

    // external controler
    //  eevee.external.press() -- allows an input to be pressed without a traditional input device
    public class external {
        static external() {
            eevee.inject.Parasite();
        }

        public static void press(string input, bool value = true) {
            eevee.inject.retrieve().FullConfig[input].forcePressed = value;
        }
    }

    // livespace ?? I dont actually know what to call this but when making it originally i wrote that
    //  eevee.inject.install();
    //  eevee.inject.add();
    //  eevee.inject.OverWrite(); -- does the same thing as add but i got confused with the naming
    //  eevee.inject.Parasite();
    //  eevee.inject.retrieve(); -- returns the parasite
    public class inject {
        static inject() {
            Parasite();
        }

        public static void install(Dictionary<string, eevee.config> config) {
            eev eevee = retrieve();
            eevee.FullConfig = config;
            Qlock.push();
        }

        public static void add(eevee.config config) {
            eev eevee = retrieve();
            eevee.FullConfig[config.displayName] = config;
            Qlock.push();
        }

        // this exists just for readability / useablilty
        public static void OverWrite(eevee.config config) {
            eev eevee = retrieve();
            eevee.FullConfig[config.displayName] = config;
            Qlock.push();
        }
        
        public static void Parasite() {
            if (GameObject.Find(var.name) == null){
                GameObject Parasect = new GameObject();
                Parasect.name = var.name;
                Parasect.AddComponent<eev>();
                install(Qlock.extractr());
            }
        }

        public static eev retrieve() {
            // make this make the object if null
            GameObject eeveeOBJ = GameObject.Find(var.name);
            if (eeveeOBJ == null){
                inject.Parasite();
                eeveeOBJ = GameObject.Find(var.name);
            }

            eev eevee = eeveeOBJ.GetComponent<eev>();
            return eevee;
        }
    }

    // inputs
    //  eevee.input.Collect("a") -- returns if its pressed and if it is will wait before checking again
    //  eevee.input.Check("a") -- returns if its pressed
    //  eevee.input.Grab("a") -- if the button is pressed itll return true and wait until it is let go before allowing it to return true again
    //  eevee.input.multiTap("a") -- will check if the button is pressed any amount within a delay
    //  eevee.input.CheckAxis("a", "d") -- returns -1-1 based on the inputs (treating it like an axis) using the check input collection method
    //  eevee.input.CollectAxis("a", "d") -- returns -1-1 based on the inputs (treating it like an axis) using the collect input collection method
    //  eevee.input.accessUnityAxis() -- returns the result from checking a unity axis
    //  eevee.input.accessUnityButton() -- returns the result from checking unity button
    public class input {
        static input() {
            inject.Parasite();
        }

        // basic inputs

        public static bool Collect(string input, string contact = "...", float delay = 1f) {
            return inject.retrieve().pressed(input, contact, delay);
        }

        public static bool Check(string input) {
            return inject.retrieve().Check(input);
        }

        public static bool Grab(string input, string contact = "...") {
            return inject.retrieve().Grab(input, contact);
        }

        // complex inputs

        public static async Task<bool> multiTap(string input, int loop = 2, int grace = 250, string contact = "...", int delay = 50) {
            while(loop > 0) {
                if (!(await eevee.input.multiTapHelper(input, grace, contact, delay))) return false;
                loop--;
            }

            return true;
        }

        private static async Task<bool> multiTapHelper(string input, int grace = 250, string contact = "...", int delay = 50) {
            while (grace > 0f) {
                if (eevee.input.Grab(input, contact)) return true;
                await Task.Delay(delay);
                grace -= delay;
            }
            return false;
        }

        // axis

        public static int CheckAxis(string positive, string negative) {
            return input.Check(positive)?1:input.Check(negative)?-1:0;
        }

        public static int CollectAxis(string positive, string negative, string contact = "...", float delay = 1f) {
            return input.Collect(positive, contact, delay)?1:input.Collect(negative, contact, delay)?-1:0;
        }

        // unity parsing

        public static float accessUnityAxis(string axisName, bool rounded = false) {
            float axis = Input.GetAxis(axisName);
            
            if (rounded) {
                if (axis > 0) return 1;
                if (axis < 0) return -1;
            }
            
            return axis;
        }

        public static bool accessUnityButton(string buttonName) {
            return Input.GetButtonDown(buttonName);
        }
    }

    // config lock
    //  eevee.Qlock.wrap();
    //  eevee.Qlock.unwrap();
    //  eevee.Qlock.push();
    //  eevee.Qlock.extractr();
    //  eevee.Qlock.clear();
    public class Qlock {
        static Qlock() {
            inject.Parasite();
        }

        public static string wrap(Dictionary<string, eevee.config> FullConfig) {
            List<KeyValue> keyValueList = new List<KeyValue>();

            foreach (KeyValuePair<string, eevee.config> entry in FullConfig) {
                keyValueList.Add(new KeyValue { key = entry.Key, value = entry.Value });
            }

            ConfigWrapper wrapper = new ConfigWrapper { 
                ver = eevee.var.ver,
                configData = keyValueList 
            };

            return JsonUtility.ToJson(wrapper);
        }

        public static Dictionary<string, eevee.config> unwrap(string json) {
            ConfigWrapper wrapper = JsonUtility.FromJson<ConfigWrapper>(json);
            
            Dictionary<string, eevee.config> FullConfig = new Dictionary<string, eevee.config>();
            
            if (wrapper.ver == eevee.var.ver) foreach (KeyValue entry in wrapper.configData) FullConfig.Add(entry.key, entry.value);

            return FullConfig;
        }

        public static void push() {
            Dictionary<string, eevee.config> FullConfig = inject.retrieve().FullConfig;

            File.WriteAllText(var.ConfPath, wrap(FullConfig));
        }

        public static Dictionary<string, eevee.config> extractr() {
            if (File.Exists(var.ConfPath)) {
                string json = File.ReadAllText(var.ConfPath);

                return unwrap(json);
            }

            return new Dictionary<string, eevee.config>() {
                {
                    "one", new eevee.config {
                        displayName = "get injected fool",
                        KEYBOARD_code = new int[] {(int)KeyCode.A}
                    }
                }
            };
        }

        public static void clear() {
            if (File.Exists(var.ConfPath)) {
                File.Delete(var.ConfPath);   

                inject.install(new Dictionary<string, eevee.config>());
            }
        }
    }

    // test 
    //  eevee.test.Test();
    public class test {
        public static void Test() {

        }
    }
}

// THIS IS OUTDATED FOR V1.0.5 BUT IT HASNT BEEN UPDATED YET SINCE ITS PRETTY MUCH THE SAME
    // there is an issue somewhere where it wont work with timescale 0, i intend to find this... FOUND IT!!!! I forgot to use `WaitForSecondsRealtime`

// the componant for live data sifting,
//  This is as Efficient as i could make it but feel free to try your hand at it, in basic how this works:
//  Once an input is requested it will begin the track it, by every frame checking if it is held down either 
//  by `UnityEngine.Input.GetKeyDown()` and by grabbing all the currently pressed buttons on the first gamepad (if there is a gamepad)
//  and mapping that into a ~~hashtable~~ dictionary.
// If anyone needs more help with editing this file please feel free to ask me personally through my socials, i will help out if i have the time (:
public class eev : MonoBehaviour {
    public Dictionary<string, eevee.config> FullConfig = new Dictionary<string, eevee.config>();

    public Dictionary<string, Dictionary<string, Coroutine>> activeCoroutines = new Dictionary<string, Dictionary<string, Coroutine>>();
    public Dictionary<string, Dictionary<string, Coroutine>> activeCoroutines_grab = new Dictionary<string, Dictionary<string, Coroutine>> ();

    public List<ButtonControl> current_pressed_buttons = new List<ButtonControl>();
    public List<string> current_pressed_names = new List<string>();

    // test
    public List<eevee.config> displayConf = new List<eevee.config>();

    void Start() {
        displayConf = FullConfig.Values.ToList();
    }

    public bool Check(string input){
        if (FullConfig.ContainsKey(input)) {
            // keycode
            if(eevee.var.inputType != eevee.inputCL.controller) foreach (int key in FullConfig[input].KEYBOARD_code) if (Input.GetKey((KeyCode)key)) {
                eevee.var.lastUsed = eevee.inputCL.keyboard;
                return true;
            }

            // gamepad
            if(eevee.var.inputType != eevee.inputCL.keyboard) foreach (string key in FullConfig[input].CONTROLLER_name) if (IsControllerInputPressed(key)) {
                Debug.Log("GAMEPAD INPUT");
                eevee.var.lastUsed = eevee.inputCL.controller;
                return true;
            }

            // catch
            if (FullConfig[input].forcePressed) {
                FullConfig[input].forcePressed = false;
                return true;
            }

            return false;

        } else {
            return false;
        }
    }

    public bool Grab(string input, string contact = "..."){
        if (!activeCoroutines_grab.ContainsKey(contact)) activeCoroutines_grab.Add(contact, new Dictionary<string, Coroutine>());


        if (FullConfig.ContainsKey(input)) {

            if (activeCoroutines_grab[contact].ContainsKey(input) && activeCoroutines_grab[contact][input] != null){
                return false;
            } else if (Check(input)) {
                activeCoroutines_grab[contact][input] = StartCoroutine(Track_Grab(input, contact));
                
                return true;
            }

            return false;

        } else {
            return false;
        }
    }

    public bool pressed(string input, string contact = "...", float delay = 1f){
        if (!activeCoroutines.ContainsKey(contact)) activeCoroutines.Add(contact, new Dictionary<string, Coroutine>());

        if (FullConfig.ContainsKey(input)) {

            if (activeCoroutines[contact].ContainsKey(input) && activeCoroutines[contact][input] != null){
            
                if (FullConfig[input].Pressed){
                    FullConfig[input].Pressed = false;

                    return true;
                }

                return false;
            
            } else {
                activeCoroutines[contact][input] = StartCoroutine(Track(input, contact, delay));
                
                if (FullConfig[input].Pressed){
                    FullConfig[input].Pressed = false;

                    return true;
                }

                return false;
            }

        } else {
            return false;
        }
    }

    private IEnumerator Track(string keyName, string contact, float delay = 1f) {
        const float minDelay = 0.01f;

        while (true) {
            if (Check(keyName)) {
                FullConfig[keyName].Pressed = true;
            } else {
                activeCoroutines[contact].Remove(keyName);
                yield break;
            }

            delay = Mathf.Max(minDelay, delay / 2);
            
            // my new wait for seconds :relaxed:
            float elapsedTime = 0f;
            while (elapsedTime < delay) {
                elapsedTime += Time.fixedDeltaTime;
                float opacityMod = 1f * (elapsedTime / delay);

                if (!Check(keyName)) {
                    activeCoroutines[contact].Remove(keyName);
                    yield break;
                }
                
                yield return null;
            }
        }
    }

    private IEnumerator Track_Grab(string keyName, string contact) {
        while (true) {
            if (!Check(keyName)) {
                activeCoroutines_grab[contact].Remove(keyName);
                yield break;
            }
            yield return 0;
        }
    }

    void Update() {
        if (Gamepad.current != null) {

            current_pressed_buttons = Gamepad.current.allControls.OfType<ButtonControl>()
                .Where(control => control.isPressed)
                .ToList();

            current_pressed_names = current_pressed_buttons.ConvertAll<string>(control => control.displayName);
        }
    }

    bool IsControllerInputPressed(string inputName) {
        return current_pressed_names.Contains(inputName);
    }
}