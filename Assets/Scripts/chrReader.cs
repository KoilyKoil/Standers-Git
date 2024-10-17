using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class chrReader : MonoBehaviour
{
    //�޾ƿ� path ����
    public string pathFromMain;
    public string txtPath;

    //�� ������ �����ϰ� �ҷ��� ������Ʈ
    public InputField[] textBox;

    public GameObject target;       //�����͸Ŵ����� �޾ƿ� ������Ʈ
    public string textValue;            //ĳ���� ����

    //����Ʈ ������
    public GameObject makeNewList;


    ////���ҽ� �ε� ����
    public chrReader forPath;
    //��� ����
    string patho;
    string pathForImg;
    string pathForSnd;
    string[] allFilesName;
    //���ҽ� ������Ʈ ���
    public GameObject imageFrame;
    public GameObject soundFrame;
    public GameObject destination;
    public GameObject destination2;

    //�ҷ��� ����
    public Texture2D tex=null;
    public Sprite spr=null;
    public byte[] fileData;
    public AudioClip audclip=null;

    //���� �б�� ��Ʈ��
    BinaryReader rscReader;
    ///////////////////
    //���� ������
    public GameObject[] bigList;
    int listNow=0;
    


    //��� ����
    private void SetPath()
    {
        GameObject PathFinder;

        PathFinder=GameObject.Find("LoadingManagerIn");             //�ε����� �Ѱܹ��� ��� Ž��
        pathFromMain=PathFinder.GetComponent<ActiveLoading>().path; //����� ��� �ʱ�ȭ
        Destroy(PathFinder);            //���� ��� ����
    }

    //chr ���� ����
    /*
�⺻����->����->ȿ��->����ü->����->�̹���->���� ������ ���� �о�� �Ҵ� ������ �� �Է�.

- ��ŸƮ�� �����̹Ƿ�. ���������� ���� 1�� ��ġ���� ��ŸƮ

- *=�� �������� ���� ����, �������� ������ ����, **=�������� ���� ����, |=�������� ���� ����, ���밪 ����

- ��ü���� ���� ��Ģ
** �������Ӽ� * �������Ӽ� | ���밪 | ������ * �������Ӽ� | ���밪 | ������ * �������Ӽ� | ���밪 | ������ ** �������Ӽ� * �������Ӽ� | ���밪 | ������ * �������Ӽ� | ���밪 | ������ * �������Ӽ� | ���밪 | ������ ** �������Ӽ� * �������Ӽ� | ���밪 | ������ * �������Ӽ� | ���밪 | ������ * �������Ӽ� | ���밪 | ������ **

- ��ü���� ���� ��Ģ (�ٳ�������)
- ��ü���� ���� ��Ģ
** �������Ӽ� * �������Ӽ� | ���밪 | ������ *
�������Ӽ� | ���밪 | ������ *
�������Ӽ� | ���밪 | ������ **
�������Ӽ� * �������Ӽ� | ���밪 | ������ *
�������Ӽ� | ���밪 | ������ *
�������Ӽ� | ���밪 | ������ **
�������Ӽ� * �������Ӽ� | ���밪 | ������ *
�������Ӽ� | ���밪 | ������ *
�������Ӽ� | ���밪 | ������ **

������ �д� ���
- *, | ������ ����
- ��ŸƮ : 1������ ���� �б� ������

- ���� ** ���� ���� : �� �������Ӱ����� �о ��ݺ� ����
------------------------��ݺ� (�� �������Ӱ�����ŭ)
- �ݺ� ���� ������ : * ���� �� �������Ӽ� �о �ҹݺ� ����
-------------------------------�ҹݺ� (�������Ӱ�����ŭ)
- �ҹݺ� ���� ������ : | �ѱ�� ������ ������ ���밪 �о����
- ���밪 ���� : | �ѱ�� ������ ������ �о����
- ������ ���� : �ݺ� �������� * ���� ��, *������ *�̸� ��ݺ� ���� (��ݺ� ���ǰ��� ������������ �ʱ�ȭ), �ƴϸ� �ҹݺ� ����� (������ ������ �� ��)
-----------------------------�ҹݺ� ����
------------------------��ݺ� ����
    
    ���� ȿ�� ����ü �д� ���
�ش� ���������� �ִ� �ּҸ� ã�Ƴ��� ���������� ������ ������ ����
������ �ݺ��� ������ �־ �ݺ� ����.
	�������� ���� FrameData (SameValue, ����<3)�� ���� �ҷ���
	�ش� ��ġ ��°�� �� ������ ���ο� �������� ������ ������ ����
		�ش� ���������� FrameData (motValue, ����<25)�� ����� ���� �������� �о�� ������ chr ���ڿ��� �� �߰�
    */
    public void SaveAs_chr()
    {
        //�ʿ��� ����
        string savtextValue=" "; //�ҷ��� ���� ������ ���ڿ�

        //�⺻���� �о
        for(int i=0;i<30;i++)
        {
            Debug.Log(i);
            savtextValue+=textBox[i].transform.GetChild(2).gameObject.GetComponent<Text>().text;
            savtextValue+=" ";
        }

        savtextValue+="** ";

        //�� �ؽ�Ʈ�� ���� �����Ǿ� �ִ� ���� �ҷ���
        for(int i=0;i<6;i++)
        {
            listNow=i;
            //���� ���������� ������ Ȯ��
            int howManyB=bigList[listNow].transform.childCount-1;
            savtextValue+=howManyB.ToString();
            savtextValue+=" * ";
            for(int j=1;j<howManyB+1;j++)
            {
                //���� ���������� ���� Ȯ��
                int howManyS=bigList[listNow].transform.GetChild(j).GetChild(1).childCount-1;
                savtextValue+=howManyS.ToString();
                savtextValue+=" | ";
                //�ش� ���������� �⺻���� ������
                //���� ������ ���� �о���� �⺻���� ������ �޸���
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
                    //�ش� ���������� �������� ������
                    //���� ������ ���� �о���� �⺻���� ������ �޸���
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

                //���� ������ ���������� �ƴ��� �Ǻ�
                if(j+1>=howManyB)
                    savtextValue+="** ";
                else
                    savtextValue+="* ";
            }
        }
        
        //�� ������ �ϳ��� string value�� �߰�����
            //string value�� �̸��� textValue�� ����
        
        //���� �ۼ�
        System.IO.File.WriteAllText(txtPath, savtextValue);
        //���� Ȯ���� ����
        txtPath=Path.ChangeExtension(txtPath, ".chr");
    }
    

    //�� ������ ���� ������ �� ���� �� �ؽ�Ʈ ������Ʈ�� �Ҵ�

        public int txtNow=0;     //�ؽ�Ʈ�� ���� �б� ����
        public int boxNow=0;    //�ؽ�Ʈ �ڽ��� ���� �Ҵ��ϱ� ����
        public string dummybox="";      //�ּ� ���� �� �ӽ� ������ ���� ����
        public string resultbox="";
        public int dumNow=0;            //���̹ڽ���
    
    //�� �о���
    //�����̸� ����ĭ�� �������� Ȯ�� �� �ѱ�
    //������ĭ�� ������ �ƴϸ� ��� ��ȯ
    //������ �ƴϸ� ���� �ӽ� �迭 �� ĭ�� ���� �� �������� �Ѱ���
    public string readTxtValue()
    {
        string RTVdummybox="";              //���� ���� ���� ����
        string RTVresultbox="";               //���� ���� ���� ����
        while(true)
        {
            //���� ĭ�� ������ ��
            if((int)textValue[txtNow]==' ')
            {
                //����ĭ�� ����(�ּ�ó��?)�� ��
                if(txtNow<textValue.Length && (int)textValue[txtNow+1]==32)
                //�ƽ�Ű 96==`
                //�ƽ�Ű 32==�����̽��� ����
                {
                    txtNow+=2;          //���� ĭ���� �ּҸ� �ű��
                    return null;            //�� �� ��ȯ
                }
                //����ĭ�� ���� ������� ��
                else
                {
                    RTVresultbox=RTVdummybox;           //��� ���� ��
                    //�ּ����� ó��
                    if(RTVresultbox=="`")
                    {
                        txtNow++;
                        return null;
                    }
                    //Debug.Log("Result="+RTVresultbox);
                    txtNow++;
                    return RTVresultbox;                        //��� ��ȯ
                }
            }
            //���� ĭ�� ������ �ƴ� ��
            else
            {
                RTVdummybox+=textValue[txtNow];         //���� ĭ�� ���� �׾���
                txtNow++;                                                   //���� ĭ���� �ű�
            }
        }
    }

    private void ReadFrames(int Count, Transform bigAdr, Transform smallAdr, GameObject ingre, GameObject ingre2, int menu)
    {
        if(Count!=0)
        {
            for(int i=0;i<Count;i++)                     //���� ������ŭ �ݺ�
            {
                //���� ����
                GameObject tempObj1=Instantiate(ingre, bigAdr.localPosition, Quaternion.identity);    //Ŭ�� ����
                tempObj1.transform.SetParent(bigAdr);        //�θ� ����
                tempObj1.transform.localPosition=new Vector3(300,81-(30*i),0);          //������Ʈ ��ġ ����
                tempObj1.transform.localScale=new Vector3(1f,1f,1f);        //������Ʈ ũ�� ����
                tempObj1.GetComponent<Text>().text=i.ToString();    //��ġ ó��

                //�ν��Ͻ� ����
                FrameData bigInstance=tempObj1.GetComponent<FrameData>();
                int valueNow=0;

                //������ �о����
                int frameCount=Convert.ToInt32(readTxtValue());
                txtNow+=2;          //|ó��

                //���밪 ����
                int limittt=0;
                switch(menu)
                {
                    case 1:                 //����
                        limittt=3;
                        break;
                    case 2:                 //ȿ��
                        limittt=4;
                        break;
                    case 3:                 //����ü
                        limittt=18;     
                        break;
                    case 4:                 //����
                        limittt=3;
                        break;
                    case 5:                 //�̹���
                        limittt=14;
                        break;
                    default:
                        Debug.Log("�޴� �Է� ����, ������ �о���̱� �ߴ�");
                        break;
                }

                //�⺻�� �а� �����ϱ�
                for(int j=0;j<limittt;j++)
                {
                    bigInstance.sameValue[valueNow]=readTxtValue();
                    valueNow++;
                }
                //������ �޼ҵ�� ���� ���� �Ѱ���
                txtNow+=2;          //|ó��
                valueNow=0;        //�迭 ��ġ ó��

                //�������� ó�� ����
                /////////////�����Ӻ� �� �о���̱�
                for(int j=0;j<frameCount;j++)
                {
                    //���� ������ ����
                    GameObject tempObj2=Instantiate(ingre2, bigAdr.transform.GetChild(i+1).GetChild(1).localPosition, Quaternion.identity);    //Ŭ�� ����
                    tempObj2.transform.SetParent(bigAdr.transform.GetChild(i+1).GetChild(1));        //�θ� ����
                    tempObj2.transform.localPosition=new Vector3(360,81-(30*i),0);          //������Ʈ ��ġ ����
                    tempObj2.transform.localScale=new Vector3(1f,1f,1f);        //������Ʈ ũ�� ����
                    tempObj2.GetComponent<Text>().text=j.ToString();    //��ġ ó��
                    bigAdr.transform.GetChild(i+1).GetChild(1).gameObject.SetActive(false);

                    //�ν��Ͻ� ����
                    FrameData smallInstance=tempObj2.GetComponent<FrameData>();

                    //�� ���� ���� ����
                    int smallLimit=0;
                    switch(menu)
                    {
                        case 1:                 //����
                            boxNow=34;  //25+1��
                            smallLimit=26;
                            break;
                        case 2:                 //ȿ��
                            boxNow=34;  //68+1��
                            smallLimit=68;
                            break;
                        case 3:                 //����ü
                            boxNow=34;  //6+1��
                            smallLimit=6;
                            break;
                        case 4:                 //����
                            boxNow=34;  //4+1��
                            smallLimit=4;
                            break;
                        case 5:             //�̹���
                            boxNow=34;  //13+1��
                            smallLimit=13;
                            break;
                        default:
                            Debug.Log("�޴� �Է� ����, ������ �о���̱� �ߴ�");
                            break;
                    }            

                    //���� ����
                    for(int k=0;k<smallLimit;k++)
                    {
                        //�� ����
                        switch(menu)
                        {
                            case 1:                 //����
                                smallInstance.motValue[valueNow]=readTxtValue();
                                break;
                            case 2:                 //ȿ��
                                smallInstance.effectValue[valueNow]=readTxtValue();
                                break;
                            case 3:                 //����ü
                                smallInstance.bulletValue[valueNow]=readTxtValue();
                                break;
                            case 4:                 //����
                                smallInstance.variableValue[valueNow]=readTxtValue();
                                break;
                            case 5:                 //�̹���
                                smallInstance.imageValue[valueNow]=readTxtValue();
                                break;
                            default:
                                Debug.Log("�޴� �Է� ����!!!!!!!!!!!!!!!!!!!!");
                                break;
                        }
                        valueNow++;
                    }
                    /*
                    //��ư ����
                    switch(menu)
                    {
                        case 1:             //����
                            Debug.Log("��ư �Է� ����1");
                            tempObj2.GetComponent<Button>().onClick.AddListener(()=>smallInstance.SetBoxValue(textBox, smallInstance.motValue,1,0));
                            break;
                        case 2:             //ȿ��
                            Debug.Log("��ư �Է� ����2");
                            tempObj2.GetComponent<Button>().onClick.AddListener(()=>smallInstance.SetBoxValue(textBox, smallInstance.motValue,1,0));
                            break;
                        case 3:             //����ü
                            Debug.Log("��ư �Է� ����3");
                            tempObj2.GetComponent<Button>().onClick.AddListener(()=>smallInstance.SetBoxValue(textBox, smallInstance.motValue,1,0));
                            break;
                        case 4:             //����
                            Debug.Log("��ư �Է� ����4");
                            tempObj2.GetComponent<Button>().onClick.AddListener(()=>smallInstance.SetBoxValue(textBox, smallInstance.motValue,1,0));
                            break;
                        case 5:             //�̹���
                            Debug.Log("��ư �Է� ����5");
                            tempObj2.GetComponent<Button>().onClick.AddListener(()=>smallInstance.SetBoxValue(textBox, smallInstance.motValue,1,0));
                            break;
                        default:
                            Debug.Log("�޴� �Է� ����, ������ �о���̱� �ߴ�");
                            break;
                    }
                    */
                }
                txtNow+=2;              //* ó��
                //��ư ����
                Debug.Log(i);
                //tempObj1.GetComponent<Button>().onClick.AddListener(()=>bigInstance.ShowFrames(bigAdr.GetChild(i+1).GetChild(1), tempObj1));
                tempObj1.GetComponent<Button>().onClick.AddListener(()=>bigInstance.ShowFrames(tempObj1.transform.GetChild(1), tempObj1, smallAdr, frameCount, menu, textBox));
                tempObj1.GetComponent<Button>().onClick.AddListener(()=>bigInstance.SetBoxValue(textBox, bigInstance.sameValue,1,1));
            }
        }
    }

    private void Awake()
    {
        //��� ��������
        target=GameObject.Find("LoadingManagerInUsed");             //������Ʈ�� ã�Ƴ�
        textValue=target.GetComponent<DataManager>().textValue; //�ؽ�Ʈ ��������
        txtPath=target.GetComponent<DataManager>().textPath;
        Destroy(target);            //�� �� �Ŵ��������� ����
        ////////////////////////
        //�⺻���� ���� ����
        txtNow=1;
        int defaultData=29;
        for(int i=0;i<defaultData;i++)
        {
            //�� ���� �Է¹޾Ƽ� �ؽ�Ʈ �ڽ� �ȿ� �־
            string inputData=readTxtValue();
            if(inputData==null)
            {
                continue;
            }
            textBox[boxNow].GetComponent<UnityEngine.UI.InputField>().text=inputData;
            boxNow++;
        }

        //���� ���� ����
        txtNow+=6;              //**ó��
        int motCount=Convert.ToInt32(readTxtValue());         //���� ���� �о����
        txtNow+=2;             //* ó��

        //������ �ּ� ����
        Transform motTemp1=GameObject.Find("Canvas").transform.GetChild(1).GetChild(1).GetChild(17).GetChild(0).GetChild(0);                //���� ������ �ּ�
        GameObject motTemp2=GameObject.Find("FrameGotMotData");         //����� ���� ������Ʈ
        Transform motTemp3=GameObject.Find("Canvas").transform.GetChild(1).GetChild(1).GetChild(18).GetChild(0).GetChild(0);                //���� ������ �ּ�
        GameObject motTemp4=GameObject.Find("FrameGotFrame");         //����� ���� ������Ʈ
        //������ �о
        ReadFrames(motCount, motTemp1, motTemp3, motTemp2, motTemp4, 1);

        //ȿ�� ���� ����
        txtNow+=1;              //**ó��
        int effCount=Convert.ToInt32(readTxtValue());         //ȿ�� ���� �о����
        txtNow+=2;             //* ó��

        //������ �ּ� ����
        Transform effTemp1=GameObject.Find("Canvas").transform.GetChild(4).GetChild(1).GetChild(20).GetChild(0).GetChild(0);                //���� ������ �ּ�
        GameObject effTemp2=GameObject.Find("FrameGotEffData");         //����� ȿ�� ������Ʈ
        Transform effTemp3=GameObject.Find("Canvas").transform.GetChild(4).GetChild(1).GetChild(21).GetChild(0).GetChild(0);                //���� ������ �ּ�
        GameObject effTemp4=GameObject.Find("FrameGotFrame");         //����� ���� ������Ʈ

        ReadFrames(effCount, effTemp1, effTemp3, effTemp2, effTemp4, 2);


        //����ü ���� ����
        txtNow+=3;              //**ó��
        int bulCount=Convert.ToInt32(readTxtValue());         //ȿ�� ���� �о����
        txtNow+=2;             //* ó��

        //������ �ּ� ����
        Transform bulTemp1=GameObject.Find("Canvas").transform.GetChild(5).GetChild(1).GetChild(19).GetChild(0).GetChild(0);                //���� ������ �ּ�
        GameObject bulTemp2=GameObject.Find("FrameGotBulData");         //����� ȿ�� ������Ʈ
        Transform bulTemp3=GameObject.Find("Canvas").transform.GetChild(5).GetChild(1).GetChild(20).GetChild(0).GetChild(0);                //���� ������ �ּ�
        GameObject bulTemp4=GameObject.Find("FrameGotFrame");         //����� ���� ������Ʈ

        ReadFrames(bulCount, bulTemp1, bulTemp3, bulTemp2, bulTemp4, 3);


        //���� ���� ����
        txtNow+=3;              //**ó��
        int varCount=Convert.ToInt32(readTxtValue());         //ȿ�� ���� �о����
        txtNow+=2;             //* ó��

        //������ �ּ� ����
        Transform varTemp1=GameObject.Find("Canvas").transform.GetChild(6).GetChild(1).GetChild(10).GetChild(0).GetChild(0);                //���� ������ �ּ�
        GameObject varTemp2=GameObject.Find("FrameGotVarData");         //����� ȿ�� ������Ʈ
        Transform varTemp3=GameObject.Find("Canvas").transform.GetChild(6).GetChild(1).GetChild(11).GetChild(0).GetChild(0);                //���� ������ �ּ�
        GameObject varTemp4=GameObject.Find("FrameGotFrame");         //����� ���� ������Ʈ

        ReadFrames(varCount, varTemp1, varTemp3, varTemp2, varTemp4, 4);


        //�̹��� ���� ����
        ////�����ؾ� �Ұ�
        /*
        image �������� �̹��� ����Ʈ�� �ҷ��� V
        �ҷ��� �̹����� ��ȣ�� �Ű��� V
        �ҷ��� ������� �̹��� ����Ʈȭ �� ��� V
        ����Ʈ������ �ҷ��� �̹��� �̸����� ���
        �̹��� ��ȣ Ŭ���� ������ �̹��� ������ ��
        */
        //��θ� �޾ƿ�
        patho=Path.GetFullPath(Path.Combine(txtPath,@"../"));
        //�̹��� ���� ���, ���� ���� ��� ����
        pathForImg=patho+"image";
        pathForSnd=patho+"sound";

        //������ �о����
        ReadImage();

        allFilesName=null;
        fileData=null;
        //���� ���� ����
        ////�����ؾ� �Ұ�
        /*
        sound �������� ���� ����Ʈ�� �ҷ��� V
        �ҷ���  ���忡 ��ȣ�� �Ű���
        �ҷ��� ������� ���� ����Ʈȭ �� ���
        ���� ��ȣ Ŭ���� ���� ���
        �ٸ� ���� Ŭ���ϸ� ��� �ߴ�
        */
        ReadSound();

        //���⼭ �̹��� �ε�?
        
    }

    public void ReadImage()
    {
        //������ �ҷ����� �� ��� png ���ϸ��� ������
        allFilesName=Directory.GetFiles(pathForImg, "*.png", SearchOption.AllDirectories);
        //���ϵ��� �ϳ��ϳ� �ҷ����� ������
        for(int i=0;i<allFilesName.Length;i++)
        {
            //������ ���������� �ҷ���
            rscReader=new BinaryReader(new FileStream(allFilesName[i], FileMode.Open));
            fileData=rscReader.ReadBytes(int.MaxValue);

            //�ҷ��� ���� �����͸� �̹����� ó��
            tex=new Texture2D(2,2);
            tex.LoadImage(fileData);


            //�̹��� ó���� ���� ������Ʈ�� �߰�
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
        //������ �ҷ����� �� ��� ogg ���ϸ��� ������
        allFilesName=Directory.GetFiles(pathForSnd, "*.ogg", SearchOption.AllDirectories);

        //���ϵ��� �ϳ��ϳ� �ҷ����� ������
        for(int i=0;i<allFilesName.Length;i++)
        {
            //������ ���������� �ҷ���
            rscReader=new BinaryReader(new FileStream(allFilesName[i], FileMode.Open));
            fileData=rscReader.ReadBytes(int.MaxValue);

            //�̹��� ó���� ���� ������Ʈ�� �߰�
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
�⺻����->����->ȿ��->����ü->����->�̹���->���� ������ ���� �о�� �Ҵ� ������ �� �Է�.

- ��ŸƮ�� �����̹Ƿ�. ���������� ���� 1�� ��ġ���� ��ŸƮ

- *=�� �������� ���� ����, �������� ������ ����, **=�������� ���� ����, |=�������� ���� ����, ���밪 ����

- ��ü���� ���� ��Ģ
** �������Ӽ� * �������Ӽ� | ���밪 | ������ * �������Ӽ� | ���밪 | ������ * �������Ӽ� | ���밪 | ������ ** �������Ӽ� * �������Ӽ� | ���밪 | ������ * �������Ӽ� | ���밪 | ������ * �������Ӽ� | ���밪 | ������ ** �������Ӽ� * �������Ӽ� | ���밪 | ������ * �������Ӽ� | ���밪 | ������ * �������Ӽ� | ���밪 | ������ **

- ��ü���� ���� ��Ģ (�ٳ�������)
- ��ü���� ���� ��Ģ
** �������Ӽ� * �������Ӽ� | ���밪 | ������ *
�������Ӽ� | ���밪 | ������ *
�������Ӽ� | ���밪 | ������ **
�������Ӽ� * �������Ӽ� | ���밪 | ������ *
�������Ӽ� | ���밪 | ������ *
�������Ӽ� | ���밪 | ������ **
�������Ӽ� * �������Ӽ� | ���밪 | ������ *
�������Ӽ� | ���밪 | ������ *
�������Ӽ� | ���밪 | ������ **

������ �д� ���
- *, | ������ ����
- ��ŸƮ : 1������ ���� �б� ������

- ���� ** ���� ���� : �� �������Ӱ����� �о ��ݺ� ����
------------------------��ݺ� (�� �������Ӱ�����ŭ)
- �ݺ� ���� ������ : * ���� �� �������Ӽ� �о �ҹݺ� ����
-------------------------------�ҹݺ� (�������Ӱ�����ŭ)
- �ҹݺ� ���� ������ : | �ѱ�� ������ ������ ���밪 �о����
- ���밪 ���� : | �ѱ�� ������ ������ �о����
- ������ ���� : �ݺ� �������� * ���� ��, *������ *�̸� ��ݺ� ���� (��ݺ� ���ǰ��� ������������ �ʱ�ȭ), �ƴϸ� �ҹݺ� ����� (������ ������ �� ��)
-----------------------------�ҹݺ� ����
------------------------��ݺ� ����
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
    //�޾ƿ� path ����
    public string pathFromMain;
    public string txtPath;

    //�� ������ �����ϰ� �ҷ��� ������Ʈ
    public InputField[] textBox;

    public GameObject target;       //�����͸Ŵ����� �޾ƿ� ������Ʈ
    public string textValue;            //ĳ���� ����

    //����Ʈ ������
    public GameObject makeNewList;

    //��� ����
    private void SetPath()
    {
        GameObject PathFinder;

        PathFinder=GameObject.Find("LoadingManagerIn");             //�ε����� �Ѱܹ��� ��� Ž��
        pathFromMain=PathFinder.GetComponent<ActiveLoading>().path; //����� ��� �ʱ�ȭ
        Destroy(PathFinder);            //���� ��� ����
    }


    //chr ���� ����
    public void SaveAs_chr()
    {
        //�� �ؽ�Ʈ�� ���� �����Ǿ� �ִ� ���� �ҷ���
        //�� ������ �ϳ��� string value�� �߰�����
            //string value�� �̸��� textValue�� ����
        
        //���� �ۼ�
        //System.IO.File.WriteAllText(txtPath, textValue, Encoding.Default);
        
        //���� Ȯ���� ����

    }

    //�� ������ ���� ������ �� ���� �� �ؽ�Ʈ ������Ʈ�� �Ҵ�

        public int txtNow=2;     //�ؽ�Ʈ�� ���� �б� ����
        public int boxNow=0;    //�ؽ�Ʈ �ڽ��� ���� �Ҵ��ϱ� ����
        public string dummybox="";      //�ּ� ���� �� �ӽ� ������ ���� ����
        public string resultbox="";
        public int dumNow=0;            //���̹ڽ���


    public string readTxtValue()
    {
        string giveResult="";
        while(true)
        {
            if(textValue[txtNow]!=0)       //������ �ƴϸ�
            {//�� ����
                dummybox+=textValue[txtNow].ToString();
                //Debug.Log("dummybox="+dummybox+"+"+textValue[txtNow].ToString());
                txtNow++;
                dumNow++;
            }
            else        //�����̸�
            {//�� �Ҵ�
                //Debug.Log("End, "+dumNow+"+"+txtNow+"+"+textValue[txtNow]);
                for(int l=0;l<dumNow;l++)        //���̹ڽ��� �� �Ҵ�
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

    //chr ���� �ҷ�����
    public void Awake()
    {
        target=GameObject.Find("LoadingManagerInUsed");             //������Ʈ�� ã�Ƴ�
        textValue=target.GetComponent<DataManager>().textValue; //�ؽ�Ʈ ��������
        Destroy(target);            //�� �� �Ŵ��������� ����
        ////////////////////////////////////
        //�⺻���� �ҷ�����
        boxNow--;
        while(boxNow<30)     //������ ���� �����ϱ� ������
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
        //���� �ҷ�����
        while(textValue[txtNow]!=0)      //������ ���ʷ� �����ϱ� ������
        {
            dummybox+=textValue[txtNow].ToString();
            txtNow++;
            dumNow++;
        }
        //���� �� �о�����
        for(int l=0;l<dumNow;l++)        //���̹ڽ��� �� �Ҵ�
        {
            resultbox+=dummybox[l];
        }
        dumNow=0;
        //����Ʈ ����
        Transform motTemp1=GameObject.Find("Canvas").transform.GetChild(1).GetChild(1).GetChild(17).GetChild(0).GetChild(0);                //���� ������ �ּ�
        GameObject motTemp2=GameObject.Find("FrameGotMotData");         //����� ���� ������Ʈ
        Transform motTemp3=GameObject.Find("Canvas").transform.GetChild(1).GetChild(1).GetChild(18).GetChild(0).GetChild(0);                //���� ������ �ּ�
        Debug.Log("Some Check,"+resultbox+","+Int32.Parse(resultbox));      //���� ���� Ȯ��

        Debug.Log("Begin Point="+txtNow);
        //���� ������ ����
        CallFrame(motTemp1, motTemp2, motTemp3, Int32.Parse(resultbox),1);
        
        //�ʱ�ȭ
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
        while(temp1<countOfMot)     //���ۼ��� �о��
        {
            //������ ����
            GameObject tempObj1=Instantiate(ingObject, addressOfBody.localPosition, Quaternion.identity);    //Ŭ�� ����
            tempObj1.transform.SetParent(addressOfBody);        //�θ� ����
            tempObj1.transform.localPosition=new Vector3(300,81-(30*temp1),0);          //������Ʈ ��ġ ����
            tempObj1.transform.localScale=new Vector3(1f,1f,1f);        //������Ʈ ũ�� ����
            tempObj1.GetComponent<Text>().text=temp1.ToString();    //��ġ ó��
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
            //������ �� �б�
            string[] valueList=new string[valueLimitation];            //������ ���� �����ص� �迭
            string targetValue=readTxtValue();      //������ ���� �о��
            //resultbox="";           //�ӽ�������� �ʱ�ȭ
            //Debug.Log("targetValue="+targetValue+"////"+txtNow);
            int countOfFrame=Convert.ToInt32(targetValue);      //������ ���� ���������ν� �޾Ƴ�

            Debug.Log("Set for reading");
            int temp2=0;            //������ �� �б� ��������
            int temp3=0;            //�о���� �� �����
            switch(menu)
            {
                case 1:                 //������ �д� ���
                    //��Ÿ��, �ʿ信���� �б�
                    for(int i=0;i<3;i++)
                    {
                        Debug.Log("Inner Range,"+i);
                        valueList[i]=readTxtValue();                //���� ��Ƴ��� ��
                        //resultbox="";
                    }
                    //���� �ּ� �б�
                    if(readT)     //�ּ��� ������ �� �ּ� ó��
                    {
                        tempObj1.GetComponent<Text>().text+=;
                    }
                    //resultbox="";
                    
                    temp2=2;
                    temp3=27;           //25+2
                    break;

                case 2:                 //ȿ���� �д� ���
                    //������ ��, �̹��� ����, �Ӽ� �б�
                    for(int i=0;i<3;i++)
                    {
                        valueList[i]=readTxtValue();
                        //resultbox="";
                    }
                    //ȿ�� �ּ� �б�
                    string com2=readTxtValue();
                    if(com2!="")     //�ּ��� ������ �� �ּ� ó��
                    {
                        tempObj1.GetComponent<Text>().text+=com2;
                    }
                    //resultbox="";

                    temp2=3;
                    temp3=71;            //68+3
                    break;

                case 3:                 //����ü�� �д� ���
                    //������ ��, �̹��� ����, �Ӽ�, ���ط�, ����˹�, �����˹�, �����ð�, �ٿ�ð�, Ÿ��ȿ��, �ǰݼҸ�, �ǰݽÿ�������ȭ, ����ȿ��, ����ȿ��, ����ȿ��, ���ȿ��, ����ȿ��, �ı�ȿ�� �б�
                    for(int i=0;i<17;i++)
                    {
                        valueList[i]=readTxtValue();
                        //resultbox="";
                    }
                    //����ü �ּ� �б�
                    string com3=readTxtValue();
                    if(com3!="")     //�ּ��� ������ �� �ּ� ó��
                    {
                        tempObj1.GetComponent<Text>().text+=com3;
                    }
                    //resultbox="";

                    temp2=17;
                    temp3=23;           //6+17
                    break;

                case 4:                 //������ �д� ���
                    //�⺻�� �б�
                    valueList[0]=readTxtValue();
                    //resultbox="";
                    //������ �б�
                    tempObj1.GetComponent<Text>().text=readTxtValue();
                    //resultbox="";

                    //���� �ּ� �б�
                    string com4=readTxtValue();
                    if(com4!="")     //�ּ��� ������ �� �ּ� ó��
                    {
                        tempObj1.GetComponent<Text>().text+=com4;
                    }
                    //resultbox="";

                    temp2=1;
                    temp3=7;            //6+1
                    break;

                case 5:                 //�̹����� �д� ���
                    break;

                default:                //����
                    Debug.Log("�޴� �Է� ���� �߻�.");
                    break;
            }

            Debug.Log("After Reading each values");
            //�� �������� �о
            //�⺻ ���� �б�
            //�� ������ ���� �б�

            %%%%%%%%%%%%%%%%%%%%%%
            ���� ���� : �� �б⸦ ������ �������� ������ ���� (ù ���ۿ��� 
            ���� �������� �а� �Է����� ������ ���ܸ� ������ ����)
            �ذ� ��� : ���� �д� ������ �����ؼ�, �ʿ��� ������ŭ�� ���� �аԲ� ����
            %%%%%%%%%%%%%%%%%%%%%%

            //Debug.Log(textValue[108]+"///"+textValue[109]);           //����� 6(109)/// (110)
            for(int i=0;i<countOfFrame;i++)         //������ �� ��ŭ ���� �о����
            {
                //for(int j=temp2;j<temp3;j++)
                while(temp2<temp3)
                {   //�� �������� ���� �迭�� �߰��س�.
                    while(true)
                    {
                        if(String.IsNullOrWhiteSpace(textValue[txtNow].ToString())==false)       //������ �ƴϸ�
                        {//�� ����
                            dummybox+=textValue[txtNow];
                            txtNow++;
                            dumNow++;
                        }
                        else        //�����̸�
                        {               //�� �Ҵ�
                            try             //���� �˻�
                            {
                                if(textValue[txtNow+1]==0&&textValue[txtNow-1]==0)      //���� ĭ �յڷ� �����̸� == �Էµ� �ּ��� ���ٸ�
                                {
                                txtNow++;
                                continue;       //����ĭ���� �ѱ�
                                }
                            }
                            catch(Exception e)
                            {
                                Debug.Log("���� �ʰ� ���� �߻� : "+txtNow+","+temp2+"//////"+e);
                            }   
                            finally
                            {
                                txtNow++;
                            }
                            for(int l=0;l<dumNow;l++)        //���̹ڽ��� �� �Ҵ�
                            {
                                resultbox+=dummybox[l];
                            }
                            //�� ����
                            valueList[temp2]=resultbox;
                            //�ʱ�ȭ
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
            //���� 25+1
            //ȿ�� 68+1
            //����ü 6+1
            //���� 6+1
            Debug.Log("Finally go to here");
            /*
            //���������� ����
            for(int i=0;i<Convert.ToInt32(valueList[0]);i++)
            {
                GameObject tempObj2=Instantiate(ingObject, tempObj1.transform.localPosition, Quaternion.identity);    //Ŭ�� ����
                tempObj2.transform.SetParent(tempObj1.transform);        //�θ� ����
                tempObj2.transform.localPosition=new Vector3(300,81-(30*i),0);          //������Ʈ ��ġ ����
                tempObj2.transform.localScale=new Vector3(1f,1f,1f);        //������Ʈ ũ�� ����
                tempObj2.GetComponent<Text>().text=i.ToString();    //��ġ ó��
            }
            //������ ��ư ���� (�ش� ��ư�� ������ �� �Ҵ� �޼ҵ忡 �̸� �о �ؽ�Ʈ ����� �Ѱ���)
        }
    }

    public void SetFrameData()
    {
        //�ؽ�Ʈ ��� �� �м�
        //�м� �� ���� ������ �ڽ��� ����
    }
}
*/