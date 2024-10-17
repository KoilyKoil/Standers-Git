using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class rscReader : MonoBehaviour
{
    public chrReader forPath;
    //��� ����
    public string path;
    string pathForImg;
    string pathForSnd;
    //���ҽ� ������Ʈ ���
    public GameObject imageFrame;
    public GameObject destination;

    //�ҷ��� ����
    public Texture2D tex=null;
    public byte[] fileData;
    

    void Awake()
    {
        //��θ� �޾ƿ�
        path=forPath.GetComponent<chrReader>().txtPath;
        path=Path.GetFullPath(Path.Combine(path,@"../"));

        //�̹��� ���� ���, ���� ���� ��� ����
        pathForImg=path+"image";
        pathForSnd=path+"sound";
        
        Debug.Log(pathForImg);

        //���� ���� ����
        //.....�ؾ� ��....
        //�б� ���� ���� ����!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        File.SetAttributes(pathForImg, FileAttributes.Normal);

        ReadImage();
    }
    
    /////////////////////���� �б����
    public void ReadImage()
    {
        //��� �̹��� ������ �о����
        Debug.Log("�̹��� �ҷ����� ��");
        fileData=File.ReadAllBytes(pathForImg);     //���� �߻� ����
        tex=new Texture2D(2,2);
        tex.LoadImage(fileData);
        //sprites=Resources.LoadAll<Sprite>(pathForImg);
        //files= new System.IO.DirectoryInfo().GetFiles(pathForImg+"*.png").OrderBy(file => file.Name).ToArray();
        Debug.Log("�̹��� �ҷ���!");
        Debug.Log(tex);
        
    }
}

