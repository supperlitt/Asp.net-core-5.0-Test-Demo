using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreTest5.Model
{
    [Table("userinfo2")]
    public class userinfo2
    {
        [Key]
        [Display(Name = "主键")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string user_name { get; set; }
    }
}