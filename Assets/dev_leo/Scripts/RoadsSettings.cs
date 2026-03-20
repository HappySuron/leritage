using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RoadsSettings : MonoBehaviour
{
    [Header("Road settings")]
    [Tooltip("Ширина дороги (по X).")]
    public float roadWidth = 8f;
    [Tooltip("Длина дороги вдоль Z.")]
    public float roadLength = 30f;
    [Tooltip("Количество полос движения.")]
    public int laneCount = 3;

    [Header("Divider settings")]
    [Tooltip("Префаб разделительной линии.")]
    public GameObject dividerPrefab;
    [Tooltip("Длина одного разделителя по Z.")]
    public float dividerLength = 30f;
    [Tooltip("Ширина разделителя по X.")]
    public float dividerWidth = 0.2f;
    [Tooltip("Толщина разделителя по Y.")]
    public float dividerThickness = 0.05f;

    [Header("Optional road surface")]
    [Tooltip("Префаб поверхности дороги (опционально).")]
    public GameObject roadSurfacePrefab;


    // SLV EDITTED
    // added the points in the beginning and end of the roads so we
    // could spawn objects
    [Header("Lane Points - slv")]
    public bool createLanePoints = true;

    [Header("Editor")]
    public bool rebuildOnValidate = false;

    [ContextMenu("Build Road")]
    public void BuildRoad()
    {
        if (dividerPrefab == null)
        {
            Debug.LogWarning("RoadsSettings: dividerPrefab не задан.");
            return;
        }

        ClearRoad();

        laneCount = Mathf.Max(1, laneCount);
        roadWidth = Mathf.Max(0.1f, roadWidth);
        roadLength = Mathf.Max(0.1f, roadLength);
        dividerLength = Mathf.Max(0.01f, dividerLength);
        dividerWidth = Mathf.Max(0.01f, dividerWidth);
        dividerThickness = Mathf.Max(0.01f, dividerThickness);

        float laneWidth = roadWidth / laneCount;
        int separatorCount = Mathf.Max(0, laneCount - 1);

        if (roadSurfacePrefab != null)
        {
            var surface = Instantiate(roadSurfacePrefab, transform);
            surface.name = "Road_Surface";
            surface.transform.localPosition = new Vector3(0f, 0f, 0f);
            //surface.transform.localRotation = Quaternion.identity;
            surface.transform.localScale = new Vector3(roadWidth, 1f, roadLength);
        }

        for (int i = 1; i <= separatorCount; i++)
        {
            float x = -roadWidth * 0.5f + laneWidth * i;
            BuildDividerLine(i, x);
        }


        BuildLanePoints();

        Debug.Log("RoadsSettings: Дорога построена.");
    }

    private void BuildDividerLine(int separatorIndex, float x)
    {
        var divider = Instantiate(dividerPrefab, transform);
        divider.name = $"Divider_{separatorIndex}";
        divider.transform.localPosition = new Vector3(x, 0.01f, 0);
        divider.transform.localRotation = Quaternion.identity;
        divider.transform.localScale = new Vector3(dividerWidth, dividerThickness, dividerLength);
    }

    [ContextMenu("Clear Road")]
    public void ClearRoad()
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in transform)
        {
            children.Add(child.gameObject);
        }

        foreach (var child in children)
        {
            if (Application.isEditor)
                DestroyImmediate(child);
            else
                Destroy(child);
        }
    }

    private void OnValidate()
    {
        if (!Application.isPlaying && rebuildOnValidate)
            BuildRoad();
    }


    // SLV EDITTED

    [ContextMenu("Build Lane Points")]
    public void BuildLanePoints()
    {
        if (!createLanePoints) return;

        float laneWidth = roadWidth / laneCount;

        for (int i = 0; i < laneCount; i++)
        {
            // центр полосы
            float centerX = -roadWidth * 0.5f + laneWidth * (i + 0.5f);

            // начало дороги
            Vector3 startPos = new Vector3(centerX, 0f, -roadLength * 0.5f);

            // конец дороги
            Vector3 endPos = new Vector3(centerX, 0f, roadLength * 0.5f);

            // создаем объекты
            CreatePoint($"Lane_{i}_Start", startPos);
            CreatePoint($"Lane_{i}_End", endPos);
        }

        Debug.Log("Lane points созданы.");
    }

    private void CreatePoint(string name, Vector3 localPos)
    {
        GameObject point = new GameObject(name);
        point.transform.parent = transform;
        point.transform.localPosition = localPos;
        point.transform.localRotation = Quaternion.identity;
        point.transform.localScale = Vector3.one;
    }

}
