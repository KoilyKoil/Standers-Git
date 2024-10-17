using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameData : MonoBehaviour
{
                                                                        //��������+�⺻����+�ּ�
    public string[] sameValue=new string[18];
    public string[] motValue=new string[26];       //26+2+1
    public string[] effectValue=new string[68];    //68+3+1
    public string[] bulletValue=new string[6];     //6+17+1
    public string[] variableValue=new string[4];       //4+2+1
    public string[] imageValue=new string[13];

    //�� �ۼ�
    public void SetBoxValue(InputField[] textBox, string[] inputValue, int menu, int sameMenu)
    {
        int boxNow;
        //�������� Ŭ����
        if(sameMenu==1)
        {
            //�� ���
            switch(menu)
            {
                case 1:             //����
                    boxNow=31;
                    for(int i=0;i<2;i++)
                    {
                        textBox[boxNow].GetComponent<UnityEngine.UI.InputField>().text=inputValue[i];
                        boxNow++;
                    }
                    break;
                case 2:             //ȿ��
                    boxNow=61;
                    for(int i=0;i<3;i++)
                    {
                        textBox[boxNow].GetComponent<UnityEngine.UI.InputField>().text=inputValue[i];
                        boxNow++;
                    }
                    break;
                case 3:             //����ü
                    boxNow=133;
                    for(int i=0;i<17;i++)
                    {
                        textBox[boxNow].GetComponent<UnityEngine.UI.InputField>().text=inputValue[i];
                        boxNow++;
                    }
                    break;
                case 4:                 //����
                    boxNow=158;
                    for(int i=0;i<2;i++)
                    {
                        textBox[boxNow].GetComponent<UnityEngine.UI.InputField>().text=inputValue[i];
                        boxNow++;
                    }
                    break;
                case 5:                 //�̹���
                    boxNow=167;
                    for(int i=0;i<13;i++)
                    {
                        textBox[boxNow].GetComponent<UnityEngine.UI.InputField>().text=inputValue[i];
                        boxNow++;
                    }
                    break;
                default:
                    Debug.Log("�޴� �Է� ����!!!!!!!!!!!!!!!!!!!!!!");
                    break;
            }
        }
        //�������� Ŭ����
        else
        {
            //�� ���
            switch(menu)
            {
                case 1:                //����
                    boxNow=33;
                    for(int i=0;i<26;i++)
                    {
                        textBox[boxNow].GetComponent<UnityEngine.UI.InputField>().text=inputValue[i];
                        boxNow++;
                    }
                    break;
                case 2:                 //ȿ��
                    boxNow=64;
                    for(int i=0;i<68;i++)
                    {
                        textBox[boxNow+i].GetComponent<UnityEngine.UI.InputField>().text=inputValue[i];
                    }
                    break;
                case 3:             //����ü
                    boxNow=150;
                    for(int i=0;i<6;i++)
                    {
                        textBox[boxNow+i].GetComponent<UnityEngine.UI.InputField>().text=inputValue[i];
                    }
                    break;
                case 4:                 //����
                    boxNow=160;
                    for(int i=0;i<4;i++)
                    {
                        textBox[boxNow+i].GetComponent<UnityEngine.UI.InputField>().text=inputValue[i];
                    }   
                    break;
                case 5:                 //�̹���
                    boxNow=167;
                    for(int i=0;i<13;i++)
                    {
                        textBox[boxNow+i].GetComponent<UnityEngine.UI.InputField>().text=inputValue[i];
                    }
                    break;
                default:
                    Debug.Log("�޴� �Է� ����!!!!!!!!!!!!!!!!!!!!!!");
                    break;
            }
        }
    }

    public void ShowFrames(Transform address, GameObject ingrediants, Transform destination, int frameCount, int menu, InputField[] textBox)
    {                                       //�������� ���� �ּ� (FrameList?)                 //FrameList,                //MotiveFrameList�ּ�
        Debug.Log("text1");
        //���� ������ ����
        Destroy(destination.GetChild(0).gameObject);

        //������ ����
        GameObject targetObject=Instantiate(ingrediants.transform.GetChild(1).gameObject, address.localPosition, Quaternion.identity);
        targetObject.transform.SetParent(destination);
        targetObject.SetActive(true);
        targetObject.transform.localScale=new Vector3(1f,1f,1f);
        
        //��ư �Է� ������
        for(int i=1;i<=frameCount;i++)
        {
            GameObject longName=targetObject.transform.GetChild(1).gameObject;
            //��ư ����
            switch(menu)
            {
                case 1:             //����
                    Debug.Log("��ư �Է� ����1");
                    longName.GetComponent<Button>().onClick.AddListener(()=>SetBoxValue(textBox, longName.GetComponent<FrameData>().motValue,1,0));
                    break;
                case 2:             //ȿ��
                    Debug.Log("��ư �Է� ����2");
                    longName.GetComponent<Button>().onClick.AddListener(()=>SetBoxValue(textBox, longName.GetComponent<FrameData>().effectValue,2,0));
                    break;
                case 3:             //����ü
                    Debug.Log("��ư �Է� ����3");
                    longName.GetComponent<Button>().onClick.AddListener(()=>SetBoxValue(textBox, longName.GetComponent<FrameData>().bulletValue,3,0));
                    break;
                case 4:             //����
                    Debug.Log("��ư �Է� ����4");
                    longName.GetComponent<Button>().onClick.AddListener(()=>SetBoxValue(textBox, longName.GetComponent<FrameData>().variableValue,4,0));
                    break;
                case 5:                 //�̹���
                    Debug.Log("��ư �Է� ����5");
                    longName.GetComponent<Button>().onClick.AddListener(()=>SetBoxValue(textBox, longName.GetComponent<FrameData>().imageValue,5,0));
                    break;
                default:
                    Debug.Log("�޴� �Է� ����, ������ �о���̱� �ߴ�");
                    break;
            }
        }
    }
}