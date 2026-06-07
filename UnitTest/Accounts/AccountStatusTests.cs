using Accounts.Features.ChangeAccountStatus;
using Accounts.Models;
using Accounts.Models.Context;
using Microsoft.Identity.Client;
using System.Data;

namespace UnitTest.Accounts
{
    public class AccountStatusTests
    {
        
        Account newAccount = new Account
        {
            AccountId = Guid.NewGuid(),
            PartyId = Guid.NewGuid(),
            AccountNumber = "123",
            Balance = 1000,
            Currency = "VND",
            Role = Role.User,
            CreatedAt = DateTime.Now,
            Status = Status.Active
        };

        [Fact]
        public async Task ExecuteAsync_ShouldReturnFailure_WhenAccountDoesNotExist()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();

            context.Accounts.Add(newAccount);
            await context.SaveChangesAsync();

            var service = new ChangeAccountStatusService(context);
            var newStatus = new ChangeAccountStatusRequestDto(Status.Suspended);

            // Act
            var result = await service.ExecuteAsync("111123", newStatus);

            // Assert
            Assert.False(result.IsSuccess);
            //Assert.Equal("Suspended", result.Data.CurrentStatus.ToString());
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnSuccess_WhenAccountDoesExist()
        {
            // Arrange
            using var context = TestDbContextFactory.Create();
            var newAccount = new Account
            {
                AccountId = Guid.NewGuid(),
                PartyId = Guid.NewGuid(),
                AccountNumber = "123",
                Balance = 1000,
                Currency = "VND",
                Role = Role.User,
                CreatedAt = DateTime.Now,
                Status = Status.Active
            };

            context.Accounts.Add(newAccount);
            await context.SaveChangesAsync();

            var service = new ChangeAccountStatusService(context);
            var newStatus = new ChangeAccountStatusRequestDto(Status.Suspended);

            // Act
            var result = await service.ExecuteAsync(newAccount.AccountNumber, newStatus);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Suspended", result.Data.CurrentStatus.ToString());
        }
    }
}
