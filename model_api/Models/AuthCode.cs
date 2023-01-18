using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace model_api.Models
{
    public class AuthCode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public int Id { get; set; }

        [Column("ExDate")]
        public DateTime ExDate { get; set; }
     
        [Column("code")]
        public string code { get; set; }

        public int Customer_ID { get; set; }
        [ForeignKey("Customer_ID")]
        public Customer Customer { get; set; }
    }
}
