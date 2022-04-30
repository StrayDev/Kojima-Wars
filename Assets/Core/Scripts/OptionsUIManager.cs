using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsUIManager : MonoBehaviour
{
    [SerializeField] GameObject[] optionsContainers;

    GameObject activeContainer;

    private void Start()
    {
        activeContainer = optionsContainers[0];

        foreach (GameObject container in optionsContainers)
        {
            if(container != activeContainer)
            {
                container.SetActive(false);
            }
        }
    }

    public void OnOptionsButtonPressed(GameObject containerToActivate)
    {
        activeContainer.SetActive(false);
        containerToActivate.SetActive(true);
        activeContainer = containerToActivate;
    }

}
