using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Text.RegularExpressions;

public delegate void GMCommandEventHandler(string commond, string[] args);
/// <summary>
/// GM命令标准格式：
/// !!command=args1,args2,argsN...
/// !!commond=arg
/// !!commond
/// 
/// 使用方法：
/// 1、将GMCommandInput.prefab放到Canvas下面
/// 2、注册GMCommondSubmit事件
/// </summary>
public class GMCommandInput : MonoBehaviour
{
  //  private static Regex regex = new Regex(@"(\w+)(?:=(.+),?)?");


 //   private static Regex regex = new Regex(@"(\w+)(?:=)?(.+,?)+");

    public event GMCommandEventHandler GMCommondSubmit;
    public InputField m_InputField;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnSubmit()
    {
        string input = m_InputField.text;

        string command;
        string[] args = null;
        if (input.IndexOf('=') != -1)
        {
            string[] group = input.Split('=');
            command = group[0];
            args = group[1].Split(',');
        }
        else
        {
            command = input;
        }

        if (GMCommondSubmit != null)
            GMCommondSubmit(command, args);

        /*
        if (regex.IsMatch(input))
        {
            

            Match match = regex.Match(input);
            GroupCollection group = match.Groups;
            if (group.Count > 2)
            {
                int length = group.Count - 2;
                args = new string[length];
                for (int i = 0; i < length; i++)
                {
                    args[i] = group[2 + i].Value;
                }
            }

            if(GMCommondSubmit != null)
                GMCommondSubmit(command, args);
        }
        else
        {
            Debug.LogWarning("GM命令格式错误！");
        }
         */
    }
}
