using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class Loading_Scenee : MonoBehaviour
{
    ////////////////////////////////
    //�޾ƿ� path ����
    public string pathFromMain;
    public string txtPath;

    public GameObject PathFinder;
    //ĳ���� �ε� ������
    private void SetPath()
    {   
        PathFinder=GameObject.Find("LoadingManagerIn");             //�ε����� �Ѱܹ��� ��� Ž��
        pathFromMain=PathFinder.GetComponent<ActiveLoading>().path; //����� ��� �ʱ�ȭ
        //Debug.Log("Operate Set Path,"+pathFromMain);
        PathFinder.name="LoadingManagerInUsed";
    }

    ////////////////////////////////
    [SerializeField]
    Image loadingBar;           //�ε��� �̹��� ����

    public GameObject sceneFinder;
    public string sceneName;
    private void Start()
    {
        sceneFinder=GameObject.Find("LoadingManagerIn");
        sceneName=sceneFinder.GetComponent<ActiveLoading>().toWhere;
        loadingBar.fillAmount=0;       //���̴� ���� 0���� �ʱ�ȭ
        StartCoroutine(LoadAsyncScene());       //�ε� ����
    }

    public static void LoadScene()      //�ε��� ȣ���
    {
        SceneManager.LoadScene("Loading");      //�ε� �� ȣ��
    }

    IEnumerator LoadAsyncScene()    //��� - �� �ε� ����
    {
        yield return null;
        AsyncOperation asyncScene=SceneManager.LoadSceneAsync(sceneName);  //���ϴ� �� �ε�
        asyncScene.allowSceneActivation=false;      //�ϴ� ���� �ε��ǵ�, �ٷ� ���� ���������� ����
        float timeC=0;
        //���� �ε��� ������ �ݺ�
        while(!asyncScene.isDone)
        {
            yield return null;
            timeC+=Time.deltaTime/0.8f;      //��ŸŸ������ ����
            if(asyncScene.progress>=0.9f)       //���൵�� 90%�̻� �ƴٸ�
            {
                loadingBar.fillAmount=Mathf.Lerp(loadingBar.fillAmount, 0.8f, timeC);      //�ε��ٴ� �̸� �� ä����

                if(loadingBar.fillAmount==0.8f)     //���൵�� 100%�� ���� ������
                {
                    if(sceneName!="Choose_Two")
                    {
                        //ĳ���͸� �ε��� ����
                        SetPath();          //��� �ҷ�����
                        //chr->txt�� Ȯ���ں���
                        txtPath=Path.ChangeExtension(pathFromMain, ".txt");       //txt���Ϸ� ��ȯ �غ�
                        Debug.Log(txtPath);
                        File.Move(pathFromMain, txtPath);             //�����
                        loadingBar.fillAmount=Mathf.Lerp(loadingBar.fillAmount, 0.85f, timeC);        //�ε��� ���̸� ���൵�� �°� ����

                        //Debug.Log("Call Files");
                        //���� �ҷ�����
                        string txtValue=File.ReadAllText(@txtPath);
                        sceneFinder.GetComponent<DataManager>().textValue=txtValue;
                        sceneFinder.GetComponent<DataManager>().textPath=txtPath.Replace(".txt",".chr");         //�ؽ�Ʈ �ʱ�ȭ
                        loadingBar.fillAmount=Mathf.Lerp(loadingBar.fillAmount, 0.9f, timeC);        //�ε��� ���̸� ���൵�� �°� ����

                        //Debug.Log("Read Files,"+sceneFinder.GetComponent<DataManager>().textValue);

                        //chr���Ϸ� �ٽ� �ٲ���
                        pathFromMain=Path.ChangeExtension(txtPath, ".chr");
                        File.Move(txtPath, pathFromMain);
                    }
                    //System.IO.File.Dispose();
                    loadingBar.fillAmount=Mathf.Lerp(loadingBar.fillAmount, 1f, timeC);        //�ε��� ���̸� ���൵�� �°� ����
                    asyncScene.allowSceneActivation=true;       //��μ� ���� �� �ε�
                    StopAllCoroutines();

                    string tempPath=Path.GetFullPath(Path.Combine(pathFromMain,@"../"));
                    //File.SetAttributes(tempPath, FileAttributes.Normal);
                    //File.Close();
                    Invoke("DEL",2f);
                    //Destroy(PathFinder);            //���� ��� ����
                }
            }
            else            //���� ���� ���Ҵٸ�
            {
                loadingBar.fillAmount=Mathf.Lerp(loadingBar.fillAmount, asyncScene.progress, timeC);        //�ε��� ���̸� ���൵�� �°� ����
                if(loadingBar.fillAmount >=asyncScene.progress)         //�ε��� ���� ���� �ӵ��� ���� ����ӵ��� �Ѿ��
                {
                    timeC=0f;        //��� ����Ŵ
                }
            }
        }
    }


    private void DEL()
    {
        while(true)
        {
            try
            {
                var obj=GameObject.Find("LoadingManagerIn");
                Destroy(obj);     //�ʿ���� ������Ʈ ����
            }
            catch
            {
                break;
            }
        }
    }
    
}
