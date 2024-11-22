using System.ComponentModel.DataAnnotations;

namespace EmployeesApp.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "RFC is required.")]
        [RegularExpression(@"^[A-Z]{4}\d{6}[A-Z0-9]{3}$", ErrorMessage = "RFC must have a valid format.")]
        public string RFC { get; set; }

        [Required(ErrorMessage = "Born Date is required.")]
        public DateTime BornDate { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [EnumDataType(typeof(EmployeeStatus), ErrorMessage = "Invalid status.")]
        public EmployeeStatus Status { get; set; }
    }

    public enum EmployeeStatus
    {
        NotSet,
        Active,
        Inactive,
    }
}
