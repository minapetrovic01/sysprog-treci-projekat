using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekat3
{
    class Result : IObserver<string>
    {
        public string content;
        public bool created;
        public Result()
        {
            content = "";
            created = false;
        }
        public void OnCompleted()
        {
            if(content == "")
            {
                content = "No results found";
            }
           created = true;
        }
        public void SetContent(string content)
        {
            this.content = content;
            created = true;
        }

        public void OnError(Exception error)
        {
            content += $"{error.Message}\n";
            created = true;
        }

        public void OnNext(string value)
        {
            content += $"{value}\n";
        }
    }
}
