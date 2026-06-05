using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace POMO {
    /// <summery> the information required to run </summery>
    public static class var {
        // the created objects
        public static List<POMO_ui_obj> objects = new List<POMO_ui_obj>();
    }

    /// <summery> the class for generating ui items within photo mode </summery>
    public static class ui {
        // the kinds of items which can be generated
        public enum types {
            button,
            colorPicker,
            hr,
            text,
            slider,
            input,
            vector
        }

        // creates ui object
        public static GameObject createEl(ui.types type) {
            if (POMO_Cont.self == null) return null; // no refrence
            
            GameObject creation = null;

            switch (type) {
                case ui.types.button:
                    creation = GameObject.Instantiate(POMO_Cont.self.button, new Vector3(), Quaternion.identity);

                    break;
                case ui.types.colorPicker:
                    creation = GameObject.Instantiate(POMO_Cont.self.colorPicker, new Vector3(), Quaternion.identity);

                    break;
                case ui.types.hr:
                    creation = GameObject.Instantiate(POMO_Cont.self.hr, new Vector3(), Quaternion.identity);
                    
                    break;
                case ui.types.text:
                    creation = GameObject.Instantiate(POMO_Cont.self.text, new Vector3(), Quaternion.identity);

                    break;
                case ui.types.slider:
                    creation = GameObject.Instantiate(POMO_Cont.self.slider, new Vector3(), Quaternion.identity);

                    break;
                case ui.types.input:
                    creation = GameObject.Instantiate(POMO_Cont.self.input, new Vector3(), Quaternion.identity);

                    break;
                case ui.types.vector:
                    creation = GameObject.Instantiate(POMO_Cont.self.vector, new Vector3(), Quaternion.identity);

                    break;
            }

            return creation;
        }
    }

    // the ui object class
    public class POMO_ui_obj {
        public GameObject self;
        public sys.Text name;
        public System.Action<string, POMO_ui_obj> act;

        public POMO_Interactor interactor;

        public POMO_ui_obj(ui.types type, sys.Text name = null, System.Action<string, POMO_ui_obj> act = null, bool runOnStart = false) {
            self = ui.createEl(type);

            if (self == null) throw new InvalidOperationException("you fucked up for the last time dream");

            POMO_Interactor interaction = self.transform.GetComponent<POMO_Interactor>();

            if (interaction == null) throw new InvalidOperationException("you fucked up for the last time dream");

            interactor = interaction;
            if (interaction.text != null) interaction.text.text = name.localise();

            // on type
            switch (type) {
                case ui.types.button: 
                    interaction.button.onClick.AddListener(() => {act("", this);});

                    break;
                case ui.types.slider:
                    interaction.slider.onValueChanged.AddListener(delegate {act("", this);});

                    break;
                case ui.types.input:
                    interaction.input.onValueChanged.AddListener(delegate {act("", this);});

                    break;
                case ui.types.vector:
                    interaction.VectorX.onValueChanged.AddListener(delegate {act("", this);});
                    interaction.VectorY.onValueChanged.AddListener(delegate {act("", this);});
                    interaction.VectorZ.onValueChanged.AddListener(delegate {act("", this);});

                    break;
            }

            // add child
            self.transform.parent = POMO_Cont.self.root;
            self.transform.localScale = new Vector3(1, 1, 1);

            // set variables
            this.name = name;
            this.act = act;

            // add to list
            POMO.var.objects.Add(this);

            // run if can
            if (runOnStart) act("", this);
        }
    }
}

// this is a good button example
// new POMO_ui_obj(ui.types.button, new sys.Text("im a button"), (string interact, POMO_ui_obj self) => {
//     if (float.TryParse(self.interactor.buttonText.text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float test) == false) {
//         test = 0f;
//     } else {
//         test++;
//     }

//     self.interactor.buttonText.text = $"{test}";
// }, true);