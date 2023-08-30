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

            Send("카페인 과다 섭취에 대해 설명해줘.");
        }

        public void Send(string msg)
        {
            if (isWaitingRecieveMessage)
                return;

            SendReply(msg).Forget();
        }

        private void ResultMessage(string message)
        {
            Debug.Log("RESULT\n\n" + message);
        }

        private async UniTaskVoid SendReply(string msg)
        {
            isWaitingRecieveMessage = true;

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
                ResultMessage(message.Content);
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }
            isWaitingRecieveMessage = false;
        }
    }
}
