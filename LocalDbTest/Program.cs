using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace LocalDbTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //try
            {
                new Program().PeopleOrderedByCountry();
            }
            //catch (Exception e)
            //{
                //Console.WriteLine(e);
            //}
        }


        public void PeopleOrderedByCountry()
        {
            StringBuilder sb = new StringBuilder();

            var people = GetPeople();
            foreach (var p in people.OrderBy(x => x.CountryName).ThenBy(x => x.LastName))
            {
                sb.Append(p.ToString());
            }

            Console.WriteLine(sb.ToString());
        }


        public List<Person> GetPeople()
        {
            using (SqlConnection sc = new SqlConnection(_localDb))
            {
                sc.Open();

                string sql = "SELECT p.personid, p.firstname, p.lastname, c.countryname " +
                             "FROM   person p inner join country c on p.countryid = c.countryid";

                List<Person> people = new List<Person>();
                using (SqlCommand command = new SqlCommand(sql, sc))
                {
                    using (SqlDataReader sqlDataReader = command.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                        {
                            people.Add( new Person(sqlDataReader));
                        }
                        return people;
                    }
                }
            }
        }

        private string _localDb
        {
            get
            {
                var path = new FileInfo(System.IO.Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "..", "data"));
                var datafile = path.FullName + "\\peopledb.mdf";

                return "Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;AttachDbFileName=" + datafile;
            }
        }
    }



    public class Person
    {
        public int PersonID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CountryID { get; set; }
        public string CountryName { get; set; }

        public Person(SqlDataReader sqlDataReader)
        {
            this.PersonID = sqlDataReader.GetInt32(0);
            this.FirstName = sqlDataReader.GetString(1);
            this.LastName = sqlDataReader.GetString(2);
            this.CountryName = sqlDataReader.GetString(3);
        }

        public override string ToString()
        {
            return this.CountryName + " - " + this.FirstName + " " + this.LastName + Environment.NewLine;
        }
    }
}
