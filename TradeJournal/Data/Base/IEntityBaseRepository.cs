using System.Linq.Expressions;

namespace TradeJournal.Data.Base
{
    public interface IEntityBaseRepository<T> where T: class, IEntityBase, new()
    {
        //Zwraca listę wszystkich obiektów typu T w postaci kolekcji IEnumerable<T>.
        Task<IEnumerable<T>> GetAllAsync();
        //Zwraca listę wszystkich obiektów typu T wraz z powiązanymi danymi określonymi w includeProperties.
        //operacja asynchroniczna, która zwraca kolekcję obiektów T.
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includeProperties);
        Task<T> GetByIdAsync(int id);
        Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includeProperties);
        Task AddAsync(T entity);
        Task RemoveAsync(T entity);
        Task UpdateAsync(int id, T entity);
        Task DeleteAsync(int id);
    }
}


/*
    OPIS!
    Interfejs IEntityBaseRepository<T> jest generycznym repozytorium danych, które obsługuje instancje typu T.
    Typ T musi być klasą (class), implementować interfejs IEntityBase i posiadać domyślny konstruktor (new()). 
    Interfejs ten definiuje standardowe operacje CRUD oraz wspiera ładowanie powiązanych danych przy użyciu wyrażeń lambda (Expression<Func<T, object>>).


    task:
    Task jest klasą z przestrzeni nazw System.Threading.
    Tasks, która reprezentuje operację asynchroniczną.
    W kontekście metod tego interfejsu, Task oznacza, że operacja będzie wykonywana asynchronicznie, co pozwala na nieblokowanie wątku wywołującego metodę, 
    dopóki operacja nie zostanie zakończona.
    Task<T> zwraca wynik operacji asynchronicznej typu T.
 
     params Expression<Func<T, object>>[] includeProperties:
     Jest parametrem metody umożliwiającym przekazywanie zmiennej liczby wyrażeń lambda, które definiują właściwości do załadowania wraz z głównym obiektem T
 
 */