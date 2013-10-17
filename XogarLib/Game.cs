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

        /// <summary>
        /// Launches this game
        /// </summary>
        public abstract void Launch();

        /// <summary>
        /// Opens the store page associated with this game
        /// </summary>
        public abstract void Store();
    }
}
