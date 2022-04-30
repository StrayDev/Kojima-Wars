using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnvironmentHelper : MonoBehaviour
{
  [Header("Pop-In")]
  public Transform regionParent;
  public float popInDistance = 100.0F;
  private Transform player;

  private void Start()
  {
    player = Camera.main.transform;
    StartCoroutine(ProcessPopIn());
  }

  private void OnDisable()
  {
    StopAllCoroutines();
  }

  private IEnumerator ProcessPopIn()
  {
    while (popInDistance > 0.0F)
    {
      float dist2 = popInDistance * popInDistance;
      float startTime = Time.time;
      // TODO: Do some region stuff to reduce workload.
      for (int i = 0; i < regionParent.childCount; i++)
      {
        // TODO: Take into account bounding box size
        Transform child = regionParent.GetChild(i);
        if (child.gameObject.activeSelf && (player.position - child.position).sqrMagnitude > dist2)
          StartCoroutine(PopOut(child));
        else if (!child.gameObject.activeSelf && (player.position - child.position).sqrMagnitude < dist2)
          StartCoroutine(PopIn(child));
        yield return null;
      }
      yield return new WaitForSeconds(0.5F - (Time.time - startTime));
    }
  }

  private IEnumerator PopIn(Transform t)
  {
    if (t.gameObject.activeSelf) yield break;
    t.gameObject.SetActive(true);
    Renderer r = t.GetComponent<Renderer>();
    if (r != null)
    {
      float height = r.bounds.size.y;
      float initialY = t.position.y;
      float time = 0.0F;
      while (time < 0.1F)
      {
        t.position += Vector3.up * (height * Time.deltaTime * 10.0F);
        time += Time.deltaTime;
        yield return null;
      }
      t.position = new Vector3(t.position.x,initialY + height, t.position.z);
    }
  }
  private IEnumerator PopOut(Transform t)
  {
    if (!t.gameObject.activeSelf) yield break;
    Renderer r = t.GetComponent<Renderer>();
    if (r != null)
    {
      float height = r.bounds.size.y;
      float initialY = t.position.y;
      float time = 0.0F;
      while (time < 0.1F)
      {
        t.position += Vector3.down * (height * Time.deltaTime * 10.0F);
        time += Time.deltaTime;
        yield return null;
      }
      t.position = new Vector3(t.position.x,initialY - height, t.position.z);
    }
    t.gameObject.SetActive(false);
  }
}
