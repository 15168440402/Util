using System.Collections.Generic;

namespace Util.ORM.InteractionLayer.SqlCmd
{
    public class JoinFrom<T1, T2> : JoinFromBase
    {
        public JoinFrom(JoinInfo joinInfo) : base(new[] { joinInfo })
        {

        }
    }
    public class JoinFrom<T1, T2, T3> : JoinFromBase
    {
        public JoinFrom(IEnumerable<JoinInfo> joinInfos) : base(joinInfos)
        {

        }
    }
    public class JoinFrom<T1, T2, T3, T4> : JoinFromBase
    {
        public JoinFrom(IEnumerable<JoinInfo> joinInfos) : base(joinInfos)
        {

        }
    }
    public class JoinFrom<T1, T2, T3, T4, T5>
    {
    }
    public class JoinFrom<T1, T2, T3, T4, T5, T6>
    {
    }
    public class JoinFrom<T1, T2, T3, T4, T5, T6, T7>
    {
    }
    public class JoinFrom<T1, T2, T3, T4, T5, T6, T7, T8>
    {
    }
    public class JoinFrom<T1, T2, T3, T4, T5, T6, T7, T8, T9>
    {
    }


}
