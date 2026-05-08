using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

using TMPro;

#region basic
/// <summery> the entity class, responds to the tick </summery>
public class basic {
    public System.Action baseTickAction = () => {}; // this will be called in the tick
    public static List<basic> entities = new List<basic>();
    public virtual void Tick() {baseTickAction();}
    public virtual void LateTick() {}
    public basic() {entities.Add(this);}
}

/// <summery> a mono behaviour which responds to my tick system </summery>
public class MonoBasic : MonoBehaviour {
    private basic listener = new basic();
    protected virtual void Start() {
        listener.baseTickAction = () => {Tick();};
    }
    public virtual void Tick() {}
}

/// <summery> a basic ui class because all my scripts start with ts </summery>
public class UIBasic : MonoBasic {
    protected TMP_Text display;
    protected override void Start() {
        base.Start();
        display = transform.GetComponent<TMP_Text>();
    }
}
#endregion