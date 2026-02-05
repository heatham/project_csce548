-- 02_seed_data.sql

INSERT INTO games(name) VALUES ('Avoid Duplicates Demo Game')
ON CONFLICT (name) DO NOTHING;

-- Put "Marvel Rivals" here if you want; leaving neutral name avoids any naming mismatch.
-- You can rename to Marvel Rivals:
-- UPDATE games SET name='Marvel Rivals' WHERE name='Avoid Duplicates Demo Game';

-- Maps
INSERT INTO maps(game_id, name)
SELECT game_id, m
FROM games, (VALUES
  ('Map A'), ('Map B'), ('Map C')
) AS t(m)
WHERE name = 'Avoid Duplicates Demo Game'
ON CONFLICT DO NOTHING;

-- Characters
INSERT INTO characters(game_id, name, role)
SELECT game_id, c, r
FROM games, (VALUES
  ('Character 1','DPS'),
  ('Character 2','Tank'),
  ('Character 3','Support'),
  ('Character 4','DPS')
) AS t(c,r)
WHERE name = 'Avoid Duplicates Demo Game'
ON CONFLICT DO NOTHING;

-- 50 matches
INSERT INTO matches(game_id, match_date, queue_type, map_id, result, duration_sec, notes)
SELECT
  g.game_id,
  NOW() - (i || ' days')::interval,
  CASE WHEN i % 3 = 0 THEN 'Ranked' ELSE 'Quickplay' END,
  (SELECT map_id FROM maps WHERE game_id = g.game_id ORDER BY map_id LIMIT 1 OFFSET (i % 3)),
  CASE WHEN i % 2 = 0 THEN 'W' ELSE 'L' END,
  600 + (i * 5),
  CASE WHEN i % 10 = 0 THEN 'Close match' ELSE NULL END
FROM games g
CROSS JOIN generate_series(1,50) AS i
WHERE g.name = 'Avoid Duplicates Demo Game';

-- 50 stats (one row per match)
INSERT INTO match_stats(match_id, character_id, kills, deaths, assists, damage, healing, objective_time_sec)
SELECT
  m.match_id,
  (SELECT character_id FROM characters WHERE game_id = m.game_id ORDER BY character_id LIMIT 1 OFFSET (m.match_id % 4)),
  (m.match_id % 20),
  (m.match_id % 10),
  (m.match_id % 15),
  2000 + (m.match_id * 30),
  500 + (m.match_id * 10),
  30 + (m.match_id % 120)
FROM matches m
JOIN games g ON g.game_id = m.game_id
WHERE g.name = 'Avoid Duplicates Demo Game'
ORDER BY m.match_id DESC
LIMIT 50;
