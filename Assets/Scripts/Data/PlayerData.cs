using System;

[Serializable]
public class PlayerData
{
    public string id;
    public string nickname;
    public int score;

    [NonSerialized]
    public int rank;

    public bool IsCurrentPlayer => id == "me";

    public PlayerData(string id, string nickname, int score)
    {
        this.id = id;
        this.nickname = nickname;
        this.score = score;
    }

    public PlayerData Clone()
    {
        return new PlayerData(id, nickname, score);
    }
}
