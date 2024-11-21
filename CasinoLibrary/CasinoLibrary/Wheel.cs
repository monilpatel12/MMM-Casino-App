using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasinoLibrary
{
    internal class Wheel
    {
        public int resultNum;
        public Outcome outcome;

        public Wheel(int num)
        {
            resultNum = num;
            outcome = new Outcome(num);
        }

        public void spin()
        {
            outcome.setOutcome(resultNum);
        }
    }
}
