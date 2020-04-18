using System;

namespace LiquidProjections.EntityFramework
{
    public interface IProjectorState
    {
        string Id { get; set; }
        long Checkpoint { get; set; }
        DateTime LastUpdateUtc { get; set; }
    }

    public enum PersistStateBehavior
    {
        /// <summary>
        /// Persist the state to the database after every batch of transactions
        /// </summary>
        EveryBatch,

        /// <summary>
        /// Persist the state to the database only when an event in the batch of transanctions was handled by the projector and
        /// at the end of the collection of transactions
        /// </summary>
        DirtyBatch
    }
}
