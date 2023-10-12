using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void Button_Start()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void Button_Quit()
    {
        //预处理指令，会判断程序是否在指定平台上运行
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        //此行代码将在游戏独立运行中关闭游戏
        Application.Quit();
    }
    
    
}
