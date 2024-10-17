using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_Operator : MonoBehaviour
{
    public GameObject[] target;
    bool saveme=false;

    public void MenuOperator(int menu)
    {
        //지목한 본인 메뉴가 꺼져있을 때만 메뉴 켜줌
        if(target[menu].activeSelf==false)
        {
            target[menu].SetActive(true);
            saveme=true;
        }   

        //모든 메뉴 닫음
        for(int i=0;i<target.Length;i++)
        {
            if(target[i].activeSelf==true)
            {
                target[i].SetActive(false);   
                //현재 메뉴 유지
                if(i==menu && saveme==true)
                {
                    saveme=false;
                    target[i].SetActive(true);   
                }
            }
        }
    }

    public void BeforeOperation(GameObject[] arrays)
    {
        for(int i=0;i<arrays.Length;i++)
        {
            if(arrays[i].activeSelf==true)
            {
                arrays[i].SetActive(false);
            }
        }
    }
}
