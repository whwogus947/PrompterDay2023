using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AgendaComment : MonoBehaviour
{
    public Image bg;
    public TMP_Text speakerText;
    public TMP_Text commentText;

    void Start()
    {

    }

    public void SetDetail(string speaker, string comment, bool isPositive)
    {
        var img = GetComponent<Image>();
        img.sprite = isPositive == true ? ContentVisualManager.Inst.sense[0] : ContentVisualManager.Inst.sense[1];
        speakerText.text = speaker;
        commentText.text = comment;
    }
}
