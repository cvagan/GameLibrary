using GameLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameLibrary.Data
{
    public interface IDeveloperRepo
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<IEnumerable<Developer>> GetDevelopers();
        IEnumerable<Developer> GetDevelopersSync();
        Task<Developer> GetDeveloper(int? id);
        Task<Game> GetGame(int? id);
        Task<IEnumerable<Game>> GetGames();
        IEnumerable<Game> GetGamesSync();
        Task<bool> SaveAll();
    }
}
