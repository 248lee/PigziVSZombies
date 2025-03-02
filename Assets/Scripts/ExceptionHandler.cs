using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExceptionHandler
{
    public static string GetFriendlyMessage(System.Exception ex)
    {
        // If we have an AggregateException, flatten it and pick the first inner exception
        if (ex is AggregateException aggregateEx)
        {
            aggregateEx = aggregateEx.Flatten();
            if (aggregateEx.InnerExceptions.Count > 0)
            {
                return GetFriendlyMessage(aggregateEx.InnerExceptions[0]);
            }
            return aggregateEx.Message;
        }
        else if (ex is TimeoutException)
        {
            return "\"Request Timeout\" 這可能是OpenAI伺服器那邊的問題。請稍等幾半小時後(根據以前的經驗)再重試";
        }
        else if (ex is TaskCanceledException)
        {
            return "\"Task is canceled here!\" 這可能是OpenAI伺服器那邊的問題。請稍等幾半小時後(根據以前的經驗)再重試";
        }
        else if (ex is System.Net.Http.HttpRequestException)
        {
            return "\"HttpRequestException\" 您可能沒有連上網際網路。";
        }
        else
        {
            Debug.LogError(ex);
            return "\"" + ex.ToString() + "\" 這屬於其他未知的錯誤。請將錯誤訊息截圖並聯系fantasy10final@gmail.com";
        }
    }

}
