.proto 协议文件规范

1.以模块命名文件名。如家园协议文件，则为 Home.proto
2.协议枚举命名为 业务名ProtocolID，例如 SystemProtocolID。
3.协议ID命名为：[S2C]|[C2S]_行为，例如：C2S_SetName。
4.结构命名为：MSG_ 。


定义：
ProtocolClassName	生成协议文件名
ProtocolIDEnumName	协议Id枚举名
ProtocolID			协议Id

侦听函数方法名：On/*ProtocolID*/
请求函数方法名：/*ProtocolID*/
回调函数方法名：/*ProtocolID*/


代理层的存在，不仅仅是为了收发协议、而且还需要涉及到游戏逻辑的编写，所以没办法全自动。



    public void /*ProtocolID*/(/*Message*/ msg)
    {
        GlobalVars.network.Send((uint)/*ProtocolClassName*/./*ProtocolEnum*/./*ProtocolID*/, msg);
    }

/*
	private void /*S2CFunction*/(MemoryStream stream)\n\t{
        /*MSGStruct*/ msg = Serializer.Deserialize</*MSGStruct*/>(stream);
    }
*/


 private void OnResBuild(MemoryStream stream)
    {
        pto_S2C_ResBuild res = Serializer.Deserialize<pto_S2C_ResBuild>(stream);
        
        if (onResBuild != null) onResBuild(res);
    }

public Action<pto_S2C_ResCancelBuild> onResCancelBuild;
    public Action<pto_S2C_ResRemove> onResRemove;
    public Action<pto_S2C_ResQuickComplete> onResQuickComplete

"\tpublic Action</*Message*/> /*ProtocolID*/;\n"

"\tprivate void /*ProtocolID*/(MemoryStream stream)\n\t{\n\t\t/*Message*/ msg = Serializer.Deserialize</*Message*/>(stream);\n\t}"