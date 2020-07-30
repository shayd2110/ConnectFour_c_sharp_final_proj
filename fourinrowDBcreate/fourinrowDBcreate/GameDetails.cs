using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/*fourinrowDBcreate namespace*/
namespace fourinrowDBcreate
{
    /*GameDetails class*/
    public class GameDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GameId { get; set; }

        public string User1Name { get; set; }

        public string User2Name { get; set; }

        public string WinningUserName { get; set; }

        public int PointsUser1 { get; set; }

        public int PointsUser2 { get; set; }

        public DateTime GameDateStart { get; set; }

        public DateTime GameDateEnd { get; set; }

    }/*end of -GameDetails- class*/

}/*end of -fourinrowDBcreate- namespace*/
