using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CTUpdater
{
    public class RandFact
    {
        #region Variables
        public List<string> Facts;
        private int factIndex;
        #endregion

        #region Constructor / Init
        public RandFact()
        {
            Init();
        }

        public void Init()
        {
            Facts = new List<string>();
            PopulateFacts();
            factIndex = 0;
        }

        public void PopulateFacts()
        {
            Facts.Clear();
            Facts.Add("A Rubik's Cube has over 43,000,000,000,000,000,000 possible configurations.");
            Facts.Add("A duel between three people is called a 'truel'.");
            Facts.Add("On average, an American will watch over 7 years of television in his/her lifetime.");
            Facts.Add("A fear of wines is known as Oenophobia.");
            Facts.Add("In England, the Speaker of the House is not allowed to speak during in a debate.");
            Facts.Add("Ribbon worms will eat themselves if they can't find any food.");
            Facts.Add("More people in China speak English than in the United States.");
            Facts.Add("You share your birthday with at least 9 million other people in the world.");
            Facts.Add("The average galaxy contains between 100 billion, and 1 trillion stars.");
            Facts.Add("The speed of a typical raindrop is 17 miles per hour.");
            Facts.Add("On average, a person will spend about five years eating during his or her lifetime.");
            Facts.Add("Hypnotism is banned by public schools in San Diego.");
            Facts.Add("Rougly 90% of people who sit at their computer desk do so slouching, supporting their head with their left hand.");
            Facts.Add("Buttermilk contains no butter.");
            Facts.Add("If your stomach didn't produce a new layer of mucous every two weeks, it would digest itself.");
            Facts.Add("Muhammad is the most common name in the world.");
            Facts.Add("The most expensive material on Earth is worth $62.5 trillion per gram - it's anti-matter.");
            Facts.Add("Panophobia is the fear of everything.");
            Facts.Add("The average person laughs 13 times a day.");
            //Room to add more.










            Shuffler.Shuffle<string>(Facts);
        }
        #endregion

        #region Get Fact

        public string GetNextFact()
        {
             if (factIndex >= Facts.Count - 1)
                factIndex = 0;
            else
                factIndex++;

            return Facts[factIndex];
        }

        #endregion


    }

    public static class Shuffler
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
