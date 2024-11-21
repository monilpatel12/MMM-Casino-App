using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasinoLibrary
{
    enum Colour { red, black, green };
    enum Type { odd, even, zero };
    
    internal class Outcome
    {
        public int number;
        public Colour colour;
        public Type type;

        public Outcome(int resultNum)
        {
            number = resultNum;
            determineColour();
            determineType();
        }

        internal void setOutcome(int resultNum)
        {
            number = resultNum;
            determineColour();
            determineType();
        }

        void determineColour()
        {
            if (number == 0)
            {
                colour = Colour.green;
            }
            else if (number > 0 && number < 10)
            {
                if (number % 2 != 0)
                {
                    colour = Colour.red;
                }
                else
                {
                    colour = Colour.black;
                }
            }
            else if (number < 12)
            {
                colour = Colour.black;
            }
            else if (number < 19)
            {
                if (number % 2 != 0)
                {
                    colour = Colour.black;
                }
                else
                {
                    colour = Colour.red;
                }
            }
            else if (number < 28)
            {
                if (number % 2 != 0)
                {
                    colour = Colour.red;
                }
                else
                {
                    colour = Colour.black;
                }
            }
            else if (number < 30)
            {
                colour = Colour.black;
            }
            else
            {
                if (number % 2 != 0)
                {
                    colour = Colour.black;
                }
                else
                {
                    colour = Colour.red;
                }
            }
        }

        void determineType()
        {
            if (number == 0) 
            { 
                type = Type.zero;
            }
            else if (number % 2 == 0)
            {
                type = Type.even;
            }
            else
            {
                type = Type.odd;
            }
        }
    }
}
