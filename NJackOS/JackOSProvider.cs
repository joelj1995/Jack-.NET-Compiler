using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NJackOS.Interface
{
    public class JackOSProvider 
    {
        public static char CurrentKey => currentKeyListener.CurrentKey;

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

        public static IJackString String
        {
            get
            {
                if (jackString != null) return jackString;

                jackString = GetProvider<IJackString>();

                return jackString;
            }
        }

        public static IJackKeyboard Keyboard
        {
            get
            {
                if (jackKeyboard != null) return jackKeyboard;

                jackKeyboard = GetProvider<IJackKeyboard>();

                return jackKeyboard;
            }
        }

        public static IJackMath JMath
        {
            get
            {
                if (jackMath != null) return jackMath;

                jackMath = GetProvider<IJackMath>();

                return jackMath;
            }
        }

        public static IJackSys Sys
        {
            get
            {
                if (jackSys != null) return jackSys;

                jackSys = GetProvider<IJackSys>();

                return jackSys;
            }
        }

        private static T GetProvider<T>()
        {
            /*
             * Load the OS implementation and instantiate underlying objects with reflection.
             * 
             * Some standard dependency injection tooling could probably do this better,
             * but it gets the job done well enough.
             */

            if (!loaded)
            {
                /*
                 * In theory, this could be decoupled better by reading this name in as a configuration
                 * item from the environment variables or file system.
                 */
                Assembly.Load("NJackOS.Implementation");
                loaded = true;
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.Equals(Assembly.GetExecutingAssembly())))
            {
                var assignable = assembly.GetTypes().FirstOrDefault(t => typeof(T).IsAssignableFrom(t));

                if (assignable != null)
                {
                    var result = (T)Activator.CreateInstance(assignable);

                    if (typeof(ICurrentKeyObservable).IsAssignableFrom(assignable))
                    {
                        /*
                         * In the current implementation, this happens to be a shared
                         * concern with the IJackScreen provider. But ideally, JackOSProvider
                         * shouldn't have to have any knowledge of this. Again, some
                         * standard DI tooling could handle this more cleanly.
                         */
                        Console.WriteLine("Jack Current Key provider found.");
                        var keyObservable = (ICurrentKeyObservable)result;
                        keyObservable.Subscribe(currentKeyListener);
                    }

                    return result;
                }
            }

            throw new ApplicationException($"{typeof(T)} is not implemented.");
        }

        private static IJackOutput jackOutput;
        private static IJackScreen jackScreen;
        private static IJackMemory jackMemory;
        private static IJackArray jackArray;
        private static IJackString jackString;
        private static IJackKeyboard jackKeyboard;
        private static IJackMath jackMath;
        private static IJackSys jackSys;
        private static bool loaded = false;
        private static CurrentKeyListener currentKeyListener = new CurrentKeyListener();
    }
}
