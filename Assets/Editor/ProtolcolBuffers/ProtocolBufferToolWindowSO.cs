using UnityEngine;

[CreateAssetMenu(menuName = "proto/ProtocolBufferToolWindowSO")]
public class ProtocolBufferToolWindowSO : ScriptableObject
{
    public string protoFilePath;
    public string messageXMLFilePath;
    public string outputFilePath;
    public string protoExepath;
    public bool overridHandler;
}
