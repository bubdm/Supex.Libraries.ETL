﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleExamples.Models;
using Supex.Libraries.ETL;

namespace SimpleExamples.Stages
{
    public class ConsoleOutputStage : OutputStage<Employee>
    {
        public ConsoleOutputStage(string name) : base(name)
        {
        }

        public override Task ExecuteAsync()
        {
            var link = GetInputLink();
            Employee employee;

            while ((employee = link.Dequeue()) != null)
            {
                Console.WriteLine($"{Name} - {employee.Id} {employee.Name} {employee.Salary} {employee.Age}");
            }

            return Task.CompletedTask;
        }
    }
}
