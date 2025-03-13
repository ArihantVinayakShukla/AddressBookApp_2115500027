using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class AddressBookRL : IAddressBookRL
    {
        private readonly AddressBookContext _context;

        public AddressBookRL(AddressBookContext context)
        {
            _context = context;
        }

        public List<AddressBookEntry> GetAllContacts()
        {
            return _context.AddressBooks.ToList();
        }

        public AddressBookEntry GetById(int id)
        {
            return _context.AddressBooks.Find(id);
        }

        public AddressBookEntry AddContact(AddressBookEntry contact)
        {
            _context.AddressBooks.Add(contact);
            _context.SaveChanges();
            return contact;
        }

        public AddressBookEntry UpdateContact(int id, AddressBookEntry updatedContact)
        {
            var contact = _context.AddressBooks.Find(id);
            if (contact == null) return null;

            contact.Name = updatedContact.Name;
            contact.Email = updatedContact.Email;
            contact.Phone = updatedContact.Phone;
            contact.Address = updatedContact.Address;

            _context.SaveChanges();
            return contact;
        }

        public bool DeleteContact(int id)
        {
            var contact = _context.AddressBooks.Find(id);
            if (contact == null) return false;

            _context.AddressBooks.Remove(contact);
            _context.SaveChanges();
            return true;
        }
    }
}
