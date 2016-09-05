using System;
using System.ComponentModel.DataAnnotations;

namespace NSwag.Integration.WebAPI.Models
{
    public class Person
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        [Required]
        public Address Address { get; set; }

        [Required]
        public Person[] Children { get; set; }
    }
}