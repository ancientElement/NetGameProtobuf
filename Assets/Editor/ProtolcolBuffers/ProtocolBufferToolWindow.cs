using System.Linq;
using UnityEditor;
using UnityEngine;
using CustomProtolcolBuffers;
using System;
using System.Text;
using ProtocolGenerateTool;

public class ProtocolBufferToolWindow : EditorWindow
{
    public ProtocolBufferToolWindowSO data;

    private int lauage;

    [MenuItem("Protobuf/配置生成Window")]
    public static void ShowWindow()
    {
        ProtocolBufferToolWindow window = GetWindowWithRect<ProtocolBufferToolWindow>(new Rect(0, 0, 550, 330));
        window.Show();
    }

    private void OnGUI()
    {
        float height = 0;

        GUI.Label(new Rect(0, 0, 150, 30), "proto文件夹位置");
        data.protoFilePath = GUI.TextField(new Rect(150, 0, 400, 30), data.protoFilePath);
        height += 35;

        GUI.Label(new Rect(0, height, 150, 30), "输出位置");
        data.outputFilePath = GUI.TextField(new Rect(150, height, 400, 30), data.outputFilePath);
        height += 35;


        GUI.Label(new Rect(0, height, 150, 30), "proto.exe位置");
        data.protoExepath = GUI.TextField(new Rect(150, height, 400, 30), data.protoExepath);
        height += 35;

        lauage = GUI.Toolbar(new Rect(0, height, 550, 50), lauage, new string[] { "C#", "Java", "C++" });
        height += 55;

        if (GUI.Button(new Rect(0, height, 550, 50), "生成Proto类"))
        {
            if (data.protoFilePath != string.Empty)
                switch (lauage)
                {
                    case 0:
                        ProtobufTool.Generate(data.outputFilePath, data.protoFilePath, ProtobufTool.csharp, data.protoExepath);
                        break;
                    case 1:
                        ProtobufTool.Generate(data.outputFilePath, data.protoFilePath, ProtobufTool.java, data.protoExepath);
                        break;
                    case 2:
                        ProtobufTool.Generate(data.outputFilePath, data.protoFilePath, ProtobufTool.cpp, data.protoExepath);
                        break;
                }
        };
        height += 100;

        GUI.Label(new Rect(0, height, 150, 30), "消息文件XML位置");
        data.messageXMLFilePath = GUI.TextField(new Rect(150, height, 400, 30), data.messageXMLFilePath);
        height += 35;

        data.overridHandler = GUI.Toggle(new Rect(5, height, 150, 30), data.overridHandler, "是否覆盖Handler类");

        if (GUI.Button(new Rect(150, height, 400, 30), "生成消息类"))
        {
            if (data.messageXMLFilePath != string.Empty)
            {
                ProtocolTool.GenerateCSharp(data.messageXMLFilePath,data.overridHandler);
            }
        }
    }
}
