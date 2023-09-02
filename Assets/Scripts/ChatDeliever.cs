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

            //Send("ī���� ���� ���뿡 ���� ��������.");
        }

        public async UniTask<string> Send(string msg)
        {
            if (isWaitingRecieveMessage)
                return null;

            string baseMsg = "���� ȸ�� ������ �������. ȸ�� ������ ����ϰ�, ���õ� ������ ���̵��� �߾��ڿ� ��� ��ȣ�� �ٿ� ������ ��, ä�õ� ���̵���� ��ȣ�� ���, ���� ���� ������ �˷��ָ� ��.\n\n";
            await SendReply(baseMsg + msg);

            string mainMsg = "������ ������ ȸ�� ������� json ������ �ڵ�� �ٲپ���. Comments�� �� ���̵� �ٸ� ������� ������ �ǰ��� ���� �����;�. ������ ������ ������ null ������ ó���ص� ��. ����� 30�� ���ܷ� �����ϰ� ����ϰ�, ���� ���ǻ����� ��ü������ �Է�����. ������ ������ų� �ٹٲ��� �ʿ��ϸ� ���� ���� attribute�� ����ص� ����.\r\n\r\njson ����:\r\n{\r\n    \"MeetingTopic\": ȸ�� ����,\r\n    \"MeetingAgenda\": [\r\n      {\r\n        \"NumberOfIdea\": \"���̵�� ��ȣ (int)\",\r\n        \"SummaryOfIdea\": \"���̵��\",\r\n        \"Speaker\": \"�߾���\",\r\n        \"ContentsOfIdea\": \"���̵�� ���롱,\r\n��Comments��: [\r\n{\r\n��Speaker��: ���߾��ڡ�,\r\n��ContentsOfComment��: ���ڸ�Ʈ ���롱,\r\n��IsPositive��: ���ش� ���̵� �������� �ǰ��ΰ���\r\n}\r\n]\r\n      }\r\n    ],\r\n    \"SelctedIdea\": \"ä�õ� ���̵�� ��ȣ\" (���� attribute ��� ����)\r\n    \"Conclusion\": \"���\",\r\n    \"TopicForFurtherDiscussion\": \"���� ���� ����\"\r\n}";
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
