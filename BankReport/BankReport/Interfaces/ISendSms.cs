using System.Collections.Generic;

namespace BankReport.Interfaces
{
    public interface ISendSms
    {
        string GetSimCardNumberBySlot(int slotIndex);
        void SendSmsWithSlot(string number, string message, int simSlot);
        int GetSimCardNumbers();
        void Send(string address, string message);
        void SendAsync(string address, string message);
        string GetSimOperator();
        List<string> GetSimCards();
    }
}