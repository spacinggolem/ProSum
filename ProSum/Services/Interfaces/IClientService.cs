using ProSum.Models;
using System;
using System.Collections.Generic;

namespace ProSum.Services.Interfaces
{
    public interface IClientService
    {
        public void Create(Client client);

        public List<Client> Get();

        public Client Get(Guid id);

        public void Edit(Guid guidToUpdate, Client client);

        public Client GetByEmail(string email);
    }
}
