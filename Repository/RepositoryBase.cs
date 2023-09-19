using System.Linq;
using System.Linq.Expressions;
using TodoApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Serilog;

namespace TodoApi.Repositories
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected TodoContext RepositoryContext { get; set; }
        public string? OldObjString { get; set; }

        public RepositoryBase(TodoContext repositoryContext)
        {
            this.RepositoryContext = repositoryContext;
        }
        public T? FindByID(long ID)
        {
            return this.RepositoryContext.Set<T>().Find(ID);
        }
        public T? FindByID(int ID)
        {
            return this.RepositoryContext.Set<T>().Find(ID);
        }
        public async Task<T?> FindByIDAsync(long ID)
        {
            return await this.RepositoryContext.Set<T>().FindAsync(ID);
        }
        public async Task<T?> FindByIDAsync(int ID)
        {
            return await this.RepositoryContext.Set<T>().FindAsync(ID);
        }

        public IEnumerable<T> FindAll()
        {
            return this.RepositoryContext.Set<T>();
        }

        public async Task<IEnumerable<T>> FindAllAsync()
        {
            return await this.RepositoryContext.Set<T>().ToListAsync();
        }

        public IEnumerable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return this.RepositoryContext.Set<T>().Where(expression);
        }
        public async Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> expression)
        {
            return await  this.RepositoryContext.Set<T>().Where(expression).ToListAsync();
        }

        public bool AnyByCondition(Expression<Func<T, bool>> expression)
        {
            return this.RepositoryContext.Set<T>().Any(expression);
        }

        public async Task<bool> AnyByConditionAsync(Expression<Func<T, bool>> expression)
        {
            return await this.RepositoryContext.Set<T>().AnyAsync(expression);
        }

        public T? FindByCompositeID(long ID1, long ID2)
        {
            return this.RepositoryContext.Set<T>().Find(ID1, ID2);
        }
        public T? FindByCompositeID(int ID1, int ID2)
        {
            return this.RepositoryContext.Set<T>().Find(ID1, ID2);
        }

        public async Task<T?> FindByCompositeIDAsync(long ID1, long ID2)
        {
            return await this.RepositoryContext.Set<T>().FindAsync(ID1, ID2);
        }
        public async Task<T?> FindByCompositeIDAsync(int ID1, int ID2)
        {
            return await this.RepositoryContext.Set<T>().FindAsync(ID1, ID2);
        }

        public void Create(dynamic entity, bool flush = true)
        {
            this.RepositoryContext.Set<T>().Add(entity);
            if (flush) this.Save();
        }

        public void CreateRange(dynamic entity, bool flush = true)
        {
            this.RepositoryContext.Set<T>().AddRange(entity);
            if (flush) this.Save();
        }

        public void Update(dynamic entity, bool flush = true)
        {
            entity.SetEventLogMessage(this.GetUpdateEventLogString(entity));
            this.RepositoryContext.Set<T>().Update(entity);
            if (flush)
            {
                this.Save();

            }
        }

        public void UpdateRange(dynamic entity, bool flush = true)
        {
            entity.SetEventLogMessage(this.GetUpdateEventLogString(entity));
            this.RepositoryContext.Set<T>().UpdateRange(entity);
            if (flush) this.Save();
        }

        public void Delete(dynamic entity, bool flush = true)
        {
            entity.SetEventLogMessage(this.SetOldObjectToString(entity));
            this.RepositoryContext.Set<T>().Remove(entity);
            if (flush) this.Save();
        }

        public void DeleteRange(dynamic entity, bool flush = true)
        {
            entity.SetEventLogMessage(this.SetOldObjectToString(entity));
            this.RepositoryContext.Set<T>().RemoveRange(entity);
            if (flush) this.Save();
        }

        public void DiscardChanges()
        {
            this.RepositoryContext.ChangeTracker.Entries()
            .Where(e => e.Entity != null).ToList()
            .ForEach(e => e.State = EntityState.Detached);

        }

        public void Save()
        {
            this.RepositoryContext.SaveChanges();
        }

        
        public async Task CreateAsync(dynamic entity, bool flush = true)
        {
            this.RepositoryContext.Set<T>().Add(entity);
            if (flush) await this.SaveAsync();
        }

        public async Task UpdateAsync(dynamic entity, bool flush = true)
        {
            entity.SetEventLogMessage(this.GetUpdateEventLogString(entity));
            this.RepositoryContext.Set<T>().Update(entity);
            if (flush) await this.SaveAsync();
        }

        public async Task DeleteAsync(dynamic entity, bool flush = true)
        {
            entity.SetEventLogMessage(this.SetOldObjectToString(entity));
            this.RepositoryContext.Set<T>().Remove(entity);
            if (flush) await this.SaveAsync();
        }

        public async Task SaveAsync()
        {
            await this.RepositoryContext.SaveChangesAsync();
        }

        
        public string SetOldObjectToString(dynamic OldObj)
        {
            this.OldObjString = "";
            JObject _duplicateObj = JObject.FromObject(OldObj);
            var _List = _duplicateObj.ToObject<Dictionary<string, object>>();
            if(_List != null) {
                foreach (var item in _List)
                {
                    var name = item.Key;
                    var val = item.Value;
                    string msg = name + " : " + val + "\r\n";
                    this.OldObjString += msg;
                }
            }
            return this.OldObjString;
        }

        public string GetOldObjectString()
        {
            return this.OldObjString ?? "";
        }
        
        public String GetUpdateEventLogString(dynamic entity)
        {
            PropertyValues  oldObj;
            string oldObjString = "";
            try
            {
                oldObj = this.RepositoryContext.Entry(entity).OriginalValues;
                if (oldObj == null) return "";
                JObject _newObj = JObject.FromObject(entity);
                var _newList = _newObj.ToObject<Dictionary<string, object>>();

                foreach (var item in oldObj.Properties)
                {
                    var name = item.Name;
                    var val = oldObj[name] != null ? oldObj[name]!.ToString()!.Trim() : "";
                    var newval = _newList != null ? (_newList.GetValueOrDefault(name) != null ? _newList.GetValueOrDefault(name)!.ToString()!.Trim() : "") : "";
                    string msg = "";
                    if(val != newval || item.IsKey()) msg = name + " : " + val + " >>> " + newval + "\r\n";   //include primary key and changes fields only
                    oldObjString += msg;
                }
            }
            catch (Exception ex)
            {
                Log.Error("GetUpdateEventLogString Exception :" + ex.Message);
            }
            return oldObjString;
        }
       

    }
    
}
