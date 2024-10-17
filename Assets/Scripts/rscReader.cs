using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class rscReader : MonoBehaviour
{
    public chrReader forPath;
    //경로 변수
    public string path;
    string pathForImg;
    string pathForSnd;
    //리소스 오브젝트 재료
    public GameObject imageFrame;
    public GameObject destination;

    //불러온 파일
    public Texture2D tex=null;
    public byte[] fileData;
    

    void Awake()
    {
        //경로를 받아옴
        path=forPath.GetComponent<chrReader>().txtPath;
        path=Path.GetFullPath(Path.Combine(path,@"../"));

        //이미지 폴더 경로, 사운드 폴더 경로 설정
        pathForImg=path+"image";
        pathForSnd=path+"sound";
        
        Debug.Log(pathForImg);

        //폴더 권한 설정
        //.....해야 함....
        //읽기 전용 제발 멈춰!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        File.SetAttributes(pathForImg, FileAttributes.Normal);

        ReadImage();
    }
    
    /////////////////////파일 읽기부터
    public void ReadImage()
    {
        //모든 이미지 파일을 읽어들임
        Debug.Log("이미지 불러오기 전");
        fileData=File.ReadAllBytes(pathForImg);     //문제 발생 구간
        tex=new Texture2D(2,2);
        tex.LoadImage(fileData);
        //sprites=Resources.LoadAll<Sprite>(pathForImg);
        //files= new System.IO.DirectoryInfo().GetFiles(pathForImg+"*.png").OrderBy(file => file.Name).ToArray();
        Debug.Log("이미지 불러옴!");
        Debug.Log(tex);
        
    }
}

