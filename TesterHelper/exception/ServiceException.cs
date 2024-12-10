using System;
using TesterHelper.util.logger;

namespace TesterHelper.exception
{
    public class ServiceException : Exception
    {
        private static string msg;
        public ServiceException(RCode code, Exception ex) :base(msg, ex) {
            msg = Result.detail(code).ToString();
        }
        public ServiceException(RCode code) :base(msg) {
            msg = Result.detail(code).ToString();
        }

        public ServiceException(string mg) : base(mg) {
            
        }
        public ServiceException(string mg, Exception e) : base(mg, e) {
            
        }
    }
}