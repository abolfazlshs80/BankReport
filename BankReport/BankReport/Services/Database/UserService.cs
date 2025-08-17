using BankReport.Models.Database;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BankReport.Services.Database
{
    public class UserService : IUserService
    {
        private DatabaseService dbDatabaseService;

        public UserService()
        {
            dbDatabaseService = new DatabaseService();
        }

        public void InsertUser(User User) => dbDatabaseService.db.Insert(User);

        public List<User> GetUser() => dbDatabaseService.db.Table<User>().ToList();
        public async Task CreateUser(User User)
        {
            dbDatabaseService.db.Insert(User);
        }
        public void CreateUser1(User User)
        {
            dbDatabaseService.db.Insert(User);
        }
        public async Task<int> DeleteUser(User User)
        {
            return dbDatabaseService.db.Delete(User);
        }

        public async Task<int> UpdateUser(User User)
        {
            return dbDatabaseService.db.Update(User);
        }

        public async Task<List<User>> GetUsers()
        {
            return dbDatabaseService.db.Table<User>().ToList();
        }

        public async Task<List<User>> FindUsers(int Id)
        {
            return dbDatabaseService.db.Table<User>().Where(a => a.Id.Equals(Id)).ToList();
        }

        public async Task<User> FindUser(int Id)
        {
            var tables = dbDatabaseService.db.Table<User>();
            return tables.Where(a => a.Id==Id).FirstOrDefault();
        }
        public async Task<User> firstUser()
        {
            var tables = dbDatabaseService.db.Table<User>();
            return tables.FirstOrDefault();
        }

        public async Task<User> FindUser(User User)
        {
            return dbDatabaseService.db.Table<User>().Where(a => a.Id.Equals(User.Id)).FirstOrDefault();
        }
    }

    public interface IUserService
    {
        Task CreateUser(User User);
        void CreateUser1(User User);
        Task<int> DeleteUser(User User);
        Task<int> UpdateUser(User User);
        Task<List<User>> GetUsers();
        Task<List<User>> FindUsers(int Id);
        Task<User> FindUser(int Id);
        Task<User> firstUser();
        Task<User> FindUser(User User);
    }
}
