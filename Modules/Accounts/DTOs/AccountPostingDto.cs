using Accounts.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Accounts.DTOs
{
    public class AccountPostingDto
    {
        [Required(ErrorMessage = "Account Number can not leave blank.")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "Entry Type (Debit or Credit) is required.")]
        public EntryType Type { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be a positive value.")]
        public double Amount { get; set; }

        [Required(ErrorMessage = "Transaction ID is required.")]
        public Guid TransactionId { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        public string Description { get; set; }
    }
}
