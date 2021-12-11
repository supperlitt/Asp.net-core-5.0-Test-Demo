using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreTest5.Model
{
    [Table("userinfo")]
    public class userinfo
    {
        [Key]
        [Display(Name = "主键")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int userId { get; set; }

        public string user_name { get; set; }

        public List<cardinfo> cards { get; }

        public List<cardinfo> cards2 { get; }
    }
}