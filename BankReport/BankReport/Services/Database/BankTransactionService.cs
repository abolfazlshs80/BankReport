using BankReport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankReport.Extentions;

namespace BankReport.Services.Database
{
    public class BankTransactionService : IBankTransactionService
    {
        public readonly object dbLock = new object();
        private DatabaseService dbDatabaseService;

        public BankTransactionService()
        {
            dbDatabaseService = new DatabaseService();
        }

        public void InsertBankTransaction(BankTransaction BankTransaction) => dbDatabaseService.db.Insert(BankTransaction);

        public List<BankTransaction> GetBankTransaction() => dbDatabaseService.db.Table<BankTransaction>().ToList();
        public async Task CreateBankTransaction(BankTransaction BankTransaction)
        {
            lock (dbLock)  // اگر Lock برای دیتابیس داری
            {
                dbDatabaseService.db.Insert(BankTransaction);
            }
    //        dbDatabaseService.db.Insert(BankTransaction);
        }

        public async Task<int> DeleteBankTransaction(BankTransaction BankTransaction)
        {
            return dbDatabaseService.db.Delete(BankTransaction);
        }

        public async Task<int> UpdateBankTransaction(BankTransaction BankTransaction)
        {
            return dbDatabaseService.db.Update(BankTransaction);
        }

        public async Task<List<BankTransaction>> GetBankTransactions()
        {
            return dbDatabaseService.db.Table<BankTransaction>().ToList();
        }

        public async Task<List<BankTransaction>> FindBankTransactions(int Id)
        {
            return dbDatabaseService.db.Table<BankTransaction>().Where(a => a.Id.Equals(Id)).ToList();
        }

        public async Task<BankTransaction> FindBankTransaction(int Id)
        {
            var tables = dbDatabaseService.db.Table<BankTransaction>();
            return tables.Where(a => a.Id == Id).FirstOrDefault();
        }
        public async Task<List<BankTransactionResult>> FindBankTransactionResult(int Id)
        {
            var tablesBankTransaction = dbDatabaseService.db.Table<BankTransaction>();
            var tablesBankItem = dbDatabaseService.db.Table<BankItem>();
            var bankItem = tablesBankItem.FirstOrDefault(_ => _.Id.Equals(Id));
            if (bankItem != null)
            {
                var transactions = tablesBankTransaction.Where(a => a.BankName.Contains(bankItem.Name)).ToList();
                var transactions1 = tablesBankTransaction.ToList();
                if (transactions.Any())
                {
                    var currentDate1 = DateTime.Now.GetMonthToShamsi();
                    var currentDate = DateTime.Now;
                    int row=1;
                    var bankTransactionResult = transactions.Select(_ => new BankTransactionResult
                    {
                        Row=row++,
                        Amout = _.Amount,
                        BankName = _.BankName,
                        CardNumber = _.AccountId,
                        Type=_.Amount>0?"واریز":"برداشت",
                        Desc =_.Description,
                        Date=_.TransactionDate.ToShamsi(),
                        //  MonyOutOfDay =
                        //  _.Amount < 0 &&
                        //  _.TransactionDate.GetDayToShamsi() == currentDate.GetDayToShamsi()
                        //  && _.TransactionDate.GetMonthToShamsi() == currentDate.GetMonthToShamsi()
                        //  && _.TransactionDate.GetYearToShamsi() == currentDate.GetYearToShamsi()).Sum(_ => _.Amount)
                        //  ,
                        //  MonyComOfDay = transactions.Where(_ =>
                        //_.Amount > 0 &&
                        //_.TransactionDate.GetDayToShamsi() == currentDate.GetDayToShamsi()
                        //&& _.TransactionDate.GetMonthToShamsi() == currentDate.GetMonthToShamsi()
                        //&& _.TransactionDate.GetYearToShamsi() == currentDate.GetYearToShamsi()).Sum(_ => _.Amount)
                        //  ,
                        //  MonyOutOfMonth = transactions.Where(_ =>
                        //  _.Amount < 0 &&
                        //   _.TransactionDate.GetMonthToShamsi() == currentDate.GetMonthToShamsi()
                        //  && _.TransactionDate.GetYearToShamsi() == currentDate.GetYearToShamsi()).Sum(_ => _.Amount)
                        //  ,
                        //  MonyComOfMonth = transactions.Where(_ =>
                        //  _.Amount > 0 &&
                        //  _.TransactionDate.GetDayToShamsi() == currentDate.GetDayToShamsi()
                        //  && _.TransactionDate.GetMonthToShamsi() == currentDate.GetMonthToShamsi()
                        //  && _.TransactionDate.GetYearToShamsi() == currentDate.GetYearToShamsi()).Sum(_ => _.Amount)

                    }).ToList();
                    return bankTransactionResult;
                }
            }
               
            return null;
        }

        public async Task<BankTransaction> FindBankTransaction(BankTransaction BankTransaction)
        {
            return dbDatabaseService.db.Table<BankTransaction>().Where(a => a.Id.Equals(BankTransaction.Id)).FirstOrDefault();
        }
    }

    public interface IBankTransactionService
    {
        Task<List<BankTransactionResult>> FindBankTransactionResult(int Id);
        Task CreateBankTransaction(BankTransaction BankTransaction);
        Task<int> DeleteBankTransaction(BankTransaction BankTransaction);
        Task<int> UpdateBankTransaction(BankTransaction BankTransaction);
        Task<List<BankTransaction>> GetBankTransactions();
        Task<List<BankTransaction>> FindBankTransactions(int Id);
        Task<BankTransaction> FindBankTransaction(int Id);
        Task<BankTransaction> FindBankTransaction(BankTransaction BankTransaction);
    }
}
