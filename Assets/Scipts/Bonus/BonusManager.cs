using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusManager : MonoBehaviour
{
    public List<GameObject> bonusList;

    void Start()
    {
        GameEvents.ShowBonusScreen += ShowBonusScreen;
    }

    private void OnDisable()
    {
        GameEvents.ShowBonusScreen -= ShowBonusScreen;
    }

    private void ShowBonusScreen(Config.SqColor color)
    {
        GameObject obj = null;
        foreach (var bonus in bonusList)
        {
            var bonusCmp = bonus.GetComponent<Bonus>();
            if (bonusCmp.color == color)
            {
                obj = bonus;
                bonus.SetActive(true);
            }
        }
        StartCoroutine(DeactivateBonus(obj));
    }

    private IEnumerator DeactivateBonus(GameObject obj)
    {
        yield return new WaitForSeconds(2);
        obj.SetActive(false);
    }
}
