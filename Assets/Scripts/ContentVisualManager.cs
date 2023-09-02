using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentVisualManager : MonoBehaviour
{
    public Canvas storage;
    public TopicBox topicBox;
    public string visualizerTarget;

    private float boxYInterval = 5; 
    private float boxXInterval = 7; 

    void Start()
    {
        //JsonConverter.GPTResult sample = JsonConverter.TestAsSample();
        //ResultToUI(sample);
    }

    public void VisualizeFromServer()
    {
        var rs = JsonConverter.Convert(visualizerTarget);
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
            title.BottomToTop(ideaBox).Forget();
            ideaBox.SetText(ideas[i].SummaryOfIdea);
        }
    }

    private TopicBox GenerateTopicBox(Vector3 pos)
    {
        var tb = Instantiate(topicBox, storage.transform);
        tb.transform.position = pos;

        return tb;
    }
}
