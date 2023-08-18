using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RandomNameGenerator : MonoBehaviour
{
    [SerializeField] string[] name1, name2;
    [SerializeField] TMP_InputField userName;
    

    // Start is called before the first frame update
    void Start()
    {
        GenerateUserName();
    }


    public void GenerateUserName(){
        string name = name1[Random.Range(0, name1.Length)] + name2[Random.Range(0, name2.Length)] + Random.Range(0, 999).ToString();
        userName.text = name;
    }
}
