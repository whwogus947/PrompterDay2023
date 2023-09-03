using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class ContentVisualManager : MonoBehaviour
{
    public static ContentVisualManager Inst { get; private set; }
    public Canvas storage;
    public TopicBox topicBox;
    public string visualizerTarget;
    public TMP_Text summaryText;
    public TMP_Text agendaText;
    public GameObject detailBox;

    private float boxYInterval = 5; 
    private float boxXInterval = 7;
    private JsonConverter.GPTResult jsonResult = new();

    void Start()
    {
        if (Inst == null)
            Inst = this;

        JsonConverter.GPTResult sample = JsonConverter.TestAsSample();
        ResultToUI(sample);
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

        var ideas = result.MeetingAgenda;
        float leftEnd = -boxXInterval * (ideas.Length - 1) / 2f;

        for (var i = 0; i < ideas.Length; i++)
        {
            var pos = (leftEnd + i * boxXInterval) * Vector3.right + startPosition + -boxYInterval * Vector3.up;
            var ideaBox = GenerateTopicBox(pos);
            ideaBox.topicIndex = i;
            title.BottomToTop(ideaBox).Forget();
            ideaBox.SetText(ideas[i].SummaryOfIdea);
        }
    }

    public void ShowDetailedMessage(int agendaIndex)
    {
        detailBox.SetActive(true);

        var comments = jsonResult.MeetingAgenda[agendaIndex].Comments;

        string commentsToText = "";
        for (int i = 0; i < comments.Length; i++)
        {
            var comment = comments[i];
            string person = "\n발언자 : " + comment.Speaker;
            string content = "\n\n내용 : " + comment.ContentsOfComment;
            commentsToText += person + content + "\n\n";
        }
        agendaText.text = commentsToText;
    }

    private TopicBox GenerateTopicBox(Vector3 pos)
    {
        var tb = Instantiate(topicBox, storage.transform);
        tb.transform.position = pos;

        return tb;
    }
}
