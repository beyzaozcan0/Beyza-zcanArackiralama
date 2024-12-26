using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using AracKiralama.Repositories;
using AracKiralama.Models;

public class MessageRepository : GenericRepository<Message>
{
    private readonly AppDbContext _context;

    public MessageRepository(AppDbContext context) : base(context, context.Messages)
    {
        _context = context;
    }

    public async Task<List<Message>> GetConversation(string user1Id, string user2Id)
    {
        return await _context.Messages
            .Where(m => (m.SenderId == user1Id && m.ReceiverId == user2Id) ||
                       (m.SenderId == user2Id && m.ReceiverId == user1Id))
            .OrderBy(m => m.Timestamp)
            .ToListAsync();
    }

    public async Task<List<Message>> GetUnreadMessages(string userId)
    {
        return await _context.Messages
            .Where(m => m.ReceiverId == userId && !m.IsRead)
            .ToListAsync();
    }
} 