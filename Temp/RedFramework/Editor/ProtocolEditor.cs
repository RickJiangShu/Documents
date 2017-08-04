#if TCP
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.IO;

public class ProtocolEditor : Editor
{

    private static string ListenTemplete = "\t\tGlobal.net.Listen((uint)/*ProtocolClassName*/./*ProtocolEnum*/./*ProtocolID*/, On/*ProtocolID*/);\n";
    private static string C2SFunctionTemplete = "\tpublic void /*ProtocolID*/(/*Message*/ msg)\n\t{\n\t\tGlobal.net.Send((uint)/*ProtocolClassName*/./*ProtocolEnum*/./*ProtocolID*/, msg);\n\t}\n";
    private static string S2CFunctionTemplete = "\tprivate void On/*ProtocolID*/(MemoryStream stream)\n\t{\n\t\t/*Message*/ msg = Serializer.Deserialize</*Message*/>(stream);\n\n\t\tif(/*ProtocolID*/ != null) /*ProtocolID*/(msg);\n\t}\n";
    private static string CallbackTemplete = "\tpublic Action</*Message*/> /*ProtocolID*/;\n";

    private static string MessageName = "MSG_{0}";//结构规定{0}为协议ID
    private static Regex ProtocolEnumRegex = new Regex(@"enum\s+(\w+ProtocolID)\s+{(?:\s*(\w+)\s*=\s*\d+;.*)*");//匹配协议枚举以及子ID

    private static string ProtogenCmd = "cd {0};protogen -i:{1} -o:{2};move -Force {3} {4}";

    [MenuItem("Red/Protocol/Update")]
    static void Update()
    {
        CommandUtils.ProcessCommand("TortoiseProc.exe", "/command:update /path:" + Path.ProtocolLibrary + " /closeonend:0");

    }

    [MenuItem("Red/Protocol/Output")]
    static void Output()
    {
        if (!FileUtils.IsFolderExists(Path.ProtocolLibrary))
            FileUtils.CreateFolder(Path.ProtocolLibrary);

        if (!FileUtils.IsFolderExists(Path.ProtocolOutput))
            FileUtils.CreateFolder(Path.ProtocolOutput);

        FileInfo[] protoFiles = FileUtils.GetFiles(Path.ProtocolLibrary, "*.proto");
        for (int i = 0, l = protoFiles.Length; i < l; i++)
        {
            FileInfo file = protoFiles[i];
            string FileFullName = file.FullName;
            string pureName = FileUtils.GetPureName(file);
            string outputName = pureName + ".cs";
            string OutputFullName = FileUtils.WindowsPath(FileUtils.GetFullPath(Path.ProtocolOutput + outputName));

            string cmd = string.Format(ProtogenCmd, file.DirectoryName, file.Name, outputName, outputName, OutputFullName);
            CommandUtils.ProcessCommand("powershell", cmd);  
        }
    }

    [MenuItem("Red/Protocol/Initialize Proxy")]
    static void GenerateProxy()
    {
        //读取模板文件
        string proxyTempletePath = Path.Framework + "Network/ProxyTemplete";
        string proxyTemplete = FileUtils.Read(proxyTempletePath);

        FileInfo[] protoFiles = FileUtils.GetFiles(Path.ProtocolLibrary, "*.proto");
        for (int i = 0, l = protoFiles.Length; i < l; i++)
        {
            FileInfo file = protoFiles[i];
            string FileName = FileUtils.GetPureName(file);
            string ClassName = FileName + "Proxy";
            string outputFileName = Path.ProxyOutput + ClassName + ".cs";//输出文件名

            if (FileUtils.IsFileExists(outputFileName))//已存在不覆盖
                continue;

            string ProtocolClassName = FileName;
            string fileString = FileUtils.Read(file);
            MatchCollection ProtocolEnumMatchList = ProtocolEnumRegex.Matches(fileString);//这里的List匹配了所有ProtocolID枚举并捕获了它们

            string ProxyCode = "";
            string ListenCode = "";//侦听函数
            string C2SCode = "";//C2S请求
            string S2CCode = "";//S2C处理
            string CallbackCode = "";//回调函数
            for(int j = 0,k = ProtocolEnumMatchList.Count;j<k;j++)
            {
                Match enumMatch = ProtocolEnumMatchList[j];
                Group enumNameGroup = enumMatch.Groups[1];
                Group IDGroup = enumMatch.Groups[2];

                string ProtocolEnum = enumNameGroup.Captures[0].Value;
                if (IDGroup.Success)//有可能是空结构
                {
                    for (int n = 0, m = IDGroup.Captures.Count; n < m; n++)
                    {
                        string ProtocolID = IDGroup.Captures[n].Value;
                        if (ProtocolID.IndexOf("C2S") != -1)//C2S协议
                        {
                            string C2SLine = C2SFunction(ProtocolClassName, ProtocolEnum, ProtocolID);
                            C2SCode += C2SLine;
                        }
                        else if (ProtocolID.IndexOf("S2C") != -1)//S2C协议
                        {
                            string ListenLine = ListenTemplete.Replace("/*ProtocolClassName*/", ProtocolClassName).Replace("/*ProtocolEnum*/", ProtocolEnum).Replace("/*ProtocolID*/", ProtocolID);
                            ListenCode += ListenLine;

                            string S2CLine = S2CFunction(ProtocolID);
                            S2CCode += S2CLine;

                            string CallbackLine = CallbackAction(ProtocolID);
                            CallbackCode += CallbackLine;
                        }
                        else//其他如：服务器和DB通信
                        {
                        }
                    }
                }
            }

            ProxyCode = proxyTemplete.Replace("/*ClassName*/", ClassName).Replace(
                   "/*CallbackCode*/", CallbackCode).Replace(
                   "/*ListenCode*/", ListenCode).Replace(
                   "/*C2SCode*/", C2SCode).Replace(
                   "/*S2CCode*/", S2CCode);

            FileUtils.CreateFile(outputFileName);
            FileUtils.Write(outputFileName, ProxyCode);
        }


       // Regex ProtocolIDRegex = new Regex(@"enum\s+(\w+ProtocolID)");
        
    //    Match ProtocolIDMatch = ProtocolIDRegex.Match(testStr);
        
    }
    private static string C2SFunction(string ProtocolClassName,string ProtocolEnum,string ProtocolID)
    {
        string Message = string.Format(MessageName, ProtocolID);

        return C2SFunctionTemplete.Replace("/*Message*/", Message).Replace(
            "/*ProtocolClassName*/", ProtocolClassName).Replace(
            "/*ProtocolEnum*/", ProtocolEnum).Replace(
            "/*ProtocolID*/", ProtocolID);
    }
    private static string S2CFunction(string ProtocolID)
    {
        string Message = string.Format(MessageName, ProtocolID);

        return S2CFunctionTemplete.Replace("/*Message*/", Message).Replace(
            "/*ProtocolID*/", ProtocolID);
    }
    private static string CallbackAction(string ProtocolID)
    {
        string Message = string.Format(MessageName, ProtocolID);

        return CallbackTemplete.Replace("/*Message*/", Message).Replace(
            "/*ProtocolID*/", ProtocolID);
    }

}
#endif