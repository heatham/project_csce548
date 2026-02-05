-- 01_create_tables.sql
DROP TABLE IF EXISTS match_stats;
DROP TABLE IF EXISTS matches;
DROP TABLE IF EXISTS characters;
DROP TABLE IF EXISTS maps;
DROP TABLE IF EXISTS games;

CREATE TABLE games (
  game_id  SERIAL PRIMARY KEY,
  name     VARCHAR(80) UNIQUE NOT NULL
);

CREATE TABLE maps (
  map_id   SERIAL PRIMARY KEY,
  game_id  INT NOT NULL REFERENCES games(game_id) ON DELETE CASCADE,
  name     VARCHAR(80) NOT NULL,
  UNIQUE (game_id, name)
);

CREATE TABLE characters (
  character_id SERIAL PRIMARY KEY,
  game_id      INT NOT NULL REFERENCES games(game_id) ON DELETE CASCADE,
  name         VARCHAR(80) NOT NULL,
  role         VARCHAR(40),
  UNIQUE (game_id, name)
);

CREATE TABLE matches (
  match_id      SERIAL PRIMARY KEY,
  game_id       INT NOT NULL REFERENCES games(game_id) ON DELETE CASCADE,
  match_date    TIMESTAMP NOT NULL DEFAULT NOW(),
  queue_type    VARCHAR(40) NOT NULL,
  map_id        INT REFERENCES maps(map_id),
  result        CHAR(1) NOT NULL CHECK (result IN ('W','L')),
  duration_sec  INT CHECK (duration_sec >= 0),
  notes         TEXT
);

CREATE TABLE match_stats (
  stat_id             SERIAL PRIMARY KEY,
  match_id            INT NOT NULL REFERENCES matches(match_id) ON DELETE CASCADE,
  character_id        INT REFERENCES characters(character_id),
  kills               INT NOT NULL CHECK (kills >= 0),
  deaths              INT NOT NULL CHECK (deaths >= 0),
  assists             INT NOT NULL CHECK (assists >= 0),
  damage              INT CHECK (damage >= 0),
  healing             INT CHECK (healing >= 0),
  objective_time_sec  INT CHECK (objective_time_sec >= 0)
);

CREATE INDEX idx_matches_game_date ON matches(game_id, match_date DESC);
CREATE INDEX idx_stats_character ON match_stats(character_id);