using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Helper;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class AddressBookRL : IAddressBookRL
    {
        private readonly AddressBookContext _context;
        private readonly IRedisCacheHelper _cacheHelper;

        public AddressBookRL(AddressBookContext context, IRedisCacheHelper cacheHelper)
        {
            _context = context;
            _cacheHelper = cacheHelper;
        }

        public List<AddressBookEntry> GetAllContacts(int userId)
        {
            var cachedContacts = _cacheHelper.GetCacheAsync<List<AddressBookEntry>>($"addressbook_{userId}").Result;

            if (cachedContacts != null)
            {
                return cachedContacts;
            }

            var contacts = _context.AddressBooks.Where(c => c.UserId == userId).ToList();

            _cacheHelper.SetCacheAsync($"addressbook_user_{userId}", contacts).Wait();

            return contacts;
        }

        public List<AddressBookEntry> GetAllContactsForAdmin()
        {
            return _context.AddressBooks.ToList();
        }

        public AddressBookEntry GetById(int id, int userId)
        {
            var cachedContact = _cacheHelper.GetCacheAsync<AddressBookEntry>($"addressbook_{userId}_{id}").Result;

            if (cachedContact != null)
            {
                return cachedContact;
            }

            var contact = _context.AddressBooks.FirstOrDefault(c => c.Id == id && c.UserId == userId);

            if (contact != null)
            {
                _cacheHelper.SetCacheAsync($"addressbook_{userId}_{id}", contact).Wait();
            }

            return contact;
        }

        public AddressBookEntry AddContact(AddressBookEntry contact, int userId)
        {
            contact.UserId = userId;
            _context.AddressBooks.Add(contact);
            _context.SaveChanges();

            _cacheHelper.RemoveCacheAsync($"addressbook_user_{userId}").Wait();

            _cacheHelper.SetCacheAsync($"addressbook_contact_{contact.Id}", contact).Wait();
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

            _cacheHelper.RemoveCacheAsync($"addressbook_user_{userId}").Wait();

            _cacheHelper.SetCacheAsync($"addressbook_user{userId}_contact{id}", contact).Wait();
            return contact;
        }

        public bool DeleteContact(int id, int userId)
        {
            var contact = _context.AddressBooks.FirstOrDefault(c => c.Id == id && c.UserId == userId);
            if (contact == null) return false;

            _context.AddressBooks.Remove(contact);
            _context.SaveChanges();

            _cacheHelper.RemoveCacheAsync($"addressbook_user{userId}_contact{id}").Wait();

            _cacheHelper.RemoveCacheAsync($"addressbook_user_{userId}").Wait();
            return true;
        }

        public bool DeleteContactAsAdmin(int id)
        {
            var contact = _context.AddressBooks.FirstOrDefault(c => c.Id == id);
            if (contact == null) return false;

            _context.AddressBooks.Remove(contact);
            _context.SaveChanges();

            return true;
        }
    }
}
