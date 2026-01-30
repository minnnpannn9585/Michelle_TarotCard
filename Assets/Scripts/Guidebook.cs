using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guidebook : MonoBehaviour
{
    public GameObject bookContent;
    bool isOpen = false;
    public void OpenCloseBook()
    {
        isOpen = !isOpen;
        bookContent.SetActive(isOpen);
    }
}
