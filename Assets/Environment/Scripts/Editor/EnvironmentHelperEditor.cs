using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnvironmentHelper))]
public class EnvironmentHelperEditor : Editor
{
    public EnvironmentHelper env
    {
        get { return (EnvironmentHelper) target; }
    }
    private int paletteSelection = 0;
    private Rect paletteRect;
    public GameObject[] palette = new GameObject[0];
    private Vector3 previewPosition;
    private Material material;
    private bool buildingMode = false;
    private bool randomRotation = false;
    private void OnSceneGUI()
    {
        Event e = Event.current;
        Handles.BeginGUI();
        Rect sceneView = SceneView.lastActiveSceneView.position;
        float paletteRectHeight = buildingMode ? (palette.Length / ((int) sceneView.width / 52) + 1) * 50 : 0;
        buildingMode = GUI.Toggle(new Rect(sceneView.width / 2.0F - 62.5F, sceneView.height - paletteRectHeight - 100, 125, 20), buildingMode, "Feature Placement");
        paletteRect = new Rect(10, sceneView.height - paletteRectHeight - 45, sceneView.width - 20, paletteRectHeight + 45);
        GUI.Window(0, paletteRect, PaletteWindow, "", GUIStyle.none);
        if (buildingMode)
        {
            randomRotation = GUI.Toggle(new Rect(10, 10, 125, 20), randomRotation, "Random Rotation");
        }
        Handles.EndGUI();
        if (buildingMode)
        {
            Placement(e);
            Selection.activeGameObject = env.gameObject;
        }
    }

    private void PaletteWindow(int id)
    {
        EnvironmentHelper e = env;
        GUIContent[] contents = new GUIContent[palette.Length];
        for (int i = 0; i < palette.Length; i++)
        {
            GameObject paletteItem = palette[i];
            contents[i] = new GUIContent { tooltip = paletteItem.name, image = AssetPreview.GetAssetPreview(paletteItem)};
        }
        GUIStyle style = GUI.skin.button;
        paletteSelection = GUI.SelectionGrid(
            new Rect(paletteRect.width / 2 - Math.Min(contents.Length * 25,paletteRect.width / 2), 25, paletteRect.width - 20, paletteRect.height - 45), 
            paletteSelection, 
            contents, 
            ((int) paletteRect.width - 20) / 50,
            style);
    }

    private void Placement(Event e)
    {
        if (e.type == EventType.MouseDown && e.button == 0 && paletteSelection < palette.Length)
        {
            GameObject selected = palette[paletteSelection];
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit)) return;
            previewPosition = hit.point;
            Vector3 defaultEulers = selected.transform.eulerAngles;
            GameObject instance = PrefabUtility.InstantiatePrefab(selected, env.regionParent) as GameObject;
            instance.transform.position = hit.point;
            instance.transform.rotation = Quaternion.Euler(defaultEulers.x, (randomRotation ? UnityEngine.Random.Range(-180.0F, 180.0F) : SceneView.lastActiveSceneView.camera.transform.rotation.eulerAngles.y) + defaultEulers.y, defaultEulers.z);
            instance.name = selected.name;
            Undo.RegisterCreatedObjectUndo(instance, "Placed " + instance.name);
        }
    }

    private void OnEnable()
    {
        string[] db = AssetDatabase.FindAssets("", new [] {"Assets/Environment/Features/Prefabs"});
        List<GameObject> gos = new List<GameObject>();
        foreach (string d in db)
        {
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(d));
            if (asset == null) continue;
            gos.Add(asset);
        }
        palette = gos.ToArray();

    }
}
