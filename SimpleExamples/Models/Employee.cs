using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SimpleExamples.Models
{
    public class Employee
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("employee_name")]
        public string Name { get; set; }

        [JsonProperty("employee_salary")]
        public decimal Salary { get; set; }

        [JsonProperty("employee_age")]
        public int Age { get; set; }

        [JsonProperty("profile_image")]
        public string ProfileImage { get; set; }
    }
}
