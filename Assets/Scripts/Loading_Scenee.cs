using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class Loading_Scenee : MonoBehaviour
{
    ////////////////////////////////
    //받아온 path 저장
    public string pathFromMain;
    public string txtPath;

    public GameObject PathFinder;
    //캐릭터 로딩 구현부
    private void SetPath()
    {   
        PathFinder=GameObject.Find("LoadingManagerIn");             //로딩에서 넘겨받은 경로 탐색
        pathFromMain=PathFinder.GetComponent<ActiveLoading>().path; //저장된 경로 초기화
        //Debug.Log("Operate Set Path,"+pathFromMain);
        PathFinder.name="LoadingManagerInUsed";
    }

    ////////////////////////////////
    [SerializeField]
    Image loadingBar;           //로딩바 이미지 설정

    public GameObject sceneFinder;
    public string sceneName;
    private void Start()
    {
        sceneFinder=GameObject.Find("LoadingManagerIn");
        sceneName=sceneFinder.GetComponent<ActiveLoading>().toWhere;
        loadingBar.fillAmount=0;       //길이는 최초 0으로 초기화
        StartCoroutine(LoadAsyncScene());       //로딩 시작
    }

    public static void LoadScene()      //로딩씬 호출용
    {
        SceneManager.LoadScene("Loading");      //로딩 씬 호출
    }

    IEnumerator LoadAsyncScene()    //상속 - 씬 로딩 관할
    {
        yield return null;
        AsyncOperation asyncScene=SceneManager.LoadSceneAsync(sceneName);  //원하는 씬 로딩
        asyncScene.allowSceneActivation=false;      //일단 씬이 로딩되도, 바로 씬을 연결하지는 않음
        float timeC=0;
        //씬이 로딩될 때까지 반복
        while(!asyncScene.isDone)
        {
            yield return null;
            timeC+=Time.deltaTime/0.8f;      //델타타임으로 설정
            if(asyncScene.progress>=0.9f)       //진행도가 90%이상 됐다면
            {
                loadingBar.fillAmount=Mathf.Lerp(loadingBar.fillAmount, 0.8f, timeC);      //로딩바는 미리 꽉 채워줌

                if(loadingBar.fillAmount==0.8f)     //진행도가 100%가 됐을 때서야
                {
                    if(sceneName!="Choose_Two")
                    {
                        //캐릭터를 로딩할 공간
                        SetPath();          //경로 불러오기
                        //chr->txt로 확장자변경
                        txtPath=Path.ChangeExtension(pathFromMain, ".txt");       //txt파일로 변환 준비
                        Debug.Log(txtPath);
                        File.Move(pathFromMain, txtPath);             //덮어씌움
                        loadingBar.fillAmount=Mathf.Lerp(loadingBar.fillAmount, 0.85f, timeC);        //로딩바 길이를 진행도에 맞게 설정

                        //Debug.Log("Call Files");
                        //파일 불러오기
                        string txtValue=File.ReadAllText(@txtPath);
                        sceneFinder.GetComponent<DataManager>().textValue=txtValue;
                        sceneFinder.GetComponent<DataManager>().textPath=txtPath.Replace(".txt",".chr");         //텍스트 초기화
                        loadingBar.fillAmount=Mathf.Lerp(loadingBar.fillAmount, 0.9f, timeC);        //로딩바 길이를 진행도에 맞게 설정

                        //Debug.Log("Read Files,"+sceneFinder.GetComponent<DataManager>().textValue);

                        //chr파일로 다시 바꿔줌
                        pathFromMain=Path.ChangeExtension(txtPath, ".chr");
                        File.Move(txtPath, pathFromMain);
                    }
                    //System.IO.File.Dispose();
                    loadingBar.fillAmount=Mathf.Lerp(loadingBar.fillAmount, 1f, timeC);        //로딩바 길이를 진행도에 맞게 설정
                    asyncScene.allowSceneActivation=true;       //비로소 다음 씬 로딩
                    StopAllCoroutines();

                    string tempPath=Path.GetFullPath(Path.Combine(pathFromMain,@"../"));
                    //File.SetAttributes(tempPath, FileAttributes.Normal);
                    //File.Close();
                    Invoke("DEL",2f);
                    //Destroy(PathFinder);            //기존 경로 제거
                }
            }
            else            //아직 한참 남았다면
            {
                loadingBar.fillAmount=Mathf.Lerp(loadingBar.fillAmount, asyncScene.progress, timeC);        //로딩바 길이를 진행도에 맞게 설정
                if(loadingBar.fillAmount >=asyncScene.progress)         //로딩바 길이 증가 속도가 실제 진행속도를 넘어서면
                {
                    timeC=0f;        //잠깐 대기시킴
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
                Destroy(obj);     //필요없는 오브젝트 제거
            }
            catch
            {
                break;
            }
        }
    }
    
}
