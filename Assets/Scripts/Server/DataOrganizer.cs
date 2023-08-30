using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataOrganizer
{
    public static List<string> participants = new List<string>();
    public static Dictionary<string, string> FieldsData = new Dictionary<string, string>()
    {
        { "Text", "" },
        { "Time", "" },
        { "UserName", "" },
    };
    
    public static void ClearAll()
    {
        participants.Clear();
        FieldsData.Clear();
    }
}
