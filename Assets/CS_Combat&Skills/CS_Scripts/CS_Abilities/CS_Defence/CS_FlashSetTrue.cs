using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_FlashSetTrue : MonoBehaviour
{
    private CS_PlayerStats PlayerStats;
    public float maxRange;
    public GameObject player;
    private InputActionManager playerInput;

    // Start is called before the first frame update
    void Start()
    {
        PlayerStats = player.gameObject.GetComponent<CS_PlayerStats>();

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject Player in players)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, (Player.transform.position - transform.position), out hit, maxRange))
            {
                if (hit.transform == Player.transform)
                {
                    Player.GetComponent<CS_PlayerStats>().flashed = true;
                }
            }
        }
    }
}
