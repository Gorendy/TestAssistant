using TesterHelper.exception;
using TesterHelper.util.logger;

namespace TesterHelper.util
{
    public class CommonUtil
    {
        public static bool checkParams(params object[] objs) {
            foreach (object o in objs) {
                if (o == null) {
                    return true;
                }
            }

            return false;
        }
    }
}