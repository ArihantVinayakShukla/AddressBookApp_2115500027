using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.DTO;
using RepositoryLayer.Entity;

namespace BusinessLayer.Interface
{
    public interface IAddressBookBL
    {
        List<AddressBookDTO> GetAllContacts();
        AddressBookDTO GetById(int id);
        AddressBookDTO AddContact(AddressBookDTO contact);
        AddressBookDTO UpdateContact(int id, AddressBookDTO updatedContact);
        bool DeleteContact(int id);
    }
}
