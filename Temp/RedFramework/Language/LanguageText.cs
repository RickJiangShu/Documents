using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;


/**
 * 挂载在 UGUI.Text 文本上的脚本，只需设置 id 自动给Text 赋值
 * 
 * 建议所有有文本的地方都挂载这个脚本，因为涉及之后的语言更改时的动态切换。
 */
public class LanguageText : MonoBehaviour
{
    //语言包
    public string langID;
    public string paramter1;
    public string paramter2;
    public string paramter3;

    [HideInInspector]
    public string text;//最终显示的文本

    [HideInInspector]
    public Text m_Text;
    // Use this for initialization
    void Start()
    {
        m_Text = GetComponent<Text>();
        if (m_Text == null)
        {
            Debug.LogWarning("LetterSpacing: Missing Text component");
            return;
        }

        if (!string.IsNullOrEmpty(text))//文字已赋值
            Redraw();
        else
            SetLang(langID,paramter1,paramter2,paramter3);

        Language.onChanged += OnLangChanged;
    }

    public void OnDestroy()
    {
        Language.onChanged -= OnLangChanged;
    }


    //保证同步设置
    public void SetLang(string id, string para1 = "", string para2 = "", string para3 = "")
    {
        if (string.IsNullOrEmpty(id)) return;

        this.langID = id;
        paramter1 = para1;
        paramter2 = para2;
        paramter3 = para3;

        string langTxt = Language.GetLocal(id);
        if (para1 != "")
            text = string.Format(langTxt, para1, para2, para3);
        else
            text = langTxt;

        if (m_Text != null)
            Redraw();
    }



    public void SetText(string text)
    {
        this.text = text;

        if (m_Text != null)
            Redraw();
    }
    private void Redraw()
    {
        m_Text.text = text;
    }

    public void Clear()
    {
        langID = "";
        paramter1 = "";
        paramter2 = "";
        paramter3 = "";
        text = "";
        if (m_Text != null)
            Redraw();
    }

    private void OnLangChanged()
    {
        SetLang(langID, paramter1, paramter2, paramter3);
    }

}
