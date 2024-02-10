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

                jackOutput = GetProvider<IJackOutput>();

                return jackOutput;
            }
        }

        public static IJackScreen Screen
        {
            get
            {
                if (jackScreen != null) return jackScreen;

                jackScreen = GetProvider<IJackScreen>();

                return jackScreen;
            }
        }

        public static IJackMemory Memory
        {
            get
            {
                if (jackMemory != null) return jackMemory;

                jackMemory = GetProvider<IJackMemory>();

                return jackMemory;
            }
        }

        public static IJackArray Array
        {
            get
            {
                if (jackArray != null) return jackArray;

                jackArray = GetProvider<IJackArray>();

                return jackArray;
            }
        }

        private static T GetProvider<T>()
        {
            if (!loaded)
            {
                Assembly.Load("NJackOS.Implementation");
                loaded = true;
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.Equals(Assembly.GetExecutingAssembly())))
            {
                var assignable = assembly.GetTypes().FirstOrDefault(t => typeof(T).IsAssignableFrom(t));

                if (assignable != null)
                {
                    var result = (T)Activator.CreateInstance(assignable);
                    return result;
                }
            }

            throw new ApplicationException($"{typeof(T)} is not implemented.");
        }


        private static IJackOutput jackOutput;
        private static IJackScreen jackScreen;
        private static IJackMemory jackMemory;
        private static IJackArray jackArray;
        private static bool loaded = false;
    }
}
