using Fina.Api.Data;
using Fina.Core.Handlers;
using Fina.Core.Models;
using Fina.Core.Requests.Categories;
using Fina.Core.Responses;
using Microsoft.EntityFrameworkCore;

namespace Fina.Api.Handlers;


// TODO: Aplicar um padrão de logging mais robusto, como Serilog ou NLog, para capturar e armazenar logs de forma estruturada e eficiente. Isso ajudará a identificar e diagnosticar problemas de forma mais eficaz, além de fornecer insights sobre o comportamento da aplicação em produção.
public class CategoryHandler(AppDataContext context) : ICategoryHandler
{
    public async Task<Response<Category?>> CreateAsync(CreateCategoryRequest request)
    {
        await Task.Delay(10000);
        var category = new Category
        {
            Title = request.Title,
            Description = request.Description,
            UserId = request.UserId
        };

        try
        {
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();

            return new Response<Category?>(category, 201, "Category created successfully.");
        }
        catch (DbUpdateException dbEx)
        {
            Console.WriteLine($"Database update error: {dbEx.Message}");
            return new Response<Category?>(null, 500, "An error occurred while creating the category.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            return new Response<Category?>(null, 500, "An error occurred while creating the category.");
        }
    }

    public async Task<Response<Category?>> UpdateAsync(UpdateCategoryRequest request)
    {
        try
        {
            var category = await context
                .Categories
                .FirstOrDefaultAsync(c => c.Id == request.Id && c.UserId == request.UserId);

            if (category is null)
                return new Response<Category?>(null, 404, "Category not found.");

            category.Title = request.Title;
            category.Description = request.Description;

            context.Categories.Update(category);
            await context.SaveChangesAsync();

            return new Response<Category?>(category, message: "Category updated successfully.");
        }
        catch (DbUpdateException dbEx)
        {
            Console.WriteLine($"Database update error: {dbEx.Message}");
            return new Response<Category?>(null, 500, "An error occurred while updating the category.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            return new Response<Category?>(null, 500, "An error occurred while updating the category.");
        }
    }

    public async Task<Response<Category?>> DeleteAsync(DeleteCategoryRequest request)
    {
        try
        {
            var category = await context
                .Categories
                .FirstOrDefaultAsync(c => c.Id == request.Id && c.UserId == request.UserId);

            if (category is null)
                return new Response<Category?>(null, 404, "Category not found.");

            context.Categories.Remove(category);
            await context.SaveChangesAsync();

            return new Response<Category?>(category, message: "Category deleted successfully.");
        }
        catch (DbUpdateException dbEx)
        {
            Console.WriteLine($"Database update error: {dbEx.Message}");
            return new Response<Category?>(null, 500, "An error occurred while deleting the category.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            return new Response<Category?>(null, 500, "An error occurred while deleting the category.");
        }
    }

    public async Task<Response<Category?>> GetByIdAsync(GetCategoryByIdRequest request)
    {
        try
        {
            var category = await context
                .Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.Id && c.UserId == request.UserId);

            if (category is null)
                return new Response<Category?>(null, 404, "Category not found.");

            return new Response<Category?>(category);
        }
        catch (DbUpdateException dbEx)
        {
            Console.WriteLine($"Database update error: {dbEx.Message}");
            return new Response<Category?>(null, 500, "An error occurred while retrieving the category.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            return new Response<Category?>(null, 500, "An error occurred while retrieving the category.");
        }
    }

    public async Task<PagedResponse<List<Category>?>> GetAllAsync(GetAllCategoriesRequest request)
    {
        try
        {
            var query = context
                .Categories
                .AsNoTracking()
                .Where(c => c.UserId == request.UserId)
                .OrderBy(c => c.Title);

            var categories = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var count = await query.CountAsync();

            return new PagedResponse<List<Category>?>(
                categories,
                count,
                request.PageNumber,
                request.PageSize);
        }
        catch (DbUpdateException dbEx)
        {
            Console.WriteLine($"Database update error: {dbEx.Message}");
            return new PagedResponse<List<Category>?>(null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            return new PagedResponse<List<Category>?>(null);
        }
    }
}