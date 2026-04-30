using System;
using System.Collections.Generic;
using IFix;

[Configure]
public class InterpertConfig
{
    [IFix]
    static IEnumerable<Type> ToProcess
    {
        get
        {
            var types = new List<Type>();
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetName().Name == "Plugins" || assembly.GetName().Name == "Main")
                {
                    try
                    {
                        types.AddRange(assembly.GetTypes());
                    }
                    catch { }
                }
            }
            return types;
        }
    }
}