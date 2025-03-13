using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IAddressBookRL
    {
        List<AddressBookEntry> GetAllContacts(int userId);
        AddressBookEntry GetById(int id, int userId);
        AddressBookEntry AddContact(AddressBookEntry contact, int userId);
        AddressBookEntry UpdateContact(int id, AddressBookEntry updatedContact, int userId);
        bool DeleteContact(int id, int userId);
    }
}
