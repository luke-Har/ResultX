using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultMonad
{
    public class ResultNoValueException : Exception
    {
        public ResultNoValueException() : base("A value was attempted to be accessed where the Result contains an error") { }
    }
}
