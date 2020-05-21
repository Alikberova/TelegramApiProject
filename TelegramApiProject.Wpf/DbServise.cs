using Microsoft.EntityFrameworkCore;
using System.Linq;
using TelegramApiProject.Search;

namespace TelegramApiProject.Wpf
{
    public class DbServise
    {
        public readonly UserContext _context;
        public readonly UserSearchResult _userResult;

        public DbServise(UserContext context)
        {
            _context = context;
        }

        public void AddDefinedUsers(UserSearchResult userResult)
        {
            foreach (var user in userResult.Users)
            {
                if (!_context.SearchResult.Contains(user))
                {
                    _context.SearchResult.Add(user);
                }
            }
            _context?.SearchResult.Load();
            _context?.SearchResult.OrderBy(u => u.FirstName);
            _context.SaveChanges();
        }
    }
}
