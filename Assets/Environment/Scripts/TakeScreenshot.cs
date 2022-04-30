using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeScreenshot : MonoBehaviour
{
  public string filePath;
  public int superSize = 2;

  [ContextMenu("Take Screenshot")]
  public void Capture()
  {
    ScreenCapture.CaptureScreenshot(filePath,superSize);
    Debug.Log($"Saved Screenshot to {filePath}");
  }
}
