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
    public Dictionary<float, AB_timeTravel_space.spaceTracking> TimeDevice = new Dictionary<float, AB_timeTravel_space.spaceTracking>();

    /// <summery> the main functions </summery>
    #region Main    
        /// <summery> the start function, use to load values etc </summery>
        public override void start(playerController character) {
            TimeDevice = new Dictionary<float, AB_timeTravel_space.spaceTracking>();
        }

        /// <summery> code ran every frame </summery>
        public override void update(playerController character) {
            if (Time.timeScale == 0) return;

            TimeDevice.Add(Time.time, new AB_timeTravel_space.spaceTracking(character.transform.position, character.health));
            Dictionary<float, AB_timeTravel_space.spaceTracking> tmp = new Dictionary<float, AB_timeTravel_space.spaceTracking>(TimeDevice);

            foreach (float key in tmp.Keys) if (key < Time.time - (distance + 1)) TimeDevice.Remove(key);
        }

        /// <summery> code ran on the end of the scene </summery>
        public override void end(playerController character) {Debug.Log("ended");}
        
        /// <summery> the main ability </summery>
        public override void use(playerController character) {
            if (TimeDevice.Count == 0) return;

            float selectedTime = Mathf.Clamp(character.charge, 0, distance);
            float mod = selectedTime;
            selectedTime = TimeDevice.Keys.ToList().FindClosestIndex(Time.time - selectedTime);

            if (character.charge < 1) return;

            character.ScreenEffect.Play("glitch");

            character.transform.position = TimeDevice[selectedTime].pos;
            if (character.health < TimeDevice[selectedTime].health) character.health = TimeDevice[selectedTime].health;
            character.charge -= (int)mod;

            Debug.Log($"match {TimeDevice[selectedTime].pos == character.transform.position} \n target pos was {TimeDevice[selectedTime].pos}, resulted position is {character.transform.position}");

            TimeDevice = new Dictionary<float, AB_timeTravel_space.spaceTracking>(); // reset
        }
    #endregion
}

namespace AB_timeTravel_space {
    public class spaceTracking {
        public Vector3 pos;
        public int health;

        public spaceTracking(Vector3 newPos, int newHealth) {
            this.pos = newPos;
            this.health = newHealth;
        }
    }
}