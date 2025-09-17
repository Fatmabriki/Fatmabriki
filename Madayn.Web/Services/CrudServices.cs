using Madayn.Web.Data;
using Madayn.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Madayn.Web.Services;

public interface ICRUDService<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(bool includeDeleted = false);
    Task<T?> GetByIdAsync(int id);
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(int id); // soft
    Task<bool> RestoreAsync(int id);
    Task<bool> HideAsync(int id);
    Task<bool> UnhideAsync(int id);
    Task<IEnumerable<T>> SearchAsync(string searchTerm);
}

public abstract class CrudServiceBase<T> : ICRUDService<T> where T : class
{
    protected readonly ApplicationDbContext Context;
    protected CrudServiceBase(ApplicationDbContext ctx) { Context = ctx; }

    public virtual async Task<T> CreateAsync(T entity)
    {
        Context.Set<T>().Add(entity);
        await Context.SaveChangesAsync();
        return entity;
    }
    public virtual async Task<bool> DeleteAsync(int id)
    {
        var entity = await Context.Set<T>().FindAsync(id);
        if (entity == null) return false;
        var prop = entity.GetType().GetProperty("IsDeleted");
        if (prop != null) { prop.SetValue(entity, true); }
        var delAt = entity.GetType().GetProperty("DeletedAt");
        if (delAt != null) delAt.SetValue(entity, DateTime.UtcNow);
        await Context.SaveChangesAsync();
        return true;
    }
    public virtual async Task<IEnumerable<T>> GetAllAsync(bool includeDeleted = false)
    {
        var query = Context.Set<T>().AsQueryable();
        var prop = typeof(T).GetProperty("IsDeleted");
        if (prop != null && !includeDeleted)
        {
            query = query.Where(e => (bool)(prop.GetValue(e) ?? false) == false);
        }
        return await query.ToListAsync();
    }
    public virtual async Task<T?> GetByIdAsync(int id) => await Context.Set<T>().FindAsync(id);
    public virtual async Task<bool> HideAsync(int id)
    {
        var entity = await Context.Set<T>().FindAsync(id);
        if (entity == null) return false;
        var prop = entity.GetType().GetProperty("IsHidden");
        if (prop == null) return false;
        prop.SetValue(entity, true);
        await Context.SaveChangesAsync();
        return true;
    }
    public virtual async Task<bool> RestoreAsync(int id)
    {
        var entity = await Context.Set<T>().FindAsync(id);
        if (entity == null) return false;
        var prop = entity.GetType().GetProperty("IsDeleted");
        if (prop == null) return false;
        prop.SetValue(entity, false);
        var delAt = entity.GetType().GetProperty("DeletedAt");
        if (delAt != null) delAt.SetValue(entity, null);
        await Context.SaveChangesAsync();
        return true;
    }
    public virtual async Task<IEnumerable<T>> SearchAsync(string searchTerm)
    {
        return await Context.Set<T>().ToListAsync();
    }
    public virtual async Task<T> UpdateAsync(T entity)
    {
        Context.Entry(entity).State = EntityState.Modified;
        await Context.SaveChangesAsync();
        return entity;
    }
    public virtual async Task<bool> UnhideAsync(int id)
    {
        var entity = await Context.Set<T>().FindAsync(id);
        if (entity == null) return false;
        var prop = entity.GetType().GetProperty("IsHidden");
        if (prop == null) return false;
        prop.SetValue(entity, false);
        await Context.SaveChangesAsync();
        return true;
    }
}

public class SurveyService : CrudServiceBase<Survey>
{
    public SurveyService(ApplicationDbContext ctx) : base(ctx) { }
}

public class NewsService : CrudServiceBase<News>
{
    public NewsService(ApplicationDbContext ctx) : base(ctx) { }
}

public class ConsultantService : CrudServiceBase<Consultant>
{
    public ConsultantService(ApplicationDbContext ctx) : base(ctx) { }
}

public class ContractorService : CrudServiceBase<Contractor>
{
    public ContractorService(ApplicationDbContext ctx) : base(ctx) { }
}
