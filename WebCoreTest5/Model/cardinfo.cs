using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebCoreTest5.Model
{
    [Table("card")]
    public class cardinfo
    {
        [Key]
        [Display(Name = "主键")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int cardId { get; set; }

        public string card_num { get; set; }

        public int openId { get; set; }

        public userinfo open { get; set; }

        public int addId { get; set; }

        public userinfo add { get; set; }
    }
}