using System;
using Appalachia.Utility.Logging;

namespace Appalachia.CI.Constants
{
    public class AppaContext
    {
        public AppaContext(Type t)
        {
            _ownerType = t;
        }

        public AppaContext(object owner)
        {
            if (owner is Type t)
            {
                _ownerType = t;
            }
            else
            {
                _ownerType = owner.GetType();
            }
        }

        #region Fields and Autoproperties

        private AppaLogContext _log;

        private Type _ownerType;

        #endregion

        public AppaLogContext Log
        {
            get
            {
                if (_log == null)
                {
                    _log = AppaLog.Context.GetByType(_ownerType);
                }

                return _log;
            }
        }
    }

    public class AppaContext<T>
    {
        #region Fields and Autoproperties

        private AppaLogContext _log;

        #endregion

        public AppaLogContext Log
        {
            get
            {
                if (_log == null)
                {
                    _log = AppaLog.Context.GetByType<T>();
                }

                return _log;
            }
        }
    }
}
