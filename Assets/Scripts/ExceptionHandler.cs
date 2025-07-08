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
            return "\"Request Timeout\" �o�i��OOpenAI���A�����䪺���D�C�еy���X�b�p�ɫ�(�ھڥH�e���g��)�A����";
        }
        else if (ex is TaskCanceledException)
        {
            return "\"Task is canceled here!\" �o�i��OOpenAI���A�����䪺���D�C�еy���X�b�p�ɫ�(�ھڥH�e���g��)�A����";
        }
        else if (ex is System.Net.Http.HttpRequestException)
        {
            Debug.Log(ex);
            if (ex.Message.Contains("TooManyRequests"))
                return "�X������r: " + ex.Data["word"] + "\n��GPT4��request�L���W�c! (�o�̭n���ܪ��a�R��StreamAssets�̭���SentenceBank�̭�����r)\n�p�G�������Ʀ����~�A���I���u�M���ҥy�֨��v���s�C";
            return "\"HttpRequestException\" �z�i��S���s�W���ں����C";
        }
        else
        {
            Debug.LogError(ex);
            string error_message = "�X������r: " + ex.Data["word"] + "\n�o�ݩ��L���������~�C�бNxxx_Data/StreamingAssets/logs/error_log.txt�������~�T���ƻs���p�tfantasy10final@gmail.com\n" + ex.ToString();
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
