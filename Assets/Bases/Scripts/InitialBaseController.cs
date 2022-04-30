using UnityEngine;

public class InitialBaseController : BaseController
{
    public string InitialBaseOwner => m_baseOwner;

    [Header("Initial Base Set Up")]
    [SerializeField] private string m_baseOwner = default;

    protected override void Awake()
    {
        base.Awake();
        ChangeTeamOwner(m_baseOwner);
    }
}