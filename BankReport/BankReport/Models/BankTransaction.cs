using System;
using System.Globalization;

namespace BankReport.Services
{
    /// <summary>
    /// Represents a parsed banking transaction or message from an SMS.
    /// This is a Data Transfer Object (DTO) to hold extracted information.
    /// </summary>
    public class BankTransaction
    {

        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the name of the bank (e.g., "Bank Mellat", "Bank Melli", "Resalat Bank").
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// Gets or sets the type of transaction (e.g., Credit, Debit, BalanceInquiry).
        /// </summary>
        public TransactionType Type { get; set; }

        /// <summary>
        /// Gets or sets the amount of the transaction. For debit, it's the amount withdrawn. For credit, it's the amount deposited.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the balance of the account after the transaction, if available in the SMS.
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// Gets or sets the date and time of the transaction.
        /// This should be converted to a Gregorian DateTime object even if the SMS uses Persian dates.
        /// </summary>
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// Gets or sets the last few digits of the card number involved in the transaction, if available.
        /// </summary>
        public string CardNumberLastDigits { get; set; }

        /// <summary>
        /// Gets or sets the account ID or number involved in the transaction, if available.
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// Gets or sets any additional description or details provided in the SMS.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the raw, original SMS message for debugging or logging purposes.
        /// </summary>
        public string RawMessage { get; set; }

        /// <summary>
        /// Gets or sets the phone number from which the SMS was received.
        /// </summary>
        public string SenderPhoneNumber { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BankTransaction"/> class.
        /// Sets default values for properties.
        /// </summary>
        public BankTransaction()
        {
            BankName = "Unknown";
            Type = TransactionType.Unknown;
            Amount = 0m;
            Balance = 0m;
            TransactionDate = DateTime.MinValue; // Or DateTime.UtcNow
            CardNumberLastDigits = string.Empty;
            AccountId = string.Empty;
            Description = string.Empty;
            RawMessage = string.Empty;
            SenderPhoneNumber = string.Empty;
        }

        /// <summary>
        /// Provides a string representation of the BankTransaction object for easy debugging/logging.
        /// </summary>
        /// <returns>A string containing key transaction details.</returns>
        public override string ToString()
        {
            return $"Bank: {BankName}, Type: {Type}, Amount: {Amount.ToString("N0", CultureInfo.InvariantCulture)}, " +
                   $"Balance: {Balance.ToString("N0", CultureInfo.InvariantCulture)}, Date: {TransactionDate:yyyy/MM/dd HH:mm:ss}, " +
                   $"Card: {CardNumberLastDigits}, Account: {AccountId}, Desc: {Description}";
        }
    }
    public class BankTransactionResult
    {

        public string BankName { get; set; }
        public decimal Amout { get; set; }
        public decimal MonyOutOfDay { get; set; }
        public decimal MonyComOfDay { get; set; }
        public decimal MonyOutOfMonth { get; set; }
        public decimal MonyComOfMonth { get; set; }
  

    }

}
