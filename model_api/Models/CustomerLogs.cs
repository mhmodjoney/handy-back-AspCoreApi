using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace model_api.Models
{
    public class CustomerLogs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("Date")]
        public DateTime Date { get; set; }
        [Column("state")]
        public string state { get; set; }
       
        public int Customer_ID { get; set; }
        [ForeignKey("Customer_ID")]
        public  Customer Customer { get; set; }
    }
}
