using System.ComponentModel.DataAnnotations;

namespace CustomerApi.Models
{
    public class CreateCustomerRequest
    {
        [Required]
        [MinLength(1)]
        public string FirstName { get; set; }
        [Required]
        [MinLength(1)]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public Customer ToCustomer()
        {
            return new Customer {Email = Email, FirstName = FirstName, LastName = LastName};
        }
    }
}
