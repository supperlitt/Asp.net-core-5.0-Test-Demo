using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreTest5.Model
{
    [Table("card2")]
    public class cardinfo2
    {
        [Key]
        [Display(Name = "主键")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string card_num { get; set; }

        public int open_id { get; set; }

        public userinfo2 open { get; set; }

        public int add_id { get; set; }

        public userinfo2 add { get; set; }
    }
}