using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace model_api.Models
{   [Table("Customer",Schema= "dbo")]
    public class Customer
    {   [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }
      
        [Column("name")]
        public string Name { get; set; }
        [Column("Email")]
        public string Email { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("Gender")]
        public string Gender { get; set; }

        [Column("state")]
        public string state { get; set; }


        [Column("BirthDate")]
        public DateTime BirthDate { get; set; }

    }
}
