/*fourinrowDBcreate namespace*/
namespace fourinrowDBcreate
{
    /*Program class*/
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog= fourinrowDB_shay_shalev;AttachDbFilename=C:\fourinrow\databases\fourinrowDB_shay_shalev.mdf;Integrated Security=True";
            
            using (var ctx = new FourinrowContext(connectionString))
            {
                ctx.Database.Initialize(force:true);
            }
        }

    }/*end of -fourinrowDBcreate- namespace*/

}/*end of -Program- class*/
