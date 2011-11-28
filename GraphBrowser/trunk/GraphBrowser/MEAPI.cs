using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetaCase.GraphBrowser
{
    public class MEAPI : MetaEditAPI.MetaEditAPI
    {
        public MEAPI() 
            : base()
        { }

        public MEAPI(String Url)
            : base()
        {
            this.Url = Url;
        }

        public MEAPI(String Url, int _timeout) 
            : base()
        {
            this.Url = Url;
            this.Timeout = _timeout;
        }

        public void _SetTimeOut(int _timeout)
        {
            this.Timeout = _timeout;
        }

        protected object[] Invoke2(string methodName, object[] parameters)
        {
            try
            {
                return base.Invoke(methodName, parameters);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
