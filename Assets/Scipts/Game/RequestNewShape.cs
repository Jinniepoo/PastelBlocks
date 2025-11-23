using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RequestNewShape : MonoBehaviour
{
    public int RequestNum = 3;
    public TextMeshProUGUI txt;
    public Image CntBG;

    private int curNumOfRequests;
    private Button button;
    private bool isLocked;

    void Start()
    {
        curNumOfRequests = RequestNum;
        txt.text = curNumOfRequests.ToString();
        txt.faceColor = new Color(1f, 1f, 0.9f, 1f);
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonDown);
        Unlock();
    }

    private void OnButtonDown()
    {
        if (isLocked == false)
        {
            curNumOfRequests--;
            GameEvents.RequestNewShape();
            GameEvents.CheckIfPlayerLost();

            if (curNumOfRequests <= 0)
            {
                Lock();
            }
        }
        txt.text = curNumOfRequests.ToString();

        if (curNumOfRequests == 0)
        {
            CntBG.gameObject.SetActive(false);
            txt.gameObject.SetActive(false);
        }
    }

    private void Lock()
    {
        isLocked = true;
        button.interactable = false;
        txt.text = curNumOfRequests.ToString();
    }

    private void Unlock()
    {
        isLocked = false;
        button.interactable = true;
    }
}
