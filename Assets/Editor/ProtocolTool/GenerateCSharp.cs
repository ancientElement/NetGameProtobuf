using System.Xml;
using UnityEngine;

namespace ProtocolGenerateTool.GenerateTool
{
    public class GenerateCSharp
    {
        public static void GenerateMessage(XmlNodeList nodeList)
        {
            //字段名
            foreach (XmlNode messageNode in nodeList)
            {
                //消息id
                string messageID = messageNode.Attributes["id"].Value;

                //是否是系统消息
                string systemMessage = messageNode.Attributes["systemMessage"].Value;

                //命名空间
                string namespaceStr = messageNode.Attributes["namespace"].Value;
                //类名字
                string classNameStr = messageNode.Attributes["name"].Value;
                //data类型
                string dataType = messageNode.Attributes["datatype"]?.Value;

                //GetMessageID
                string GetMessageIDMethod = $"public override int GetMessageID()\r\n" +
                     "{\r\n" +
                        $"return {messageID};" +
                      "\r\n}";

                //WriteIn
                string WriteInMethod = $"public override void WriteIn(byte[] buffer, int beginIndex)\r\n" +
                     "{\r\n" +
                        $" data = {dataType}.Parser.ParseFrom(buffer, beginIndex, buffer.Length - beginIndex);" +
                      "\r\n}";

                string messageStr;

                if (systemMessage != null && systemMessage == "1")
                {
                    //所有数据
                    messageStr = $"namespace {namespaceStr}" +
                                    "{\r\n" +
                                        $"public class {classNameStr} : AE_NetWork.BaseSystemMessage" +
                                         "{\r\n" +
                                           $"{GetMessageIDMethod}" +
                                           "\r\n}" +
                                    "\r\n}";
                }
                else
                {
                    //所有数据
                    messageStr = $"namespace {namespaceStr}" +
                                    "{\r\n" +
                                        $"public class {classNameStr} : AE_NetWork.BaseMessage<{dataType}>" +
                                         "{\r\n" +
                                           $"{GetMessageIDMethod}" +
                                           $"{WriteInMethod}" +
                                           "\r\n}" +
                                    "\r\n}";
                }


                GenerateFileTool.Generate($"{Application.dataPath}/Protocal/{namespaceStr}/Message/", $"{classNameStr}.cs", messageStr);
            }
        }

        public static void GenerateHandler(XmlNodeList nodeList)
        {
            //字段名
            foreach (XmlNode messageNode in nodeList)
            {
                //命名空间
                string namespaceStr = messageNode.Attributes["namespace"].Value;
                //类名字
                string classNameStr = messageNode.Attributes["name"].Value;
                //消息id
                string messageID = messageNode.Attributes["id"].Value;

                //所有数据
                string handlerStr = $"namespace {namespaceStr}" +
                                 "{\r\n" +
                                     $"public class {classNameStr}Handler : AE_NetWork.BaseHandler" +
                                      "{\r\n" +
                                            "public override void Handle()" +
                                            "{\r\n" +
                                                
                                            "\r\n}" +
                                        "\r\n}" +
                                 "\r\n}";
                GenerateFileTool.Generate($"{Application.dataPath}/Protocal/{namespaceStr}/Handler/", $"{classNameStr}Handler.cs", handlerStr);
            }
        }

        public static void GenerateMessagePool(XmlNodeList nodeList)
        {
            //类名字
            string classNameStr = string.Empty;
            //消息id
            string messageID = string.Empty;

            string str = string.Empty;

            str += "\r\n";

            foreach (XmlNode messageNode in nodeList)
            {
                classNameStr = messageNode.Attributes["name"].Value;
                messageID = messageNode.Attributes["id"].Value;

                //所有数据
                str += $"Register({messageID}, typeof({classNameStr}), typeof({classNameStr}Handler));\r\n";
            }

            Debug.Log(str);
        }

    }
}
