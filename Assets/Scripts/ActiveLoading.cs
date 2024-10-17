using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ActiveLoading : MonoBehaviour
{
    public string path;                //받아올 경로

    public string toWhere;
    public GameObject loadWithPath;
    public void LoadScene()      //로딩씬 호출용
    {
        SceneManager.LoadScene("Loading");      //로딩 씬 호출zz
        DontDestroyOnLoad(loadWithPath);
    }

        private void DEL()
        {
            GameObject[] obj=GameObject.FindGameObjectsWithTag("Loader");
            while(obj.Length>0)
            {
                Destroy(obj[0]);
            }
        }
}
