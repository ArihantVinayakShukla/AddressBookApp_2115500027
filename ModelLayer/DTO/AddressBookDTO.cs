using System;

namespace ModelLayer.DTO
{
    /// <summary>
    /// Represents an address book entry containing contact details.
    /// </summary>
    public class AddressBookDTO
    {
        /// <summary>
        /// Unique identifier for the contact.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the contact.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Email address of the contact.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Phone number of the contact.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Physical address of the contact.
        /// </summary>
        public string Address { get; set; }
    }
}
