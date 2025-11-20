using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpMessage : MonoBehaviour
{
    public List<GameObject> msgs;
    private void Start()
    {
        GameEvents.ShowMessage += ShowMessage;
    }
    private void OnDisable()
    {
        GameEvents.ShowMessage -= ShowMessage;
    }

    public void ShowMessage()
    {
        var idx = UnityEngine.Random.Range(0, msgs.Count);
        msgs[idx].SetActive(true);
    }
}
