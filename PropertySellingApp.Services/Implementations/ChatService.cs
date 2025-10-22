using Microsoft.Extensions.Configuration;
using PropertySellingApp.DataAccess.Interfaces;
using PropertySellingApp.Models.DTOs;
using PropertySellingApp.Models.Entities;
using PropertySellingApp.Services.Interfaces;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace PropertySellingApp.Services.Implementations
{
    public class ChatService : IChatService
    {
        private readonly IConfiguration _config;
        private readonly IChatRepository _chatRepository;
        private readonly string _groqApiKey;

        public ChatService(IConfiguration config, IChatRepository chatRepository)
        {
            _config = config;
            _chatRepository = chatRepository;
            _groqApiKey = _config["Groq:ChatBot"];
        }

        public async Task<ChatResponseDto> GetGroqReplyAsync(ChatRequestDto request)
        {
            var client = new RestClient("https://api.groq.com/openai/v1/chat/completions");
            var apiRequest = new RestRequest { Method = Method.Post };
            apiRequest.AddHeader("Authorization", $"Bearer {_groqApiKey}");
            apiRequest.AddHeader("Content-Type", "application/json");

            var body = new
            {
               // model = "llama3-8b-8192",
                model = "llama-3.1-8b-instant",
                messages = new object[]
                {
                    new { role = "system", content = "You are a helpful AI assistant integrated in .NET 6." },
                    new { role = "user", content = request.Message }
                }
            };

            apiRequest.AddJsonBody(body);
            var response = await client.ExecuteAsync(apiRequest);

            if (!response.IsSuccessful)
                throw new Exception($"Groq API call failed: {response.Content}");

            var json = JObject.Parse(response.Content);
            var reply = json["choices"]?[0]?["message"]?["content"]?.ToString();

            var result = new ChatResponseDto { Reply = reply ?? "No response from Groq AI." };

            // Save chat history in DB
            await _chatRepository.SaveChatAsync(new ChatHistory
            {
                UserMessage = request.Message,
                BotReply = result.Reply
            });

            return result;
        }
    }
}
