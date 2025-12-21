using UnityEngine;

using System.Linq;
using System.Collections.Generic;

public class ControlInjector : MonoBehaviour {

    public bool overwrite = false;

    void Start() {
        Dictionary<string, eevee.config> controls = eevee.Qlock.extractr();

        foreach (string key in eeveeLive.var.config.Keys){
            if (controls.Keys.Contains(key) && overwrite){
                Debug.Log($"overwritng {key}");
                eevee.inject.OverWrite(eeveeLive.var.config[key]);
            } else if (!controls.Keys.Contains(key)){
                eevee.inject.add(eeveeLive.var.config[key]);
            }
        }
    }
}
