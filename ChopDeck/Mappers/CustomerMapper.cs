using ChopDeck.Dtos.Customers;
using ChopDeck.Models;

namespace ChopDeck.Mappers
{
    public static class CustomerMapper
    {
        public static CustomerDto? ToCustomerDto(this Customer customer)
        {
            if (customer == null)
            {
                return null;
            }

            return new CustomerDto
            {
                Id = customer.Id,
                UserType = customer.ApplicationUser.UserType,
                Name = customer.ApplicationUser.Name,
                Address = customer.ApplicationUser.Address,
                Email = customer.ApplicationUser.Email,
                Lga = customer.ApplicationUser.Lga,
                State = customer.ApplicationUser.State,
                CreatedAt = customer.ApplicationUser.CreatedAt,
            };
        }
    }
}