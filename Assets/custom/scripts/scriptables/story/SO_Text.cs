using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using save;

[CreateAssetMenu(fileName = "text", menuName = "text/story text")]
public class SO_Text : ScriptableObject {
    public enum displayType {
        regular,
        speach,
        description,
        narrator
    }

    [Header("text")]
    public sys.inlineText episodeName = new sys.inlineText();
    public List<sys.storyText> text = new List<sys.storyText>();
    // public sys.inlineText text = new sys.inlineText();

    [Header("images")]
    public Sprite display;

    [Header("unlock")]
    public string unlockKey;
    public bool developerLock;

    /// <summery> checks if it is unlocked </summery>
    public bool readAble() {
        if (developerLock && !getData.isDev()) return false;
        if (unlockKey == "") return true;
        saveData currentSave = getData.viewSave();

        return currentSave.unlockedStory.Contains(unlockKey);
    }

    /// <summery> unlocks the story on the player </summery>
    public void unlock() {
        saveData currentSave = getData.viewSave();
        if (unlockKey == "" || currentSave.unlockedStory.Contains(unlockKey)) return;
        
        currentSave.unlockedStory.Add(unlockKey);
        getData.save(currentSave);
    }
}
