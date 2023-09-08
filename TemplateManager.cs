using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using System;
using TMPro;

public class TemplateManager : EditorWindow
{
    private List<TemplateData> templates = new List<TemplateData>();
    private int selectedTemplateIndex = -1;
    private string jsonFilePath = "Assets/Templates.json";

    [MenuItem("Custom/TemplateManager")]
    public static void ShowWindow()
    {
        GetWindow<TemplateManager>("TemplateManager");
    }

    private void OnEnable()
    {
        LoadTemplatesFromJson();
    }

    private void OnGUI()
    {
        GUILayout.Label("Select a Template:", EditorStyles.boldLabel);

        if (templates.Count == 0)
        {
            GUILayout.Label("No templates available. Create one first.");

        }
        else
        {
            string[] templateNames = new string[templates.Count];

            for (int i = 0; i < templates.Count; i++)
            {
                templateNames[i] = templates[i].name;
            }

            selectedTemplateIndex = EditorGUILayout.Popup(selectedTemplateIndex, templateNames);

            GUILayout.Label("Customize Template Properties:");
            if (selectedTemplateIndex >= 0 && selectedTemplateIndex < templates.Count)
            {
                TemplateData selectedTemplate = templates[selectedTemplateIndex];

                selectedTemplate.name = EditorGUILayout.TextField("Name", selectedTemplate.name);
                selectedTemplate.position = EditorGUILayout.Vector2Field("Position", selectedTemplate.position);
                selectedTemplate.rotation = EditorGUILayout.FloatField("Rotation", selectedTemplate.rotation);
                selectedTemplate.scale = EditorGUILayout.Vector2Field("Scale", selectedTemplate.scale);
                selectedTemplate.color = EditorGUILayout.ColorField("Color", selectedTemplate.color);

            }
        
    

        }
        if (GUILayout.Button("Create New Template"))
        {
            CreateNewTemplate();
        }
        if (GUILayout.Button("Instantiate Selected Template"))
        {
            InstantiateSelectedTemplate();
        }
        if (GUILayout.Button("Save to JSON"))
        {
            SaveTemplatesToJson();
        }
    }
    private void CreateNewTemplate()
    {
        TemplateData newTemplate = new TemplateData();
        newTemplate.name = "New Template";
        templates.Add(newTemplate);
        selectedTemplateIndex = templates.Count - 1;
    }

    private void InstantiateSelectedTemplate()
    {
        if (selectedTemplateIndex >= 0 && selectedTemplateIndex < templates.Count)
        {
            TemplateData selectedTemplate = templates[selectedTemplateIndex];

            //parent canvas instantiation
            GameObject canvasObj = new GameObject("Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            //ui object based on the selected template
            GameObject uiObj= new GameObject(selectedTemplate.name);
            uiObj.transform.SetParent(canvas.transform, false);
            uiObj.transform.localPosition = new Vector3(selectedTemplate.position.x, selectedTemplate.position.y, 0f);
            uiObj.transform.localRotation = Quaternion.Euler(0, 0, selectedTemplate.rotation);
            uiObj.transform.localScale = new Vector3(selectedTemplate.scale.x, selectedTemplate.scale.y, 1f);
        }
        else
        {
            Debug.LogError("No template selected.");
        }
    }

    private void SaveTemplatesToJson()
    {
        string json = JsonUtility.ToJson(new TemplateList { templates = templates }, true);
        try
        {
            File.WriteAllText(jsonFilePath, json);
        }
        catch (Exception error)
        {
            Debug.LogError("Failed to save JSON file: " + error.Message);
        }
    }

    private void LoadTemplatesFromJson()
    {
        if (File.Exists(jsonFilePath))
        {
            try
            {
                string json = File.ReadAllText(jsonFilePath);
                TemplateList templateList = JsonUtility.FromJson<TemplateList>(json);
                templates = templateList.templates;
            }
            catch (Exception error)
            {
                Debug.LogError("Error loading JSON file: " + error.Message);
            }
        }
        else
        {
            Debug.LogWarning("JSON file not found at path: " + jsonFilePath);
        }
    }
}

[System.Serializable]
public class TemplateList
{
    public List<TemplateData> templates;
}

[System.Serializable]
public class TemplateData
{
    public string name;
    public Vector2 position;
    public float rotation;
    public Vector2 scale;
    public Color color;
}
