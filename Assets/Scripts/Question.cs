using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Question
{
    public string answer;
    public string sentence;
    public Question(string vocabulary, string sentence)
    {
        this.answer = vocabulary;
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
