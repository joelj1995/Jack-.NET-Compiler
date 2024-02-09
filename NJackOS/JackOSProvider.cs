using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NJackOS.Interface
{
    public class JackOSProvider
    {
        public static IJackOutput Output
        {
            get
            {
                if (jackOutput != null) return jackOutput;

                Assembly.Load("NJackOS.Implementation");

                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.Equals(Assembly.GetExecutingAssembly())))
                {
                    var assignable = assembly.GetTypes().FirstOrDefault(t => typeof(IJackOutput).IsAssignableFrom(t));

                    if (assignable != null)
                    {
                        jackOutput = (IJackOutput)Activator.CreateInstance(assignable);
                        return jackOutput;
                    }
                }

                throw new ApplicationException("Output is not implement");
            }
        }


        private static IJackOutput jackOutput;
    }
}
