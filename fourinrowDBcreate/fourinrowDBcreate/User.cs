using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/*fourinrowDBcreate namespace*/
namespace fourinrowDBcreate
{
    /*User class*/
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string UserName { get; set; }
        
        public string HashedPassword { get; set; }

    }/*end of -User- class*/

}/*end of -fourinrowDBcreate- namespace*/
