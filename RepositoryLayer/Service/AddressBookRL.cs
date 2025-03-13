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

        public List<AddressBookEntry> GetAllContacts(int userId)
        {
            return _context.AddressBooks.Where(c => c.UserId == userId).ToList();
        }

        public AddressBookEntry GetById(int id, int userId)
        {
            return _context.AddressBooks.FirstOrDefault(c => c.Id == id && c.UserId == userId);
        }

        public AddressBookEntry AddContact(AddressBookEntry contact, int userId)
        {
            contact.UserId = userId;
            _context.AddressBooks.Add(contact);
            _context.SaveChanges();
            return contact;
        }

        public AddressBookEntry UpdateContact(int id, AddressBookEntry updatedContact, int userId)
        {
            var contact = _context.AddressBooks.FirstOrDefault(c => c.Id == id && c.UserId == userId);
            if (contact == null) return null;

            contact.Name = updatedContact.Name;
            contact.Email = updatedContact.Email;
            contact.Phone = updatedContact.Phone;
            contact.Address = updatedContact.Address;

            _context.SaveChanges();
            return contact;
        }

        public bool DeleteContact(int id, int userId)
        {
            var contact = _context.AddressBooks.FirstOrDefault(c => c.Id == id && c.UserId == userId);
            if (contact == null) return false;

            _context.AddressBooks.Remove(contact);
            _context.SaveChanges();
            return true;
        }
    }
}
