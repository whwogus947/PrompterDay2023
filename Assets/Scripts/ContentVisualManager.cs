using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;

public class ContentVisualManager : MonoBehaviour
{
    public static ContentVisualManager Inst { get; private set; }
    public Canvas storage;
    public TopicBox topicBox;
    public string visualizerTarget;
    public TMP_Text summaryText;
    public GameObject detailBox;
    public TMP_Text detailedAgendaSummary;
    public TMP_Text detailedAgendaFull;
    public TMP_Text detailedAgendaSpeaker;
    public AgendaComment agendaComment;
    public Transform commentsStorage;
    public Sprite[] sense;

    [Header("Visualize Boxes")]
    public Sprite titleImage;
    public Sprite agendaImage;

    private float boxYInterval = 5; 
    private float boxXInterval = 3.5f;
    private JsonConverter.GPTResult jsonResult = new();

    void Awake()
    {
        if (Inst == null)
            Inst = this;

        //JsonConverter.GPTResult sample = JsonConverter.TestAsSample();
        //jsonResult= sample;
        //ResultToUI(sample);
    }

    public void Summarize()
    {
        var rs = JsonConverter.Convert(visualizerTarget);
        jsonResult = rs;
        string result = $"회의 주제 : {rs.MeetingTopic}\n\n 회의 결론 : {rs.Conclusion}";
        summaryText.text = result;
    }

    public void VisualizeFromServer()
    {
        var rs = JsonConverter.Convert(visualizerTarget);
        jsonResult = rs;
        ResultToUI(rs);
    }

    public void ResultToUI(JsonConverter.GPTResult result)
    {
        var startPosition = transform.position;
        var title = GenerateTopicBox(startPosition);
        title.SetText(result.MeetingTopic);
        title.gameObject.GetComponent<Image>().sprite = titleImage;
        title.gameObject.GetComponent<Button>().interactable = false;

        var ideas = result.MeetingAgenda;
        float leftEnd = -boxXInterval * (ideas.Length - 1) / 2f;

        for (var i = 0; i < ideas.Length; i++)
        {
            var pos = (leftEnd + i * boxXInterval) * Vector3.up + startPosition + boxYInterval * Vector3.right;
            var ideaBox = GenerateTopicBox(pos);
            ideaBox.topicIndex = i;
            title.BottomToTop(ideaBox).Forget();
            ideaBox.SetText(ideas[i].SummaryOfIdea);
        }
    }

    public void ShowDetailedMessage(int agendaIndex)
    {
        detailBox.SetActive(true);

        detailedAgendaSummary.text = jsonResult.MeetingAgenda[agendaIndex].SummaryOfIdea;
        detailedAgendaFull.text = jsonResult.MeetingAgenda[agendaIndex].ContentsOfIdea;
        detailedAgendaSpeaker.text = jsonResult.MeetingAgenda[agendaIndex].Speaker;

        var comments = jsonResult.MeetingAgenda[agendaIndex].Comments;

        for (int i = 0; i < comments.Length; i++)
        {
            var commentBox = Instantiate(agendaComment, commentsStorage);
            commentBox.SetDetail(comments[i].Speaker, comments[i].ContentsOfComment, comments[i].IsPositive);
        }
    }

    public void ExitDetailedMessage()
    {
        ClearAllComments();
    }

    private void ClearAllComments()
    {
        for (int i = 1; i < commentsStorage.childCount; i++)
        {
            Destroy(commentsStorage.GetChild(i).gameObject);
        }
    }

    private TopicBox GenerateTopicBox(Vector3 pos)
    {
        var tb = Instantiate(topicBox, storage.transform);
        tb.transform.position = pos;

        return tb;
    }
}
