using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class Faction
{
    public enum FactionName { Performers, Monoliths, PerformersTeamA, PerformersTeamB };
    public FactionName name = FactionName.Performers;

    public FactionName[] allies;
    public FactionName[] neutrals;
    public FactionName[] enemies;

    public bool isHostile(Faction faction)
    {
        return enemies.Contains(faction.name);
    }

    public bool isAllied(Faction faction)
    {
        return allies.Contains(faction.name);
    }

    public bool isNeutrals(Faction faction)
    {
        return neutrals.Contains(faction.name);
    }
}