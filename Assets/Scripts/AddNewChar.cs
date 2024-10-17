using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Windows.Forms;        //파일 선택창 구현을 위함
using Ookii.Dialogs;                        //파일 선택창 출력을 위함

public class AddNewChar : MonoBehaviour
{
    public InputField inputField;               //입력창
    public GameObject myObject;             //로딩씬 스크립트를 가진 오브젝트    

    private OpenFileDialog dialog=new OpenFileDialog();       //다이얼로그용 인스턴스

    public ActiveLoading giveValue;                 //파일 경로 넘겨주기용
    [SerializeField]
    private string path;          //파일 경로

    //파일 경로 넘겨주기
    public void GiveFileRead()
    {
        dialog.Title="캐릭터 파일 열기";                        //파일 열기창 설정
        dialog.Filter="chr(*.chr)|*.chr|모든 파일(*.*)|*.*";    //파일 선택 조건 설정
        //dialog.ShowDialog();
        //선택창 열기
        var filenames=dialog.ShowDialog()==DialogResult.OK ? dialog.FileNames:new string[0];        //파일 선택이 되면 해당 파일명으로, 아니면 쓰레기값 저장
        string[] path2=filenames;              //선택한 파일명으로 경로 지정
        path=path2[0];
        //myObject.SetActive(false);        //IndexOutOfRange 에 대한 예외처리 해둘 것
        Debug.Log(path);

        giveValue.path=path;                //경로를 에디터로 넘겨줌
    }

    //새 캐릭터 생성
    public void MakeChar(string name)
    {
        int sameNumber=0;
        bool sameDetect=false;
        string sameNumbStr="1";
        //Search same char name
        if(Directory.Exists("Assets/Target/"+name))
        {
            sameNumber++;
            sameDetect=true;
            while(Directory.Exists("Assets/Target/"+name+sameNumber.ToString()))
            {
                sameNumber++;
            }
            sameNumbStr=sameNumber.ToString();
        }

        string newFile;

        //폴더명 변경
        if(!sameDetect)
        {
            newFile="Assets/Target/"+name+"/";
        }
        else
        {
            newFile="Assets/Target/"+name+sameNumbStr+"/";
        }
        
        //캐릭터 폴더 생성
        Directory.CreateDirectory(newFile);

        //폴더 내에 기본 요소 생성
        Directory.CreateDirectory(newFile+"ai/");
        Directory.CreateDirectory(newFile+"image/");
        Directory.CreateDirectory(newFile+"sound/");
        //chr파일 생성
        File.Create(newFile+name+".chr");

        //새 파일을 경로로 지정
        path=newFile+name+".chr";
        Debug.Log(path);
        giveValue.path=path;                //경로를 에디터로 넘겨줌
    }

    //파일 생성 코드 실행
    public void DoProgress()
    {
        string charName="";
        charName=inputField.text;
        MakeChar(charName);
    }

    //로딩씬 작동
    public void Operation()
    {
        myObject.SetActive(true);
    }
}
