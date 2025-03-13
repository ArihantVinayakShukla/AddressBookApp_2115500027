using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly IMapper _mapper;

        public AddressBookBL(IAddressBookRL addressBookRL, IMapper mapper)
        {
            _addressBookRL = addressBookRL;
            _mapper = mapper;
        }

        public List<AddressBookDTO> GetAllContacts()
        {
            var contacts = _addressBookRL.GetAllContacts();
            return _mapper.Map<List<AddressBookDTO>>(contacts);
        }

        public AddressBookDTO GetById(int id)
        {
            var contact = _addressBookRL.GetById(id);
            return _mapper.Map<AddressBookDTO>(contact);
        }

        public AddressBookDTO AddContact(AddressBookDTO contact)
        {
            var contactEntity = _mapper.Map<AddressBookEntry>(contact);
            var newContact = _addressBookRL.AddContact(contactEntity);
            return _mapper.Map<AddressBookDTO>(newContact);
        }

        public AddressBookDTO UpdateContact(int id, AddressBookDTO updatedContact)
        {
            var contactEntity = _mapper.Map<AddressBookEntry>(updatedContact);
            var updatedEntity = _addressBookRL.UpdateContact(id, contactEntity);
            return _mapper.Map<AddressBookDTO>(updatedEntity);
        }

        public bool DeleteContact(int id)
        {
            return _addressBookRL.DeleteContact(id);
        }
    }
}
