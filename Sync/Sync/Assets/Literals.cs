using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class Literals
{
    public static class IntLiterals
    {
        public static class Physics
        {
            public static class Layers
            {
                public static int floors = 8;
                public static int walls = 9;
                public static int obstacles = 10;
                public static int entities = 11;
                public static int players = 12;
                public static int projectiles = 13;
                public static int particles = 14;
                public static int graffiti = 15;
            }
        }
    }

    public static class StringLiterals
    {
        public static class Tags
        {
            public static string Floor = "floor";
            public static string Wall = "wall";
            public static string Player = "player";
            public static string NeroGraffitiController = "nero";
            public static string NeroGraffiti = "nero graffiti";
        }

        public static class Blackboard
        {
            public static string Synchroniser = "synchroniser";
        }
    }
}
