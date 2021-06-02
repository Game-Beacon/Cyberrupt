using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneManage : MonoBehaviour
{
    public void GoToFightScene()
    {
        //這是為了確保不會陷入遊戲開始時time scale為0的情況
        //總覺得這麼做好像不太好...?
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    public void GoToTitleScene()
    {
        //這是為了確保不會陷入遊戲開始時time scale為0的情況
        //總覺得這麼做好像不太好...?
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
