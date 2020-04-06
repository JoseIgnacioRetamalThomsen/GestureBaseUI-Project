using FLS;
using FLS.Rules;


namespace GestureBaseUI_Project
{
    /// <summary>
    /// Calculate the mouse speed using the variation of movement of the hand/
    /// </summary>
    public class FuzzyMouseSpeed
    {
        /// <summary>
        /// The fuzzy logic engine
        /// </summary>
        IFuzzyEngine fuzzyEngine;

        public FuzzyMouseSpeed()
        {
          
        }

        /// <summary>
        /// Returns the mouse speed given the hand movement variation.
        /// </summary>
        /// <param name="value">variation of movement</param>
        /// <returns></returns>
        public double Get(float value)
        {
            //create fuzzy engine
            fuzzyEngine = new FuzzyEngineFactory().Default();

            // movement fuzzy sets
            var variation = new LinguisticVariable("Variation");
            var litle = variation.MembershipFunctions.AddTrapezoid("Litle", 0, 0, 40, 80);
            var medium = variation.MembershipFunctions.AddTriangle("Medium", 30, 90, 110);
            var big = variation.MembershipFunctions.AddTrapezoid("Big", 75, 110, 400, 400);

            // speed fuzzy sets
            var speed = new LinguisticVariable("Speed");
            var slow = speed.MembershipFunctions.AddTrapezoid("slow", 0, 0,5, 10);
            var normal = speed.MembershipFunctions.AddTrapezoid("normal", 10, 20, 30, 40);
            var fast = speed.MembershipFunctions.AddTriangle("fast", 40, 70, 70);
            
            //fuzzy rules
            var rule1 = Rule.If(variation.Is(litle)).Then(speed.Is(slow));
            var rule2 = Rule.If(variation.Is(medium)).Then(speed.Is(normal));
            var rule3 = Rule.If(variation.Is(big)).Then(speed.Is(fast));

            // add rules
            fuzzyEngine.Rules.Add(rule1, rule2, rule3);

            //defuzify and return results
            return fuzzyEngine.Defuzzify(new { variation = (int)value });
        }
    }
}

