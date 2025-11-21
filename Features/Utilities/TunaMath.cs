using Godot;
using System;
namespace Utilities
{
    public class TunaMath
    {
        public static double USecToSec(ulong usec)
        {
            return usec / 1_000_000.0;
        }
        public static ulong SecToUSec(double sec)
        {
            return (ulong)(sec * 1_000_000.0);
        }
    }
}