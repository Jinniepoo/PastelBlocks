using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSquare : MonoBehaviour
{
    public Image normalImage;
    public List<Sprite> normalImages;
    void Start()
    {
        
    }

    public void SetImage(bool firstImage)
    {
        normalImage.GetComponent<Image>().sprite = firstImage ? normalImages[1] : normalImages[0];
    }
}
