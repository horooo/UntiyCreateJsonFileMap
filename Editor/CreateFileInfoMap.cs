using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.CodeDom;
using Newtonsoft.Json;
using System.Reflection;
using System.CodeDom.Compiler;
using System.Collections.Generic;

public class CreateFileInfoMap
{
    public static SearchOption searchScope = SearchOption.TopDirectoryOnly;
    public static string tryFindFile = "*.prefab";
    public static string enumName = "UIPanelType";
    public static string mapClassName = "UIPanelMapInfo";
    public static string jsonName = "UIPanelMap.json";
    public static string readPath = "Resources/";
    public static string outputEnumFilePath = "";
    public static string outputMapFilePath = "";
    public static string outputJsonFilePath = "";
    public static string state;

    public static void CreateFileMapInfo()
    {

        string _outputEnumFilePath = string.Concat(Application.dataPath, "/", outputEnumFilePath);
        string _outputMapFilePath = string.Concat(Application.dataPath, "/", outputMapFilePath);
        string _outputJsonFilePath = string.Concat(Application.dataPath, "/", outputJsonFilePath);
        string _readPath = string.Concat(Application.dataPath + "/" + readPath);

        if (!string.IsNullOrEmpty(readPath) && !Directory.Exists(_readPath))
        {
            state = "读取路径未找到!";
            return;
        }
        if (!string.IsNullOrEmpty(outputJsonFilePath) && !Directory.Exists(_outputJsonFilePath))
        {
            state = "输出json路径未找到!";
            return;
        }
        if (!string.IsNullOrEmpty(outputMapFilePath) && !Directory.Exists(_outputMapFilePath))
        {
            state = "输出映射信息类路径未找到!";
            return;
        }
        if (!string.IsNullOrEmpty(outputEnumFilePath) && !Directory.Exists(_outputEnumFilePath))
        {
            state = "输出枚举路径未找到!";
            return;
        }

        //合法文件
        List<FileInfo> fileList = new List<FileInfo>();
        CodeCompileUnit unit = new CodeCompileUnit();
        CodeTypeDeclaration customerclass = CreateClassAttributes(unit, enumName);

        #region FileInfo

        //获取指定路径下面的所有资源文件  
        DirectoryInfo direction = new DirectoryInfo(_readPath);
        FileInfo[] files = direction.GetFiles(tryFindFile, searchScope);

        if (files.Length == 0)
        {
            state = "未找到有效文件！";
            return;
        }

        Debug.Log("共查找有效文件: " + files.Length);

        state = "正在读取文件信息中...";
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Name.EndsWith(".meta", StringComparison.Ordinal))
                continue;

            //添加字段
            CodeMemberField field = new CodeMemberField(typeof(Enum), files[i].Name.Remove(files[i].Name.Length - 7, 7));
            customerclass.Members.Add(field);
            fileList.Add(files[i]);
            //设置进度条  
            EditorUtility.DisplayProgressBar("File", state, (i * 1.0f) / files.Length);
        }

        #endregion

        //生成代码
        state = "正在生成Enum...";
        EditorUtility.DisplayProgressBar("Enum", state, 0.5f);
        CodeGenerator(unit, string.Concat(_outputEnumFilePath, "/", enumName));

        //创建映射类
        state = "正在生成MapClass...";
        EditorUtility.DisplayProgressBar("Class", state, 0.5f);
        CreateMapClass();

        #region Json

        //添加到dict
        Dictionary<int, string> dict = new Dictionary<int, string>();
        state = "正在添加映射关系中...";
        for (int i = 0; i < fileList.Count; i++)
        {
            string str = fileList[i].FullName.Remove(0, Application.dataPath.Length + readPath.Length + 1);
            dict.Add(i, str.Remove(str.Length - 7, 7));
            EditorUtility.DisplayProgressBar("Map", state, (i * 1.0f) / fileList.Count);
        }

        //转成json并保存出文件
        state = "正在保存json信息...";
        using (StreamWriter sw = new StreamWriter(string.Concat(_outputJsonFilePath, "/", jsonName), false))
        {
            EditorUtility.DisplayProgressBar("StreamWriter", state, 0.5f);
            sw.Write(JsonConvert.SerializeObject(dict));
        }

        #endregion
        state = "导出json成功!";
        Debug.Log("导出json成功! path : " + string.Concat(_outputJsonFilePath, "/", jsonName));

        EditorUtility.ClearProgressBar();
    }

    #region privateMethod

    private static void CreateMapClass()
    {
        //准备一个代码编译器单元
        CodeCompileUnit unit = new CodeCompileUnit();
        //准备必要的命名空间（这个是指要生成的类的空间）
        CodeTypeDeclaration customerclass = CreateClassAttributes(unit, mapClassName, false);

        //添加字段
        CodeMemberField field1 = new CodeMemberField(enumName, "type");
        field1.Attributes = MemberAttributes.Public;
        customerclass.Members.Add(field1);
        CodeMemberField field2 = new CodeMemberField(typeof(string), "path");
        field2.Attributes = MemberAttributes.Public;
        customerclass.Members.Add(field2);

        //生成代码
        CodeGenerator(unit, string.Concat(Application.dataPath, "/" + outputMapFilePath, "/", mapClassName));

    }

    private static CodeTypeDeclaration CreateClassAttributes(CodeCompileUnit unit, string className, bool IsEnum = true)
    {
        CodeNamespace sampleNamespace = new CodeNamespace();
        //导入必要的命名空间
        //sampleNamespace.Imports.Add(new CodeNamespaceImport("System"));
        CodeTypeDeclaration customerclass = new CodeTypeDeclaration(className);
        if (IsEnum)
            customerclass.IsEnum = true;
        else
            customerclass.IsClass = true;
        customerclass.TypeAttributes = TypeAttributes.Public;
        sampleNamespace.Types.Add(customerclass);
        unit.Namespaces.Add(sampleNamespace);
        return customerclass;
    }

    private static void CodeGenerator(CodeCompileUnit unit, string outPath)
    {
        //生成代码
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");

        using (StreamWriter sw = new StreamWriter(string.Concat(outPath, ".cs"), false))
        {
            provider.GenerateCodeFromCompileUnit(unit, sw, new CodeGeneratorOptions());
        }
    }

    #endregion

}
