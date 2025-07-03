using System;
using System.Collections.Generic;

[Serializable]
public class TournamentData
{
    public List<PlayerData> players;

    public TournamentData()
    {
        players = new List<PlayerData>();
    }
}
