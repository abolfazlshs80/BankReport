namespace BankReport.Services
{
    /// <summary>
    /// Defines the types of banking transactions.
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// The transaction type is unknown or could not be determined.
        /// </summary>
        Unknown,
        /// <summary>
        /// A debit transaction (money withdrawn or spent).
        /// </summary>
        Debit,
        /// <summary>
        /// A credit transaction (money deposited).
        /// </summary>
        Credit,
        /// <summary>
        /// An inquiry about the account balance.
        /// </summary>
        BalanceInquiry
    }
}
