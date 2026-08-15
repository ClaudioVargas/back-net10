using back.Core.Entity;
using back.Core.Interfaces;
using System.Collections.Concurrent;

namespace back.Repositories
{
    public class ContactoRepository : IContactoRepository
    {

        //private readonly List<Contacto> contactos = new();
        private static readonly ConcurrentDictionary<int, Contacto> _contactos = new();
        private static int _ultimoId = 0;


        public async Task<IEnumerable<Contacto>> GetAll()
        {
            return await Task.FromResult(_contactos.Values);
        }

        public async Task<Contacto?> GetById(int id)
        {
            _contactos.TryGetValue(id, out var contacto);
            return await Task.FromResult(contacto);
        }

        public async Task<Contacto?> Add(Contacto contacto)
        {

            var existe = await isDuplicado(contacto.Telefono);
            if (!existe)
            {
                int nuevoId = Interlocked.Increment(ref _ultimoId);
                _ultimoId = nuevoId;
                var nuevoContacto = new Contacto
                {
                    Id = nuevoId,
                    Nombre = contacto.Nombre,
                    Telefono = contacto.Telefono
                };
                _contactos.TryAdd(nuevoId, nuevoContacto);
                return nuevoContacto;
            }
            else {
                return null;
            }
        }

        public async Task<bool> Update(Contacto contacto)
        {
            if (_contactos.ContainsKey(contacto.Id))
            {
                _contactos[contacto.Id] = contacto;
                return await Task.FromResult(true) ;
            }
            return await Task.FromResult(false);
        }

        public async Task<bool> Delete(int id)
        {
            var contacto = await GetById(id);
            if (contacto != null)
            {
                _contactos.TryRemove(contacto.Id, out _);
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        public async Task<bool> isDuplicado(string phone)
        {
            foreach (var item in _contactos.Values)
            {
                if (item.Telefono.Equals(phone)) return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }
    }
}

