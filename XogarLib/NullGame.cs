using System;

namespace XogarLib
{
    public class NullGame:Game
    {
        public NullGame() : base(-1) { }

        public override bool IsReal()
        {
            return false;
        }

        public override void Launch()
        {
            throw new NotImplementedException();
        }
    }
}
