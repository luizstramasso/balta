using Fina.Api.Data;
using Fina.Core.Common;
using Fina.Core.Enums;
using Fina.Core.Handlers;
using Fina.Core.Models;
using Fina.Core.Requests.Transactions;
using Fina.Core.Responses;
using Microsoft.EntityFrameworkCore;

namespace Fina.Api.Handlers;

// TODO: Aplicar um padrão de logging mais robusto, como Serilog ou NLog, para capturar e armazenar logs de forma estruturada e eficiente. Isso ajudará a identificar e diagnosticar problemas de forma mais eficaz, além de fornecer insights sobre o comportamento da aplicação em produção.
public class TransactionHandler(AppDataContext context) : ITransactionHandler
{
    public async Task<Response<Transaction?>> CreateAsync(CreateTransactionRequest request)
    {
        if (request is { Type: ETransactionType.Withdraw, Amount: >= 0 })
            request.Amount *= -1;

        var transaction = new Transaction
        {
            Title = request.Title,
            Amount = request.Amount,
            Type = request.Type,
            PaidOrReceivedAt = request.PaidOrReceivedAt,
            CategoryId = request.CategoryId,
            UserId = request.UserId
        };

        try
        {
            await context.Transactions.AddAsync(transaction);
            await context.SaveChangesAsync();

            return new Response<Transaction?>(transaction, 201, "Transaction created successfully.");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"Database update error: {ex.Message}");
            return new Response<Transaction?>(null, 500, "An error occurred while creating the transaction");
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            return new Response<Transaction?>(null, 500, "An error occurred while creating the transaction");
        }
    }

    public async Task<Response<Transaction?>> UpdateAsync(UpdateTransactionRequest request)
    {
        try
        {
            if (request is { Type: ETransactionType.Withdraw, Amount: >= 0 })
                request.Amount *= -1;

            var transaction = await context
                .Transactions
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

            if (transaction is null)
                return new Response<Transaction?>(null, 404, "Transaction not found.");

            transaction.CategoryId = request.CategoryId;
            transaction.Title = request.Title;
            transaction.Amount = request.Amount;
            transaction.Type = request.Type;
            transaction.PaidOrReceivedAt = request.PaidOrReceivedAt;

            context.Transactions.Update(transaction);
            await context.SaveChangesAsync();

            return new Response<Transaction?>(transaction, message: "Transaction updated successfully.");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"Database update error: {ex.Message}");
            return new Response<Transaction?>(null, 500, "An error occurred while updating the transaction");
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            return new Response<Transaction?>(null, 500, "An error occurred while updating the transaction");
        }
    }

    public async Task<Response<Transaction?>> DeleteAsync(DeleteTransactionRequest request)
    {
        try
        {
            var transaction = await context
                .Transactions
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

            if (transaction is null)
                return new Response<Transaction?>(null, 404, "Transaction not found.");

            context.Transactions.Remove(transaction);
            await context.SaveChangesAsync();

            return new Response<Transaction?>(transaction, message: "Transaction deleted successfully.");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"Database update error: {ex.Message}");
            return new Response<Transaction?>(null, 500, "An error occurred while deleting the transaction");
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            return new Response<Transaction?>(null, 500, "An error occurred while deleting the transaction");
        }
    }

    public async Task<Response<Transaction?>> GetByIdAsync(GetTransactionByIdRequest request)
    {
        try
        {
            var transaction = await context
                .Transactions
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

            if (transaction is null)
                return new Response<Transaction?>(null, 404, "Transaction not found.");

            return new Response<Transaction?>(transaction);
        }
        catch (DbUpdateException dbEx)
        {
            Console.WriteLine($"Database update error: {dbEx.Message}");
            return new Response<Transaction?>(null, 500, "An error occurred while retrieving the transaction.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            return new Response<Transaction?>(null, 500, "An error occurred while retrieving the transaction");
        }
    }

    public async Task<PagedResponse<List<Transaction>?>> GetByPeriodAsync(GetTransactionsByPeriodRequest request)
    {
        try
        {
            request.StartDate ??= DateTime.Now.GetFirstDayOfMonth();
            request.EndDate ??= DateTime.Now.GetLastDayOfMonth();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            return new PagedResponse<List<Transaction>?>(null, 500, "An error occurred while by invalid dates.");
        }

        try
        {
            var query = context
                .Transactions
                .AsNoTracking()
                .Where(x =>
                    x.UserId == request.UserId &&
                    x.PaidOrReceivedAt >= request.StartDate &&
                    x.PaidOrReceivedAt <= request.EndDate)
                .OrderByDescending(x => x.PaidOrReceivedAt);

            var transactions = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var count = await query.CountAsync();

            return new PagedResponse<List<Transaction>?>(
                transactions,
                count,
                request.PageNumber,
                request.PageSize);
        }
        catch (DbUpdateException dbEx)
        {
            Console.WriteLine($"Database update error: {dbEx.Message}");
            return new PagedResponse<List<Transaction>?>(null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            return new PagedResponse<List<Transaction>?>(null);
        }
    }
}