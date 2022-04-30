using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[ExecuteAlways]
public class VibrantSkyController : MonoBehaviour
{
  [SerializeField] private Transform sun;
  [SerializeField] private Transform moon;

  public bool dayNightCycle = false;
  public float timeScale = 1.0F;

  private void LateUpdate()
  {
    if (sun != null ) Shader.SetGlobalVector("_SunDir", -sun.transform.forward);
    if (moon != null) Shader.SetGlobalVector("_MoonDir", -moon.transform.forward);
    
    if (dayNightCycle && Application.isPlaying)
    {
      sun.transform.Rotate(Vector3.right, Time.deltaTime * timeScale);
      moon.transform.Rotate(Vector3.right, Time.deltaTime * timeScale);
    }
  }
}
