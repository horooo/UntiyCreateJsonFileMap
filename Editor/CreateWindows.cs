using UnityEngine;
using System.IO;
using UnityEditor;

public class CreateWindows : EditorWindow
{

    CreateWindows()
    {
        this.titleContent = new GUIContent("Create Json");
    }

    [MenuItem("Tool/Create File Map")]
    static void showWindow()
    {
        GetWindow(typeof(CreateWindows));
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();

        GUILayout.Space(15);
        GUI.skin.label.fontSize = 24;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label("导出文件位置映射关系Json");

        GUILayout.Space(15);
        EditorGUILayout.LabelField(" * 请索引到 Resources 或 Resources以下文件夹!");
        CreateFileInfoMap.readPath = EditorGUILayout.TextField("读取路径:", CreateFileInfoMap.readPath);
        GUILayout.Space(5);
        CreateFileInfoMap.tryFindFile = EditorGUILayout.TextField("查找文件:", CreateFileInfoMap.tryFindFile);
        GUILayout.Space(5);
        CreateFileInfoMap.searchScope = (SearchOption)EditorGUILayout.EnumPopup("查找范围", CreateFileInfoMap.searchScope);

        GUILayout.Space(15);
        CreateFileInfoMap.enumName = EditorGUILayout.TextField("创建枚举文件名:", CreateFileInfoMap.enumName);
        GUILayout.Space(5);
        CreateFileInfoMap.outputEnumFilePath = EditorGUILayout.TextField("创建枚举文件在:", CreateFileInfoMap.outputEnumFilePath);

        GUILayout.Space(15);
        CreateFileInfoMap.mapClassName = EditorGUILayout.TextField("创建映射类名:", CreateFileInfoMap.mapClassName);
        GUILayout.Space(5);
        CreateFileInfoMap.outputMapFilePath = EditorGUILayout.TextField("创建映射类文件在:", CreateFileInfoMap.outputMapFilePath);

        GUILayout.Space(15);
        EditorGUILayout.LabelField(" * 读取使用 Dictionary<enum,string> 接收");
        CreateFileInfoMap.jsonName = EditorGUILayout.TextField("创建Json文件名:", CreateFileInfoMap.jsonName);
        GUILayout.Space(5);
        CreateFileInfoMap.outputJsonFilePath = EditorGUILayout.TextField("创建映Json文件在:", CreateFileInfoMap.outputJsonFilePath);

        GUILayout.Space(20);
        if (GUILayout.Button("Create"))
        {
            CreateFileInfoMap.CreateFileMapInfo();
        }

        GUILayout.Space(20);
        GUI.skin.label.fontSize = 15;
        GUILayout.Label(CreateFileInfoMap.state);

        EditorGUILayout.EndVertical();
    }

}