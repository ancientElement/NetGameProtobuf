using System.Xml;
using UnityEditor;

namespace ProtocolGenerateTool
{
    public class ProtocolTool
    {
        public static void GenerateCSharp(string xmlPath, bool overridHandler)
        {
            GenerateTool.GenerateCSharp.GenerateMessage(GetNodeList("message", xmlPath));
            if (overridHandler) GenerateTool.GenerateCSharp.GenerateHandler(GetNodeList("message", xmlPath));
            GenerateTool.GenerateCSharp.GenerateMessagePool(GetNodeList("message", xmlPath));
            AssetDatabase.Refresh();
        }

        private static XmlNodeList GetNodeList(string name, string xmlPath)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlPath);
            XmlNode root = xml.SelectSingleNode("messages");
            return root.SelectNodes(name);
        }
    }
}
