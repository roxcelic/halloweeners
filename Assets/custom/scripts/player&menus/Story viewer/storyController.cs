using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using ext;

using index;

/*
    This script is going to make me kms
*/
public class storyController : MonoBehaviour {
    [Header("story")]
    public story.episode fullStory;

    [Header("data")]
    [Range(0, 1)]public int state = 0;
    public int selected = 0;
    public int lineSelected = 0;

    [Header("components")]
    public Image image;
    public Animator anim;
    public TMP_Text selectorDisplay, titleDisplay, pageCountDisplay;

    // start of the new stuff
    [Header("type")]
    public TMP_Text pseudoDisplay;
    public TMP_Text mainDisplay;
    [Range(0f, 2f)] public float delayBetweenChars = 0.1f;
    public AudioClip onChar;
    public AudioClip onFinish;
    public Vector3 soundPoint;

    private Coroutine proxy;
    private bool FinishedTyping = true;

    void Start() {
        FinishedTyping = true;
    }

    void Update() {
        switch(state) {
            case 0:
                selectorDisplay.text = generateRows(viewUnlockedEpisodes(fullStory));

                if(eevee.input.Collect("interact", "SM")) {
                    state = 1;
                    lineSelected = 0;
                    anim.Play("2");
                    image.sprite = getEpisodeByIndex(selected).story.display;
                    type(getLine());
                }

                if (eevee.input.Collect("back", "SM")) {
                    SceneManager.LoadScene(0);
                }

                selected += eevee.input.CollectAxis("down", "up");
                clampSelected();

                break;
            case 1:
                if (eevee.input.Collect("back", "SM")) {
                    state = 0;
                    anim.Play("1");
                }

                if (eevee.input.Collect("down", "SM") || (eevee.input.Collect("interact", "SM") && FinishedTyping)) {
                    changeLine(1);
                    type(getLine());
                }

                if (eevee.input.Collect("up", "SM")) {
                    changeLine(-1);
                    type(getLine());
                }

                break;
        }
    }

    /// <summery> display story lines </summery>
    private void changeLine(int input = 0) {
        lineSelected += input;
        int max = getEpisodeByIndex(selected).story.text.Count - 1;

        if (lineSelected > max) lineSelected = 0;
        if (lineSelected < 0) lineSelected = max;

        if (getLine() == "") changeLine(input);
    } 

    private string getLine() {
        return getEpisodeByIndex(selected).story.text[lineSelected].localise();
    }

    /// <summery> below here are util functions for managing episode content </summery>
    #region  utils 
        private void gettitle() {
            if (titleDisplay != null) titleDisplay.text = getEpisodeByIndex(selected).story.episodeName.localise();
        }
        private void getpage() {
            if (pageCountDisplay != null) pageCountDisplay.text = $"{lineSelected + 1}/{getEpisodeByIndex(selected).story.text.Count}";
        }

        private void getImage() {
            if(getEpisodeByIndex(selected).story.text[lineSelected].newImage != null) image.sprite = getEpisodeByIndex(selected).story.text[lineSelected].newImage;
            else image.sprite = getEpisodeByIndex(selected).story.display;
        }

        private void type(string input) {
            if (proxy != null) StopCoroutine(proxy);
            pseudoDisplay.text = "";

            FinishedTyping = false;
            proxy = StartCoroutine(prescript.storyProxyDevice(
                pseudoDisplay,
                mainDisplay,
                input,
                () => {
                    FinishedTyping = true;
                },
                delayBetweenChars,
                onChar,
                onFinish,
                soundPoint
            ));

            getpage();
            gettitle();
            getImage();
        }

        void clampSelected() {
            Dictionary<int, story.episode> ahh = mapEpisodes(viewUnlockedEpisodes(fullStory));
            int max = ahh.Keys.ToList()[ahh.Keys.Count - 1];
            if (max < 1) max = 1;
            
            if (selected > max) selected = 0;
            if (selected < 0) selected = max;
        }

        /// <summery> reccursivly cycle through to get the full list </summery>
        private string generateRows(story.episode ep, int reccurse = 0) {
            string finalString = "";

            finalString += $"{"  ".Multiply(reccurse)}{(ep == getEpisodeByIndex(selected) ? ">" : "")}{ep.story.episodeName.localise()}\n";
            foreach (story.episode childEp in ep.children) finalString += generateRows(childEp, reccurse + 1);

            return finalString;
        }

        /// <summery> get the selected item </summery>
        private story.episode getEpisodeByIndex(int index) {
            Dictionary<int, story.episode> mappedEpisodes = mapEpisodes(viewUnlockedEpisodes(fullStory));
            if (mappedEpisodes.ContainsKey(index)) return mappedEpisodes[index];
            return null;
        }

        /// <summery> maps the episodes to a dictionary </summery>
        private Dictionary<int, story.episode> mapEpisodes(story.episode ep, Dictionary<int, story.episode> episodes = null) {
            if (episodes == null) episodes = new Dictionary<int, story.episode> ();

            int cr = 0;
            if (episodes.Keys.Count != 0) cr = episodes.Keys.ToList()[episodes.Keys.Count - 1] + 1;

            episodes.Add(cr, ep);
            List<story.episode> children = ep.children;
            foreach (story.episode childEpisode in children) episodes = mapEpisodes(childEpisode, episodes );

            return episodes;
        }

        /// <summery> lock to available only storys </summery>
        private story.episode viewUnlockedEpisodes(story.episode ep) {
            if (!ep.story.readAble()) return null;

            List<story.episode> children = ep.children;
            ep.children = new List<story.episode>();
            foreach (story.episode childEp in children) ep.children.Add(viewUnlockedEpisodes(childEp));
            ep.children = ep.children.removeAllNull();

            return ep;
        }
    }

    /// <summery> this is what will hold the episodes </summery>
    namespace story {
        [System.Serializable]
        public class episode {
            public SO_Text story;
            public List<episode> children = new List<episode>();
            public episode(){}
        }
    #endregion
}