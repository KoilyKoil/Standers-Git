using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ActiveLoading : MonoBehaviour
{
    public string path;                //�޾ƿ� ���

    public string toWhere;
    public GameObject loadWithPath;
    public void LoadScene()      //�ε��� ȣ���
    {
        SceneManager.LoadScene("Loading");      //�ε� �� ȣ��zz
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
