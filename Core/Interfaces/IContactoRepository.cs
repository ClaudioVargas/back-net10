using back.Core.Entity;

namespace back.Core.Interfaces
{
    public interface IContactoRepository
    {
        Task<IEnumerable<Contacto>> GetAll();
        Task<Contacto?> GetById(int id);
        Task<Contacto?> Add(Contacto contacto);
        Task<bool> Update(Contacto contacto);
        Task<bool> Delete(int id);
    }
}
