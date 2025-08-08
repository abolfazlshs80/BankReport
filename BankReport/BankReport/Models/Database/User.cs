using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankReport.Models.Database
{
    public class User
    {
        public string UserName { get; set; } = null;
        public string Password { get; set; } = null;
        


        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
  
    }
}
