using System;
using System.Data.Common;

namespace Systemic.Sif.Demo.Publishing.Database
{

    /// <summary>
    /// This class represents general database exceptions.
    /// </summary>
    class AgentDbException : DbException
    {

        /// <summary>
        /// Initialises a new instance of this class.
        /// </summary>
        public AgentDbException()
            : base()
        {
        }

        /// <summary>
        /// Initialises a new instance of this class with the specified error message.
        /// </summary>
        /// <param name="message">The message to display for this exception.</param>
        public AgentDbException(String message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialises a new instance of this class with the specified error message and a reference to the inner
        /// exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message string.</param>
        /// <param name="inner">The inner exception reference.</param>
        public AgentDbException(String message, Exception inner)
            : base(message)
        {
        }

    }

}
