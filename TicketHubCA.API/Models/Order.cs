using System.ComponentModel.DataAnnotations;

namespace TicketHubCA.API.Models
{
    public class Order
    {
        [Required(ErrorMessage = "A Concert ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Concert ID must be greater than 1.")]
        public int ConcertId { get; set; }

        [Required(ErrorMessage = "An email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = String.Empty;

        [Required(ErrorMessage = "A name is required.")]
        [StringLength(100, ErrorMessage = "Please enter a name less than 100 characters.")]
        public string Name { get; set; } = String.Empty;

        [Required(ErrorMessage = "A phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string Phone { get; set; } = String.Empty;

        [Required(ErrorMessage = "A quantity is required. (Limit 5 per order)")]
        [Range(1, 5, ErrorMessage = "Maximum 5 tickets per order.")]
        public int Quantity { get; set; }

        //Must pass LUHN check- 12345674 for testing
        [Required(ErrorMessage = "A credit card number is required.")]
        [CreditCard(ErrorMessage = "Invalid credit card number.")]
        public string CreditCard { get; set; } = String.Empty;

        [Required(ErrorMessage = "A valid expiration date is required.")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/([0-9]{4})$", ErrorMessage = "Invalid expiration date. (Format should be MM/YYYY)")]
        public string Expiration { get; set; } = String.Empty;

        [Required(ErrorMessage = "A security code is required.")]
        [RegularExpression(@"\d{3,4}", ErrorMessage = "Invalid security code.")]
        public string SecurityCode { get; set; } = String.Empty;

        [Required(ErrorMessage = "An address is required.")]
        [StringLength(120, ErrorMessage = "Address must be less than 120 characters.")]
        public string Address { get; set; } = String.Empty;

        [Required(ErrorMessage = "A city is required.")]
        [StringLength(31)]
        public string City { get; set; } = String.Empty;

        [Required(ErrorMessage = "A province is required.")]
        [RegularExpression(@"^(AB|BC|MB|NB|NL|NT|NS|NU|ON|PE|QC|SK|YT)$", ErrorMessage = "Invalid province.")]
        public string Province { get; set; } = String.Empty;

        [Required(ErrorMessage = "A postal code is required.")]
        [RegularExpression(@"^([AaBbCcEeGgHhJjKkLlMmNnPpRrSsTtVvXxYy][0-9][A-Za-z])[\s-]?([0-9][A-Za-z][0-9])$", ErrorMessage = "Invalid postal code. (Format should be A1A 1A1)")]
        public string PostalCode { get; set; } = String.Empty;

        [Required(ErrorMessage = "Country is required.")]
        [RegularExpression(@"Canada", ErrorMessage = "Sorry, Canadian customers only!")]
        public string Country { get; set; } = String.Empty;
    }
}
