using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Team Logo Information", menuName = "Scriptable Objects/Team Logo Information", order = 1)]
public class TeamInfoSO : ScriptableObject
{
    [Header("Group Information (Paired by index)")]
    [SerializeField] private List<Sprite> teamLogos;
    [SerializeField] private List<string> teamNames;

    [Header("Misc Data")]
    [SerializeField] private List<Color> validTeamColours;

    public List<Sprite> getLogos() => teamLogos;
    public List<string> getNames() => teamNames;

    public (string,Sprite) getNameAndLogo(int _id) => (teamNames[_id],teamLogos[_id]);

    public Color getUniqueRandomColourPerTeam(int _team_id, int _seed) {
        System.Random rnd = new System.Random(_seed);
        List<Color> colour_list = validTeamColours;
        int quant = colour_list.Count;  
        while (quant > 1) {  
            quant--;  
            int ind = rnd.Next(quant + 1);  
            Color value = colour_list[ind];  
            colour_list[ind] = colour_list[quant];  
            colour_list[quant] = value;  
        }
        return colour_list[_team_id];
    } 
}
