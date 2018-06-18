using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stutton.DocumentCreator.Fields.List.Document
{
    public class MoveListFieldStepEventArgs : EventArgs
    {
        public int Direction { get; }

        public MoveListFieldStepEventArgs(MoveDirection direction)
        {
            Direction = direction;
        }

        public enum MoveDirection
        {
            Up,
            Down
        }
    }
}
