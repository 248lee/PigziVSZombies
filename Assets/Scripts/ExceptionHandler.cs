using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
            Debug.Log(ex);
            if (ex.Message.Contains("TooManyRequests"))
                return "出錯的單字: " + ex.Data["word"] + "\n對GPT4的request過於頻繁! (這裡要指示玩家刪掉StreamAssets裡面的SentenceBank裡面的單字)\n如果不停重複此錯誤，請點擊「清除例句快取」按鈕。";
            return "\"HttpRequestException\" 您可能沒有連上網際網路。";
        }
        else
        {
            Debug.LogError(ex);
            string error_message = "出錯的單字: " + ex.Data["word"] + "\n這屬於其他未知的錯誤。請將xxx_Data/StreamingAssets/logs/error_log.txt中的錯誤訊息複製並聯系fantasy10final@gmail.com\n" + ex.ToString();
            // Create the file for the error message
            string filename = "error_log.txt";
            string pathname = Application.streamingAssetsPath + "/logs" + "/" + filename;
            if (!File.Exists(pathname))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath + "/logs/");
            }
            // Append the error message to the file, with date and time
            File.AppendAllText(pathname,"\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n" + error_message + "\n-------------------------------\n");
            return error_message;
        }
    }

}
