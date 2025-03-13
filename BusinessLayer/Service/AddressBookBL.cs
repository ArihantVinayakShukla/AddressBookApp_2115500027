using System.Collections.Generic;
using AutoMapper;
using BusinessLayer.Interface;
using ModelLayer.DTO;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service
{
    public class AddressBookBL : IAddressBookBL
    {
        private readonly IAddressBookRL _addressBookRL;
        private readonly IUserRL _userRL;  // Add UserRL to fetch UserId
        private readonly IMapper _mapper;

        public AddressBookBL(IAddressBookRL addressBookRL, IUserRL userRL, IMapper mapper)
        {
            _addressBookRL = addressBookRL;
            _userRL = userRL;
            _mapper = mapper;
        }

        public List<AddressBookDTO> GetAllContacts(string userEmail)
        {
            int userId = _userRL.GetUserIdByEmail(userEmail);
            return _mapper.Map<List<AddressBookDTO>>(_addressBookRL.GetAllContacts(userId));
        }

        public AddressBookDTO GetById(int id, string userEmail)
        {
            int userId = _userRL.GetUserIdByEmail(userEmail);
            var contact = _addressBookRL.GetById(id, userId);
            return contact != null ? _mapper.Map<AddressBookDTO>(contact) : null;
        }

        public AddressBookDTO AddContact(AddressBookDTO contact, string userEmail)
        {
            int userId = _userRL.GetUserIdByEmail(userEmail);
            var entity = _mapper.Map<AddressBookEntry>(contact);
            var savedEntity = _addressBookRL.AddContact(entity, userId);
            return _mapper.Map<AddressBookDTO>(savedEntity);
        }

        public AddressBookDTO UpdateContact(int id, AddressBookDTO updatedContact, string userEmail)
        {
            int userId = _userRL.GetUserIdByEmail(userEmail);
            var entity = _mapper.Map<AddressBookEntry>(updatedContact);
            var updatedEntity = _addressBookRL.UpdateContact(id, entity, userId);
            return updatedEntity != null ? _mapper.Map<AddressBookDTO>(updatedEntity) : null;
        }

        public bool DeleteContact(int id, string userEmail)
        {
            int userId = _userRL.GetUserIdByEmail(userEmail);
            return _addressBookRL.DeleteContact(id, userId);
        }
    }
}
