using Microsoft.EntityFrameworkCore;
using TradeJournal.Data.Base;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using TradeJournal.Data.Base;

namespace TradeJournal.Data.Base
{
    public class EntityBaseRepository<T> : IEntityBaseRepository<T> where T : class, IEntityBase, new()
    {
        // handler do BD 
        private readonly AppDbContext _context;
        public EntityBaseRepository(AppDbContext context)
        {
            _context = context;
        }

        // asynchroniczna metoda zwracająca wszystkie obiekty tyu T 
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        // Asynchroniczna metoda zwracająca obiekt typu T na podstawie identyfikatora wraz z powiązanymi danymi określonymi w includeProperties
        public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return await query.ToListAsync();
        }

        // Asynchroniczna metoda zwracająca obiekt typu T na podstawie identyfikatora
        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        // Asynchroniczna metoda zwracająca obiekt typu T na podstawie identyfikatora wraz z powiązanymi danymi określonymi w includeProperties
        public async Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return await query.FirstOrDefaultAsync(entity => entity.Id == id);
        }

        // Asynchroniczna metoda dodająca nowy obiekt typu T do bazy danych
        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        // Asynchroniczna metoda usuwająca obiekt typu T z bazy danych
        public async Task RemoveAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        // Asynchroniczna metoda aktualizująca obiekt typu T w bazie danych
        public async Task UpdateAsync(int id, T entity)
        {
            var existingEntity = await _context.Set<T>().FindAsync(id);
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
            }
        }

        // Asynchroniczna metoda usuwająca obiekt typu T z bazy danych na podstawie identyfikatora
        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

    }
}

/*
    Wytłumaczenie kodu: 

    Framework
    DbContext i DbSet są częścią Entity Framework, popularnego ORM (Object-Relational Mapper) dla .NET, 
    który upraszcza operacje bazodanowe poprzez mapowanie klas na tabele w bazie danych. 
    Dzięki temu nie musisz ręcznie pisać SQL-a do wykonywania operacji CRUD, co znacząco upraszcza i przyspiesza rozwój aplikacji.
    Używając DbContext i DbSet, możesz skupić się na logice biznesowej aplikacji, podczas gdy Entity Framework zajmuje się szczegółami interakcji z bazą danych.


    Metody DbContext.Set<T>()
    DbContext.Set<T>() jest częścią DbContext w Entity Framework.
    Jest to metoda, która umożliwia dostęp do tabeli w bazie danych bez konieczności jawnego definiowania właściwości DbSet dla każdego typu encji.


    _context.Set<T>() jest metodą dostępną w kontekście bazy danych dostarczanym przez Entity Framework. 
    Działa ona dzięki mechanizmom generowania kodu w Entity Framework, który automatycznie tworzy odpowiednie zestawy (sets) encji na podstawie zdefiniowanych modeli.

 */