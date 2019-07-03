using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Supex.Libraries.ETL;

namespace Supex.Libraries.Examples.Stages
{
    public class RestInputStage<TInput> : InputStage
    {
        private readonly string _uri;

        public RestInputStage(string name, string uri) : base(name)
        {
            _uri = uri;
        }

        public override async Task ExecuteAsync()
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(_uri);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var employees = JsonConvert.DeserializeObject<IEnumerable<TInput>>(content);

                var link = GetOutputLink();
                foreach (var employee in employees)
                {
                    link.Enqueue(employee);
                }

                link.Complete();
            }   
        }
    }
}
