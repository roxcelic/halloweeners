using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using save;

public class MainMenuIntro : MonoBehaviour {
    [Header("texts")]
    public sys.Text welcomeMessage = new sys.Text(); 
    public sys.Text versionMessage = new sys.Text();
    public sys.Text devBuildMessage = new sys.Text();
    public sys.Text saveVersion = new sys.Text();
    public sys.Text dashVersion = new sys.Text();
    public sys.Text languageMessage = new sys.Text();
    public sys.Text instantRespawnMessage = new sys.Text();

    void Start() {StartCoroutine(logger());} /// start the logger

    public IEnumerator logger() {
        pauseMenuController PMC = null;
        yield return new WaitUntil(() => (PMC = sys.var.components.pauseMenu()) != null);

        PMC.log(welcomeMessage.localise(), sys.programNames.system.localise(), "red");
        yield return new WaitForSeconds(0.1f);
        PMC.log(versionMessage.localise() + $" {save.var.ver}", sys.programNames.system.localise(), "red");
        yield return new WaitForSeconds(0.1f);
        PMC.log(devBuildMessage.localise() + $" {sys.var.config.devBuild}", sys.programNames.system.localise(), "red");
        yield return new WaitForSeconds(0.1f);
        PMC.log(saveVersion.localise() + $" {getData.config().ver}", sys.programNames.system.localise(), "red");
        yield return new WaitForSeconds(0.1f);
        PMC.log(dashVersion.localise() + $" {sys.var.config.flatDash}", sys.programNames.system.localise(), "red");
        yield return new WaitForSeconds(0.1f);
        PMC.log(languageMessage.localise() + $" {getData.config().language}", sys.programNames.system.localise(), "red");
        yield return new WaitForSeconds(0.1f);
        PMC.log(instantRespawnMessage.localise() + $" {getData.config().instantRespawn}", sys.programNames.system.localise(), "red");
    }
}