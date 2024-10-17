using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameData : MonoBehaviour
{
                                                                        //유동정보+기본정보+주석
    public string[] sameValue=new string[18];
    public string[] motValue=new string[26];       //26+2+1
    public string[] effectValue=new string[68];    //68+3+1
    public string[] bulletValue=new string[6];     //6+17+1
    public string[] variableValue=new string[4];       //4+2+1
    public string[] imageValue=new string[13];

    //값 작성
    public void SetBoxValue(InputField[] textBox, string[] inputValue, int menu, int sameMenu)
    {
        int boxNow;
        //대프레임 클릭시
        if(sameMenu==1)
        {
            //값 출력
            switch(menu)
            {
                case 1:             //동작
                    boxNow=31;
                    for(int i=0;i<2;i++)
                    {
                        textBox[boxNow].GetComponent<UnityEngine.UI.InputField>().text=inputValue[i];
                        boxNow++;
                    }
                    break;
                case 2:             //효과
                    boxNow=61;
                    for(int i=0;i<3;i++)
                    {
                        textBox[boxNow].GetComponent<UnityEngine.UI.InputField>().text=inputValue[i];
                        boxNow++;
                    }
                    break;
                case 3:             //투사체
                    boxNow=133;
                    for(int i=0;i<17;i++)
                    {
                        textBox[boxNow].GetComponent<UnityEngine.UI.InputField>().text=inputValue[i];
                        boxNow++;
                    }
                    break;
                case 4:                 //변수
                    boxNow=158;
                    for(int i=0;i<2;i++)
                    {
                        textBox[boxNow].GetComponent<UnityEngine.UI.InputField>().text=inputValue[i];
                        boxNow++;
                    }
                    break;
                case 5:                 //이미지
                    boxNow=167;
                    for(int i=0;i<13;i++)
                    {
                        textBox[boxNow].GetComponent<UnityEngine.UI.InputField>().text=inputValue[i];
                        boxNow++;
                    }
                    break;
                default:
                    Debug.Log("메뉴 입력 오류!!!!!!!!!!!!!!!!!!!!!!");
                    break;
            }
        }
        //소프레임 클릭시
        else
        {
            //값 출력
            switch(menu)
            {
                case 1:                //동작
                    boxNow=33;
                    for(int i=0;i<26;i++)
                    {
                        textBox[boxNow].GetComponent<UnityEngine.UI.InputField>().text=inputValue[i];
                        boxNow++;
                    }
                    break;
                case 2:                 //효과
                    boxNow=64;
                    for(int i=0;i<68;i++)
                    {
                        textBox[boxNow+i].GetComponent<UnityEngine.UI.InputField>().text=inputValue[i];
                    }
                    break;
                case 3:             //투사체
                    boxNow=150;
                    for(int i=0;i<6;i++)
                    {
                        textBox[boxNow+i].GetComponent<UnityEngine.UI.InputField>().text=inputValue[i];
                    }
                    break;
                case 4:                 //변수
                    boxNow=160;
                    for(int i=0;i<4;i++)
                    {
                        textBox[boxNow+i].GetComponent<UnityEngine.UI.InputField>().text=inputValue[i];
                    }   
                    break;
                case 5:                 //이미지
                    boxNow=167;
                    for(int i=0;i<13;i++)
                    {
                        textBox[boxNow+i].GetComponent<UnityEngine.UI.InputField>().text=inputValue[i];
                    }
                    break;
                default:
                    Debug.Log("메뉴 입력 오류!!!!!!!!!!!!!!!!!!!!!!");
                    break;
            }
        }
    }

    public void ShowFrames(Transform address, GameObject ingrediants, Transform destination, int frameCount, int menu, InputField[] textBox)
    {                                       //대프레임 하위 주소 (FrameList?)                 //FrameList,                //MotiveFrameList주소
        Debug.Log("text1");
        //이전 프레임 제거
        Destroy(destination.GetChild(0).gameObject);

        //프레임 복제
        GameObject targetObject=Instantiate(ingrediants.transform.GetChild(1).gameObject, address.localPosition, Quaternion.identity);
        targetObject.transform.SetParent(destination);
        targetObject.SetActive(true);
        targetObject.transform.localScale=new Vector3(1f,1f,1f);
        
        //버튼 입력 설정부
        for(int i=1;i<=frameCount;i++)
        {
            GameObject longName=targetObject.transform.GetChild(1).gameObject;
            //버튼 설정
            switch(menu)
            {
                case 1:             //동작
                    Debug.Log("버튼 입력 설정1");
                    longName.GetComponent<Button>().onClick.AddListener(()=>SetBoxValue(textBox, longName.GetComponent<FrameData>().motValue,1,0));
                    break;
                case 2:             //효과
                    Debug.Log("버튼 입력 설정2");
                    longName.GetComponent<Button>().onClick.AddListener(()=>SetBoxValue(textBox, longName.GetComponent<FrameData>().effectValue,2,0));
                    break;
                case 3:             //투사체
                    Debug.Log("버튼 입력 설정3");
                    longName.GetComponent<Button>().onClick.AddListener(()=>SetBoxValue(textBox, longName.GetComponent<FrameData>().bulletValue,3,0));
                    break;
                case 4:             //변수
                    Debug.Log("버튼 입력 설정4");
                    longName.GetComponent<Button>().onClick.AddListener(()=>SetBoxValue(textBox, longName.GetComponent<FrameData>().variableValue,4,0));
                    break;
                case 5:                 //이미지
                    Debug.Log("버튼 입력 설정5");
                    longName.GetComponent<Button>().onClick.AddListener(()=>SetBoxValue(textBox, longName.GetComponent<FrameData>().imageValue,5,0));
                    break;
                default:
                    Debug.Log("메뉴 입력 오류, 프레임 읽어들이기 중단");
                    break;
            }
        }
    }
}