using System.ComponentModel.DataAnnotations;
using ChopDeck.Enums;
using ChopDeck.Validation;

namespace ChopDeck.helpers
{
    public class CustomerOrdersQueryObject : PaginationQueryObject
    {
        [Required]
        [EnumValueValidation(typeof(CustomerOrderStatus), ErrorMessage = "Invalid status. Allowed values are Pending, Ongoing, Completed, Cancelled.")]
        public string Status { get; set; } = CustomerOrderStatus.Pending.ToString();
    }
}