using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameStateData", menuName = "Scriptable Objects/GameStateData")]
public class GameStateDataSO : ScriptableObject
{
    [Tooltip("The maximum length of the game in minutes.")]
    public float maxGameLength = 10;

    [Tooltip("The score required for a team to win the game.")]
    public int scoreThreshold = 1000;

    [Tooltip("Whether players can damage their teammates")]
    public bool friendlyFire = false;
}
