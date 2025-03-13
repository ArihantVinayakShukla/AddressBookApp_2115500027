using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTO;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;

namespace AddressBookAPI.Controllers
{
        [ApiController]
        [Route("api/addressbook")]
    public class AddressBookController : ControllerBase
    {
        private readonly IAddressBookBL _addressBookBL;

        public AddressBookController(IAddressBookBL addressBookBL)
        {
            _addressBookBL = addressBookBL;
        }


        /// <summary>
        /// Fetch all contacts from the address book
        /// </summary>
        /// <returns>List of AddressBookDTO entries</returns>
        [HttpGet]
        public ActionResult<IEnumerable<AddressBookDTO>> GetAllContacts()
        {
            try
            {
                var contacts = _addressBookBL.GetAllContacts();
                if (contacts == null || contacts.Count == 0)
                {
                    return NotFound("No contacts found.");
                }
                return Ok(contacts);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }


        /// <summary>
        /// Get a contact by ID.
        /// </summary>
        /// <param name="id">ID of the contact</param>
        /// <returns>Contact details</returns>
        [HttpGet("{id}")]
        public ActionResult<AddressBookDTO> GetById(int id)
        {
            try
            {
                var contact = _addressBookBL.GetById(id);
                if (contact == null)
                {
                    return NotFound($"Contact with ID {id} not found.");
                }
                return Ok(contact);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }


        /// <summary>
        /// Add a new contact to the address book
        /// </summary>
        /// <param name="contact">New contact details</param>
        /// <returns>Created Contact</returns>
        [HttpPost]
        public ActionResult<AddressBookDTO> AddContact(AddressBookDTO contact)
        {
            try
            {
                var newContact = _addressBookBL.AddContact(contact);
                return CreatedAtAction(nameof(GetById), new { id = newContact }, newContact);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }


        /// <summary>
        /// Update an existing contact
        /// </summary>
        /// <param name="id">Id of the contact</param>
        /// <param name="updatedContact">Updated contact details</param>
        /// <returns>Updated Contact</returns>
        [HttpPut("{id}")]
        public IActionResult UpdateContact(int id, AddressBookDTO updatedContact)
        {
            try
            {
                var contact = _addressBookBL.UpdateContact(id, updatedContact);
                if (contact == null)
                {
                    return NotFound($"Contact with ID {id} not found.");
                }
                return Ok(contact);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }


        /// <summary>
        /// Delete a contact by ID
        /// </summary>
        /// <param name="id">ID of the contact</param>
        /// <returns>Deletion status</returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteContact(int id)
        {
            try
            {
                var result = _addressBookBL.DeleteContact(id);
                if (!result)
                {
                    return NotFound($"Contact with ID {id} not found.");
                }
                return Ok("Contact deleted successfully.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }


    }
}
