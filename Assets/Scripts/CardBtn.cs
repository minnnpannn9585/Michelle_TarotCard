using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBtn : MonoBehaviour
{
    public GameObject cardGO;
    public void ClickCard()
    {
        cardGO.SetActive(true);
        transform.parent.gameObject.SetActive(false);
    }


}
