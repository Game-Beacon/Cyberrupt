using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneManage : MonoBehaviour
{
    [SerializeField]
    private GameObject loadingScreen;
    private List<AsyncOperation> asyncs = new List<AsyncOperation>(); 

    public void GoToFightScene()
    {
        //這是為了確保不會陷入遊戲開始時time scale為0的情況
        //總覺得這麼做好像不太好...?
        //TimeManager.UnpauseGame();
        //SceneManager.LoadScene(1);
        GameSystem.instance.StartCoroutine(LoadScene(1));
    }

    public void GoToTitleScene()
    {
        //這是為了確保不會陷入遊戲開始時time scale為0的情況
        //總覺得這麼做好像不太好...?
        //TimeManager.UnpauseGame();
        //SceneManager.LoadScene(0);
        GameSystem.instance.StartCoroutine(LoadScene(0));
    }

    IEnumerator LoadScene(int sceneIndex)
    {
        TimeManager.PauseGame();
        GameObject newLoadingScreen = Instantiate(loadingScreen);
        DontDestroyOnLoad(newLoadingScreen);
        CanvasGroup group = newLoadingScreen.GetComponent<CanvasGroup>();

        group.blocksRaycasts = true;
        group.interactable = true;
        group.DOFade(1, 0.75f).SetUpdate(true);

        yield return new WaitForSecondsRealtime(0.75f);

        asyncs.Add(SceneManager.LoadSceneAsync(sceneIndex));

        for (int i = 0; i < asyncs.Count; i++)
            while (!asyncs[i].isDone)
                yield return null;

        asyncs.Clear();

        group.DOFade(0, 0.75f).SetUpdate(true);

        yield return new WaitForSecondsRealtime(0.75f);
        group.blocksRaycasts = false;
        group.interactable = false;

        Destroy(newLoadingScreen);
        TimeManager.UnpauseGame();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
