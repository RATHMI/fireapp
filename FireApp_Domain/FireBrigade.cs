using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireApp.Domain
{
    public class FireBrigade
    {
        private FireBrigade() {}

        public FireBrigade(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a string with the names of the CSV values</returns>
        public static string GetCsvHeader()
        {
            return "id;name";
        }

        /// <summary>
        /// Turns this FireBrigade into a CSV line
        /// </summary>
        /// <returns>returns a CSV line with the values of the FireBrigade</returns>
        public string ToCsv()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Id);
            sb.Append(';');
            sb.Append(Name);           

            return sb.ToString();
        }

        /// <summary>
        /// This method turns a line of a CSV-File into a new FireBrigade
        /// </summary>
        /// <param name="csv">a line of a CSV-File you want to convert</param>
        /// <returns>returns a new FireBrigade or null if an error occures</returns>
        public static FireBrigade GetFireBrigadeFromCsv(string csv)
        {
            string[] values;

            if (csv != null)
            {
                try
                {
                    values = csv.Split(';');
                    FireBrigade fb = new FireBrigade(Convert.ToInt32(values[0]), values[1]);
                    return fb;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }
    }
}
