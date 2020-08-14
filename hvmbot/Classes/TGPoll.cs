using System;
using System.Collections.Generic;
using System.Text;

namespace hvmbot.Classes
{
    public class TGPoll
    {
        public string Question { get; set; }
        public IEnumerable<string> Answers { get; set; }
        public TGPoll(string question, IEnumerable<string> answers)
        {
            this.Question = question;
            this.Answers = answers;
        }
    }
}
