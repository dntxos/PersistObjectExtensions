using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.PersistObjectExtension;

namespace DemoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Pessoa kazuo = new Pessoa();
            kazuo.Nome = "Rafael Kazuo";
            kazuo.Email = "dntxos@gmail.com";
            kazuo.FilePersist(".\\t.txt");

            Pessoa nova = new Pessoa();
            nova.LoadFromFile(".\\t.txt");


            //String connectionString = "";
            //SqlObjectExtension.SqlConnection = new System.Data.SqlClient.SqlConnection(connectionString);

            //kazuo.SqlPersist("tb_pessoas");

            //Pessoa novofromdb = new Pessoa();
            //novofromdb.LoadFromQuery("select top 1 * from tb_pessoas");

            //List<Pessoa> todosfromdb = new List<Pessoa>();
            //todosfromdb.LoadFromQuery("select * from tb_pessoas");

        }
    }
}
