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
        List<AddressBookDTO> GetAllContacts(string userEmail);
        List<AddressBookDTO> GetAllContactsForAdmin();
        AddressBookDTO GetById(int id, string userEmail);
        AddressBookDTO AddContact(AddressBookDTO contact, string userEmail);
        AddressBookDTO UpdateContact(int id, AddressBookDTO updatedContact, string userEmail);
        bool DeleteContact(int id, string userEmail);
        bool DeleteContactAsAdmin(int id);
    }
}
