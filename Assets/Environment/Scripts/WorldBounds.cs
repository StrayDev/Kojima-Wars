using System;
using System.Collections;
using System.Collections.Generic;
using Networking;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class WorldBounds : MonoBehaviour
{
  [SerializeField] private float turnAroundTime = 0.5F;
  private BoxCollider boxCollider;

  private void Start()
  {
    boxCollider = GetComponent<BoxCollider>();
  }

  public void OnTriggerExit(Collider _other)
  {
    if (_other.tag != "Player") return;
    StartCoroutine(I_TurnAround(_other.attachedRigidbody)); // Handle all axis the same
    //if (_other.transform.position.y < boxCollider.bounds.max.y) StartCoroutine(I_TurnAround(_other.attachedRigidbody));
    //else StartCoroutine(I_TurnDown(_other.attachedRigidbody));
  }

  private IEnumerator I_TurnAround(Rigidbody _rb)
  {
    if (_rb == null) yield break;
    if (!(_rb.GetComponent<NetworkTransformComponent>() is NetworkTransformComponent tf)) yield break;
    float t = 0.0F;
    float v = _rb.velocity.magnitude;
    while (t < turnAroundTime)
    {
      _rb.transform.Rotate(-180.0F * Time.deltaTime / turnAroundTime, 0.0F, 0.0F, Space.Self);
      _rb.velocity = _rb.transform.forward * v;
      t += Time.deltaTime;
      yield return null;
    }
  }

  //private IEnumerator I_TurnDown(Rigidbody _rb)
  //{
  //      float s = _rb.velocity.magnitude;
  //      float dir = _rb.transform.up.y > 0.0F ? 1.0F : 0.0F;

  //      _rb.transform.Rotate(dir * 180.0F * Time.deltaTime / turnAroundTime, 0.0F, 0.0F, Space.Self);
  //      _rb.velocity = _rb.transform.forward * s;
  //      yield return null;
  //}
}
