using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpMessage : MonoBehaviour
{
    public List<GameObject> msgs;
    void Start()
    {
        GameEvents.ShowMessage += ShowMessage;
    }
    private void OnDisable()
    {
        GameEvents.ShowMessage -= ShowMessage;
    }

    private void ShowMessage()
    {
        var idx = UnityEngine.Random.Range(0, msgs.Count);
        msgs[idx].SetActive(true);
    }
}
