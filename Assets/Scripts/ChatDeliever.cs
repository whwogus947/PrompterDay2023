using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace OpenAI
{
    public class ChatDeliever : MonoBehaviour
    {
        public static ChatDeliever Inst { get; private set; }

        [TextArea(5, 10)]public string baseDirection;
        [TextArea(5, 10)]public string jsonDirection;

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

            string users = ChatRoomMaker.roomUsers.Trim(' ', ',');
            string baseMsg = $"회의에는 [{users}]이 참여했어. \n" + baseDirection;
            Debug.Log(baseMsg);
            await SendReply(baseMsg + msg);

            string mainMsg = $"회의에는 [{users}]이 참여했어. \n" + jsonDirection;
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
                Messages = messages,
                Temperature = 0.9f,
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
