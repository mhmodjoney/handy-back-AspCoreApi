using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace model_api.Models
{
    public class Payment_info
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Date")]
        public DateTime Date { get; set; }
        [Column("product_id")]
        public int? product_id { get; set; }
        [Column("type")]
        public string type { get; set; }
        [Column("description")]
        public string description { get; set; }
        [Column("amount")]
        public long amount { get; set; }
        [Column("name")]
        public string name { get; set; }
        [Column("state")]
        public string state { get; set; }

        [Column("Quantity")]
        public int Quantity { get; set; }


        [Column("payment_method")]
        public string Payment_method { get; set; }

        public int Customer_ID { get; set; }
        [ForeignKey("Customer_ID")]
        public Customer Customer { get; set; }

    }
}
