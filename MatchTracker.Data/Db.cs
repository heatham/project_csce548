using Npgsql;

namespace MatchTracker;

public static class Db
{
    // Fill in YOURPASS with the password you set for the postgres user.
    private const string ConnectionString =
        "Host=localhost;Port=5432;Username=postgres;Password=Winner29;Database=postgres";

    public static NpgsqlConnection Open()
    {
        var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();
        return conn;
    }
}
