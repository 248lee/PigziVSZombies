using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class Question
{
    public string vocabulary;
    public string sentence;
    public Question(string vocabulary, string sentence)
    {
        this.vocabulary = vocabulary;
        this.sentence = sentence;
    }
    public void setAns()
    {
        //switch (this.operater)
        //{
        //    case "+":
        //        this.answer = this.operand_1 + this.operand_2;
        //        break;
        //    case "-":
        //        this.answer = this.operand_1 - this.operand_2;
        //        break;
        //    case "X":
        //        this.answer = this.operand_1 * this.operand_2;
        //        break;
        //    default:
        //        Debug.LogError("你的運算子打錯了啦傻B");
        //        break;
        //}
    }
}
