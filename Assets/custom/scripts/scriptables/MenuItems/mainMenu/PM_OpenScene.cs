using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

[CreateAssetMenu(fileName = "menu item", menuName = "menu items/main menu/open scene")]
public class PM_OpenScene : PM_Base {
    [Header("open scene")]
    public string sceneName = "";
    public string id = "";
    public bool unlockedByDefault = true;

    public override bool active() {
        return save.getData.viewSave().unlockedLevels.Contains(id) || unlockedByDefault;
    }

    public override void action(pauseMenuController PMC, string input = "") {
        Time.timeScale = 1f; // reset time
        PMC.interactable = false;
        playerController.mainPlayer.ScreenEffect.Play("fadeOut");
        PMC.StartCoroutine(wait());
        PMC.StartCoroutine(yap(1.1f, PMC));
    }

    public IEnumerator wait() {
        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadScene(sceneName);
    }

    public IEnumerator yap(float duration, pauseMenuController PMC) {
        float elapsedTime = 0f;
        while (elapsedTime < duration) {
            PMC.log(genString(67), "", "red", false);

            elapsedTime += Time.fixedDeltaTime;
            yield return 0;
        }
    }

    /// <summery> a util to make a random string </summery>
    private string genString(int len) {
        System.Random random = new System.Random();

        return new string(Enumerable.Repeat(sys.var.keywords.characters, len)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
