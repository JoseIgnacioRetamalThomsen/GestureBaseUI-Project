using FLS;
using FLS.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestureBaseUI_Project
{
    public class FuzzyMouseSpeed
    {
        IFuzzyEngine fuzzyEngine;

        public FuzzyMouseSpeed()
        {
          
        }

        public double Get(float value)
        {
            fuzzyEngine = new FuzzyEngineFactory().Default();

            var variation = new LinguisticVariable("Variation");
            var litle = variation.MembershipFunctions.AddTrapezoid("Litle", 0, 0, 40, 80);
            var medium = variation.MembershipFunctions.AddTriangle("Medium", 30, 90, 110);
            var big = variation.MembershipFunctions.AddTrapezoid("Big", 75, 110, 400, 400);

            var speed = new LinguisticVariable("Speed");
            var slow = speed.MembershipFunctions.AddTrapezoid("slow", 0, 0,5, 10);
            var normal = speed.MembershipFunctions.AddTrapezoid("normal", 10, 20, 30, 40);
            var fast = speed.MembershipFunctions.AddTriangle("fast", 40, 70, 70);



            var rule1 = Rule.If(variation.Is(litle)).Then(speed.Is(slow));
            var rule2 = Rule.If(variation.Is(medium)).Then(speed.Is(normal));
            var rule3 = Rule.If(variation.Is(big)).Then(speed.Is(fast));

            fuzzyEngine.Rules.Add(rule1, rule2, rule3);
            return fuzzyEngine.Defuzzify(new { variation = (int)value });
        }
    }
}

