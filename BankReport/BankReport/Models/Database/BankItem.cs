using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankReport.Models
{
    public class BankItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }    
        public string CardNumber { get; set; }    
    }
}
