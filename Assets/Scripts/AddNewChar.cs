using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Windows.Forms;        //���� ����â ������ ����
using Ookii.Dialogs;                        //���� ����â ����� ����

public class AddNewChar : MonoBehaviour
{
    public InputField inputField;               //�Է�â
    public GameObject myObject;             //�ε��� ��ũ��Ʈ�� ���� ������Ʈ    

    private OpenFileDialog dialog=new OpenFileDialog();       //���̾�α׿� �ν��Ͻ�

    public ActiveLoading giveValue;                 //���� ��� �Ѱ��ֱ��
    [SerializeField]
    private string path;          //���� ���

    //���� ��� �Ѱ��ֱ�
    public void GiveFileRead()
    {
        dialog.Title="ĳ���� ���� ����";                        //���� ����â ����
        dialog.Filter="chr(*.chr)|*.chr|��� ����(*.*)|*.*";    //���� ���� ���� ����
        //dialog.ShowDialog();
        //����â ����
        var filenames=dialog.ShowDialog()==DialogResult.OK ? dialog.FileNames:new string[0];        //���� ������ �Ǹ� �ش� ���ϸ�����, �ƴϸ� �����Ⱚ ����
        string[] path2=filenames;              //������ ���ϸ����� ��� ����
        path=path2[0];
        //myObject.SetActive(false);        //IndexOutOfRange �� ���� ����ó�� �ص� ��
        Debug.Log(path);

        giveValue.path=path;                //��θ� �����ͷ� �Ѱ���
    }

    //�� ĳ���� ����
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

        //������ ����
        if(!sameDetect)
        {
            newFile="Assets/Target/"+name+"/";
        }
        else
        {
            newFile="Assets/Target/"+name+sameNumbStr+"/";
        }
        
        //ĳ���� ���� ����
        Directory.CreateDirectory(newFile);

        //���� ���� �⺻ ��� ����
        Directory.CreateDirectory(newFile+"ai/");
        Directory.CreateDirectory(newFile+"image/");
        Directory.CreateDirectory(newFile+"sound/");
        //chr���� ����
        File.Create(newFile+name+".chr");

        //�� ������ ��η� ����
        path=newFile+name+".chr";
        Debug.Log(path);
        giveValue.path=path;                //��θ� �����ͷ� �Ѱ���
    }

    //���� ���� �ڵ� ����
    public void DoProgress()
    {
        string charName="";
        charName=inputField.text;
        MakeChar(charName);
    }

    //�ε��� �۵�
    public void Operation()
    {
        myObject.SetActive(true);
    }
}
