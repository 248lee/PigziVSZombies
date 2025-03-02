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
            return "\"Request Timeout\" �o�i��OOpenAI���A�����䪺���D�C�еy���X�b�p�ɫ�(�ھڥH�e���g��)�A����";
        }
        else if (ex is TaskCanceledException)
        {
            return "\"Task is canceled here!\" �o�i��OOpenAI���A�����䪺���D�C�еy���X�b�p�ɫ�(�ھڥH�e���g��)�A����";
        }
        else if (ex is System.Net.Http.HttpRequestException)
        {
            return "\"HttpRequestException\" �z�i��S���s�W���ں����C";
        }
        else
        {
            Debug.LogError(ex);
            return "\"" + ex.ToString() + "\" �o�ݩ��L���������~�C�бN���~�T���I�Ϩ��p�tfantasy10final@gmail.com";
        }
    }

}
