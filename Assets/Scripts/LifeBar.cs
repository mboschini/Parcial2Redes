using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{
    [SerializeField] Image img;
    
    public void UpdateBar(float amount)
    {
        img.fillAmount = amount;
    }
}
