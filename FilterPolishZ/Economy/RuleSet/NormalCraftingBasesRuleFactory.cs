﻿using FilterEconomy.Processor;
using FilterPolishUtil.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterPolishZ.Economy.RuleSet
{
    public class NormalCraftingBasesRuleFactory
    {
        public static FilterEconomyRuleSet Generate(ConcreteEconomyRules ruleHost, string segment)
        {
            float valueMultiplierEffectiveness = 0.4f;

            var builder = new RuleSetBuilder(ruleHost)
                .SetSection(segment)
                .UseDefaultQuery()
                .AddDefaultPostProcessing()
                .AddDefaultIntegrationTarget();

            builder.AddRule("ANCHOR", "ANCHOR",
                new Func<string, bool>((string s) =>
                {
                    return builder.RuleSet.DefaultSet.HasAspect("AnchorAspect");
                }));

            builder.AddRule("unknown", "unknown",
                new Func<string, bool>((string s) =>
                {
                    return !ruleHost.EconomyInformation.EconomyTierlistOverview[segment].ContainsKey(s);
                }));

            builder.AddRule("t1-86", "t1-1",
                new Func<string, bool>((string s) =>
                {
                    if (builder.RuleSet.DefaultSet.ValueMultiplier < 0.8f)
                    {
                        return false;
                    }

                    var price = Math.Max(GetPrice(86), GetPrice(85)) * (1 + ((builder.RuleSet.DefaultSet.ValueMultiplier - 1) * valueMultiplierEffectiveness));
                    return price > FilterPolishConstants.T1BaseTypeBreakPoint;
                }), nextgroup: "t2");

            builder.AddRule("t2-84", "t2-1",
                new Func<string, bool>((string s) =>
                {
                    var price = GetPrice(84) * (1 + ((builder.RuleSet.DefaultSet.ValueMultiplier - 1) * valueMultiplierEffectiveness));
                    return price > FilterPolishConstants.T2BaseTypeBreakPoint;
                }), group: "t2");


            builder.AddRule("t2-86", "t2-2",
                new Func<string, bool>((string s) =>
                {
                    var price = Math.Max(GetPrice(86), GetPrice(85)) * (1 + ((builder.RuleSet.DefaultSet.ValueMultiplier - 1) * valueMultiplierEffectiveness));
                    return price > FilterPolishConstants.T2BaseTypeBreakPoint;
                }));

            builder.AddRule("rest", "rest",
                new Func<string, bool>((string s) =>
                {
                    return true;
                }));

            return builder.Build();

            float GetPrice(int level)
            {
                if (level > 86)
                {
                    return 0;
                }

                if (builder.RuleSet.DefaultSet.ftPrice.ContainsKey(level))
                {
                    return builder.RuleSet.DefaultSet.ftPrice[level];
                }
                else
                {
                    return GetPrice(level + 1);
                }
            }
        }
    }
}
