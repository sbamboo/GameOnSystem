using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOnSystem {
    internal class AttempedRemoveOnProtected : Exception {
        // Parameterless constructor
        public AttempedRemoveOnProtected() : base("Attempted remove on protected entry!") {
        }

        // Constructor that takes a custom message
        public AttempedRemoveOnProtected(string message) : base(message) {
        }

        // Constructor that takes a custom message and an inner exception
        public AttempedRemoveOnProtected(string message, Exception innerException)
            : base(message, innerException) {
        }

        // Constructor for serializations
        protected AttempedRemoveOnProtected(System.Runtime.Serialization.SerializationInfo info,
                                  System.Runtime.Serialization.StreamingContext context)
            : base(info, context) {
        }
    }
}
