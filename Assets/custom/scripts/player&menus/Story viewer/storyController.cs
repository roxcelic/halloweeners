using UnityEngine;
using UnityEngine.UI;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using ext;

/*
    This script is going to make me kms
*/
public class storyController : MonoBehaviour {
    [Header("story")]
    public story.episode fullStory;

    [Header("data")]
    [Range(0, 1)]public int state = 0;
    public int selected = 0;

    [Header("components")]
    public TMP_Text selector;
    public TMP_Text display;
    public Image image;
    private Animator anim;

    void Start() {
        anim = transform.GetComponent<Animator>();

        selected = 0;
        state = 0;
        clampSelected();
        displayStory();
        displayText();
    }

    void Update() {
        switch (state) {
            case 0:
                if (eevee.input.Collect("down", "SC")) {
                    selected++;

                    clampSelected();
                    displayStory();
                    displayText();
                }

                if (eevee.input.Collect("up", "SC")) {
                    selected--;

                    clampSelected();
                    displayStory();
                    displayText();
                }

                if (eevee.input.Grab("right", "SC")) {
                    anim.Play("1_5");
                    state = 1;
                }

                break;
            case 1:
                if (eevee.input.Grab("left", "SC")) {
                    anim.Play("2_5");
                    state = 0;
                }

                break;
        }
    }

    void displayText() {
        if (viewUnlockedEpisodes(fullStory) == null) selector.text = "//";
        else selector.text = generateRows(viewUnlockedEpisodes(fullStory));
    }

    void displayStory() {
        story.episode selectedEp = getEpisodeByIndex(selected);

        if (selectedEp != null) {
            display.text = selectedEp.story.text.localise();
            image.sprite = selectedEp.story.display;
        } else {
            display.text = $"you have selected episode {selected}, this doesnt exist, how have you done that";
            image.sprite = null;    
        }
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
        public storyText story;
        public List<episode> children = new List<episode>();
        public episode(){}
    }
}