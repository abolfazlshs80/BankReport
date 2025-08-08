using System.IO;
using BankReport.Models;
using BankReport.Models.Database;
using SQLite;
using Xamarin.Essentials;
namespace BankReport.Services.Database
{
   


    public class DatabaseService
    {
        public SQLiteConnection db;

        public DatabaseService()
        {
            string path = Path.Combine(FileSystem.AppDataDirectory, "mydb.db3");

            //if (File.Exists(path))
            //{
            //    File.Delete(path);
            //}
            db = new SQLiteConnection(path);
            db.CreateTable<BankTransaction>();
            db.CreateTable<BankItem>();
            db.CreateTable<User>();
        }

        //public void InsertUser(BankItem BankItem) => db.Insert(BankItem);

        //public List<BankItem> GetUsers() => db.Table<BankItem>().ToList();
    }

}
