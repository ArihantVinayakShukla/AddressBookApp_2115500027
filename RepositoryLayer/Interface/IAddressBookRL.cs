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
        List<AddressBookEntry> GetAllContacts();
        AddressBookEntry GetById(int id);
        AddressBookEntry AddContact(AddressBookEntry contact);
        AddressBookEntry UpdateContact(int id, AddressBookEntry updatedContact);
        bool DeleteContact(int id);
    }
}
