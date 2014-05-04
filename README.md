PersistObjectExtensions
=======================

This project implements Generic extensions on generic Object.

The SqlObjectExtension implements extension methods to POPULATE a instance of data class from SQL Database
and PERSIST (Insert/Update).

The FileObjectExtension implements extension methods to POPULATE a instance of data class from FILE and 
PERSIST (save on disk file).

SQL Examples:

    [Serializable]
    public class Person
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public String Email { get; set; }

    }
    
    
    ////////// Example /////////////
    
    Using System.Data.SqlClient;
    Using System.PersistObjectExtensions;
    
    SqlConnection _conn=new SqlConnection(ConnectionString);
    List<Person> people=new List<Person>();
    people.LoadFromQuery("select * from tb_users",_conn);  // Load from SQL DATABASE
    
    people[0].Name="John";
    people[0].SqlPersist(_conn,"tb_users"); // UPDATE SQL TABLE
    
    Person newperson=new Person();
    newperson.Name="Lucas";
    newperson.Email="lucas@luc.com";
    newperson.SqlPersist(_conn,"tb_users");// INSERT INTO SQL TABLE
