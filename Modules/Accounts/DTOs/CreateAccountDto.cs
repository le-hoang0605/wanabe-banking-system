using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Accounts.DTOs
{
    public class CreateAccountDto
    {
        [Required(ErrorMessage = "PartyId is required.")]
        public Guid PartyId { get; set;  }

        [Range(0, double.MaxValue, ErrorMessage = "InitialBalance must be a non-negative value.")]
        public double InitialBalance { get; set; }

        public string Currency { get; set; } = "VND";

        public bool IsManager { get; set; } = false;
    }
}
