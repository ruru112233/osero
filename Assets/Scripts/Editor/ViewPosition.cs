using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Board))]
public class ViewPosition : Editor
{
    public void OnEnable()
    {
        Debug.Log("aa");
        SceneView.duringSceneGui -= OnSceneViewClick;
        SceneView.duringSceneGui += OnSceneViewClick;
    }

    public void OnSceneViewClick(SceneView sceneView)
    {
        Event e = Event.current;
        if ((e.type == EventType.MouseDrag || e.type == EventType.MouseDown) && e.button == 0)
        {
            Vector3 mousePosition = new Vector3(e.mousePosition.x, Camera.current.pixelHeight - e.mousePosition.y, 0);
            if (Physics.Raycast(Camera.current.ScreenPointToRay(mousePosition), out var hit))
            {
                Debug.Log(hit.point);
                
            }
        }
    }
}
