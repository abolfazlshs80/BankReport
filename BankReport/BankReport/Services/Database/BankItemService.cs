using BankReport.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankReport.Services.Database
{
    public class BankItemService : IBankItemService
    {
        private DatabaseService dbDatabaseService;

        public BankItemService()
        {
            dbDatabaseService = new DatabaseService();
        }

        public void InsertBankItem(BankItem BankItem) => dbDatabaseService.db.Insert(BankItem);

        public List<BankItem> GetBankItem() => dbDatabaseService.db.Table<BankItem>().ToList();
        public async Task CreateBankItem(BankItem BankItem)
        {
            dbDatabaseService.db.Insert(BankItem);
        }

        public async Task<int> DeleteBankItem(BankItem BankItem)
        {
            return dbDatabaseService.db.Delete(BankItem);
        }

        public async Task<int> UpdateBankItem(BankItem BankItem)
        {
            return dbDatabaseService.db.Update(BankItem);
        }

        public async Task<List<BankItem>> GetBankItems()
        {
            return dbDatabaseService.db.Table<BankItem>().ToList();
        }

        public async Task<List<BankItem>> FindBankItems(int Id)
        {
            return dbDatabaseService.db.Table<BankItem>().Where(a => a.Id.Equals(Id)).ToList();
        }

        public async Task<BankItem> FindBankItem(int Id)
        {
            var tables = dbDatabaseService.db.Table<BankItem>();
            return tables.Where(a => a.Id==Id).FirstOrDefault();
        }

        public async Task<BankItem> FindBankItem(BankItem BankItem)
        {
            return dbDatabaseService.db.Table<BankItem>().Where(a => a.Id.Equals(BankItem.Id)).FirstOrDefault();
        }
    }

    public interface IBankItemService
    {
        Task CreateBankItem(BankItem BankItem);
        Task<int> DeleteBankItem(BankItem BankItem);
        Task<int> UpdateBankItem(BankItem BankItem);
        Task<List<BankItem>> GetBankItems();
        Task<List<BankItem>> FindBankItems(int Id);
        Task<BankItem> FindBankItem(int Id);
        Task<BankItem> FindBankItem(BankItem BankItem);
    }
}
