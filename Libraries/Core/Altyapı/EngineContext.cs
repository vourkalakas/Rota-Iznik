using System.Configuration;
using System.Runtime.CompilerServices;
using Core.Yapılandırma;

namespace Core.Altyapı
{
    public class EngineContext
    {
        #region methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEngine Create()
        {
            if (Singleton<IEngine>.Instance == null)
                Singleton<IEngine>.Instance = new Engine();

            return Singleton<IEngine>.Instance;
        }

        public static void Replace(IEngine engine)
        {
            Singleton<IEngine>.Instance = engine;
        }

        #endregion

        #region Properties

        public static IEngine Current
        {
            get
            {
                if (Singleton<IEngine>.Instance == null)
                {
                    Create();
                }
                return Singleton<IEngine>.Instance;
            }
        }

        #endregion
    }
}

