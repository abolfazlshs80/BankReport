using System;
using System.Collections.Generic;
using System.Text;

namespace BankReport.Models.Temp
{
    public static class Cloner
    {
        public static object Value { get; set; }
        public static Dictionary<int, object> Messages { get; set; } = new Dictionary<int, object>();
    }
    public static class ClonerNottificationId
    {
        public static int Value { get; set; }
    }

    

}
