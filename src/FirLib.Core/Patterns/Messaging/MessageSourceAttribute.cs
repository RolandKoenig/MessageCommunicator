using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirLib.Core.Patterns.Messaging;

public class MessagePossibleSourceAttribute : Attribute
{
    public string[] PossibleSourceMessengers { get; }

    public MessagePossibleSourceAttribute(params string[] possibleSourceMessengers)
    {
        this.PossibleSourceMessengers = possibleSourceMessengers;
    }
}