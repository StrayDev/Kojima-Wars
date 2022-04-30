using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnitQueueUI : MonoBehaviour
{
    [SerializeField] private GameObject m_uiContent = default;
    [SerializeField] private GameObject m_queueObject = default;

    private BaseUnitGeneration m_interactingBase = null;
    private List<BaseUnitQueueObject> m_queueObjects = new List<BaseUnitQueueObject>();

    private void OnEnable()
    {
        BaseUnitGeneration.OnInteracted += OnInteracted;
        BaseUnitGeneration.OnLocalPlayerExitedRange += OnLocalPlayerExited;
        BaseUnitGeneration.OnQueueAdded += OnUnitQueueAdded;
        BaseUnitGeneration.OnQueueEnded += OnUnitQueueEnded;
        BaseUnitGeneration.OnQueueProgressUpdate += OnUnitQueueUpdated;
    }

    private void OnDisable()
    {
        BaseUnitGeneration.OnInteracted -= OnInteracted;
        BaseUnitGeneration.OnLocalPlayerExitedRange -= OnLocalPlayerExited;
        BaseUnitGeneration.OnQueueAdded -= OnUnitQueueAdded;
        BaseUnitGeneration.OnQueueEnded -= OnUnitQueueEnded;
        BaseUnitGeneration.OnQueueProgressUpdate -= OnUnitQueueUpdated;
    }

    private void OnInteracted(BaseUnitGeneration unitGenerationBase)
    {
        foreach (var unit in m_queueObjects.ToArray())
        {
            Destroy(unit.gameObject);
            m_queueObjects.Remove(unit);
        }

        foreach (var unit in unitGenerationBase.GetUnitQueue())
        {
            OnUnitQueueAdded(unit);
        }
        
        // gets the current units in the queue, displays them appropriately
        m_uiContent.SetActive(true);
        m_interactingBase = unitGenerationBase;
    }

    private void OnLocalPlayerExited(Entity player, BaseUnitGeneration unitGenerationBase)
    {
        foreach (var unit in m_queueObjects.ToArray())
        {
            Destroy(unit.gameObject);
            m_queueObjects.Remove(unit);
        }
        
        m_uiContent.SetActive(false);
        m_interactingBase = null;
        
    }

    private void OnUnitQueueAdded(AIUnitTypesData.UnitTypeInfo unit)
    {
        GameObject go = Instantiate(m_queueObject);
        go.transform.SetParent(m_uiContent.transform);
        BaseUnitQueueObject unitQueueObject = go.GetComponent<BaseUnitQueueObject>();
        if (unitQueueObject)
        {
            m_queueObjects.Add(unitQueueObject);
            unitQueueObject.SetUnitData(unit, m_interactingBase);
        }
    }

    private void OnUnitQueueEnded(BaseUnitGeneration unitGenerationBase)
    {
        if (m_uiContent.activeSelf)
        {
            if (m_queueObjects.Count > 0)
            {
                Destroy(m_queueObjects[0].gameObject);
                m_queueObjects.RemoveAt(0);
            }
        }
    }

    private void OnUnitQueueUpdated(float amount)
    {
        if (m_queueObjects.Count > 0)
        {
            m_queueObjects[0].UpdateProgress(amount);
        }
    }

}
