using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;

namespace AddressBookAPI.Controllers
{
        [ApiController]
        [Route("api/addressbook")]
    public class AddressBookController : ControllerBase
    {
        private readonly AddressBookContext _context;
        public AddressBookController(AddressBookContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Fetch all contacts from the address book
        /// </summary>
        /// <returns>List of AddressBook entries</returns>
        [HttpGet]
        public ActionResult<IEnumerable<AddressBookEntry>> GetAllContacts()
        {
            try
            {
                var contacts = _context.AddressBooks.ToList();
                if(!contacts.Any())
                {
                    return NotFound("No contacts found");
                }
                return Ok(contacts);
            }
            catch(Exception)
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
        public ActionResult<AddressBookEntry> GetById(int id)
        {
            try
            {
                var contact = _context.AddressBooks.Find(id);
                if(contact == null)
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
        public ActionResult<AddressBookEntry> AddContact(AddressBookEntry contact)
        {
            try
            {
                _context.AddressBooks.Add(contact);
                _context.SaveChanges();
                return CreatedAtAction(nameof(GetById), new { id = contact.Id }, contact);
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
        public ActionResult<AddressBookEntry> UpdateContact(int id, AddressBookEntry updatedContact)
        {
            try
            {
                var existingContact = _context.AddressBooks.Find(id);
                if (existingContact == null)
                {
                    return NotFound($"Contact with ID {id} not found.");
                }

                existingContact.Name = updatedContact.Name;
                existingContact.Email = updatedContact.Email;
                existingContact.Phone = updatedContact.Phone;
                existingContact.Address = updatedContact.Address;

                _context.SaveChanges();
                return Ok(existingContact);
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
                var contact = _context.AddressBooks.Find(id);
                if (contact == null)
                {
                    return NotFound($"Contact with ID {id} not found.");
                }

                _context.AddressBooks.Remove(contact);
                _context.SaveChanges();
                return Ok("Contact deleted successfully.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        
    }
}
