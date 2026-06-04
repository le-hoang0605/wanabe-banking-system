using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Models
{
    public class AccountLedgerEntry
    {

        public Guid EntryId { get; set; }

        public string AccountNumber { get; set; } 

        public EntryType Type { get; set; } //Debit or Credit

        public double Amount { get; set; }

        public Guid TransactionId { get; set; } // Foreign key to the transaction that caused this ledger entry

        public string Description { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum EntryType
    {
        Debit, 
        Credit 
    }
}
