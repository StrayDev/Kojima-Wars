/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Passive : MonoBehaviour
{
    public Image backgroundImage;

    public int skillLevel = 1;
    public bool isActive;
    public bool isChosen;
    public string description;

    private void Awake()
    {
        //skillTree.passiveList.Add(this.gameObject);
    }

    public void Deactivate()
    {
        isChosen = false;
        if (skillLevel > 1)
        {
            isActive = false;
        }
        skillTree.UpdateUI();
    }

    public void SelectPassive()
    {
        if (isActive && !isChosen)
        {
            isChosen = true;
            skillTree.AddToPlayerPassive(backgroundImage.sprite, description);
            foreach (GameObject passive in skillTree.passiveList)
            {
                if (passive != this.gameObject)
                {
                    passive.GetComponent<Passive>().isChosen = false;
                }
            }
        }
        else if (isActive && isChosen)
        {
            isChosen = false;
            skillTree.RemoveFromPlayerPassive();
        }
        else
        {
            return;
        }
        skillTree.UpdateUI();
    }
}
*/