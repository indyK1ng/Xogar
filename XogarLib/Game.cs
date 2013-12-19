using System;

namespace XogarLib
{
    public abstract class Game
    {
        protected Int64 gameId;

        public Game(Int64 gameId)
        {
            this.gameId = gameId;
        }

        public virtual bool IsReal()
        {
            return true;
        }

        public abstract void Launch();

        public abstract String Hash();
    }
}
