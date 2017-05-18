using System;
using System.ComponentModel.DataAnnotations;

namespace FIX.Web.Models
{
    public class UserModel
    {
        public Int64 ID { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string Address { get; set; }
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        [Display(Name = "Created Date")]
        public DateTime CreatedTimestamp { get; set; }
    }
}