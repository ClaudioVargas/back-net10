using back.Core.Entity;
using back.Core.Interfaces;
using back.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace back.Services
{
    public class ContactoService: IContactoService
    {
        private readonly IContactoRepository _contactoRepository;

        public ContactoService(IContactoRepository contactoRepository)
        {
            _contactoRepository = contactoRepository;
        }

        public async Task<Contacto?> Add(Contacto contacto)
        {
            
            var contactoCreado = await _contactoRepository.Add(contacto);
            if (contactoCreado != null)
            {
                return contactoCreado;
            }
            return null;
           
        }

        public async Task<bool> Delete(int id)
        {
            return await _contactoRepository.Delete(id);

        }

        public async Task<IEnumerable<Contacto>> GetAll()
        {
            var contactos = await _contactoRepository.GetAll();
            if (contactos.Count() > 0)
            {
                return contactos;

            }

            return Enumerable.Empty<Contacto>();
        }

        public async Task<Contacto?> GetById(int id)
        {
            var contacto = await _contactoRepository.GetById(id);
            if (contacto == null)
            {
                return null;
            }
            return contacto;
        }

        public async Task<bool> Update(Contacto contacto)
        {
            contacto.Id = contacto.Id;
            return await _contactoRepository.Update(contacto);
     
        }
    }
}
