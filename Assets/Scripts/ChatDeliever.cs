using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace OpenAI
{
    public class ChatDeliever : MonoBehaviour
    {
        public static ChatDeliever Inst { get; private set; }

        private OpenAIApi openai = new OpenAIApi("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");
        private List<ChatMessage> messages = new List<ChatMessage>();
        private bool isWaitingRecieveMessage = false;

        private void Awake()
        {
            if (Inst == null)
            {
                Inst = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            isWaitingRecieveMessage = false;

            //Send("카페인 과다 섭취에 대해 설명해줘.");
        }

        public async UniTask<string> Send(string msg)
        {
            if (isWaitingRecieveMessage)
                return null;

            string baseMsg = "다음 회의 내용을 요약해줘. 회의 주제를 명시하고, 제시된 내용을 아이디어별로 발언자와 묶어서 번호를 붙여 정리한 뒤, 채택된 아이디어의 번호와 결론, 추후 논의 사항을 알려주면 돼.\n\n";
            await SendReply(baseMsg + msg);

            string mainMsg = "다음의 정리된 회의 내용들을 json 데이터 코드로 바꾸어줘. Comments는 각 아이디어에 다른 사람들이 덧붙인 의견을 담은 데이터야. 제공된 정보가 없으면 null 값으로 처리해도 돼. 결론은 30자 내외로 간단하게 요약하고, 추후 논의사항은 구체적으로 입력해줘. 문장이 길어지거나 줄바꿈이 필요하면 여러 개의 attribute를 사용해도 좋아.\r\n\r\njson 구조:\r\n{\r\n    \"MeetingTopic\": 회의 주제,\r\n    \"MeetingAgenda\": [\r\n      {\r\n        \"NumberOfIdea\": \"아이디어 번호 (int)\",\r\n        \"SummaryOfIdea\": \"아이디어\",\r\n        \"Speaker\": \"발언자\",\r\n        \"ContentsOfIdea\": \"아이디어 내용”,\r\n“Comments”: [\r\n{\r\n“Speaker”: “발언자”,\r\n“ContentsOfComment”: “코멘트 내용”,\r\n“IsPositive”: “해당 아이디어에 긍정적인 의견인가”\r\n}\r\n]\r\n      }\r\n    ],\r\n    \"SelctedIdea\": \"채택된 아이디어 번호\" (여러 attribute 사용 가능)\r\n    \"Conclusion\": \"결론\",\r\n    \"TopicForFurtherDiscussion\": \"추후 논의 사항\"\r\n}";
            return await SendReply(mainMsg + msg);
        }

        private void ResultMessage(string message)
        {
            Debug.Log("RESULT\n\n" + message);
        }

        private async UniTask<string> SendReply(string msg)
        {
            isWaitingRecieveMessage = true;
            string val = "";

            messages = new();
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = msg
            };

            messages.Add(newMessage);

            // Complete the instruction
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-3.5-turbo-0613",
                Messages = messages
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();

                messages.Add(message);
                val = message.Content;
                ResultMessage(message.Content);
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }
            isWaitingRecieveMessage = false;

            return val;
        }
    }
}
