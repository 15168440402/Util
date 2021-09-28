using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Util.ORM.InteractionLayer.SqlCmd
{
    public abstract class JoinFromBase
    {
        public IEnumerable<JoinInfo> JoinInfos { get; init; }
        public JoinFromBase(IEnumerable<JoinInfo> joinInfos)
        {
            JoinInfos = joinInfos;
        }
    }
    public class JoinInfo
    {
        public JoinInfo(JoinType joinType, Expression<Func<bool>> on) => (JoinType, On) = (joinType, on);
        public JoinType JoinType { get; init; }
        public Expression<Func<bool>> On { get; init; }
    }
    public enum JoinType : byte
    {
        InnerJoin,
        LeftJoin,
        RightJoin,
        FullJoin
    }  
    
    public class Join
    {
        internal Join()
        {

        }
        //List<JoinInfo> JoinInfos { get; } = new List<JoinInfo>();
        public Join InnerJoin(bool on)
        {
            //JoinInfos.Add(new JoinInfo(JoinType.InnerJoin, on));
            return this;
        }
        public Join LeftJoin(bool on)
        {
            //JoinInfos.Add(new JoinInfo(JoinType.LeftJoin,on));
            return this;
        }
        public Join RightJoin(bool on)
        {
            //JoinInfos.Add(new JoinInfo(JoinType.RightJoin, on));
            return this;
        }
        public Join FullJoin(bool on)
        {
            //JoinInfos.Add(new JoinInfo(JoinType.FullJoin, on));
            return this;
        }
        //public List<JoinInfo> GetJoinInfos() => JoinInfos;
    }
}
