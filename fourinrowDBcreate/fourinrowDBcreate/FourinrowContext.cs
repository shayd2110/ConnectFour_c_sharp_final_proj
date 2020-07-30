using System.Data.Entity;

/*fourinrowDBcreate namespace*/
namespace fourinrowDBcreate
{
    /*fourinrowContext class*/
    public class FourinrowContext : DbContext
    {
        /*constructor*/
        public FourinrowContext(string databaseName) : base(databaseName) { }
        
        /*tables*/
        public DbSet<User> Users { get; set; }
        
        public DbSet<GameDetails> GamesHistory { get; set; }

        public DbSet<GameDetailsNow> GamesNow { get; set; }

    }/*end of -fourinrowContext- class*/

}/*end of -fourinrowDBcreate- namespace*/
