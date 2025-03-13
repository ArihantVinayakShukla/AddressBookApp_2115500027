using BusinessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTO;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace AddressBookAPI.Controllers
{
    [ApiController]
    [Route("api/addressbook")]
    [Authorize]
    public class AddressBookController : ControllerBase
    {
        private readonly IAddressBookBL _addressBookBL;
        private readonly IUserBL _userBL; // Added UserBL to fetch UserId from Email

        public AddressBookController(IAddressBookBL addressBookBL, IUserBL userBL)
        {
            _addressBookBL = addressBookBL;
            _userBL = userBL;
        }

        // Extracts the Email from the JWT Token
        private string GetUserEmailFromToken()
        {
            return User.FindFirstValue(ClaimTypes.Email);
        }

        /// <summary>
        /// Fetch all contacts for the logged-in user
        /// </summary>
        /// <returns>List of AddressBookDTO entries</returns>
        [HttpGet]
        public ActionResult<IEnumerable<AddressBookDTO>> GetAllContacts()
        {
            try
            {
                string userEmail = GetUserEmailFromToken();
                int userId = _userBL.GetUserIdByEmail(userEmail);

                if (userId == 0)
                    return Unauthorized("Invalid user credentials.");

                var contacts = _addressBookBL.GetAllContacts(userEmail);
                return contacts.Count > 0 ? Ok(contacts) : NotFound("No contacts found.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// Get a contact by ID for the logged-in user
        /// </summary>
        /// <param name="id">ID of the contact</param>
        /// <returns>Contact details</returns>
        [HttpGet("{id}")]
        public ActionResult<AddressBookDTO> GetById(int id)
        {
            try
            {
                string userEmail = GetUserEmailFromToken();
                int userId = _userBL.GetUserIdByEmail(userEmail);

                if (userId == 0)
                    return Unauthorized("Invalid user credentials.");

                var contact = _addressBookBL.GetById(id, userEmail);
                return contact != null ? Ok(contact) : NotFound($"Contact with ID {id} not found.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// Add a new contact for the logged-in user
        /// </summary>
        /// <param name="contact">New contact details</param>
        /// <returns>Created Contact</returns>
        [HttpPost]
        public ActionResult<AddressBookDTO> AddContact(AddressBookDTO contact)
        {
            try
            {
                string userEmail = GetUserEmailFromToken();
                int userId = _userBL.GetUserIdByEmail(userEmail);

                if (userId == 0)
                    return Unauthorized("Invalid user credentials.");

                var newContact = _addressBookBL.AddContact(contact, userEmail);
                return CreatedAtAction(nameof(GetById), new { id = newContact.Id }, newContact);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// Update an existing contact for the logged-in user
        /// </summary>
        /// <param name="id">Id of the contact</param>
        /// <param name="updatedContact">Updated contact details</param>
        /// <returns>Updated Contact</returns>
        [HttpPut("{id}")]
        public IActionResult UpdateContact(int id, AddressBookDTO updatedContact)
        {
            try
            {
                string userEmail = GetUserEmailFromToken();
                int userId = _userBL.GetUserIdByEmail(userEmail);

                if (userId == 0)
                    return Unauthorized("Invalid user credentials.");

                var contact = _addressBookBL.UpdateContact(id, updatedContact, userEmail);
                return contact != null ? Ok(contact) : NotFound($"Contact with ID {id} not found.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// Delete a contact by ID for the logged-in user
        /// </summary>
        /// <param name="id">ID of the contact</param>
        /// <returns>Deletion status</returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteContact(int id)
        {
            try
            {
                string userEmail = GetUserEmailFromToken();
                int userId = _userBL.GetUserIdByEmail(userEmail);

                if (userId == 0)
                    return Unauthorized("Invalid user credentials.");

                var result = _addressBookBL.DeleteContact(id, userEmail);
                return result ? Ok("Contact deleted successfully.") : NotFound($"Contact with ID {id} not found.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
