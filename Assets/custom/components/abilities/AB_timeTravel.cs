using UnityEngine;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using ext;

[CreateAssetMenu(fileName = "new ability", menuName = "ability/timeTravel")]
public class AB_timeTravel : AB_base {
    [Header("timeTravel config")]
    [Range(0, 25f)] public float distance = 5f;
    public Dictionary<float, Vector3> TimeDevice = new Dictionary<float, Vector3>();

    /// <summery> the main functions </summery>
    #region Main    
        /// <summery> the start function, use to load values etc </summery>
        public override void start(playerController character) {
            TimeDevice = new Dictionary<float, Vector3>();
        }

        /// <summery> code ran every frame </summery>
        public override void update(playerController character) {
            if (Time.timeScale == 0) return;

            TimeDevice.Add(Time.time, character.transform.localPosition);
            Dictionary<float, Vector3> tmp = new Dictionary<float, Vector3>(TimeDevice);

            foreach (float key in tmp.Keys) if (key < Time.time - distance) TimeDevice.Remove(key);
        }

        /// <summery> code ran on the end of the scene </summery>
        public override void end(playerController character) {Debug.Log("ended");}
        
        /// <summery> the main ability </summery>
        public override void use(playerController character) {
            if (TimeDevice.Count == 0) return;

            float selectedTime = Mathf.Clamp(character.attack.liveKills, 0, distance);
            float mod = selectedTime;
            selectedTime = TimeDevice.Keys.ToList().FindClosestIndex(Time.time - selectedTime);

            if (character.attack.liveKills < 1) return;

            character.ScreenEffect.Play("glitch");

            character.transform.localPosition = TimeDevice[selectedTime];
            character.attack.liveKills -= (int)mod;

            Debug.Log(TimeDevice[selectedTime]);

            TimeDevice = new Dictionary<float, Vector3>(); // reset
        }
    #endregion
}