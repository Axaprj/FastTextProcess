--
-- File generated with SQLiteStudio v3.2.1 on Mon Apr 29 18:20:33 2019
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: Dict
CREATE TABLE Dict (Id INTEGER PRIMARY KEY AUTOINCREMENT, Word TEXT NOT NULL, Vect BLOB NOT NULL, LangId INTEGER NOT NULL, UNIQUE (Word, LangId));

-- Table: DictAddins
CREATE TABLE DictAddins (Id INTEGER PRIMARY KEY AUTOINCREMENT, Word TEXT NOT NULL, Vect BLOB, LangId INTEGER NOT NULL, UNIQUE (Word, LangId));

-- Table: EmbedDict
CREATE TABLE EmbedDict (Inx INTEGER UNIQUE PRIMARY KEY NOT NULL, DictId INTEGER REFERENCES Dict (Id) ON DELETE CASCADE ON UPDATE CASCADE, DictAddinsId INTEGER REFERENCES DictAddins (Id) ON DELETE CASCADE ON UPDATE CASCADE, Freq INTEGER NOT NULL);

-- Index: inxIdDictAddinsEmbedDict
CREATE INDEX inxIdDictAddinsEmbedDict ON EmbedDict (DictAddinsId);

-- Index: inxIdDictEmbedDict
CREATE INDEX inxIdDictEmbedDict ON EmbedDict (DictId);

-- Index: inxWordDict
CREATE INDEX inxWordDict ON Dict (Word, LangId);

-- Index: inxWordDictAddins
CREATE INDEX inxWordDictAddins ON DictAddins (Word, LangId);

-- View: EmbedJoin
CREATE VIEW EmbedJoin AS SELECT * FROM 
(
SELECT ed.Inx As Inx, ed.Freq As Freq, d.Word As Word, "main" As Dict, d.Vect As Vect FROM EmbedDict ed 
    JOIN Dict d ON (ed.DictId = d.Id)
UNION
SELECT ed.Inx As Inx, ed.Freq As Freq, da.Word As Word, "addin" As Dict, da.Vect As Vect FROM EmbedDict ed 
    JOIN DictAddins da ON (ed.DictAddinsId = da.Id)
)
ORDER BY Inx;

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
