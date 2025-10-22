using PropertySellingApp.DataAccess.Interfaces;
using PropertySellingApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertySellingApp.DataAccess.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly AppDbContext _context;

        public ChatRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task SaveChatAsync(ChatHistory chat)
        {
            _context.ChatHistories.Add(chat);
            await _context.SaveChangesAsync();
        }
    }
}
