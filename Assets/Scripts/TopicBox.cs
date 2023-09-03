using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TopicBox : MonoBehaviour
{
    public int topicIndex;
    public Transform top;
    public Transform bottom;
    public TMP_Text message;
    public GraphVisualizer visualizer;

    void Start()
    {

    }

    public void OnClickTopicBox()
    {
        ContentVisualManager.Inst.ShowDetailedMessage(topicIndex);
    }

    public void SetText(string _text)
    {
        message.text = _text;
    }

    public async UniTask BottomToTop(TopicBox target)
    {
        gameObject.SetActive(true);
        await BridgeTwoPoints(bottom.position, target.top.position);
        target.gameObject.SetActive(true);
        ContentVisualManager.ClearedCount++;
    }

    public async UniTask TopToBottom(TopicBox target)
    {
        await BridgeTwoPoints(top.position, target.bottom.position);
    }

    private async UniTask BridgeTwoPoints(Vector3 start, Vector3 end)
    {
        float x = end.x - start.x;
        float y = end.y - start.y;

        //Vector3 half1 = start + Vector3.up * y;
        //Vector3 half2 = half1 + Vector3.right * x / 2f;
        Vector3 half1 = start + Vector3.right * x / 2f;
        Vector3 half2 = half1 + Vector3.up * y;

        visualizer = Instantiate(visualizer);
        await visualizer.StartDrawing(start, half1, half2, end);
    }
}
