using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace GameLibrary.Data
{
    public class DeveloperRepo : IDeveloperRepo
    {
        private readonly ApplicationDbContext _context;

        public DeveloperRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Developer> GetDeveloper(int? id)
        {
            return await _context.Developers.Include(d => d.Games).FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<Developer>> GetDevelopers()
        {
            return await _context.Developers.Include(d => d.Games).ToListAsync();
        }

        public IEnumerable<Developer> GetDevelopersSync()
        {
            return _context.Developers.Include(d => d.Games).ToList();
        }

        public async Task<Game> GetGame(int? id)
        {
            return await _context.Games.Include(g => g.Developer).FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<IEnumerable<Game>> GetGames()
        {
            return await _context.Games.Include(g => g.Developer).ToListAsync();
        }

        public IEnumerable<Game> GetGamesSync()
        {
            return _context.Games.Include(g => g.Developer).ToList();
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
