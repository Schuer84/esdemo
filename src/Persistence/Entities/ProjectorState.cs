using System;
using System.Collections.Generic;
using System.Text;
using LiquidProjections.EntityFramework;

namespace SqlStreamStore.Demo.Persistence.Entities
{
    public class ProjectorState : IProjectorState
    {
        public string Id { get; set; }
        public long Checkpoint { get; set; }
        public DateTime LastUpdateUtc { get; set; }
    }
   
}
