using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class chrReader : MonoBehaviour
{
    //받아온 path 저장
    public string pathFromMain;
    public string txtPath;

    //각 값들을 설정하고 불러올 오브젝트
    public InputField[] textBox;

    public GameObject target;       //데이터매니저를 받아올 오브젝트
    public string textValue;            //캐릭터 내용

    //리스트 생성용
    public GameObject makeNewList;


    ////리소스 로딩 영역
    public chrReader forPath;
    //경로 변수
    string patho;
    string pathForImg;
    string pathForSnd;
    string[] allFilesName;
    //리소스 오브젝트 재료
    public GameObject imageFrame;
    public GameObject soundFrame;
    public GameObject destination;
    public GameObject destination2;

    //불러온 파일
    public Texture2D tex=null;
    public Sprite spr=null;
    public byte[] fileData;
    public AudioClip audclip=null;

    //파일 읽기용 스트림
    BinaryReader rscReader;
    ///////////////////
    //저장 구현용
    public GameObject[] bigList;
    int listNow=0;
    


    //경로 지정
    private void SetPath()
    {
        GameObject PathFinder;

        PathFinder=GameObject.Find("LoadingManagerIn");             //로딩에서 넘겨받은 경로 탐색
        pathFromMain=PathFinder.GetComponent<ActiveLoading>().path; //저장된 경로 초기화
        Destroy(PathFinder);            //기존 경로 제거
    }

    //chr 파일 저장
    /*
기본정보->동작->효과->투사체->변수->이미지->사운드 순으로 값을 읽어내서 할당 공간에 값 입력.

- 스타트는 공백이므로. 실질적으로 문자 1번 위치부터 스타트

- *=총 대프레임 갯수 구분, 소프레임 개별값 구분, **=대프레임 단위 구분, |=소프레임 갯수 구분, 공통값 구분

- 전체적인 파일 규칙
** 대프레임수 * 소프레임수 | 공통값 | 개별값 * 소프레임수 | 공통값 | 개별값 * 소프레임수 | 공통값 | 개별값 ** 대프레임수 * 소프레임수 | 공통값 | 개별값 * 소프레임수 | 공통값 | 개별값 * 소프레임수 | 공통값 | 개별값 ** 대프레임수 * 소프레임수 | 공통값 | 개별값 * 소프레임수 | 공통값 | 개별값 * 소프레임수 | 공통값 | 개별값 **

- 전체적인 파일 규칙 (줄내림구분)
- 전체적인 파일 규칙
** 대프레임수 * 소프레임수 | 공통값 | 개별값 *
소프레임수 | 공통값 | 개별값 *
소프레임수 | 공통값 | 개별값 **
대프레임수 * 소프레임수 | 공통값 | 개별값 *
소프레임수 | 공통값 | 개별값 *
소프레임수 | 공통값 | 개별값 **
대프레임수 * 소프레임수 | 공통값 | 개별값 *
소프레임수 | 공통값 | 개별값 *
소프레임수 | 공통값 | 개별값 **

파일을 읽는 방법
- *, | 등으로 구분
- 스타트 : 1번부터 값을 읽기 시작함

- 최초 ** 접선 이후 : 총 대프레임갯수를 읽어서 대반복 시작
------------------------대반복 (총 대프레임갯수만큼)
- 반복 시작 내에서 : * 접선 후 소프레임수 읽어서 소반복 시작
-------------------------------소반복 (소프레임갯수만큼)
- 소반복 시작 내에서 : | 넘기고 프레임 단위별 공통값 읽어들임
- 공통값 이후 : | 넘기고 단위별 개별값 읽어들임
- 개별값 이후 : 반복 종료전에 * 접선 후, *다음이 *이면 대반복 종료 (대반복 조건값을 종료조건으로 초기화), 아니면 소반복 재시작 (가만히 있으면 될 듯)
-----------------------------소반복 종료
------------------------대반복 종료
    
    동작 효과 투사체 읽는 방식
해당 대프레임이 있는 주소를 찾아내서 대프레임의 오브제 갯수를 따냄
갯수를 반복의 범위에 넣어서 반복 실행.
	대프레임 내의 FrameData (SameValue, 범위<3)의 값을 불러옴
	해당 위치 번째의 대 프레임 내부에 소프레임 오브제 갯수를 따냄
		해당 소프레임의 FrameData (motValue, 범위<25)에 저장된 값을 차례차례 읽어내서 저장할 chr 문자열에 값 추가
    */
    public void SaveAs_chr()
    {
        //필요한 변수
        string savtextValue=" "; //불러온 값을 저장할 문자열

        //기본정보 읽어냄
        for(int i=0;i<30;i++)
        {
            Debug.Log(i);
            savtextValue+=textBox[i].transform.GetChild(2).gameObject.GetComponent<Text>().text;
            savtextValue+=" ";
        }

        savtextValue+="** ";

        //각 텍스트에 현재 설정되어 있는 값을 불러옴
        for(int i=0;i<6;i++)
        {
            listNow=i;
            //현재 대프레임의 갯수를 확인
            int howManyB=bigList[listNow].transform.childCount-1;
            savtextValue+=howManyB.ToString();
            savtextValue+=" * ";
            for(int j=1;j<howManyB+1;j++)
            {
                //현재 소프레임의 갯수 확인
                int howManyS=bigList[listNow].transform.GetChild(j).GetChild(1).childCount-1;
                savtextValue+=howManyS.ToString();
                savtextValue+=" | ";
                //해당 대프레임의 기본값을 가져옴
                //현재 순번에 따라 읽어들일 기본값의 갯수를 달리함
                switch(i)
                {
                    case 0:
                        for(int temper=0;temper<3;temper++)
                        {
                            savtextValue+=bigList[listNow].transform.GetChild(j).GetComponent<FrameData>().sameValue[temper];
                            savtextValue+=" ";
                        }
                        break;
                    case 1:
                        for(int temper=0;temper<4;temper++)
                        {
                            savtextValue+=bigList[listNow].transform.GetChild(j).GetComponent<FrameData>().sameValue[temper];
                            savtextValue+=" ";
                        }
                        break;
                    case 2:
                        for(int temper=0;temper<18;temper++)
                        {
                            savtextValue+=bigList[listNow].transform.GetChild(j).GetComponent<FrameData>().sameValue[temper];
                            savtextValue+=" ";
                        }
                        break;
                    case 3:
                        for(int temper=0;temper<3;temper++)
                        {
                            savtextValue+=bigList[listNow].transform.GetChild(j).GetComponent<FrameData>().sameValue[temper];
                            savtextValue+=" ";
                        }
                        break;
                    case 4:
                        for(int temper=0;temper<13;temper++)
                        {
                            savtextValue+=bigList[listNow].transform.GetChild(j).GetComponent<FrameData>().imageValue[temper];
                            savtextValue+=" ";
                        }
                        break;
                    case 5:
                        /*
                        for(int temper=0;temper<3;temper++)
                        {
                            savtextValue+=bigList[listNow].transform.GetChild(j).GetComponent<FrameData>().sameValue[temper];
                            savtextValue+=" ";
                        }
                        */
                        break;
                    default:
                        break;
                }
                savtextValue+="| ";
                for(int k=1;k<howManyS+1;k++)
                {
                    //해당 소프레임의 개별값을 가져옴
                    //현재 순번에 따라 읽어들일 기본값의 갯수를 달리함
                    switch(i)
                    {
                        case 0:
                            for(int temper=0;temper<26;temper++)
                            {
                                savtextValue+=bigList[listNow].transform.GetChild(j).GetChild(1).GetChild(k).GetComponent<FrameData>().motValue[temper];
                                savtextValue+=" ";
                            }
                            break;
                        case 1:
                            for(int temper=0;temper<68;temper++)
                            {
                                savtextValue+=bigList[listNow].transform.GetChild(j).GetChild(1).GetChild(k).GetComponent<FrameData>().effectValue[temper];
                                savtextValue+=" ";
                            }
                            break;
                        case 2:
                            for(int temper=0;temper<6;temper++)
                            {
                                savtextValue+=bigList[listNow].transform.GetChild(j).GetChild(1).GetChild(k).GetComponent<FrameData>().bulletValue[temper];
                                savtextValue+=" ";
                            }
                            break;
                        case 3:
                            for(int temper=0;temper<4;temper++)
                            {
                                savtextValue+=bigList[listNow].transform.GetChild(j).GetChild(1).GetChild(k).GetComponent<FrameData>().variableValue[temper];
                                savtextValue+=" ";
                            }
                            break;
                        case 4:
                            /*
                            for(int temper=0;temper<13;temper++)
                            {
                                savtextValue+=bigList[listNow].transform.GetChild(j).GetChild(1).GetChild(0).GetChild(k).GetComponent<FrameData>().motValue[temper];
                                savtextValue+=" ";
                            }
                            */
                            break;
                        case 5:
                            /*
                            for(int temper=0;temper<26;temper++)
                            {
                                savtextValue+=bigList[listNow].transform.GetChild(j).GetChild(1).GetChild(0).GetChild(k).GetComponent<FrameData>().motValue[temper];
                                savtextValue+=" ";
                            }
                            */
                            break;
                        default:
                            break;
                    }
                }

                //현재 순번이 마지막인지 아닌지 판별
                if(j+1>=howManyB)
                    savtextValue+="** ";
                else
                    savtextValue+="* ";
            }
        }
        
        //그 값들을 하나의 string value에 추가해줌
            //string value의 이름은 textValue로 설정
        
        //파일 작성
        System.IO.File.WriteAllText(txtPath, savtextValue);
        //파일 확장자 변경
        txtPath=Path.ChangeExtension(txtPath, ".chr");
    }
    

    //각 순서에 따라 파일의 각 값을 각 텍스트 오브젝트에 할당

        public int txtNow=0;     //텍스트의 값을 읽기 위함
        public int boxNow=0;    //텍스트 박스에 값을 할당하기 위함
        public string dummybox="";      //주석 등의 값 임시 저장을 위한 공간
        public string resultbox="";
        public int dumNow=0;            //더미박스용
    
    //값 읽어내기용
    //공백이면 다음칸도 공백인지 확인 후 넘김
    //ㄴ다음칸이 공백이 아니면 결과 반환
    //공백이 아니면 값을 임시 배열 한 칸에 저장 후 다음으로 넘겨줌
    public string readTxtValue()
    {
        string RTVdummybox="";              //읽은 값을 넣을 공간
        string RTVresultbox="";               //보낼 값을 넣을 공간
        while(true)
        {
            //현재 칸이 공백일 때
            if((int)textValue[txtNow]==' ')
            {
                //다음칸도 공백(주석처리?)일 때
                if(txtNow<textValue.Length && (int)textValue[txtNow+1]==32)
                //아스키 96==`
                //아스키 32==스페이스바 공백
                {
                    txtNow+=2;          //다음 칸으로 주소를 옮기고
                    return null;            //빈 값 반환
                }
                //다음칸에 값이 들어있을 때
                else
                {
                    RTVresultbox=RTVdummybox;           //결과 저장 후
                    //주석공백 처리
                    if(RTVresultbox=="`")
                    {
                        txtNow++;
                        return null;
                    }
                    //Debug.Log("Result="+RTVresultbox);
                    txtNow++;
                    return RTVresultbox;                        //결과 반환
                }
            }
            //현재 칸이 공백이 아닐 때
            else
            {
                RTVdummybox+=textValue[txtNow];         //현재 칸의 값을 쌓아줌
                txtNow++;                                                   //다음 칸으로 옮김
            }
        }
    }

    private void ReadFrames(int Count, Transform bigAdr, Transform smallAdr, GameObject ingre, GameObject ingre2, int menu)
    {
        if(Count!=0)
        {
            for(int i=0;i<Count;i++)                     //동작 갯수만큼 반복
            {
                //동작 생성
                GameObject tempObj1=Instantiate(ingre, bigAdr.localPosition, Quaternion.identity);    //클론 생성
                tempObj1.transform.SetParent(bigAdr);        //부모 설정
                tempObj1.transform.localPosition=new Vector3(300,81-(30*i),0);          //오브젝트 위치 설정
                tempObj1.transform.localScale=new Vector3(1f,1f,1f);        //오브젝트 크기 설정
                tempObj1.GetComponent<Text>().text=i.ToString();    //수치 처리

                //인스턴스 생성
                FrameData bigInstance=tempObj1.GetComponent<FrameData>();
                int valueNow=0;

                //프레임 읽어들임
                int frameCount=Convert.ToInt32(readTxtValue());
                txtNow+=2;          //|처리

                //공통값 제한
                int limittt=0;
                switch(menu)
                {
                    case 1:                 //동작
                        limittt=3;
                        break;
                    case 2:                 //효과
                        limittt=4;
                        break;
                    case 3:                 //투사체
                        limittt=18;     
                        break;
                    case 4:                 //변수
                        limittt=3;
                        break;
                    case 5:                 //이미지
                        limittt=14;
                        break;
                    default:
                        Debug.Log("메뉴 입력 오류, 프레임 읽어들이기 중단");
                        break;
                }

                //기본값 읽고 적용하기
                for(int j=0;j<limittt;j++)
                {
                    bigInstance.sameValue[valueNow]=readTxtValue();
                    valueNow++;
                }
                //임의의 메소드로 모은 값을 넘겨줌
                txtNow+=2;          //|처리
                valueNow=0;        //배열 위치 처리

                //소프레임 처리 시작
                /////////////프레임별 값 읽어들이기
                for(int j=0;j<frameCount;j++)
                {
                    //하위 프레임 생성
                    GameObject tempObj2=Instantiate(ingre2, bigAdr.transform.GetChild(i+1).GetChild(1).localPosition, Quaternion.identity);    //클론 생성
                    tempObj2.transform.SetParent(bigAdr.transform.GetChild(i+1).GetChild(1));        //부모 설정
                    tempObj2.transform.localPosition=new Vector3(360,81-(30*i),0);          //오브젝트 위치 설정
                    tempObj2.transform.localScale=new Vector3(1f,1f,1f);        //오브젝트 크기 설정
                    tempObj2.GetComponent<Text>().text=j.ToString();    //수치 처리
                    bigAdr.transform.GetChild(i+1).GetChild(1).gameObject.SetActive(false);

                    //인스턴스 생성
                    FrameData smallInstance=tempObj2.GetComponent<FrameData>();

                    //값 넣을 공간 설정
                    int smallLimit=0;
                    switch(menu)
                    {
                        case 1:                 //동작
                            boxNow=34;  //25+1개
                            smallLimit=26;
                            break;
                        case 2:                 //효과
                            boxNow=34;  //68+1개
                            smallLimit=68;
                            break;
                        case 3:                 //투사체
                            boxNow=34;  //6+1개
                            smallLimit=6;
                            break;
                        case 4:                 //변수
                            boxNow=34;  //4+1개
                            smallLimit=4;
                            break;
                        case 5:             //이미지
                            boxNow=34;  //13+1개
                            smallLimit=13;
                            break;
                        default:
                            Debug.Log("메뉴 입력 오류, 프레임 읽어들이기 중단");
                            break;
                    }            

                    //값을 읽음
                    for(int k=0;k<smallLimit;k++)
                    {
                        //값 저장
                        switch(menu)
                        {
                            case 1:                 //동작
                                smallInstance.motValue[valueNow]=readTxtValue();
                                break;
                            case 2:                 //효과
                                smallInstance.effectValue[valueNow]=readTxtValue();
                                break;
                            case 3:                 //투사체
                                smallInstance.bulletValue[valueNow]=readTxtValue();
                                break;
                            case 4:                 //변수
                                smallInstance.variableValue[valueNow]=readTxtValue();
                                break;
                            case 5:                 //이미지
                                smallInstance.imageValue[valueNow]=readTxtValue();
                                break;
                            default:
                                Debug.Log("메뉴 입력 오류!!!!!!!!!!!!!!!!!!!!");
                                break;
                        }
                        valueNow++;
                    }
                    /*
                    //버튼 설정
                    switch(menu)
                    {
                        case 1:             //동작
                            Debug.Log("버튼 입력 설정1");
                            tempObj2.GetComponent<Button>().onClick.AddListener(()=>smallInstance.SetBoxValue(textBox, smallInstance.motValue,1,0));
                            break;
                        case 2:             //효과
                            Debug.Log("버튼 입력 설정2");
                            tempObj2.GetComponent<Button>().onClick.AddListener(()=>smallInstance.SetBoxValue(textBox, smallInstance.motValue,1,0));
                            break;
                        case 3:             //투사체
                            Debug.Log("버튼 입력 설정3");
                            tempObj2.GetComponent<Button>().onClick.AddListener(()=>smallInstance.SetBoxValue(textBox, smallInstance.motValue,1,0));
                            break;
                        case 4:             //변수
                            Debug.Log("버튼 입력 설정4");
                            tempObj2.GetComponent<Button>().onClick.AddListener(()=>smallInstance.SetBoxValue(textBox, smallInstance.motValue,1,0));
                            break;
                        case 5:             //이미지
                            Debug.Log("버튼 입력 설정5");
                            tempObj2.GetComponent<Button>().onClick.AddListener(()=>smallInstance.SetBoxValue(textBox, smallInstance.motValue,1,0));
                            break;
                        default:
                            Debug.Log("메뉴 입력 오류, 프레임 읽어들이기 중단");
                            break;
                    }
                    */
                }
                txtNow+=2;              //* 처리
                //버튼 지정
                Debug.Log(i);
                //tempObj1.GetComponent<Button>().onClick.AddListener(()=>bigInstance.ShowFrames(bigAdr.GetChild(i+1).GetChild(1), tempObj1));
                tempObj1.GetComponent<Button>().onClick.AddListener(()=>bigInstance.ShowFrames(tempObj1.transform.GetChild(1), tempObj1, smallAdr, frameCount, menu, textBox));
                tempObj1.GetComponent<Button>().onClick.AddListener(()=>bigInstance.SetBoxValue(textBox, bigInstance.sameValue,1,1));
            }
        }
    }

    private void Awake()
    {
        //경로 가져오기
        target=GameObject.Find("LoadingManagerInUsed");             //오브젝트를 찾아냄
        textValue=target.GetComponent<DataManager>().textValue; //텍스트 가져오기
        txtPath=target.GetComponent<DataManager>().textPath;
        Destroy(target);            //다 쓴 매니저파일은 제거
        ////////////////////////
        //기본정보 값을 읽음
        txtNow=1;
        int defaultData=29;
        for(int i=0;i<defaultData;i++)
        {
            //각 값을 입력받아서 텍스트 박시 안에 넣어봄
            string inputData=readTxtValue();
            if(inputData==null)
            {
                continue;
            }
            textBox[boxNow].GetComponent<UnityEngine.UI.InputField>().text=inputData;
            boxNow++;
        }

        //동작 값을 읽음
        txtNow+=6;              //**처리
        int motCount=Convert.ToInt32(readTxtValue());         //동작 갯수 읽어들임
        txtNow+=2;             //* 처리

        //프레임 주소 설정
        Transform motTemp1=GameObject.Find("Canvas").transform.GetChild(1).GetChild(1).GetChild(17).GetChild(0).GetChild(0);                //동작 프레임 주소
        GameObject motTemp2=GameObject.Find("FrameGotMotData");         //사용할 동작 오브젝트
        Transform motTemp3=GameObject.Find("Canvas").transform.GetChild(1).GetChild(1).GetChild(18).GetChild(0).GetChild(0);                //하위 프레임 주소
        GameObject motTemp4=GameObject.Find("FrameGotFrame");         //사용할 하위 오브젝트
        //프레임 읽어냄
        ReadFrames(motCount, motTemp1, motTemp3, motTemp2, motTemp4, 1);

        //효과 값을 읽음
        txtNow+=1;              //**처리
        int effCount=Convert.ToInt32(readTxtValue());         //효과 갯수 읽어들임
        txtNow+=2;             //* 처리

        //프레임 주소 설정
        Transform effTemp1=GameObject.Find("Canvas").transform.GetChild(4).GetChild(1).GetChild(20).GetChild(0).GetChild(0);                //동작 프레임 주소
        GameObject effTemp2=GameObject.Find("FrameGotEffData");         //사용할 효과 오브젝트
        Transform effTemp3=GameObject.Find("Canvas").transform.GetChild(4).GetChild(1).GetChild(21).GetChild(0).GetChild(0);                //하위 프레임 주소
        GameObject effTemp4=GameObject.Find("FrameGotFrame");         //사용할 하위 오브젝트

        ReadFrames(effCount, effTemp1, effTemp3, effTemp2, effTemp4, 2);


        //투사체 값을 읽음
        txtNow+=3;              //**처리
        int bulCount=Convert.ToInt32(readTxtValue());         //효과 갯수 읽어들임
        txtNow+=2;             //* 처리

        //프레임 주소 설정
        Transform bulTemp1=GameObject.Find("Canvas").transform.GetChild(5).GetChild(1).GetChild(19).GetChild(0).GetChild(0);                //동작 프레임 주소
        GameObject bulTemp2=GameObject.Find("FrameGotBulData");         //사용할 효과 오브젝트
        Transform bulTemp3=GameObject.Find("Canvas").transform.GetChild(5).GetChild(1).GetChild(20).GetChild(0).GetChild(0);                //하위 프레임 주소
        GameObject bulTemp4=GameObject.Find("FrameGotFrame");         //사용할 하위 오브젝트

        ReadFrames(bulCount, bulTemp1, bulTemp3, bulTemp2, bulTemp4, 3);


        //변수 값을 읽음
        txtNow+=3;              //**처리
        int varCount=Convert.ToInt32(readTxtValue());         //효과 갯수 읽어들임
        txtNow+=2;             //* 처리

        //프레임 주소 설정
        Transform varTemp1=GameObject.Find("Canvas").transform.GetChild(6).GetChild(1).GetChild(10).GetChild(0).GetChild(0);                //동작 프레임 주소
        GameObject varTemp2=GameObject.Find("FrameGotVarData");         //사용할 효과 오브젝트
        Transform varTemp3=GameObject.Find("Canvas").transform.GetChild(6).GetChild(1).GetChild(11).GetChild(0).GetChild(0);                //하위 프레임 주소
        GameObject varTemp4=GameObject.Find("FrameGotFrame");         //사용할 하위 오브젝트

        ReadFrames(varCount, varTemp1, varTemp3, varTemp2, varTemp4, 4);


        //이미지 값을 읽음
        ////구현해야 할거
        /*
        image 폴더에서 이미지 리스트를 불러옴 V
        불러온 이미지에 번호를 매겨줌 V
        불러온 순서대로 이미지 리스트화 및 출력 V
        리스트에서는 불러온 이미지 미리보기 출력
        이미지 번호 클릭시 우측에 이미지 보여줄 것
        */
        //경로를 받아옴
        patho=Path.GetFullPath(Path.Combine(txtPath,@"../"));
        //이미지 폴더 경로, 사운드 폴더 경로 설정
        pathForImg=patho+"image";
        pathForSnd=patho+"sound";

        //파일을 읽어들임
        ReadImage();

        allFilesName=null;
        fileData=null;
        //사운드 값을 읽음
        ////구현해야 할거
        /*
        sound 폴더에서 사운드 리스트를 불러옴 V
        불러온  사운드에 번호를 매겨줌
        불러온 순서대로 사운드 리스트화 및 출력
        사운드 번호 클릭시 사운드 재생
        다른 곳을 클릭하면 재생 중단
        */
        ReadSound();

        //여기서 이미지 로딩?
        
    }

    public void ReadImage()
    {
        //파일을 불러오기 전 모든 png 파일명을 가져옴
        allFilesName=Directory.GetFiles(pathForImg, "*.png", SearchOption.AllDirectories);
        //파일들을 하나하나 불러오기 시작함
        for(int i=0;i<allFilesName.Length;i++)
        {
            //파일을 본격적으로 불러옴
            rscReader=new BinaryReader(new FileStream(allFilesName[i], FileMode.Open));
            fileData=rscReader.ReadBytes(int.MaxValue);

            //불러온 이진 데이터를 이미지로 처리
            tex=new Texture2D(2,2);
            tex.LoadImage(fileData);


            //이미지 처리한 것을 오브젝트로 추가
            GameObject imgClone=Instantiate(imageFrame, imageFrame.transform.position, imageFrame.transform.rotation);
            imgClone.transform.SetParent(destination.transform);
            imgClone.GetComponent<Text>().text=i.ToString();
            spr=Sprite.Create(tex, new Rect(0,0,tex.width,tex.height), new Vector2(1f,1f));
            imgClone.transform.GetChild(1).gameObject.GetComponent<Image>().sprite=spr;
            imgClone.transform.localScale=new Vector3(1f, 1f, 1f);
            imgClone.transform.localPosition=new Vector3(0,34-(30*i),0);
        }
        rscReader.Close();
    }

    private float[] ConvertByteToFloat(byte[] array)
    {
        float[] floatArr=new float[array.Length/4];
        for(int i=0;i<floatArr.Length;i++)
        {
            if(BitConverter.IsLittleEndian)
            {
                Array.Reverse(array, i*4, 4);
            }
            floatArr[i]=BitConverter.ToSingle(array, i*4)/0x80000000;
        }

        return floatArr;
    }

    public void ReadSound()
    {
        //파일을 불러오기 전 모든 ogg 파일명을 가져옴
        allFilesName=Directory.GetFiles(pathForSnd, "*.ogg", SearchOption.AllDirectories);

        //파일들을 하나하나 불러오기 시작함
        for(int i=0;i<allFilesName.Length;i++)
        {
            //파일을 본격적으로 불러옴
            rscReader=new BinaryReader(new FileStream(allFilesName[i], FileMode.Open));
            fileData=rscReader.ReadBytes(int.MaxValue);

            //이미지 처리한 것을 오브젝트로 추가
            GameObject sndClone=Instantiate(soundFrame, soundFrame.transform.position, soundFrame.transform.rotation);
            sndClone.transform.SetParent(destination2.transform);
            sndClone.GetComponent<Text>().text=i.ToString();

            audclip=AudioClip.Create(allFilesName[i], allFilesName[i].Length, 1, 48000, false);
            float[] f=ConvertByteToFloat(fileData);
            audclip.SetData(f,0);
            sndClone.GetComponent<AudioSource>().clip=audclip;

            sndClone.transform.localScale=new Vector3(1f, 1f, 1f);
            sndClone.transform.localPosition=new Vector3(-30,34-(30*i),0);
        }
    }
}

/*
기본정보->동작->효과->투사체->변수->이미지->사운드 순으로 값을 읽어내서 할당 공간에 값 입력.

- 스타트는 공백이므로. 실질적으로 문자 1번 위치부터 스타트

- *=총 대프레임 갯수 구분, 소프레임 개별값 구분, **=대프레임 단위 구분, |=소프레임 갯수 구분, 공통값 구분

- 전체적인 파일 규칙
** 대프레임수 * 소프레임수 | 공통값 | 개별값 * 소프레임수 | 공통값 | 개별값 * 소프레임수 | 공통값 | 개별값 ** 대프레임수 * 소프레임수 | 공통값 | 개별값 * 소프레임수 | 공통값 | 개별값 * 소프레임수 | 공통값 | 개별값 ** 대프레임수 * 소프레임수 | 공통값 | 개별값 * 소프레임수 | 공통값 | 개별값 * 소프레임수 | 공통값 | 개별값 **

- 전체적인 파일 규칙 (줄내림구분)
- 전체적인 파일 규칙
** 대프레임수 * 소프레임수 | 공통값 | 개별값 *
소프레임수 | 공통값 | 개별값 *
소프레임수 | 공통값 | 개별값 **
대프레임수 * 소프레임수 | 공통값 | 개별값 *
소프레임수 | 공통값 | 개별값 *
소프레임수 | 공통값 | 개별값 **
대프레임수 * 소프레임수 | 공통값 | 개별값 *
소프레임수 | 공통값 | 개별값 *
소프레임수 | 공통값 | 개별값 **

파일을 읽는 방법
- *, | 등으로 구분
- 스타트 : 1번부터 값을 읽기 시작함

- 최초 ** 접선 이후 : 총 대프레임갯수를 읽어서 대반복 시작
------------------------대반복 (총 대프레임갯수만큼)
- 반복 시작 내에서 : * 접선 후 소프레임수 읽어서 소반복 시작
-------------------------------소반복 (소프레임갯수만큼)
- 소반복 시작 내에서 : | 넘기고 프레임 단위별 공통값 읽어들임
- 공통값 이후 : | 넘기고 단위별 개별값 읽어들임
- 개별값 이후 : 반복 종료전에 * 접선 후, *다음이 *이면 대반복 종료 (대반복 조건값을 종료조건으로 초기화), 아니면 소반복 재시작 (가만히 있으면 될 듯)
-----------------------------소반복 종료
------------------------대반복 종료
*/









/*
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class chrReader : MonoBehaviour
{
    //받아온 path 저장
    public string pathFromMain;
    public string txtPath;

    //각 값들을 설정하고 불러올 오브젝트
    public InputField[] textBox;

    public GameObject target;       //데이터매니저를 받아올 오브젝트
    public string textValue;            //캐릭터 내용

    //리스트 생성용
    public GameObject makeNewList;

    //경로 지정
    private void SetPath()
    {
        GameObject PathFinder;

        PathFinder=GameObject.Find("LoadingManagerIn");             //로딩에서 넘겨받은 경로 탐색
        pathFromMain=PathFinder.GetComponent<ActiveLoading>().path; //저장된 경로 초기화
        Destroy(PathFinder);            //기존 경로 제거
    }


    //chr 파일 저장
    public void SaveAs_chr()
    {
        //각 텍스트에 현재 설정되어 있는 값을 불러옴
        //그 값들을 하나의 string value에 추가해줌
            //string value의 이름은 textValue로 설정
        
        //파일 작성
        //System.IO.File.WriteAllText(txtPath, textValue, Encoding.Default);
        
        //파일 확장자 변경

    }

    //각 순서에 따라 파일의 각 값을 각 텍스트 오브젝트에 할당

        public int txtNow=2;     //텍스트의 값을 읽기 위함
        public int boxNow=0;    //텍스트 박스에 값을 할당하기 위함
        public string dummybox="";      //주석 등의 값 임시 저장을 위한 공간
        public string resultbox="";
        public int dumNow=0;            //더미박스용


    public string readTxtValue()
    {
        string giveResult="";
        while(true)
        {
            if(textValue[txtNow]!=0)       //공백이 아니면
            {//값 모음
                dummybox+=textValue[txtNow].ToString();
                //Debug.Log("dummybox="+dummybox+"+"+textValue[txtNow].ToString());
                txtNow++;
                dumNow++;
            }
            else        //공백이면
            {//값 할당
                //Debug.Log("End, "+dumNow+"+"+txtNow+"+"+textValue[txtNow]);
                for(int l=0;l<dumNow;l++)        //더미박스의 값 할당
                {
                    resultbox+=dummybox[l];
                }
                //Debug.Log("resultbox="+resultbox+",,,,,"+dumNow);
                dumNow=0;
                //Debug.Log("Actual Work?"+dumNow);
                dummybox="";
                //if(textValue[txtNow+1]==0)
                if(String.IsNullOrWhiteSpace(textValue[txtNow].ToString()))
                {
                    txtNow++;
                    boxNow--;
                    return null;
                }
                txtNow++;
                boxNow++;
                giveResult=resultbox;
                break;
            }
        }
        resultbox="";
        //Debug.Log("GiveResult="+giveResult);
        return giveResult;
    }

    //chr 파일 불러오기
    public void Awake()
    {
        target=GameObject.Find("LoadingManagerInUsed");             //오브젝트를 찾아냄
        textValue=target.GetComponent<DataManager>().textValue; //텍스트 가져오기
        Destroy(target);            //다 쓴 매니저파일은 제거
        ////////////////////////////////////
        //기본정보 불러오기
        boxNow--;
        while(boxNow<30)     //파일의 끝에 도달하기 전까지
        {
            if(boxNow==29)
            {
                txtNow+=2;
                boxNow++;
                continue;
            }
            string inData=readTxtValue();
            //Debug.Log("Check1,"+boxNow);
            textBox[boxNow].GetComponent<UnityEngine.UI.InputField>().text=inData;
            //resultbox="";
        }
        boxNow++;

        dummybox="";
        resultbox="";

        ///////////////////////////////////////////
        //동작 불러오기
        while(textValue[txtNow]!=0)      //공백을 최초로 마주하기 전까지
        {
            dummybox+=textValue[txtNow].ToString();
            txtNow++;
            dumNow++;
        }
        //값을 다 읽었으면
        for(int l=0;l<dumNow;l++)        //더미박스의 값 할당
        {
            resultbox+=dummybox[l];
        }
        dumNow=0;
        //리스트 생성
        Transform motTemp1=GameObject.Find("Canvas").transform.GetChild(1).GetChild(1).GetChild(17).GetChild(0).GetChild(0);                //동작 프레임 주소
        GameObject motTemp2=GameObject.Find("FrameGotMotData");         //사용할 동작 오브젝트
        Transform motTemp3=GameObject.Find("Canvas").transform.GetChild(1).GetChild(1).GetChild(18).GetChild(0).GetChild(0);                //하위 프레임 주소
        Debug.Log("Some Check,"+resultbox+","+Int32.Parse(resultbox));      //동작 갯수 확인

        Debug.Log("Begin Point="+txtNow);
        //동작 프레임 생성
        CallFrame(motTemp1, motTemp2, motTemp3, Int32.Parse(resultbox),1);
        
        //초기화
        dummybox="";
        resultbox="";
        txtNow++;
        boxNow++;
    }
    public void CallFrame(Transform addressOfBody, GameObject ingObject, Transform addressOfFrame, int countOfMot, int menu)
    {
        txtNow=106;
        Debug.Log("Operate method 'CallFrame");
        int temp1=0;
        while(temp1<countOfMot)     //동작수를 읽어옴
        {
            //프레임 생성
            GameObject tempObj1=Instantiate(ingObject, addressOfBody.localPosition, Quaternion.identity);    //클론 생성
            tempObj1.transform.SetParent(addressOfBody);        //부모 설정
            tempObj1.transform.localPosition=new Vector3(300,81-(30*temp1),0);          //오브젝트 위치 설정
            tempObj1.transform.localScale=new Vector3(1f,1f,1f);        //오브젝트 크기 설정
            tempObj1.GetComponent<Text>().text=temp1.ToString();    //수치 처리
            Debug.Log("Successfully cloned objects");
            int valueLimitation=0;
            switch(menu)
            {
                case 1:
                    valueLimitation=28;
                    break;
                case 2:
                    valueLimitation=72;
                    break;
                case 3:
                    valueLimitation=23;
                    break;
                case 4:
                    valueLimitation=7;
                    break;
                case 5:
                    valueLimitation=80;
                    break;
                default:
                    valueLimitation=100;
                    break;
            }
            //프레임 수 읽기
            string[] valueList=new string[valueLimitation];            //각각의 값을 저장해둘 배열
            string targetValue=readTxtValue();      //프레임 수를 읽어옴
            //resultbox="";           //임시저장공간 초기화
            //Debug.Log("targetValue="+targetValue+"////"+txtNow);
            int countOfFrame=Convert.ToInt32(targetValue);      //프레임 수를 정수형으로써 받아냄

            Debug.Log("Set for reading");
            int temp2=0;            //프레임 값 읽기 시작점용
            int temp3=0;            //읽어들일 값 저장용
            switch(menu)
            {
                case 1:                 //동작을 읽는 경우
                    //쿨타임, 필요에너지 읽기
                    for(int i=0;i<3;i++)
                    {
                        Debug.Log("Inner Range,"+i);
                        valueList[i]=readTxtValue();                //오류 잡아놓을 것
                        //resultbox="";
                    }
                    //동작 주석 읽기
                    if(readT)     //주석이 존재할 때 주석 처리
                    {
                        tempObj1.GetComponent<Text>().text+=;
                    }
                    //resultbox="";
                    
                    temp2=2;
                    temp3=27;           //25+2
                    break;

                case 2:                 //효과를 읽는 경우
                    //프레임 수, 이미지 깊이, 속성 읽기
                    for(int i=0;i<3;i++)
                    {
                        valueList[i]=readTxtValue();
                        //resultbox="";
                    }
                    //효과 주석 읽기
                    string com2=readTxtValue();
                    if(com2!="")     //주석이 존재할 때 주석 처리
                    {
                        tempObj1.GetComponent<Text>().text+=com2;
                    }
                    //resultbox="";

                    temp2=3;
                    temp3=71;            //68+3
                    break;

                case 3:                 //투사체를 읽는 경우
                    //프레임 수, 이미지 깊이, 속성, 피해량, 수평넉백, 수직넉백, 경직시간, 다운시간, 타격효과, 피격소리, 피격시에너지변화, 지면효과, 벽면효과, 차단효과, 대상효과, 만료효과, 파괴효과 읽기
                    for(int i=0;i<17;i++)
                    {
                        valueList[i]=readTxtValue();
                        //resultbox="";
                    }
                    //투사체 주석 읽기
                    string com3=readTxtValue();
                    if(com3!="")     //주석이 존재할 때 주석 처리
                    {
                        tempObj1.GetComponent<Text>().text+=com3;
                    }
                    //resultbox="";

                    temp2=17;
                    temp3=23;           //6+17
                    break;

                case 4:                 //변수를 읽는 경우
                    //기본값 읽기
                    valueList[0]=readTxtValue();
                    //resultbox="";
                    //변수명 읽기
                    tempObj1.GetComponent<Text>().text=readTxtValue();
                    //resultbox="";

                    //변수 주석 읽기
                    string com4=readTxtValue();
                    if(com4!="")     //주석이 존재할 때 주석 처리
                    {
                        tempObj1.GetComponent<Text>().text+=com4;
                    }
                    //resultbox="";

                    temp2=1;
                    temp3=7;            //6+1
                    break;

                case 5:                 //이미지를 읽는 경우
                    break;

                default:                //오류
                    Debug.Log("메뉴 입력 오류 발생.");
                    break;
            }

            Debug.Log("After Reading each values");
            //각 프레임을 읽어냄
            //기본 정보 읽기
            //각 프레임 정보 읽기

            %%%%%%%%%%%%%%%%%%%%%%
            현재 문제 : 값 읽기를 적절한 시점에서 끝내지 않음 (첫 동작에서 
            현재 변수까지 읽고 입력형식 오류로 예외를 던지고 있음)
            해결 방안 : 값을 읽는 범위를 조정해서, 필요한 범위만큼만 값을 읽게끔 조정
            %%%%%%%%%%%%%%%%%%%%%%

            //Debug.Log(textValue[108]+"///"+textValue[109]);           //결과는 6(109)/// (110)
            for(int i=0;i<countOfFrame;i++)         //프레임 수 만큼 값을 읽어들임
            {
                //for(int j=temp2;j<temp3;j++)
                while(temp2<temp3)
                {   //각 프레임의 값을 배열에 추가해냄.
                    while(true)
                    {
                        if(String.IsNullOrWhiteSpace(textValue[txtNow].ToString())==false)       //공백이 아니면
                        {//값 모음
                            dummybox+=textValue[txtNow];
                            txtNow++;
                            dumNow++;
                        }
                        else        //공백이면
                        {               //값 할당
                            try             //예외 검사
                            {
                                if(textValue[txtNow+1]==0&&textValue[txtNow-1]==0)      //현재 칸 앞뒤로 공백이면 == 입력된 주석이 없다면
                                {
                                txtNow++;
                                continue;       //다음칸으로 넘김
                                }
                            }
                            catch(Exception e)
                            {
                                Debug.Log("범위 초과 에러 발생 : "+txtNow+","+temp2+"//////"+e);
                            }   
                            finally
                            {
                                txtNow++;
                            }
                            for(int l=0;l<dumNow;l++)        //더미박스의 값 할당
                            {
                                resultbox+=dummybox[l];
                            }
                            //값 저장
                            valueList[temp2]=resultbox;
                            //초기화
                            dumNow=0;
                            dummybox="";
                            txtNow++;
                            resultbox="";
                            break;
                        }
                    }
                    temp2++;
                }
            }
            //동작 25+1
            //효과 68+1
            //투사체 6+1
            //변수 6+1
            Debug.Log("Finally go to here");
            /*
            //하위프레임 생성
            for(int i=0;i<Convert.ToInt32(valueList[0]);i++)
            {
                GameObject tempObj2=Instantiate(ingObject, tempObj1.transform.localPosition, Quaternion.identity);    //클론 생성
                tempObj2.transform.SetParent(tempObj1.transform);        //부모 설정
                tempObj2.transform.localPosition=new Vector3(300,81-(30*i),0);          //오브젝트 위치 설정
                tempObj2.transform.localScale=new Vector3(1f,1f,1f);        //오브젝트 크기 설정
                tempObj2.GetComponent<Text>().text=i.ToString();    //수치 처리
            }
            //프레임 버튼 설정 (해당 버튼을 누르면 값 할당 메소드에 미리 읽어낸 텍스트 밸류를 넘겨줌)
        }
    }

    public void SetFrameData()
    {
        //텍스트 밸류 값 분석
        //분석 된 값을 적절한 박스에 설정
    }
}
*/