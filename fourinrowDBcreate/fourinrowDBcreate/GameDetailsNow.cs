using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/*fourinrowDBcreate namespace*/
namespace fourinrowDBcreate
{
    /*GameDetailsNow class*/
    public class GameDetailsNow
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GameNowId { get; set; }

        public string User1Name { get; set; }

        public string User2Name { get; set; }

        public DateTime GameDateStart { get; set; }

    }/*end of -GameDetailsNow- class*/

}/*end of -fourinrowDBcreate- namespace*/
