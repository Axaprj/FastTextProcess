--
-- File generated with SQLiteStudio v3.2.1 on Вс ноя 4 13:54:21 2018
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: Dict
CREATE TABLE Dict (Id INTEGER PRIMARY KEY AUTOINCREMENT, Word TEXT UNIQUE NOT NULL, Vect BLOB NOT NULL);

-- Table: DictAddins
CREATE TABLE DictAddins (Id INTEGER PRIMARY KEY AUTOINCREMENT, Word TEXT UNIQUE NOT NULL, Vect BLOB NOT NULL);

-- Table: EmbedDict
CREATE TABLE EmbedDict (Inx INTEGER UNIQUE PRIMARY KEY NOT NULL, DictId INTEGER REFERENCES Dict (Id) ON DELETE SET NULL ON UPDATE CASCADE, DictAddinsId INTEGER REFERENCES DictAddins (Id) ON DELETE SET NULL ON UPDATE CASCADE, Freq INTEGER NOT NULL);

-- Index: inxWordDict
CREATE INDEX inxWordDict ON Dict (Word);

-- Index: inxWordDictAddins
CREATE INDEX inxWordDictAddins ON DictAddins (
    Word
);

-- View: EmbedJoin
CREATE VIEW EmbedJoin AS SELECT * FROM 
(
SELECT ed.Inx As Inx, ed.Freq As Freq, d.Word As Word, d.Vect As Vect FROM EmbedDict ed 
    JOIN Dict d ON (ed.DictId = d.Id)
UNION
SELECT ed.Inx As Inx, ed.Freq As Freq, da.Word As Word, da.Vect As Vect FROM EmbedDict ed 
    JOIN DictAddins da ON (ed.DictAddinsId = da.Id)
)
ORDER BY Inx;

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
